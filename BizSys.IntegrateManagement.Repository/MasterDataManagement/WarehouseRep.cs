using BizSys.IntegrateManagement.Repository.BOneCommon;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.Warehouse;
using BizSys.IntegrateManagement.IRepository.MasterDataManagement;
using System;
using System.Collections.Generic;

namespace BizSys.IntegrateManagement.Repository.MasterDataManagement
{
    public class WarehouseRep : IWarehouseRep
    {
        public Entity.MasterDataManagement.Warehouse.WarehouseRootObject GetAllWarehourse()
        {
            WarehouseRootObject warehouseRootObject = new WarehouseRootObject();
            warehouseRootObject.ResultObjects = new List<ResultObjects>();
            try
            {
                SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string sql = @"select * from OWHS";
                res.DoQuery(sql);
                while (!res.EoF)
                {
                    ResultObjects warehouse = new ResultObjects();
                    warehouse.WarehouseCode = res.Fields.Item("WhsCode").Value;
                    warehouse.WarehouseName = res.Fields.Item("WhsName").Value;
                    warehouse.WhsType = res.Fields.Item("U_WhsType").Value;
                    warehouse.CreateDate = res.Fields.Item("CreateDate").Value.ToString();
                    warehouse.UpdateDate = res.Fields.Item("UpdateDate").Value.ToString();
                    
                    warehouse.DataOwner = res.Fields.Item("OwnerCode").Value;
                    warehouse.Activated = res.Fields.Item("Inactive").Value;
                    warehouse.Workload = res.Fields.Item("BinActivat").Value;

                    warehouse.LogInst = res.Fields.Item("logInstanc").Value;
                    warehouse.CreateUserSign = res.Fields.Item("UserSign").Value;
                    warehouse.UpdateUserSign = res.Fields.Item("UserSign2").Value;

                    warehouseRootObject.ResultObjects.Add(warehouse);
                    res.MoveNext();
                }
                warehouseRootObject.ResultCode = 0;

                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
            catch (Exception ex)
            {
                warehouseRootObject.ResultCode = -1;
                warehouseRootObject.Message = ex.Message;
            }
            return warehouseRootObject;
        }

        public Entity.MasterDataManagement.Warehouse.WarehouseRootObject GetWhsByKey(string whsCode)
        {

            WarehouseRootObject warehouseRootObject = new WarehouseRootObject();
            warehouseRootObject.ResultObjects = new List<ResultObjects>();
            try
            {
                SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string sql = string.Format(@"select * from OWHS where WhsCode = '{0}'", whsCode);
                res.DoQuery(sql);
                if (res.RecordCount != 1)
                    return new WarehouseRootObject()
                    {
                        ResultCode = -1,
                        Message = "Can't found the warehouse in SAP."
                    };
                warehouseRootObject.ResultCode = 0;
                warehouseRootObject.Message = "操作成功！";
                ResultObjects warehouse = new ResultObjects();
                #region 物料属性赋值
                warehouse.WarehouseCode = res.Fields.Item("WhsCode").Value;
                warehouse.WarehouseName = res.Fields.Item("WhsName").Value;
                warehouse.WhsType = res.Fields.Item("U_WhsType").Value;
                warehouse.CreateDate = res.Fields.Item("CreateDate").Value.ToString();
                warehouse.UpdateDate = res.Fields.Item("UpdateDate").Value.ToString();
                warehouse.DataOwner = res.Fields.Item("OwnerCode").Value;
                warehouse.Activated = res.Fields.Item("Inactive").Value;
                warehouse.Workload = res.Fields.Item("BinActivat").Value;

                warehouse.LogInst = res.Fields.Item("logInstanc").Value;
                warehouse.CreateUserSign = res.Fields.Item("UserSign").Value.ToString();
                warehouse.UpdateUserSign = res.Fields.Item("UserSign2").Value.ToString();

                #endregion

                warehouseRootObject.ResultObjects.Add(warehouse);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
            catch (Exception ex)
            {
                warehouseRootObject.ResultCode = -1;
                warehouseRootObject.Message = ex.Message;
            }
            return warehouseRootObject;

        }
    }
}
