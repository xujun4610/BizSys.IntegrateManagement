using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity.CallBack;
using BizSys.IntegrateManagement.Entity;
using BizSys.IntegrateManagement.Entity.ReceiptPayment;
using BizSys.IntegrateManagement.Entity.ReceiptPayment.Receipt;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.IntegrateManagement.Entity.SalesManagement.SalesOrder;
using MagicBox.Log;
using MagicBox.WindowsServices.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.OmniChannelToSAP.Service.Service.ReceiptPaymentService
{
    public class GetReceiptService
    {
        public async static void GetReceipt()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetReceiptCount"], 30);
            string guid = "Receipt-" + Guid.NewGuid();
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
                    //},
                    //new Conditions
                    //{
                    //    Alias="DataSource",
                    //    CondVal = "13",
                    //    Operation = "co_NOT_EQUAL",
                    //    Relationship="cr_AND"
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
                NotLoadedChildren = true,
                Remarks = null
            };
            //序列化json对象
            string requestJson = await JsonConvert.SerializeObjectAsync(cri);
            #endregion
            #region 调用接口
            try
            {
                resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.RECEIPT, requestJson);
            }
            catch (Exception ex)
            {
                Logger.Writer("收款单服务-网络请求出错，错误信息：" + ex.Message);
                return;
            }
            if (string.IsNullOrEmpty(resultJson)) Logger.Writer("收款单查询服务出错，查询结果为null。");
            #endregion
            #region 收款单处理
            //反序列化
            ReceiptRootObject ReceiptPayment = await JsonConvert.DeserializeObjectAsync<ReceiptRootObject>(resultJson);
            if (ReceiptPayment.ResultObjects.Count == 0) return;
            DateTime syncDateTime = DateTime.Now;
            Logger.Writer(guid, QueueStatus.Open, "[" + ReceiptPayment.ResultObjects.Count + "]条收款单开始处理。");
            Logger.Writer(guid, QueueStatus.Open, "收款单信息：\r\n" + resultJson);
            //生成收款单
            int mSuccessCount = 0;
            foreach (var item in ReceiptPayment.ResultObjects)
            {
                try
                {
                    var documentResult = Document.ReceiptPayment.Receipt.CreateReceipt(item);
                    if (documentResult.ResultValue == ResultType.True)
                    {
                        string callBackJsonString = JsonObject.GetCallBackJsonString(item.ObjectCode, item.DocEntry.ToString(), item.B1DocEntry, syncDateTime);
                        string callBackResultStr = await BaseHttpClient.HttpCallBackAsync(callBackJsonString);
                        var callBackResult = await JsonConvert.DeserializeObjectAsync<CallBackResult>(callBackResultStr);
                        if (callBackResult.ResultCode == 0)
                            mSuccessCount++;
                    }
                    Logger.Writer(guid, QueueStatus.Open, documentResult.ResultMessage);
                }
                catch (Exception ex)
                {
                    //单据编号用中文输入法下【】标记
                    Logger.Writer(guid, QueueStatus.Open, "【" + item.DocEntry + "】收款单处理发生异常：" + ex.Message);
                }
            }
            //其他数据用英文输入法下[]标记区别
            Logger.Writer(guid, QueueStatus.Close, "[" + mSuccessCount + "]条收款单处理成功。");
            #endregion
        }
    }
}
