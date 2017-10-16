using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.Materials;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.OmniChannelToSAP.Service.B1Common;

namespace BizSys.OmniChannelToSAP.Service.Document.MasterDataManagement
{
    public class Material
    {
        public static Result CreateMaterial(ResultObjects material)
        {
            
            Result result = new Result();

            SAPbobsCOM.Items myMaterial = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems);
            bool IsExists = myMaterial.GetByKey(material.ItemCode);

            myMaterial.ItemType = SAPbobsCOM.ItemTypeEnum.itItems;
            myMaterial.ItemCode = material.ItemCode;
            myMaterial.ItemName = material.ItemDescription;
            myMaterial.ForeignName = material.ForeignDescription;
            myMaterial.ItemsGroupCode = Convert.ToInt32(material.CategoryCode);

            myMaterial.PurchaseItemsPerUnit = material.NoOfItemsPerPurchaseUnit <= 0 ? 1: material.NoOfItemsPerPurchaseUnit;
            myMaterial.PurchaseUnit =  material.PurchasingUoM;
            myMaterial.SalesUnit = material.SalesUoM;
            myMaterial.SalesItemsPerUnit = material.NoOfItemsPerSalesUnit <= 0? 1: material.NoOfItemsPerSalesUnit;
            myMaterial.SalesVATGroup = BOneCommon.GetTaxByRate(material.SaleTax, "O");
            myMaterial.PurchaseVATGroup = BOneCommon.GetTaxByRate(material.PurchaseTax, "I");
            myMaterial.DefaultWarehouse = material.DefaultWarehouse;
            //首选供应商
            SAPbobsCOM.Items_PreferredVendors pv = myMaterial.PreferredVendors;
            for (int i = 0; i < pv.Count; i++)
            {
                pv.SetCurrentLine(i);
                if (!material.PreferredVendor.Equals(pv.BPCode)){
                    pv.BPCode = material.PreferredVendor;
                    pv.Add();
                }
            }

            myMaterial.Valid = material.Active == "Yes" ? SAPbobsCOM.BoYesNoEnum.tYES : SAPbobsCOM.BoYesNoEnum.tNO;
            myMaterial.ValidFrom = Convert.ToDateTime(material.ActiveFrom);
            myMaterial.ValidTo = Convert.ToDateTime(material.ActiveTo);
            //myMaterial.Frozen = material.Inactive == "No" ? BoYesNoEnum.tNO : BoYesNoEnum.tYES;
            //myMaterial.FrozenFrom = Convert.ToDateTime(material.InactiveFrom);
            //myMaterial.FrozenTo = Convert.ToDateTime(material.InactiveTo);

            //if(!string.IsNullOrEmpty(material.TaxNumber))
            //myMaterial.GTSRegNo = material.TaxNumber;
            //myMaterial.GTSBillingAddrTel =  material.BillingAddress + '-' + material.BillingTelephone;
            //myMaterial.GTSBankAccountNo =  material.HouseBank + '-' + material.Account;

            int RntCode = 0;
            if (IsExists)
            {
                RntCode = myMaterial.Update();
            }
            else
            {
                RntCode = myMaterial.Add();
            }
            if (RntCode != 0)
            {
                result.ResultValue = ResultType.False;
                result.ResultMessage = string.Format("【{0}】物料处理失败，ErrorCode:[{1}],ErrrMsg:[{2}];", material.ItemCode, SAP.SAPCompany.GetLastErrorCode(), SAP.SAPCompany.GetLastErrorDescription());
            }
            else
            {
                result.ResultValue = ResultType.True;
                result.DocEntry = SAP.SAPCompany.GetNewObjectKey();
                result.ResultMessage = "【" + material.ItemCode + "】物料处理成功，系统数据：" + result.DocEntry;
            }
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myMaterial);
            return result;

        }
    }
}
