using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity;
using BizSys.IntegrateManagement.Entity.CallBack;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.IntegrateManagement.Entity.SalesOrder;
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
        public async static void GetSalesOrder()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetSalesOrderCount"], 30);
            string guid = "SalesOrder-" + Guid.NewGuid();
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
                     new Conditions()
                     {
                         Alias="U_SBOSynchronization",
                         Operation = "co_IS_NULL",
                         BracketOpenNum = 1
                     },
                     new Conditions()
                     {
                         Alias="U_SBOSynchronization",
                         CondVal="",
                        Operation = "co_EQUAL",
                        Relationship = "cr_OR",
                         BracketCloseNum = 1
                     },
                     new Conditions(){
                         Alias="Canceled",
                         Operation = "co_EQUAL",
                         CondVal = "N",
                        Relationship = "cr_AND" },
                     new Conditions()
                     {
                        Alias="DocumentStatus",
                        CondVal = "R",
                        Operation = "co_EQUAL",
                         Relationship="cr_AND"

                     },
                      new Conditions()
                     {
                        Alias="DataSource",
                        CondVal="",
                        Operation = "co_EQUAL",
                        Relationship = "cr_AND"
                     }
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

                Remarks = null
            };
            //序列化json对象
            string requestJson = await JsonConvert.SerializeObjectAsync(cri);
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
            Logger.Writer(guid, QueueStatus.Open, "订单信息：\r\n" + resultJson);
            //生成销售订单
            int mSuccessCount = 0;
            foreach (var item in salesOrder.ResultObjects)
            {
                try
                {
                    var documentResult = Document.SalesManagement.SalesOrder.CreateSalesOrder(item);
                    if (documentResult.ResultValue == ResultType.True)
                    {
                        var callBackJsonString = JsonObject.GetCallBackJsonString(item.ObjectCode, item.DocEntry.ToString(), item.B1DocEntry, syncDateTime);
                        if (await B1Common.ServiceCommon.CallBack(callBackJsonString, guid, item))
                            mSuccessCount++;
                    }
                    Logger.Writer(guid, QueueStatus.Open, documentResult.ResultMessage);
                }
                catch (Exception ex)
                {
                    Logger.Writer(guid, QueueStatus.Open, "【" + item.DocEntry + "】销售订单处理发生异常：" + ex.Message);
                }
            }
            Logger.Writer(guid, QueueStatus.Close, "[" + mSuccessCount + "]条销售订单处理成功。");
            #endregion
        }
    }
}
