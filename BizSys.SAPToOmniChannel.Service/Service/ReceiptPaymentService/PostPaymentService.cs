using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity;
using BizSys.IntegrateManagement.Entity.ReceiptPayment.Payment;
using BizSys.IntegrateManagement.Entity.Task;
using BizSys.IntegrateManagement.IRepository.ReceiptPaymentService;
using BizSys.IntegrateManagement.IRepository.Task;
using BizSys.IntegrateManagement.Repository.ReceiptPaymentService;
using BizSys.IntegrateManagement.Repository.Task;
using BizSys.SAPToOmniChannel.Service.B1Common;
using MagicBox.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.SAPToOmniChannel.Service.Service.ReceiptPaymentService
{
    public class PostPaymentService
    {
        public async static void PostPayment()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetPaymentsCount"], 30);
            string guid = "Payment-" + Guid.NewGuid();
            string resultJson = string.Empty;
            PaymentRootObject paymentRootObject = new PaymentRootObject();
            IPaymentRep receiptsRep = new PaymentRep();
            #region 查询任务表
            ITaskRep taskRep = new TaskRep();

            //=====================获取任务列表中未同步的付款单信息================================
            TaskRootObjects resultTasks = taskRep.GetDocumentWithNoSync<PaymentRootObject>(paymentRootObject, resultCount);

            if (resultTasks.ResultCode != 0)
            {
                Logger.Writer("从Task任务表中查询未同步付款单数据出错，" + resultTasks.Message);
                return;
            }
            if (resultTasks.ResultObjects.Count == 0) return;
            #endregion

            #region 遍历Task结果
            Logger.Writer(guid, QueueStatus.Open, "获取未同步付款单信息[" + resultTasks.ResultObjects.Count + "]条，正在同步付款单信息...");
            int mSuccessCount = 0;
            foreach (var item in resultTasks.ResultObjects)
            {
                string postJson = string.Empty;
                try
                {
                    paymentRootObject = receiptsRep.GetReceiptsByKey(item.UniqueKey);
                    if (paymentRootObject.ResultCode == 0 && paymentRootObject.ResultObjects.Count == 1)
                    {
                        var paymentOrder = paymentRootObject.ResultObjects.FirstOrDefault();
                        if (paymentOrder.Canceled == "Yes")
                        {
                            #region 查询付款单
                            #region 查询条件
                            Criteria cri = new Criteria()
                            {
                                __type = "Criteria",
                                ResultCount = 1,
                                isDbFieldName = false,
                                BusinessObjectCode = null,
                                Conditions = new List<Conditions>()
                                {
                                new Conditions(){
                                Alias = "DocEntry",
                                CondVal = item.DocEntry.ToString(),
                                Operation = "co_EQUAL"
                                }
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
                            #endregion
                            string questJson = await JsonConvert.SerializeObjectAsync(cri);
                            #region 调用查询接口
                            try
                            {
                                resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.PAYMENT, questJson);
                            }
                            catch (Exception ex)
                            {
                                Logger.Writer("付款单查询出错，错误信息：" + ex.Message);
                                return;
                            }
                            if (string.IsNullOrEmpty(resultJson)) Logger.Writer("付款单查询服务出错，查询结果为null。");
                            #endregion
                            var PaymentAsny = await JsonConvert.DeserializeObjectAsync<PaymentRootObject>(resultJson);
                            if(PaymentAsny.ResultCode ==0 && PaymentAsny.ResultObjects.Any())
                            {
                                var paymentPost = PaymentAsny.ResultObjects.FirstOrDefault();
                                paymentPost.isNew = false;
                                paymentPost.Canceled = "Yes";
                                postJson = JsonConvert.SerializeObject(paymentPost);
                            }
                            else
                            {
                                paymentOrder.isNew = true;
                                paymentOrder.ObjectCode = "AVA_RP_PAYMENT";
                                paymentOrder.type = "Payment";
                                postJson = JsonConvert.SerializeObject(paymentOrder);
                            }
                            #endregion

                        }
                        else
                        {
                            paymentOrder.isNew = true;
                            paymentOrder.ObjectCode = "AVA_RP_PAYMENT";
                            paymentOrder.type = "Payment";
                            postJson = JsonConvert.SerializeObject(paymentOrder);
                        }
                        
                        if (await ServiceCommon<IntegrateManagement.Entity.ReceiptPayment.Payment.ResultObjects, PaymentRootObject>.PostOrderWithSingleTable(postJson, guid, item, DocumentType.PAYMENT))
                            mSuccessCount++;
                    }
                    else
                    {
                        Logger.Writer(guid, QueueStatus.Open, "根据Task任务表中提供付款单主键信息查询出错。");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Writer(guid, QueueStatus.Open, $"同步付款单出现异常：{ex.InnerException}");
                }

            }
            #endregion
            Logger.Writer(guid, QueueStatus.Close, $"[{mSuccessCount}]付款单同步成功.");
        }
    }
}
