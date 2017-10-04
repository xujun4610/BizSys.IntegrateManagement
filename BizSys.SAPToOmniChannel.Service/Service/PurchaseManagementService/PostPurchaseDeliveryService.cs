using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity.PurchaseDeliveryOrder;
using BizSys.IntegrateManagement.Entity.Task;
using BizSys.IntegrateManagement.IRepository.PurchaseManagement;
using BizSys.IntegrateManagement.IRepository.Task;
using BizSys.IntegrateManagement.Repository.PurchaseManagement;
using BizSys.IntegrateManagement.Repository.Task;
using BizSys.SAPToOmniChannel.Service.B1Common;
using MagicBox.Log;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Linq;

namespace BizSys.SAPToOmniChannel.Service.Service.PurchaseManagementService
{
    public class PostPurchaseDeliveryService
    {
        public static async void PostPurchaseDelivery()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetPurchaseDeliveryOrder"], 30);
            string guid = "PurchaseDeliveryOrder-" + Guid.NewGuid();
            string resultJson = string.Empty;
            PurchaseDeliveryOrderRootObject purchaseDeliveryOrderRootObject = new PurchaseDeliveryOrderRootObject();
            IPurchaseDeliveryRep purchaseDeliveryRep = new PurchaseDeliveryRep();
            ErrorRecordRep errorRecordRep = new ErrorRecordRep();
            #region 查询任务表
            ITaskRep taskRep = new TaskRep();

            //=====================获取任务列表中未同步的采购收货单信息================================
            TaskRootObjects resultTasks = taskRep.GetDocumentWithNoSync<PurchaseDeliveryOrderRootObject>(purchaseDeliveryOrderRootObject, resultCount);

            if (resultTasks.ResultCode != 0)
            {
                Logger.Writer("从Task任务表中查询未同步采购收货单数据出错，" + resultTasks.Message);
                return;
            }
            if (resultTasks.ResultObjects.Count == 0) return;
            #endregion
            #region 遍历Task结果
            Logger.Writer(guid, QueueStatus.Open, "获取未同步采购收货单信息[" + resultTasks.ResultObjects.Count + "]条，正在同步采购收货单信息...");
            int mSuccessCount = 0;
            foreach (var item in resultTasks.ResultObjects)
            {
                try
                {
                    purchaseDeliveryOrderRootObject = purchaseDeliveryRep.GetPurchaseDeliveryOrderByKey(item.UniqueKey);
                    if (purchaseDeliveryOrderRootObject.ResultCode == 0 && purchaseDeliveryOrderRootObject.ResultObjects.Count == 1)
                    {
                        var purchaseDeliveryOrder = purchaseDeliveryOrderRootObject.ResultObjects.FirstOrDefault();
                        purchaseDeliveryOrder.isNew = true;
                        purchaseDeliveryOrder.type = "PurchaseDelivery";
                        purchaseDeliveryOrder.ObjectCode = "AVA_PM_PURCHASEDELIVERY";
                        string postJson = JsonConvert.SerializeObject(purchaseDeliveryOrder);
                        if (await ServiceCommon<IntegrateManagement.Entity.PurchaseDeliveryOrder.ResultObjects,PurchaseDeliveryOrderRootObject>.PostOrder(postJson, guid, item, DocumentType.PURCHASEDELIVERYORDER))
                            mSuccessCount++;
                    }
                    else
                    {
                        Logger.Writer(guid, QueueStatus.Open, "根据Task任务表中提供采购收货单主键信息查询出错。");
                    }
                }
                catch(Exception ex)
                {
                    Logger.Writer(guid, QueueStatus.Open, $"采购订单同步出现异常：{ex.InnerException}");
                }
                
            }
            #endregion
            Logger.Writer(guid, QueueStatus.Close, $"[{mSuccessCount}]条采购收货推送成功.");

        }
    }
}
