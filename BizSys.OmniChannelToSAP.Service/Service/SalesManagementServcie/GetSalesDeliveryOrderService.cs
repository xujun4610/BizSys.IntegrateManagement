using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity;
using BizSys.IntegrateManagement.Entity.CallBack;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.IntegrateManagement.Entity.SalesManagement.SalesDeliveryOrder;
using BizSys.OmniChannelToSAP.Service.B1Common;
using MagicBox.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;

namespace BizSys.OmniChannelToSAP.Service.Service.SalesManagementServcie
{
    public class GetSalesDeliveryOrderService
    {
        private static readonly object obj = new object();
        /// <summary>
        /// 异步方法
        /// </summary>
        /// <param name="LastResultCount"></param>
        /// <param name="LastDocEntry"></param>
        public async static void GetSalesDeliveryOrder(int LastResultCount = -1, string LastDocEntry = null)
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["ResultCount"], 30);
            string guid = "SalesDeliveryOrder-" + Guid.NewGuid();

            string resultJson = string.Empty;
            #region 查找条件
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
                        Relationship = "cr_AND",
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
                    }
                    
                     // new Conditions()
                     //{
                     //   Alias="DataSource",
                     //   CondVal = "13",
                     //   Operation = "co_EQUAL",
                     //   Relationship="cr_AND"
                     //}
                },
                Sorts = new List<Sorts>(){
                    new Sorts(){
                         __type="Sort",
                         Alias="DocEntry",
                         SortType="st_Asccending"
                    }
                },
                ChildCriterias = new List<ChildCriterias>()
                {

                },
                NotLoadedChildren = false,
                //Remarks = null
            };

            while (true)
            {

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
                //序列化json对象
                //Logger.Writer("序列化json");
                string requestJson = await JsonConvert.SerializeObjectAsync(commonCri);
                //Logger.Writer("序列化json完毕");

                #endregion
                #region 调用接口
                try
                {
                    resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.SALESDELIVERYORDER, requestJson);
                }
                catch (Exception ex)
                {
                    Logger.Writer("销售交货订单服务-网络请求出错，错误信息：" + ex.InnerException.Message + ex.Message);
                    return;
                }
                if (string.IsNullOrEmpty(resultJson)) { Logger.Writer("销售交货订单查询服务出错，查询结果为null。"); return; }
                #endregion
                #region 订单处理
                //反序列化
                SalesDeliveryOrderRootObject salesDeliveryOrder = await JsonConvert.DeserializeObjectAsync<SalesDeliveryOrderRootObject>(resultJson);
                if (salesDeliveryOrder.ResultObjects.Count == 0)
                    Logger.Writer("此次同步没有符合条件的交货单据！");
                DateTime syncDateTime = DateTime.Now;
                Logger.Writer(guid, QueueStatus.Open, "[" + salesDeliveryOrder.ResultObjects.Count + "]条销售交货订单开始处理。");
                //Logger.Writer(guid, QueueStatus.Open, "订单信息：\r\n" + resultJson);
                //生成销售交货
                int mSuccessCount = 0;
                //b1错误单号
                string b1ErrorDocEntry = "";
                foreach (var item in salesDeliveryOrder.ResultObjects)
                {
                    try
                    {
                        //var documentResult = Document.SalesManagement.SalesDeliveryOrder.CreateSalesOrder(item);

                        //if (documentResult.ResultValue == ResultType.True)
                        //{
                        //成功生成销售订单后 生成销售交货单
                        var rt = Document.SalesManagement.SalesDeliveryOrder.CreateSalesDeliveryOrder(item);
                        if (rt.ResultValue == ResultType.True)
                        {
                            string callBackJsonString = JsonObject.GetCallBackJsonString(item.ObjectCode, item.DocEntry.ToString(), rt.DocEntry, syncDateTime);
                            string callBackResultStr = await BaseHttpClient.HttpCallBackAsync(callBackJsonString);
                            var callBackResult = await JsonConvert.DeserializeObjectAsync<CallBackResult>(callBackResultStr);
                            if (callBackResult.ResultCode == 0)
                                mSuccessCount++;
                        }
                        else
                        {
                            //记录失败的单据编号
                            b1ErrorDocEntry += rt.DocEntry + ",";
                        }
                        Logger.Writer(guid, QueueStatus.Close, rt.ResultMessage);
                        //}
                        // Logger.Writer(guid, QueueStatus.Open, documentResult.ResultMessage);
                    }
                    catch (Exception ex)
                    {
                        Logger.Writer(guid, QueueStatus.Close, string.Format("【{0}】销售交货订单处理发生异常：{1}", mSuccessCount, ex.Message));
                    }
                }
                Logger.Writer(guid, QueueStatus.Close, string.Format("[{0}]条销售交货订单处理成功，[{1}]条失败单据，失败的单据[{2}]。", mSuccessCount, salesDeliveryOrder.ResultObjects.Count - mSuccessCount, b1ErrorDocEntry));

                #endregion
                LastResultCount = salesDeliveryOrder.ResultObjects.Count;
                LastDocEntry = salesDeliveryOrder.ResultObjects[salesDeliveryOrder.ResultObjects.Count - 1].DocEntry.ToString();
                if (LastResultCount < resultCount) break;
            }
            Logger.Writer(guid, QueueStatus.Close, string.Format("此次循环同步已结束！\n----------------------------------------------------------------------------------"));
        }

        /// <summary>
        /// 同步方法
        /// </summary>
        /// <param name="LastResultCount"></param>
        /// <param name="LastDocEntry"></param>
        public static void GetSalesDeliveryOrderSync(int LastResultCount = -1, string LastDocEntry = null)
        {
            lock (obj)
            {
                int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["ResultCount"], 20);
                string guid = "SalesDeliveryOrder-" + Guid.NewGuid();

                string resultJson = string.Empty;
                #region 查找条件
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
                    /*
                    new Conditions ()
                    {
                        Alias="U_SBOSynchronization",
                            CondVal="Y",
                        Operation = "co_NOT_EQUAL",
                        Relationship = "cr_AND",
                    },
                    */
                    
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
                    }
                    
                     // new Conditions()
                     //{
                     //   Alias="DataSource",
                     //   CondVal = "13",
                     //   Operation = "co_EQUAL",
                     //   Relationship="cr_AND"
                     //}
                },
                    Sorts = new List<Sorts>(){
                    new Sorts(){
                         __type="Sort",
                         Alias="DocEntry",
                         SortType="st_Asccending"
                    }
                },
                    ChildCriterias = new List<ChildCriterias>()
                    {

                    },
                    NotLoadedChildren = false,
                    //Remarks = null
                };

                while (true)
                {

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
                    //序列化json对象
                    //Logger.Writer("序列化json");
                    string requestJson = JsonConvert.SerializeObject(commonCri);
                    //Logger.Writer("序列化json完毕");

                    #endregion
                    #region 调用接口
                    try
                    {
                        resultJson = BaseHttpClient.HttpFetch(DocumentType.SALESDELIVERYORDER, requestJson);
                    }
                    catch (Exception ex)
                    {
                        Logger.Writer("销售交货订单服务-网络请求出错，错误信息：" + ex.InnerException.Message + ex.Message);
                        return;
                    }
                    if (string.IsNullOrEmpty(resultJson)) { Logger.Writer("销售交货订单查询服务出错，查询结果为null。"); return; }
                    #endregion
                    #region 订单处理
                    //反序列化
                    SalesDeliveryOrderRootObject salesDeliveryOrder = JsonConvert.DeserializeObject<SalesDeliveryOrderRootObject>(resultJson);
                    if (salesDeliveryOrder.ResultObjects.Count == 0)
                        Logger.Writer(guid, QueueStatus.Close, "此次同步没有符合条件的交货单据！");
                    DateTime syncDateTime = DateTime.Now;
                    Logger.Writer(guid, QueueStatus.Open, "[" + salesDeliveryOrder.ResultObjects.Count + "]条销售交货订单开始处理。");
                    //Logger.Writer(guid, QueueStatus.Open, "订单信息：\r\n" + resultJson);
                    //生成销售交货
                    int mSuccessCount = 0;
                    //b1 错误单据编号
                    string b1ErrorDocEntry = "";
                    foreach (var item in salesDeliveryOrder.ResultObjects)
                    {
                        try
                        {
                            //var documentResult = Document.SalesManagement.SalesDeliveryOrder.CreateSalesOrder(item);

                            //if (documentResult.ResultValue == ResultType.True)
                            //{
                            //成功生成销售订单后 生成销售交货单
                            var rt = Document.SalesManagement.SalesDeliveryOrder.CreateSalesDeliveryOrder(item);
                            if (rt.ResultValue == ResultType.True)
                            {
                                string callBackJsonString = JsonObject.GetCallBackJsonString(item.ObjectCode, item.DocEntry.ToString(), rt.DocEntry, syncDateTime);
                                string callBackResultStr = BaseHttpClient.HttpCallBack(callBackJsonString);
                                var callBackResult = JsonConvert.DeserializeObject<CallBackResult>(callBackResultStr);
                                if (callBackResult.ResultCode == 0)
                                    mSuccessCount++;
                            }
                            else
                            {
                                //记录失败的单据编号
                                b1ErrorDocEntry += rt.DocEntry + ",";
                            }
                            Logger.Writer(guid, QueueStatus.Close, rt.ResultMessage);
                            //}
                            // Logger.Writer(guid, QueueStatus.Open, documentResult.ResultMessage);
                        }
                        catch (Exception ex)
                        {
                            Logger.Writer(guid, QueueStatus.Close, string.Format("【{0}】销售交货订单处理发生异常：{1}", mSuccessCount, ex.Message));
                        }
                    }
                    Logger.Writer(guid, QueueStatus.Close, string.Format("[{0}]条销售交货订单处理成功，[{1}]条失败单据，失败的单据[{2}]。", mSuccessCount, salesDeliveryOrder.ResultObjects.Count - mSuccessCount, b1ErrorDocEntry));

                    #endregion
                    LastResultCount = salesDeliveryOrder.ResultObjects.Count;
                    LastDocEntry = salesDeliveryOrder.ResultObjects[salesDeliveryOrder.ResultObjects.Count - 1].DocEntry.ToString();
                    if (LastResultCount < resultCount) break;
                }
                Logger.Writer(guid, QueueStatus.Close, string.Format("此次循环同步已结束！\n----------------------------------------------------------------------------------"));
            }

        }


    }
}
