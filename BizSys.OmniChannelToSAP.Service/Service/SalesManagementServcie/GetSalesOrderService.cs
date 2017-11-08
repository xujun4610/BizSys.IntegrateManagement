using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity;
using BizSys.IntegrateManagement.Entity.CallBack;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.IntegrateManagement.Entity.SalesManagement.SalesOrder;
using MagicBox.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
namespace BizSys.OmniChannelToSAP.Service.Service.SalesManagementServcie
{
    public class GetSalesOrderService
    {
        //static object obj = new object();

        public async static void GetSalesOrder(int LastResultCount = -1, string LastDocEntry = null)
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["ResultCount"], 10);
            string guid = "SalesOrder-" + Guid.NewGuid();
            string resultJson = string.Empty;
            #region 查找条件
            #region 基础 Criteria
            Criteria commonCri = new Criteria()
            {
                __type = "Criteria",
                ResultCount = resultCount,
                isDbFieldName = false,
                BusinessObjectCode = null,
                Conditions = new List<Conditions>()
                {
                    new Conditions {
                        Alias = "ApprovalStatus",
                        Operation = "co_EQUAL",
                        Relationship = "",
                        CondVal = "U",
                        BracketOpenNum = 1
                    },
                    new Conditions {
                        Alias = "ApprovalStatus",
                        Operation = "co_EQUAL",
                        Relationship = "cr_OR",
                        CondVal = "A",
                        BracketCloseNum = 1
                    },
                    new Conditions {
                        Alias = "DocumentStatus",
                        Operation = "co_EQUAL",
                        Relationship = "cr_AND",
                        CondVal = "R"
                    },
                        new Conditions ()
                        {
                            Alias="U_SBOSynchronization",
                             CondVal="Y",
                            Operation = "co_NOT_EQUAL",
                            Relationship = "cr_AND",
                        },
                    //new Conditions {
                    //    Alias = "U_SBOSynchronization",
                    //    CondVal = "",
                    //    Operation = "co_EQUAL",
                    //    Relationship = "cr_OR",
                    //    BracketCloseNum = 1
                    //},
                    new Conditions {
                        Alias = "SalesPerson",
                        CondVal = "bj",
                        Operation = "co_CONTAIN",
                        BracketOpenNum = 1,
                        Relationship = "cr_AND"
                    },
                    new Conditions {
                        Alias = "SalesPerson",
                        CondVal = "xz",
                        Operation = "co_CONTAIN",
                        Relationship = "cr_OR",
                        BracketCloseNum = 1
                    }
                },
                Sorts = new List<Sorts>(){
                    new Sorts(){
                         __type="Sort",
                         Alias="DocEntry",
                         SortType="st_Ascending"
                    }
                },
                ChildCriterias = new List<ChildCriterias>()
                {

                },
                NotLoadedChildren = false,

                Remarks = null
            };

            #endregion
            if (!(LastResultCount == -1 && string.IsNullOrWhiteSpace(LastDocEntry)))
            {//意味着这不是全新的（不是第一页）
                var extraCris = new List<Conditions>();
                var exCri = new Conditions();
                exCri.Alias = "DocEntry";
                exCri.CondVal = LastDocEntry;
                exCri.Operation = "co_GRATER_THAN";
                exCri.Relationship = "cr_AND";
                extraCris.Add(exCri);
                commonCri.Conditions.AddRange(extraCris);
            }
            //if (LastResultCount >= 0 && LastResultCount < resultCount)
            //{
            //    Logger.Writer(guid, QueueStatus.Open, "结束分页数据扫描!");
            //    return;
            //}

            //序列化json对象
            string requestJson = await JsonConvert.SerializeObjectAsync(commonCri);
            #endregion
            #region 调用接口
            try
            {
                resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.SALESORDER, requestJson);
            }
            catch (Exception ex)
            {
                Logger.Writer("销售订单服务-网络请求出错，错误信息：" + ex.Message);
                return;
            }
            if (string.IsNullOrEmpty(resultJson)) Logger.Writer("销售订单查询服务出错，查询结果为null。");
            #endregion
            #region 订单处理
            //反序列化
            SalesOrderRootObject salesOrder = await JsonConvert.DeserializeObjectAsync<SalesOrderRootObject>(resultJson);
            if (salesOrder.ResultObjects.Count == 0) return;
            DateTime syncDateTime = DateTime.Now;
            Logger.Writer(guid, QueueStatus.Open, "[" + salesOrder.ResultObjects.Count + "]条销售订单开始处理。");
            //Logger.Writer(guid, QueueStatus.Open, "订单信息：\r\n" + resultJson);
            //生成销售订单
            int mSuccessCount = 0;
            string b1ErrorDocs = string.Empty;
            foreach (var item in salesOrder.ResultObjects)
            {
                try
                {
                    if (!("XZ,BJ").Contains(item.Salesperson.Substring(0, 2).ToUpper()))
                    {
                        //不是北京西藏开头的就别管！给我继续
                        continue;
                    }
                    Result documentResult = null;
                    documentResult = Document.SalesManagement.SalesOrder.CreateSalesOrder(item);
                    if (documentResult.ResultValue == ResultType.True)
                    {
                        var callBackJsonString = JsonObject.GetCallBackJsonString(item.ObjectCode, item.DocEntry.ToString(), item.B1DocEntry, syncDateTime);
                        if (await B1Common.ServiceCommon.CallBack(callBackJsonString, guid, item))
                            mSuccessCount++;
                    }
                    else
                    {
                        b1ErrorDocs += documentResult.DocEntry + ",";
                    }
                    Logger.Writer(guid, QueueStatus.Open, documentResult.ResultMessage);
                }
                catch (Exception ex)
                {
                    Logger.Writer(guid, QueueStatus.Open, "【" + item.DocEntry + "】销售订单处理发生异常：" + ex.Message);
                }
            }
            Logger.Writer(guid, QueueStatus.Open, string.Format("[{0}]条销售订单处理成功，[{1}]条失败。\n失败单据：[{2}]", mSuccessCount, (salesOrder.ResultObjects.Count - mSuccessCount).ToString(), b1ErrorDocs));

            //执行分页操作
            if (salesOrder.ResultObjects.Count == resultCount)
            {
                //下一页
                GetSalesOrder(salesOrder.ResultObjects.Count, salesOrder.ResultObjects[salesOrder.ResultObjects.Count - 1].DocEntry.ToString());
            }
            Logger.Writer(guid, QueueStatus.Close, string.Format("此次循环同步已结束！\n----------------------------------------------------------------------------------"));
            #endregion

        }

        /// <summary>
        /// 同步方式
        /// </summary>
        /// <param name="LastResultCount"></param>
        /// <param name="LastDocEntry"></param>
        public static void GetSalesOrder4Sync(int LastResultCount = -1, string LastDocEntry = null)
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["ResultCount"], 10);
            string guid = "SalesOrder-" + Guid.NewGuid();
            string resultJson = string.Empty;
            #region 查找条件
            #region 基础 Criteria
            Criteria commonCri = new Criteria()
            {
                __type = "Criteria",
                ResultCount = resultCount,
                isDbFieldName = false,
                BusinessObjectCode = null,
                Conditions = new List<Conditions>()
                {
                    new Conditions {
                        Alias = "ApprovalStatus",
                        Operation = "co_EQUAL",
                        Relationship = "",
                        CondVal = "U",
                        BracketOpenNum = 1
                    },
                    new Conditions {
                        Alias = "ApprovalStatus",
                        Operation = "co_EQUAL",
                        Relationship = "cr_OR",
                        CondVal = "A",
                        BracketCloseNum = 1
                    },
                    new Conditions {
                        Alias = "DocumentStatus",
                        Operation = "co_EQUAL",
                        Relationship = "cr_AND",
                        CondVal = "R"
                    },
                    new Conditions {
                        Alias = "U_SBOSynchronization",
                        Operation = "co_IS_NULL",
                        BracketOpenNum = 1,
                        Relationship = "cr_AND"
                    },
                    new Conditions {
                        Alias = "U_SBOSynchronization",
                        CondVal = "",
                        Operation = "co_EQUAL",
                        Relationship = "cr_OR",
                        BracketCloseNum = 1
                    },
                    new Conditions {
                        Alias = "SalesPerson",
                        CondVal = "bj",
                        Operation = "co_CONTAIN",
                        BracketOpenNum = 1,
                        Relationship = "cr_AND"
                    },
                    new Conditions {
                        Alias = "SalesPerson",
                        CondVal = "xz",
                        Operation = "co_CONTAIN",
                        Relationship = "cr_OR",
                        BracketCloseNum = 1
                    }
                },
                Sorts = new List<Sorts>(){
                    new Sorts(){
                         __type="Sort",
                         Alias="DocEntry",
                         SortType="st_Ascending"
                    }
                },
                ChildCriterias = new List<ChildCriterias>()
                {

                },
                NotLoadedChildren = false,

                Remarks = null
            };

            #endregion
            if (!(LastResultCount == -1 && string.IsNullOrWhiteSpace(LastDocEntry)))
            {//意味着这不是全新的（不是第一页）
                var extraCris = new List<Conditions>();
                var exCri = new Conditions();
                exCri.Alias = "DocEntry";
                exCri.CondVal = LastDocEntry;
                exCri.Operation = "co_GRATER_THAN";
                exCri.Relationship = "cr_AND";
                extraCris.Add(exCri);
                commonCri.Conditions.AddRange(extraCris);
            }
            //if (LastResultCount >= 0 && LastResultCount < resultCount)
            //{
            //    Logger.Writer(guid, QueueStatus.Open, "结束分页数据扫描!");
            //    return;
            //}

            //序列化json对象
            string requestJson = JsonConvert.SerializeObject(commonCri);
            #endregion
            #region 调用接口
            try
            {
                resultJson = BaseHttpClient.HttpFetch(DocumentType.SALESORDER, requestJson);
            }
            catch (Exception ex)
            {
                Logger.Writer("销售订单服务-网络请求出错，错误信息：" + ex.Message);
                return;
            }
            if (string.IsNullOrEmpty(resultJson)) Logger.Writer("销售订单查询服务出错，查询结果为null。");
            #endregion
            #region 订单处理
            //反序列化
            SalesOrderRootObject salesOrder = JsonConvert.DeserializeObject<SalesOrderRootObject>(resultJson);
            if (salesOrder.ResultObjects.Count == 0) return;
            DateTime syncDateTime = DateTime.Now;
            Logger.Writer(guid, QueueStatus.Open, "[" + salesOrder.ResultObjects.Count + "]条销售订单开始处理。");
            //Logger.Writer(guid, QueueStatus.Open, "订单信息：\r\n" + resultJson);
            //生成销售订单
            int mSuccessCount = 0;
            string b1ErrorDocs = string.Empty;
            foreach (var item in salesOrder.ResultObjects)
            {
                try
                {
                    if (!("XZ,BJ").Contains(item.Salesperson.Substring(0, 2).ToUpper()))
                    {
                        //不是北京西藏开头的就别管！给我继续
                        continue;
                    }
                    var documentResult = Document.SalesManagement.SalesOrder.CreateSalesOrder(item);
                    if (documentResult.ResultValue == ResultType.True)
                    {
                        var callBackJsonString = JsonObject.GetCallBackJsonString(item.ObjectCode, item.DocEntry.ToString(), item.B1DocEntry, syncDateTime);
                        if (B1Common.ServiceCommon.CallBackSync(callBackJsonString, guid, item))
                            mSuccessCount++;
                    }
                    else
                    {
                        b1ErrorDocs += documentResult.DocEntry + ",";
                    }
                    Logger.Writer(guid, QueueStatus.Open, documentResult.ResultMessage);
                }
                catch (Exception ex)
                {
                    Logger.Writer(guid, QueueStatus.Open, "【" + item.DocEntry + "】销售订单处理发生异常：" + ex.Message);
                }
            }
            Logger.Writer(guid, QueueStatus.Open, string.Format("[{0}]条销售订单处理成功，[{1}]条失败。\n失败单据：[{2}]", mSuccessCount, (salesOrder.ResultObjects.Count - mSuccessCount).ToString(), b1ErrorDocs));

            //执行分页操作
            if (salesOrder.ResultObjects.Count == resultCount)
            {
                //下一页
                GetSalesOrder4Sync(salesOrder.ResultObjects.Count, salesOrder.ResultObjects[salesOrder.ResultObjects.Count - 1].DocEntry.ToString());
            }
            Logger.Writer(guid, QueueStatus.Close, string.Format("此次循环同步已结束！"));
            #endregion
        }

    }
}
