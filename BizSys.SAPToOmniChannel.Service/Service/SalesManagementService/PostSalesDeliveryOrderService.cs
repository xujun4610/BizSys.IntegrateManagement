using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity.SalesManagement.SalesDeliveryOrder;
using BizSys.IntegrateManagement.Entity.Task;
using BizSys.IntegrateManagement.IRepository.SalesManagementService;
using BizSys.IntegrateManagement.IRepository.Task;
using BizSys.IntegrateManagement.Repository.SalesManagementService;
using BizSys.IntegrateManagement.Repository.Task;
using BizSys.SAPToOmniChannel.Service.B1Common;
using MagicBox.Log;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;

namespace BizSys.SAPToOmniChannel.Service.Service.SalesManagementService
{
    public class PostSalesDeliveryOrderService
    {
        public async static void PostSalesDeliveryOrder()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetSalesDeliveryOrderCount"], 30);
            string guid = "SalesDeliveryOrder-" + Guid.NewGuid();
            string resultJson = string.Empty;

            SalesDeliveryOrderRootObject salesDeliveryOrderRootObject = new SalesDeliveryOrderRootObject();
            ISalesDeliveryOrderRep salesDeliveryOrderRep = new SalesDeliveryOrderRep();
            #region 查询任务表
            ITaskRep taskRep = new TaskRep();

            //=====================获取任务列表中未同步的销售交货单信息================================
            TaskRootObjects resultTasks = taskRep.GetDocumentWithNoSync<SalesDeliveryOrderRootObject>(salesDeliveryOrderRootObject, resultCount);

            if (resultTasks.ResultCode != 0)
            {
                Logger.Writer("从Task任务表中查询未同步销售交货单数据出错，" + resultTasks.Message);
                return;
            }
            if (resultTasks.ResultObjects.Count == 0) return;
            #endregion

            #region 遍历Task结果
            Logger.Writer(guid, QueueStatus.Open, "获取未同步销售交货单信息[" + resultTasks.ResultObjects.Count + "]条，正在同步销售交货单信息...");
            int mSuccessCount = 0;
            foreach (var item in resultTasks.ResultObjects)
            {
                try
                {
                    salesDeliveryOrderRootObject = salesDeliveryOrderRep.GetSalesDeliveryOrderByKey(item.UniqueKey);
                    if (salesDeliveryOrderRootObject.ResultCode == 0 && salesDeliveryOrderRootObject.ResultObjects.Count == 1)
                    {
                        var salesDeliveryOrder = salesDeliveryOrderRootObject.ResultObjects.FirstOrDefault();
                        salesDeliveryOrder.type = "SalesDelivery";
                        salesDeliveryOrder.isNew = true;
                        salesDeliveryOrder.ObjectCode = "AVA_SM_SALESDELIVERY";
                        string postJson = JsonConvert.SerializeObject(salesDeliveryOrder);
                        Logger.Writer(guid, QueueStatus.Open, $"推送交货单数据:\r\n{postJson}");
                        if (await ServiceCommon<IntegrateManagement.Entity.SalesManagement.SalesDeliveryOrder.ResultObjects,SalesDeliveryOrderRootObject>.PostOrder(postJson, guid, item, DocumentType.SALESDELIVERYORDER))
                            mSuccessCount++;
                    }
                    else
                    {
                        Logger.Writer(guid, QueueStatus.Open, "根据Task任务表中提供销售交货单主键信息查询出错。");
                    }
                }
                catch(Exception ex)
                {
                    Logger.Writer(guid, QueueStatus.Open, $"同步销售交货出现异常：{ex.InnerException}");
                }
               
            }
            #endregion
            Logger.Writer(guid, QueueStatus.Close, $"[{mSuccessCount}]条销售交货同步成功.");


        }
    }
}
