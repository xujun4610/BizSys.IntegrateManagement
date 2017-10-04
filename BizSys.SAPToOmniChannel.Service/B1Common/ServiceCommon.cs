using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity.Base;
using BizSys.IntegrateManagement.Repository.Task;
using BizSys.SAPToOmniChannel.Service.B1Common;
using MagicBox.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BizSys.SAPToOmniChannel.Service.B1Common
{
    public class ServiceCommon<IT, ID> : IServiceCommon<IT,ID>  where ID : IBaseRootObjects<IT> where IT : IBaseResultObjects
    {
        public async static Task<bool> PostOrder(string postJson, string guid, IntegrateManagement.Entity.Task.ResultObjects order,DocumentType documentType)
        {
            string documentName = guid.Substring(0, guid.IndexOf('-'));
            TaskRep taskRep = new TaskRep();
            ErrorRecordRep errorRecordRep = new ErrorRecordRep();
            string resultJson = string.Empty;
            try
            {
                resultJson = await BaseHttpClient.HttpSaveAsync(documentType, postJson);
            }
            catch (Exception ex)
            {
                Logger.Writer($"{documentName}保存出错，错误信息：" + ex.Message);
                return false;
            }

            //反序列化Save方法返回的的json字符串
            var saveOrderSync = JsonConvert.DeserializeObject<ID>(resultJson);
           
            if (saveOrderSync.ResultCode != 0)
            {
                Logger.Writer(guid, QueueStatus.Open, $"任务【{  order.DocEntry  }】，{ documentName}【{  order.UniqueKey  }】信息推送失败.失败原因：{saveOrderSync.Message}");
                return false;
            }
            else
            {
                taskRep.UpdateDocumentOrderNo(GetTableName(documentName), order.UniqueKey.ToString(), saveOrderSync.ResultObjects.FirstOrDefault().DocEntry.ToString());
                Logger.Writer(guid, QueueStatus.Open, $"任务【{  order.DocEntry  }】，{ documentName}【{  order.UniqueKey  }】信息推送成功.");
                if (taskRep.UpdateDocumentWithSyncSucc(order.DocEntry))
                    return true;
                else
                {
                    Logger.Writer(guid, QueueStatus.Open, $"任务【{  order.DocEntry  }】，{ documentName}【{  order.UniqueKey  }】信息更新状态失败。");
                    return false;
                }
            }
        }

        /// <summary>
        /// 非主子表的推送
        /// </summary>
        /// <param name="postJson"></param>
        /// <param name="guid"></param>
        /// <param name="order"></param>
        /// <param name="documentType"></param>
        /// <returns></returns>
        public async static Task<bool> PostOrderWithSingleTable(string postJson, string guid, IntegrateManagement.Entity.Task.ResultObjects order, DocumentType documentType)
        {
            string documentName = guid.Substring(0, guid.IndexOf('-'));
            TaskRep taskRep = new TaskRep();
            ErrorRecordRep errorRecordRep = new ErrorRecordRep();
            string resultJson = string.Empty;
            try
            {
                resultJson = await BaseHttpClient.HttpSaveAsync(documentType, postJson);
            }
            catch (Exception ex)
            {
                Logger.Writer($"{documentName}保存出错，错误信息：" + ex.Message);
                return false;
            }

            //反序列化Save方法返回的的json字符串
            var saveOrderSync = JsonConvert.DeserializeObject<ID>(resultJson);

            if (saveOrderSync.ResultCode != 0)
            {
                Logger.Writer(guid, QueueStatus.Open, $"任务【{  order.DocEntry  }】，{ documentName}【{  order.UniqueKey  }】信息推送失败.失败原因：{saveOrderSync.Message}");
                return false;
            }
            else
            {
                //taskRep.UpdateDocumentOrderNo(GetTableName(documentName), order.UniqueKey.ToString(), saveOrderSync.ResultObjects.FirstOrDefault().DocEntry.ToString());
                Logger.Writer(guid, QueueStatus.Open, $"任务【{  order.DocEntry  }】，{ documentName}【{  order.UniqueKey  }】信息推送成功.");
                if (taskRep.UpdateDocumentWithSyncSucc(order.DocEntry))
                    return true;
                else
                {
                    Logger.Writer(guid, QueueStatus.Open, $"任务【{  order.DocEntry  }】，{ documentName}【{  order.UniqueKey  }】信息更新状态失败。");
                    return false;
                }
            }
        }

        public static string GetTableName(string OrderType)
        {
            switch(OrderType)
            {
                case "Invoice":return "OINV-INV1";
                case "SalesDeliveryOrder":return "ODLN-DLN1";
                case "PurchaseDeliveryOrder":return "OPDN-PDN1";
                case "Receipt":return "ORCT";
                case "PurchaseInvoice":return "OPCH-PCH1";
                default:break;

            }
            throw new Exception("can't find the Table by OrderType.");

        }
    }
}
