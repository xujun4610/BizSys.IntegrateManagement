using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity.SalesManagement.InvoiceOrder;
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
    public class PostInvoiceService
    {
        public async static void PostInvoice()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetInvoiceCount"], 3);
            string guid = "Invoice-" + Guid.NewGuid();
            string resultJson = string.Empty;

            InvoiceOrderRootObject invoiceOrderRootObject = new InvoiceOrderRootObject();
            IInvoiceOrderRep invoiceOrderRep = new InvoiceOrderRep();
            #region 查询任务表
            ITaskRep taskRep = new TaskRep();

            //=====================获取任务列表中未同步的应收发票信息================================
            TaskRootObjects resultTasks = taskRep.GetDocumentWithNoSync<InvoiceOrderRootObject>(invoiceOrderRootObject, resultCount);

            if (resultTasks.ResultCode != 0)
            {
                Logger.Writer("从Task任务表中查询未同步应收发票数据出错，" + resultTasks.Message);
                return;
            }
            if (resultTasks.ResultObjects.Count == 0) return;
            #endregion

            #region 遍历Task结果
            Logger.Writer(guid, QueueStatus.Open, "获取未同步应收发票信息[" + resultTasks.ResultObjects.Count + "]条，正在同步应收发票信息...");
            int mSuccessCount = 0;
            foreach (var item in resultTasks.ResultObjects)
            {
                try
                {
                    invoiceOrderRootObject = invoiceOrderRep.GetInvoiceByKey(DataConvert.ConvertToIntEx(item.UniqueKey));
                    if (invoiceOrderRootObject.ResultCode == 0 && invoiceOrderRootObject.ResultObjects.Count == 1)
                    {
                        var invoice = invoiceOrderRootObject.ResultObjects.FirstOrDefault();
                        invoice.type = "Receivable";
                        invoice.isNew = true;
                        invoice.ObjectCode = "AVA_SM_OINV";
                        string postJson = JsonConvert.SerializeObject(invoice);
                        if (await ServiceCommon<IntegrateManagement.Entity.SalesManagement.InvoiceOrder.ResultObjects, InvoiceOrderRootObject>.PostOrder(postJson, guid, item, DocumentType.INVOICE))
                            mSuccessCount++;
                    }
                    else
                    {
                        Logger.Writer(guid, QueueStatus.Open, "根据Task任务表中提供应收发票主键信息查询出错。");
                    }
                }
                catch(Exception ex)
                {
                    Logger.Writer(guid, QueueStatus.Open, $"同步应收发票出现异常：{ex.InnerException}");
                }
                
            }
            #endregion
            Logger.Writer(guid, QueueStatus.Close, "[" + mSuccessCount + "]条应收发票同步成功.");

        }
    }
}
