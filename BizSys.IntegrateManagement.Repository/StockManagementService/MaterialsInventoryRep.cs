using BizSys.IntegrateManagement.Repository.BOneCommon;
using BizSys.IntegrateManagement.Entity.StockManagement.MaterialsInventory;
using BizSys.IntegrateManagement.IRepository.StockManagementService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Repository.StockManagementService
{
    public class MaterialsInventoryRep : IMaterialsInventoryRep
    {
        public Entity.StockManagement.MaterialsInventory.MaterialsInventoryRootObject GetAllMaterialsInventory()
        {
            
            MaterialsInventoryRootObject materialsInventoryRootObject = new MaterialsInventoryRootObject();
            materialsInventoryRootObject.ResultObjects = new List<ResultObjects>();
            try
            {
                SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string sql = @"select * from OITW where OnHand > 0";
                res.DoQuery(sql);
                while (!res.EoF)
                {
                    ResultObjects materialsInventory = new ResultObjects();
                    materialsInventory.ItemCode = res.Fields.Item("ItemCode").Value;
                    materialsInventory.WarehouseCode = res.Fields.Item("WhsCode").Value;
                    materialsInventory.OnHand = res.Fields.Item("OnHand").Value;
                    materialsInventory.AvgPrice = res.Fields.Item("AvgPrice").Value;
                    materialsInventory.IsCommited =res.Fields.Item("IsCommited").Value;
                    materialsInventory.OnOrder = res.Fields.Item("OnOrder").Value;
                    materialsInventory.CreateDate = res.Fields.Item("CreateDate").Value;
                   // materialsInventory.CreateTime = res.Fields.Item("CreateDate").Value;
                    materialsInventory.UpdateDate = res.Fields.Item("UpdateDate").Value;
                    //materialsInventory.UpdateTime = res.Fields.Item("UpdateDate").Value;
                    materialsInventory.LogInst = res.Fields.Item("logInstanc").Value;
                    materialsInventory.CreateUserSign = res.Fields.Item("UserSign").Value.ToString();
                    materialsInventory.UpdateUserSign = res.Fields.Item("UserSign2").Value.ToString();

                    materialsInventoryRootObject.ResultObjects.Add(materialsInventory);
                    res.MoveNext();
                }
                materialsInventoryRootObject.ResultCode = 0;
                materialsInventoryRootObject.Message = "Successful operation.";
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);

            }
            catch (Exception ex)
            {
                materialsInventoryRootObject.ResultCode = -1;
                materialsInventoryRootObject.Message = ex.Message;
            }
            return materialsInventoryRootObject;
        }
    }
}
