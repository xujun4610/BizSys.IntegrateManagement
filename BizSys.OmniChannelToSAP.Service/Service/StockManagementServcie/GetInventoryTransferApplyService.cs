using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity;
using BizSys.IntegrateManagement.Entity.CallBack;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.IntegrateManagement.Entity.StockManagement.InventoryTransferApply;
using MagicBox.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;

namespace BizSys.OmniChannelToSAP.Service.Service.StockManagementServcie
{

    public class GetInventoryTransferApplyService
    {
        /// <summary>
        /// 库存转储
        /// </summary>
        public async static void GetInventoryTransfer()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetInventoryTransferApplyCount"], 30);
            string guid = "InventoryTransferApply-" + Guid.NewGuid();
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
                },
                Sorts = new List<Sorts>()
                {
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
            string requestJson = await JsonConvert.SerializeObjectAsync(cri);
            #endregion
            #region 调用接口
            try
            {
                resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.INVENTORYTRANSFER, requestJson);
            }
            catch (Exception ex)
            {
                Logger.Writer("库存转储申请服务-网络请求出错，错误信息：" + ex.Message);
                return;
            }
            if (string.IsNullOrEmpty(resultJson)) Logger.Writer("库存转储申请数据查询服务出错，查询结果为null。");
            #endregion

            #region 订单处理
            InventoryTransferApplyRootObject InventoryTransferApplyOrder = JsonConvert.DeserializeObject<InventoryTransferApplyRootObject>(resultJson);
            if (InventoryTransferApplyOrder.ResultObjects.Count == 0) return;
            DateTime syncDateTime = DateTime.Now;
            Logger.Writer(guid, QueueStatus.Open, "[" + InventoryTransferApplyOrder.ResultObjects.Count + "]条库存转储申请开始处理。");
            Logger.Writer(guid, QueueStatus.Open, "库存转储申请信息：\r\n" + resultJson);

            int mSuccessCount = 0;
            foreach (var item in InventoryTransferApplyOrder.ResultObjects)
            {
                try
                {
                    var documentResult = Document.StockManagement.InventoryTransferApply.CreateTransferOrder(item);
                    if (documentResult.ResultValue == ResultType.True)
                    {
                        string callBackJsonString = JsonObject.GetCallBackJsonString(item.ObjectCode, item.DocEntry.ToString(), item.B1DocEntry, syncDateTime);
                        string callBackResultStr = await BaseHttpClient.HttpCallBackAsync(callBackJsonString);
                        var callBackResult = await JsonConvert.DeserializeObjectAsync<CallBackResult>(callBackResultStr);
                        if (callBackResult.ResultCode == 0)
                            mSuccessCount++;
                        else
                            Logger.Writer(guid, QueueStatus.Open, "【" + item.DocEntry + "】库存转储回传错误:" + callBackResult.Message + "\r\n 回传内容为：" + callBackJsonString);
                    }
                    Logger.Writer(guid, QueueStatus.Open, documentResult.ResultMessage);
                }
                catch (Exception ex)
                {
                    Logger.Writer(guid, QueueStatus.Open, "【" + item.DocEntry + "】库存转储申请处理发生异常：" + ex.Message);
                }
            }
            Logger.Writer(guid, QueueStatus.Close, "[" + mSuccessCount + "]条库存转储申请处理成功。");
            #endregion
        }
    }
}
