using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity.ReceiptPayment.ReceiptVerification;
using BizSys.IntegrateManagement.Entity;
using MagicBox.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.IntegrateManagement.Entity.CallBack;

namespace BizSys.OmniChannelToSAP.Service.Service.ReceiptPaymentService
{
    /// <summary>
    /// 回款核销
    /// </summary>
    public class GetReceiptVerificationService
    {
        /// <summary>
        /// 回款核销
        /// </summary>
        public async static void GetReceiptVerification()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetReceiptVerificationCount"], 30);
            string guid = "ReceiptVerification-" + Guid.NewGuid();
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
                     new Conditions()
                     {
                        Alias="Status",
                        CondVal = "C",
                        Operation = "co_EQUAL",
                         Relationship="cr_AND"

                     }
                },
                Sorts = new List<Sorts>(){
                    //new Sorts(){
                    //     __type="Sort",
                    //     Alias="Code",
                    //     SortType="st_Ascending"
                    //}
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
                resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.RECEIPTVERIFICATION, requestJson);
            }
            catch (Exception ex)
            {
                Logger.Writer("回款核销服务-网络请求出错，错误信息：" + ex.Message);
                return;
            }
            if (string.IsNullOrEmpty(resultJson)) Logger.Writer("回款核销查询服务出错，查询结果为null。");
            #endregion
            #region 订单处理
            //反序列化
            ReceiptVerificationRootObject receiptVerification = JsonConvert.DeserializeObject<ReceiptVerificationRootObject>(resultJson);
            if (receiptVerification.ResultObjects.Count == 0) return;
            DateTime syncDateTime = DateTime.Now;
            Logger.Writer(guid, QueueStatus.Open, "[" + receiptVerification.ResultObjects.Count + "]条回款核销开始处理。");
            Logger.Writer(guid, QueueStatus.Open, "订单信息：\r\n" + resultJson);
            //生成回款核销
            int mSuccessCount = 0;
            foreach (var item in receiptVerification.ResultObjects)
            {
                try
                {
                    var documentResult = Document.ReceiptPayment.ReceiptVerification.CreateJournalEntry(item);
                    if (documentResult.ResultValue == ResultType.True)
                    {
                        string callBackJsonString = JsonObject.GetCallBackJsonString(item.ObjectCode, "ObjectKey", item.ObjectKey.ToString(), item.B1DocEntry, syncDateTime);
                        string callBackResultStr = await BaseHttpClient.HttpCallBackAsync(callBackJsonString);
                        var callBackResult = await JsonConvert.DeserializeObjectAsync<CallBackResult>(callBackResultStr);
                        if (callBackResult.ResultCode == 0)
                            mSuccessCount++;
                        else
                            Logger.Writer(guid, QueueStatus.Open, "【" + item.ObjectKey + "】回款核销回传错误:" + callBackResult.Message + "\r\n 回传内容为：" + callBackJsonString);
                    }
                    Logger.Writer(guid, QueueStatus.Open, documentResult.ResultMessage);
                }
                catch (Exception ex)
                {
                    Logger.Writer(guid, QueueStatus.Open, "【" + item.DocEntry + "】回款核销处理发生异常：" + ex.Message);
                }
            }
            Logger.Writer(guid, QueueStatus.Close, "[" + mSuccessCount + "]条回款核销处理成功。");
            #endregion
        }
        
    }
}
