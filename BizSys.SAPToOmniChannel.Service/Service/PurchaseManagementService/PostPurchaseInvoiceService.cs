using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity.PurchaseManagement.PurchaseInvoice;
using BizSys.IntegrateManagement.Entity.Task;
using BizSys.IntegrateManagement.IRepository.PurchaseManagement;
using BizSys.IntegrateManagement.IRepository.Task;
using BizSys.IntegrateManagement.Repository.PurchaseManagement;
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

namespace BizSys.SAPToOmniChannel.Service.Service.PurchaseManagementService
{
   public class PostPurchaseInvoiceService
    {
        public static async void PostPurchaseInvoice()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetPurchaseInvoice"], 30);
            string guid = "PurchaseInvoice-" + Guid.NewGuid();
            string resultJson = string.Empty;
            PurchaseInvoiceRootObject purchaseDeliveryOrderRootObject = new PurchaseInvoiceRootObject();
            IPurchaseInvoiceRep purchaseInvoiceRep = new PurchaseInvoiceRep();
            ErrorRecordRep errorRecordRep = new ErrorRecordRep();
            #region 查询任务表
            ITaskRep taskRep = new TaskRep();

            //=====================获取任务列表中未同步的采购收货单信息================================
            TaskRootObjects resultTasks = taskRep.GetDocumentWithNoSync<PurchaseInvoiceRootObject>(purchaseDeliveryOrderRootObject, resultCount);

            if (resultTasks.ResultCode != 0)
            {
                Logger.Writer("从Task任务表中查询未同步应付发票数据出错，" + resultTasks.Message);
                return;
            }
            if (resultTasks.ResultObjects.Count == 0) return;
            #endregion
            #region 遍历Task结果
            Logger.Writer(guid, QueueStatus.Open, "获取未同步应付发票信息[" + resultTasks.ResultObjects.Count + "]条，正在同步应付发票信息...");
            int mSuccessCount = 0;
            foreach (var item in resultTasks.ResultObjects)
            {
                try
                {
                    purchaseDeliveryOrderRootObject = purchaseInvoiceRep.GetPurchaseInvoiceByKey(item.UniqueKey);
                    if (purchaseDeliveryOrderRootObject.ResultCode == 0 && purchaseDeliveryOrderRootObject.ResultObjects.Count == 1)
                    {
                        var purchaseDeliveryOrder = purchaseDeliveryOrderRootObject.ResultObjects.FirstOrDefault();
                        purchaseDeliveryOrder.isNew = true;
                        purchaseDeliveryOrder.type = "Payable";
                        purchaseDeliveryOrder.ObjectCode = "AVA_PM_OPCH";
                        string postJson = JsonConvert.SerializeObject(purchaseDeliveryOrder);
                        if (await ServiceCommon<IntegrateManagement.Entity.PurchaseManagement.PurchaseInvoice.ResultObjects, PurchaseInvoiceRootObject>.PostOrder(postJson, guid, item, DocumentType.PAYABLE))
                            mSuccessCount++;
                    }
                    else
                    {
                        Logger.Writer(guid, QueueStatus.Open, "根据Task任务表中提供应付发票主键信息查询出错。");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Writer(guid, QueueStatus.Open, $"应付发票同步出现异常：{ex.InnerException}");
                }

            }
            #endregion
            Logger.Writer(guid, QueueStatus.Close, $"[{mSuccessCount}]条应付发票推送成功.");
        }
    }
}
