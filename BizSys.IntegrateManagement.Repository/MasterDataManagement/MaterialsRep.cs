using BizSys.IntegrateManagement.Entity.MasterDataManagement.Materials;
using BizSys.IntegrateManagement.MasterDataManagement.IRepository;
using BizSys.IntegrateManagement.Repository.BOneCommon;
using System;
using System.Collections.Generic;

namespace BizSys.IntegrateManagement.Repository.MasterDataManagement
{
    public class MaterialsRep : IMaterialsRep
    {
        private Dictionary<string, double> _DicVat;
        public Dictionary<string, double> DicVat
        {
            get
            {
                if (_DicVat == null)
                {
                    _DicVat = new Dictionary<string, double>();
                    _DicVat = GetVatList();
                }
                return _DicVat;
            }
            set
            {
                _DicVat = value;
            }
        }

        private Dictionary<string, double> GetVatList()
        {
            try
            {
                Dictionary<string, double> dicVat = new Dictionary<string, double>();
                  string sql = "select Code,Rate from OVTG";
                SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                res.DoQuery(sql);
                while(!res.EoF)
                {
                    dicVat.Add(res.Fields.Item("Code").Value, res.Fields.Item("Rate").Value);
                    res.MoveNext();
                }
                return dicVat;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public Entity.MasterDataManagement.Materials.MaterialsRootObject GetAllMaterials()
        {
            MaterialsRootObject materialsRootObject = new MaterialsRootObject();
            materialsRootObject.ResultObjects = new List<ResultObjects>();
            try
            {
                SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string sql = @"select * from OITM";
                res.DoQuery(sql);
                while (!res.EoF)
                {
                    ResultObjects material = new ResultObjects();
                    #region 属性赋值
                    material.ItemCode = res.Fields.Item("ItemCode").Value;
                    material.ItemDescription = res.Fields.Item("ItemName").Value;
                    material.Series = res.Fields.Item("Series").Value.ToString();
                    material.ForeignDescription = res.Fields.Item("FrgnName").Value;
                    material.ItemGroup = res.Fields.Item("ItmsGrpCod").Value;
                    material.CategoryCode = res.Fields.Item("U_CateCode").Value;
                    material.PreferredVendor = res.Fields.Item("CardCode").Value;
                    material.PurchaseItem = res.Fields.Item("PrchseItem").Value;
                    material.FixedAssets = res.Fields.Item("AssetItem").Value;
                    material.DefaultWarehouse = res.Fields.Item("DfltWH").Value;
                    material.PurchasingUoM = res.Fields.Item("BuyUnitMsr").Value;
                    material.NoOfItemsPerPurchaseUnit = res.Fields.Item("NumInBuy").Value;
                    material.Canceled = res.Fields.Item("Canceled").Value;
                    material.SalesUoM = res.Fields.Item("SalUnitMsr").Value;
                    material.SaleTax = DicVat[res.Fields.Item("SalesTax").Value];
                    material.PurchaseTax = DicVat[res.Fields.Item("PurchaseTax").Value];
                    material.NoOfItemsPerSalesUnit = res.Fields.Item("NumInSale").Value;
                    material.Remarks = res.Fields.Item("UserText").Value;
                    material.PhantomItem = res.Fields.Item("Phantom").Value;
                    material.InventoryUoM = res.Fields.Item("InvntryUom").Value;
                    material.ProcurementMethod = res.Fields.Item("PrcrmntMtd").Value;
                    material.OnOrder = res.Fields.Item("OnOrder").Value;
                    
                    material.ItemType = res.Fields.Item("ItemType").Value;
                    material.InventoryItem = res.Fields.Item("InvntItem").Value == "Y" ? "Yes" : "No";
                    material.SalesItem = res.Fields.Item("SellItem").Value == "Y" ? "Yes" : "No";
                    material.CreateUserSign = res.Fields.Item("UserSign").Value.ToString();
                    material.UpdateUserSign = res.Fields.Item("UserSign2").Value.ToString();
                    material.UpdateDate = res.Fields.Item("UpdateDate").Value;
                    material.UpdateTime = res.Fields.Item("UpdateTS").Value;
                    material.CreateDate = res.Fields.Item("CreateDate").Value;
                    material.CreateTime = res.Fields.Item("CreateTS").Value;
                    material.OnHand = res.Fields.Item("OnHand").Value.ToString();
                    material.AvgPrice = res.Fields.Item("AvgPrice").Value.ToString();
                    material.IsCommited = res.Fields.Item("IsCommited").Value.ToString();
                    material.LeadTime = res.Fields.Item("LeadTime").Value.ToString();
                    material.LogInst = res.Fields.Item("LogInstanc").Value;
                    material.BarCode = res.Fields.Item("CodeBars").Value.ToString();
                    material.ItemGroup = res.Fields.Item("ItmsGrpCod").Value.ToString();
                    material.Brand = res.Fields.Item("Brand").Value.ToString();
                    material.SalesUoM = res.Fields.Item("SalUnitMsr").Value;
                    material.Picture = res.Fields.Item("PicturName").Value;
                    material.CategoryName = res.Fields.Item("U_CateName").Value;
                    material.Brand = res.Fields.Item("U_Brand").Value;
                    material.Model = res.Fields.Item("U_Model").Value;
                    
                    material.CategoryCode = res.Fields.Item("U_CateCode").Value;
                    material.Length = res.Fields.Item("U_Length").Value.ToString();
                    material.Width = res.Fields.Item("U_Width").Value.ToString();
                    material.Height = res.Fields.Item("U_Height").Value.ToString();
                    material.Colour = res.Fields.Item("U_Colour").Value.ToString();
                    material.FactoryCode = res.Fields.Item("U_Factory").Value;
                    material.BatchNumberManagement = res.Fields.Item("ManBtchNum").Value == "Y" ? "Yes" : "No";
                    material.SerialNumberManagement = res.Fields.Item("ManSerNum").Value == "Y" ? "Yes" : "No";
                    material.Active = res.Fields.Item("ValidFor").Value;
                    material.ActiveFrom = res.Fields.Item("validFrom").Value.ToString();
                    material.ActiveTo = res.Fields.Item("validTo").Value.ToString();
                    material.PhantomItem = res.Fields.Item("Phantom").Value;
                    material.IssueMethod = res.Fields.Item("IssueMthd").Value;
                    material.PlanningMethod = res.Fields.Item("PlaningSys").Value;
                    material.MinimumOrderQuantity = res.Fields.Item("MinOrdrQty").Value;
                    material.MinimumInventoryLevel = res.Fields.Item("MinLevel").Value;
                    #endregion
                    materialsRootObject.ResultObjects.Add(material);
                    res.MoveNext();
                }
                materialsRootObject.ResultCode = 0;
                materialsRootObject.Message = "Successful operation.";
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
            catch (Exception ex)
            {
                materialsRootObject.ResultCode = -1;
                materialsRootObject.Message = ex.Message;
            }
            return materialsRootObject;

        }

        public Entity.MasterDataManagement.Materials.MaterialsRootObject GetMaterialsByKey(string ItemCode)
        {
            MaterialsRootObject materialsRootObject = new MaterialsRootObject();
            materialsRootObject.ResultObjects = new List<ResultObjects>();
            try
            {
                SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string sql = string.Format("select * from OITM where ItemCode = '{0}'", ItemCode);
                res.DoQuery(sql);
                if (res.RecordCount != 1)
                    return new MaterialsRootObject
                    {
                        ResultCode = -1,
                        Message = "Can not find the material in SAP."
                    };
                ResultObjects material = new ResultObjects();
                #region 物料属性赋值
                material.ItemCode = res.Fields.Item("ItemCode").Value;
                material.isNew = true;
                material.ItemDescription = res.Fields.Item("ItemName").Value;
                material.Series = res.Fields.Item("Series").Value.ToString();
                material.ForeignDescription = res.Fields.Item("FrgnName").Value;
                material.ItemGroup = res.Fields.Item("ItmsGrpCod").Value.ToString();
                material.PurchaseItem = res.Fields.Item("PrchseItem").Value;
                material.ItemType = res.Fields.Item("ItemType").Value;
                material.FixedAssets = res.Fields.Item("AssetItem").Value;
                material.DefaultWarehouse = res.Fields.Item("DfltWH").Value;
                material.PreferredVendor = res.Fields.Item("CardCode").Value;
                material.PurchasingUoM = res.Fields.Item("BuyUnitMsr").Value;
                material.NoOfItemsPerPurchaseUnit = res.Fields.Item("NumInBuy").Value;
                material.Canceled = res.Fields.Item("Canceled").Value;
                material.Remarks = res.Fields.Item("UserText").Value;
                material.SaleTax = DicVat[res.Fields.Item("VatGourpSa").Value];
                material.NoOfItemsPerSalesUnit = res.Fields.Item("NumInSale").Value;
                material.PurchaseTax = DicVat[res.Fields.Item("VatGroupPu").Value];
                material.InventoryUoM = res.Fields.Item("InvntryUom").Value;
                material.PlanningMethod = res.Fields.Item("PlaningSys").Value;
                material.ProcurementMethod = res.Fields.Item("PrcrmntMtd").Value;
                material.LeadTime = res.Fields.Item("LeadTime").Value.ToString();
                material.OnOrder = res.Fields.Item("OnOrder").Value;
                material.InventoryItem = res.Fields.Item("InvntItem").Value == "Y" ? "Yes" : "No";
                material.SalesItem = res.Fields.Item("SellItem").Value == "Y" ? "Yes" : "No";
                material.CreateUserSign = res.Fields.Item("UserSign").Value.ToString();
                material.UpdateUserSign = res.Fields.Item("UserSign2").Value.ToString();
                material.UpdateDate = res.Fields.Item("UpdateDate").Value;
                material.UpdateTime = res.Fields.Item("UpdateTS").Value;
                material.CreateDate = res.Fields.Item("CreateDate").Value;
                material.CreateTime = res.Fields.Item("CreateTS").Value;
                material.OnHand = res.Fields.Item("OnHand").Value.ToString();
                material.AvgPrice = res.Fields.Item("AvgPrice").Value.ToString();
                material.IsCommited = res.Fields.Item("IsCommited").Value.ToString();
                material.LeadTime = res.Fields.Item("LeadTime").Value.ToString();
                material.LogInst = res.Fields.Item("LogInstanc").Value + 1;
                material.BarCode = res.Fields.Item("CodeBars").Value.ToString();
                material.ItemGroup = res.Fields.Item("ItmsGrpCod").Value.ToString();
                material.SalesUoM = res.Fields.Item("SalUnitMsr").Value;
                material.Picture = res.Fields.Item("PicturName").Value;
                material.Brand = res.Fields.Item("U_Brand").Value;
                material.Model = res.Fields.Item("U_Model").Value;
                material.FactoryCode = res.Fields.Item("U_FactoryCode").Value;
                material.CategoryCode = res.Fields.Item("U_CateCode").Value;
                material.CategoryName = res.Fields.Item("U_CateName").Value;
                material.Length = res.Fields.Item("U_Length").Value.ToString();
                material.Width = res.Fields.Item("U_Width").Value.ToString();
                material.Height = res.Fields.Item("U_Height").Value.ToString();
                material.Colour = res.Fields.Item("U_Colour").Value.ToString();
                material.BatchNumberManagement = res.Fields.Item("ManBtchNum").Value == "Y" ? "Yes" : "No";
                material.SerialNumberManagement = res.Fields.Item("ManSerNum").Value == "Y" ? "Yes" : "No";
                material.Active = res.Fields.Item("ValidFor").Value == "Y" ? "Yes" : "No";
                material.ActiveFrom = res.Fields.Item("validFrom").Value.ToString();
                material.ActiveTo = res.Fields.Item("validTo").Value.ToString();
                material.PhantomItem = res.Fields.Item("Phantom").Value == "Y" ? "Yes" : "No";
                material.IssueMethod = res.Fields.Item("IssueMthd").Value;
                material.PlanningMethod = res.Fields.Item("PlaningSys").Value;
                material.MinimumOrderQuantity = res.Fields.Item("MinOrdrQty").Value.ToString();
                material.MinimumInventoryLevel = res.Fields.Item("MinLevel").Value.ToString();
                material.AssemblyItem = "No";
                #endregion
                materialsRootObject.ResultObjects.Add(material);
                materialsRootObject.ResultCode = 0;
                materialsRootObject.Message = "Successful operation.";
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
            catch (Exception ex)
            {
                materialsRootObject.ResultCode = -1;
                materialsRootObject.Message = ex.Message;
            }

            return materialsRootObject;
        }
    }
}
