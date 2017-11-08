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
                    /*
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
                    */
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
                         SortType="st_Descending"
                    }
                },
                ChildCriterias = new List<ChildCriterias>()
                {

                },
                NotLoadedChildren = false,
                //Remarks = null
            };

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
            foreach (var item in salesDeliveryOrder.ResultObjects)
            {
                try
                {
                    //var documentResult = Document.SalesManagement.SalesDeliveryOrder.CreateSalesOrder(item);

                    //if (documentResult.ResultValue == ResultType.True)
                    //{
                        //成功生成销售订单后 生成销售交货单
                        var rt = Document.SalesManagement.SalesDeliveryOrder.CreateSalesDeliveryOrder(item);
                        if(rt.ResultValue == ResultType.True)
                        {
                            string callBackJsonString = JsonObject.GetCallBackJsonString(item.ObjectCode, item.DocEntry.ToString(), rt.DocEntry, syncDateTime);
                            string callBackResultStr = await BaseHttpClient.HttpCallBackAsync(callBackJsonString);
                            var callBackResult = await JsonConvert.DeserializeObjectAsync<CallBackResult>(callBackResultStr);
                            if (callBackResult.ResultCode == 0)
                                mSuccessCount++;
                        }
                        Logger.Writer(guid, QueueStatus.Open, rt.ResultMessage);
                    //}
                   // Logger.Writer(guid, QueueStatus.Open, documentResult.ResultMessage);
                }
                catch (Exception ex)
                {
                    Logger.Writer(guid, QueueStatus.Open, "【" + item.DocEntry + "】销售交货订单处理发生异常：" + ex.Message);
                }
            }
            Logger.Writer(guid, QueueStatus.Close, "[" + mSuccessCount + "]条销售交货订单处理成功。");

            //执行分页操作
            if (salesDeliveryOrder.ResultObjects.Count == resultCount)
            {
                //下一页
                GetSalesDeliveryOrder(salesDeliveryOrder.ResultObjects.Count, salesDeliveryOrder.ResultObjects[salesDeliveryOrder.ResultObjects.Count - 1].DocEntry.ToString());
            }

            Logger.Writer(guid, QueueStatus.Close, string.Format("此次循环同步已结束！\n----------------------------------------------------------------------------------"));
            #endregion

        }
    }
}
