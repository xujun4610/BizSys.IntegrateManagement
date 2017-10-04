using BizSys.IntegrateManagement.Repository.BOneCommon;
using BizSys.IntegrateManagement.Entity.Task;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.Materials;
using BizSys.IntegrateManagement.IRepository.Task;
using System;
using System.Collections.Generic;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.Warehouse;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.MaterialsGroup;
using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.Employee;
using BizSys.IntegrateManagement.Entity.SalesManagement.SalesDeliveryOrder;
using BizSys.IntegrateManagement.Entity.PurchaseDeliveryOrder;
using BizSys.IntegrateManagement.Entity.SalesManagement.InvoiceOrder;
using BizSys.IntegrateManagement.Entity.ReceiptPayment.Payment;
using BizSys.IntegrateManagement.Entity.ReceiptPayment.Receipt;
using BizSys.IntegrateManagement.Entity.PurchaseManagement.PurchaseInvoice;

namespace BizSys.IntegrateManagement.Repository.Task
{
    public class TaskRep:ITaskRep
    {

        /// <summary>
        /// 查询未同步的单据
        /// BusinessType   单据类型
        ///    4            物料
        ///    13           应收发票
        ///    15           销售交货
        ///    20           采购收货
        ///    52           物料组
        ///    64           仓库
        ///    112          草稿
        ///    171          员工
        /// </summary>
        /// <param name="materialsCount"></param>
        /// <returns></returns>
        public Entity.Task.TaskRootObjects GetDocumentWithNoSync<T>(T Document,int Count)
        {
            TaskRootObjects documentTasks = new TaskRootObjects();
            try
            {
                string BusinessType=default(string);
                if(typeof(T) == typeof(MaterialsRootObject))
                    BusinessType = "4";
                if (typeof(T) == typeof(InvoiceOrderRootObject))
                    BusinessType = "13";
                else if (typeof(T) == typeof(SalesDeliveryOrderRootObject))
                    BusinessType = "15";
                else if (typeof(T) == typeof(PurchaseInvoiceRootObject))
                    BusinessType = "18";
                else if (typeof(T) == typeof(PurchaseDeliveryOrderRootObject))
                    BusinessType = "20";
                else if (typeof(T) == typeof(ReceiptRootObject))
                    BusinessType = "24";
                else if (typeof(T) == typeof(PaymentRootObject))
                    BusinessType = "46";
                else if (typeof(T) == typeof(MaterialsGroupRootObject))
                    BusinessType = "52";
                else if (typeof(T) == typeof(WarehouseRootObject))
                    BusinessType = "64";
                else if (typeof(T) == typeof(EmployeeRootObject))
                    BusinessType = "171";

                SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                if (string.IsNullOrEmpty(BusinessType)) throw new ArgumentException("单据对象不正确，无法确认所操作的单据类型。");
                string sql = @"select top {0} * from AVA_CZ_TASKLIST where IsSync = 'N' and BusinessType = '{1}' ";
                res.DoQuery(string.Format(sql,Count,BusinessType));
                documentTasks.ResultObjects = new List<Entity.Task.ResultObjects>();
                while (!res.EoF)
                {
                    Entity.Task.ResultObjects task = new Entity.Task.ResultObjects();
                    task.UniqueKey = res.Fields.Item("UniqueKey").Value;
                    task.DocEntry = res.Fields.Item("DocEntry").Value;
                    documentTasks.ResultObjects.Add(task);
                    res.MoveNext();
                }
                documentTasks.ResultCode = 0;
                documentTasks.Message = "Successful operation.";
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
            catch (Exception ex)
            {
                documentTasks.ResultCode = -1;
                documentTasks.Message = ex.Message;
            }

            return documentTasks;
        }

        public void UpdateDocumentOrderNo(string TableName, string DocEntry, string IMDocEntry)
        {

            try
            {
                string[] tableName = TableName.Split('-');
                SAPbobsCOM.Recordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string sql = $@"update {tableName[0]} set U_IM_DocEntry = '{IMDocEntry}' where DocEntry = {DocEntry}";
                string sqlLine = $@"update {tableName[1]} set U_IM_DocEntry = '{IMDocEntry}' where DocEntry = {DocEntry}";
                res.DoQuery(sql);
                res.DoQuery(sqlLine);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateDocumentWithSyncSucc(int DocEntry)
        {
            SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            string sql = @"update AVA_CZ_TASKLIST set IsSync = 'Y',SyncDate = '{0}',SyncTime = '{1}' where DocEntry = '{2}'";
            try
            {
                res.DoQuery(string.Format(sql, DateTime.Now,DataConvert.GetDateNowOfIntTime(),DocEntry));
            }
            catch(Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
