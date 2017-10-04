using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity;
using BizSys.IntegrateManagement.Entity.CallBack;
using BizSys.IntegrateManagement.Entity.ReceiptPayment.CostReimbursement;
using BizSys.IntegrateManagement.Entity.Result;
using MagicBox.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.OmniChannelToSAP.Service.Service.ReceiptPaymentService
{
    /// <summary>
    /// 费用报销
    /// </summary>
    public class GetCostReimbursementService
    {
        /// <summary>
        /// 费用报销
        /// </summary>
        public async static void GetCostReimbursement()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetReconciliationCount"], 30);
            string guid = "CostReimbursement-" + Guid.NewGuid();
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
                        Alias="DocumentStatus",
                        CondVal = "R",
                        Operation = "co_EQUAL",
                         Relationship="cr_AND"

                     }
                },
                Sorts = new List<Sorts>(){
                    //new Sorts(){
                       
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
                resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.COSTREIMBURSEMENT, requestJson);
            }
            catch (Exception ex)
            {
                Logger.Writer("费用报销服务-网络请求出错，错误信息：" + ex.Message);
                return;
            }
            if (string.IsNullOrEmpty(resultJson)) Logger.Writer("费用报销查询服务出错，查询结果为null。");
            #endregion
            #region 订单处理
            //反序列化
            CostReimbursementRootObject receiptVerification = JsonConvert.DeserializeObject<CostReimbursementRootObject>(resultJson);
            if (receiptVerification.ResultObjects.Count == 0) return;
            DateTime syncDateTime = DateTime.Now;
            Logger.Writer(guid, QueueStatus.Open, "[" + receiptVerification.ResultObjects.Count + "]条费用报销开始处理。");
            Logger.Writer(guid, QueueStatus.Open, "订单信息：\r\n" + resultJson);
            //生成日记账分录
            int mSuccessCount = 0;
            foreach (var item in receiptVerification.ResultObjects)
            {
                try
                {
                    var documentResult = Document.ReceiptPayment.CostReimbursement.CreateJournalEntry(item);
                    if (documentResult.ResultValue == ResultType.True)
                    {
                        string callBackJsonString = JsonObject.GetCallBackJsonString(item.ObjectCode, item.DocEntry.ToString(), item.B1DocEntry, syncDateTime);
                        string callBackResultStr = await BaseHttpClient.HttpCallBackAsync(callBackJsonString);
                        var callBackResult = await JsonConvert.DeserializeObjectAsync<CallBackResult>(callBackResultStr);
                        if (callBackResult.ResultCode == 0)
                            mSuccessCount++;
                        else
                            Logger.Writer(guid, QueueStatus.Open, "【" + item.DocEntry + "】费用报销单回传错误:" + callBackResult.Message + "\r\n 回传内容为：" + callBackJsonString);
                    }
                    Logger.Writer(guid, QueueStatus.Open, documentResult.ResultMessage);
                }
                catch (Exception ex)
                {
                    Logger.Writer(guid, QueueStatus.Open, "【" + item.DocEntry + "】费用报销处理发生异常：" + ex.Message);
                }
            }
            Logger.Writer(guid, QueueStatus.Close, "[" + mSuccessCount + "]条费用报销处理成功。");
            #endregion
        }
    }
}
