using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity.CallBack;
using BizSys.IntegrateManagement.Entity;
using BizSys.IntegrateManagement.Entity.Result;
using MagicBox.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using BizSys.IntegrateManagement.Entity.ReceiptPayment.PaymentApply;

namespace BizSys.OmniChannelToSAP.Service.Service.ReceiptPaymentService
{
    public class GetPaymentApplyService
    {
        public async static System.Threading.Tasks.Task GetPayment()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetPaymentApplyCount"], 30);
            string guid = "Payment-" + Guid.NewGuid();
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
                NotLoadedChildren = true,
                Remarks = null
            };
            //序列化json对象
            string requestJson = await JsonConvert.SerializeObjectAsync(cri);
            #endregion
            #region 调用接口
            try
            {
                resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.PAYMENT, requestJson);
            }
            catch (Exception ex)
            {
                Logger.Writer("付款单服务-网络请求出错，错误信息：" + ex.Message);
                return;
            }
            if (string.IsNullOrEmpty(resultJson)) Logger.Writer("付款单查询服务出错，查询结果为null。");
            #endregion
            #region 付款单处理
            //反序列化
            PaymentApplyRootObject ReceiptPayment = await JsonConvert.DeserializeObjectAsync<PaymentApplyRootObject>(resultJson);
            if (ReceiptPayment.ResultObjects.Count == 0) return;
            DateTime syncDateTime = DateTime.Now;
            Logger.Writer(guid, QueueStatus.Open, "[" + ReceiptPayment.ResultObjects.Count + "]条付款单开始处理。");
            Logger.Writer(guid, QueueStatus.Open, "付款单信息：\r\n" + resultJson);
            //生成付款单1
            int mSuccessCount = 0;
            foreach (var item in ReceiptPayment.ResultObjects)
            {
                try
                {
                    var documentResult = Document.ReceiptPayment.Payment.CreatePaymentDraft(item);
                    if (documentResult.ResultValue == ResultType.True)
                    {
                        string callBackJsonString = JsonObject.GetCallBackJsonString(item.ObjectCode, item.DocEntry.ToString(), item.B1DocEntry, syncDateTime);
                        string callBackResultStr = await BaseHttpClient.HttpCallBackAsync(callBackJsonString);
                        var callBackResult = await JsonConvert.DeserializeObjectAsync<CallBackResult>(callBackResultStr);
                        if (callBackResult.ResultCode == 0)
                            mSuccessCount++;
                        else
                            Logger.Writer(guid, QueueStatus.Open, "【" + item.DocEntry + "】付款单回传错误:" + callBackResult.Message + "\r\n 回传内容为：" + callBackJsonString);
                    }
                    Logger.Writer(guid, QueueStatus.Open, documentResult.ResultMessage);
                }
                catch (Exception ex)
                {
                    Logger.Writer(guid, QueueStatus.Open, "【" + item.DocEntry + "】付款单处理发生异常：" + ex.Message);
                }
            }
            Logger.Writer(guid, QueueStatus.Close, "[" + mSuccessCount + "]条付款单处理成功。");
            #endregion
        }
    }
}
