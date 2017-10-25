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
        public async static void GetSalesDeliveryOrder()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["ResultCount"], 30);
            string guid = "SalesDeliveryOrder-" + Guid.NewGuid();

            string resultJson = string.Empty;
            #region 查找条件
            Criteria cri = new Criteria()
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
                         SortType="st_Descending"
                    }
                },
                ChildCriterias = new List<ChildCriterias>()
                {

                },
                NotLoadedChildren = false,
                Remarks = null
            };
            //序列化json对象
            string requestJson = JsonConvert.SerializeObject(cri);
            #endregion
            #region 调用接口
            try
            {
                resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.SALESDELIVERYORDER, requestJson);
            }
            catch (Exception ex)
            {
                Logger.Writer("销售交货订单服务-网络请求出错，错误信息：" + ex.Message);
                return;
            }
            if (string.IsNullOrEmpty(resultJson)) Logger.Writer("销售交货订单查询服务出错，查询结果为null。");
            #endregion
            #region 订单处理
            //反序列化
            SalesDeliveryOrderRootObject salesDeliveryOrder = JsonConvert.DeserializeObject<SalesDeliveryOrderRootObject>(resultJson);
            if (salesDeliveryOrder.ResultObjects.Count == 0) return;
            DateTime syncDateTime = DateTime.Now;
            Logger.Writer(guid, QueueStatus.Open, "[" + salesDeliveryOrder.ResultObjects.Count + "]条销售交货订单开始处理。");
            Logger.Writer(guid, QueueStatus.Open, "订单信息：\r\n" + resultJson);
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
                            var callBackResult = JsonConvert.DeserializeObject<CallBackResult>(callBackResultStr);
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
            GC.Collect();
            #endregion
        
        }
    }
}
