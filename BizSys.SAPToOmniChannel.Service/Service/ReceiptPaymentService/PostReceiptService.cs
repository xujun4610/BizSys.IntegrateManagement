using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity.ReceiptPayment.Receipt;
using BizSys.IntegrateManagement.Entity.Task;
using BizSys.IntegrateManagement.IRepository.ReceiptPaymentService;
using BizSys.IntegrateManagement.IRepository.Task;
using BizSys.IntegrateManagement.Repository.ReceiptPaymentService;
using BizSys.IntegrateManagement.Repository.Task;
using BizSys.SAPToOmniChannel.Service.B1Common;
using MagicBox.Log;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;

namespace BizSys.SAPToOmniChannel.Service.Service.ReceiptPaymentService
{
    public class PostReceiptService
    {
        /// <summary>
        /// 推送收款单
        /// </summary>
        public async static void PostReceipt()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetReceiptCount"], 30);
            string guid = "Receipt-" + Guid.NewGuid();
            string resultJson = string.Empty;
            ReceiptRootObject receiptsRootObject = new ReceiptRootObject();
            IReceiptsRep receiptsRep = new ReceiptsRep();
            #region 查询任务表
            ITaskRep taskRep = new TaskRep();

            //=====================获取任务列表中未同步的收款单信息================================
            TaskRootObjects resultTasks = taskRep.GetDocumentWithNoSync<ReceiptRootObject>(receiptsRootObject, resultCount);

            if (resultTasks.ResultCode != 0)
            {
                Logger.Writer("从Task任务表中查询未同步收款单数据出错，" + resultTasks.Message);
                return;
            }
            if (resultTasks.ResultObjects.Count == 0) return;
            #endregion

            #region 遍历Task结果
            Logger.Writer(guid, QueueStatus.Open, "获取未同步收款单信息[" + resultTasks.ResultObjects.Count + "]条，正在同步收款单信息...");
            int mSuccessCount = 0;
            foreach (var item in resultTasks.ResultObjects)
            {
                try
                {
                    receiptsRootObject = receiptsRep.GetReceiptsByKey(item.UniqueKey);
                    if (receiptsRootObject.ResultCode == 0 && receiptsRootObject.ResultObjects.Count == 1)
                    {
                        var receiptsOrder = receiptsRootObject.ResultObjects.FirstOrDefault();
                        receiptsOrder.isNew = true;
                        receiptsOrder.ObjectCode = "AVA_RP_RECEIPT";
                        receiptsOrder.type = "Receipt";

                        string postJson = JsonConvert.SerializeObject(receiptsOrder);
                        if (await ServiceCommon<IntegrateManagement.Entity.ReceiptPayment.Receipt.ResultObjects,ReceiptRootObject>.PostOrderWithSingleTable(postJson, guid, item, DocumentType.RECEIPT))
                            mSuccessCount++;
                    }
                    else
                    {
                        Logger.Writer(guid, QueueStatus.Open, "根据Task任务表中提供收款单主键信息查询出错。");
                    }
                }
                catch(Exception ex)
                {
                    Logger.Writer(guid, QueueStatus.Open, $"同步收款单出现异常：{ex.InnerException}");
                }
                
            }
            #endregion
            Logger.Writer(guid, QueueStatus.Close, $"[{mSuccessCount}]条收款单同步成功.");
        }
    }
}
