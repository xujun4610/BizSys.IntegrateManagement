using BizSys.IntegrateManagement.Entity.MasterDataManagement.Supplier;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.OmniChannelToSAP.Service.B1Common;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.OmniChannelToSAP.Service.Document.MasterDataManagement
{
    public class Supplier
    {
        public static Result CreateSupplier(ResultObjects Supplier)
        {
            Result result = new Result();

            SAPbobsCOM.BusinessPartners myBP = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
            bool IsExists = myBP.GetByKey(Supplier.SupplierCode);
            myBP.CardCode = Supplier.SupplierCode;
            myBP.CardName = Supplier.SupplierName;
            myBP.CardType = SAPbobsCOM.BoCardTypes.cSupplier;
            myBP.GroupCode = Convert.ToInt32(Supplier.GroupCode);
            myBP.ZipCode = Supplier.BillToZipCode;
            myBP.UserFields.Fields.Item("U_SysGen").Value = "01";
            myBP.Valid = Supplier.Activation == "Yes" ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
            myBP.ValidFrom = Convert.ToDateTime(Supplier.ActiveFrom);
            myBP.ValidTo = Convert.ToDateTime(Supplier.ActiveTo);
            myBP.Frozen = Supplier.Inactive == "No" ? BoYesNoEnum.tNO : BoYesNoEnum.tYES;
            myBP.FrozenFrom = Convert.ToDateTime(Supplier.InactiveFrom);
            myBP.FrozenTo = Convert.ToDateTime(Supplier.InactiveTo);
            myBP.EmailAddress = Supplier.Email;
            myBP.Cellular = Supplier.MobilePhone;
            myBP.Phone1 = Supplier.Telephone1;
            myBP.Phone2 = Supplier.Telephone2;
            myBP.Currency = Supplier.BPCurrency;
            myBP.FreeText = Supplier.Remarks;
            myBP.MailZipCode = Supplier.ShipToZipCode;
            myBP.Website = Supplier.WebSite;
            if (!string.IsNullOrEmpty(Supplier.InvoiceRecipient))
                myBP.UserFields.Fields.Item("U_InvoiceRecipient").Value = Supplier.InvoiceRecipient;
            if (!string.IsNullOrEmpty(Supplier.BusinessPartnerNature))
                myBP.UserFields.Fields.Item("U_CustomerType").Value = Supplier.BusinessPartnerNature;
            if (!string.IsNullOrEmpty(Supplier.CustomerLevel))
                myBP.UserFields.Fields.Item("U_CustLevel").Value = Supplier.CustomerLevel;
            if (!string.IsNullOrEmpty(Supplier.TaxNumber))
                myBP.GTSRegNo = Supplier.TaxNumber;
            myBP.GTSBillingAddrTel = Supplier.BillingAddress + '-' + Supplier.BillingTelephone;
            myBP.GTSBankAccountNo = Supplier.HouseBank + '-' + Supplier.Account;

            string addressName = Supplier.City + Supplier.County + Supplier.BillToStreet;
            
            myBP.Addresses.Delete();

            myBP.Addresses.AddressName = addressName;
            myBP.Addresses.City = Supplier.City;
            myBP.Addresses.County = Supplier.County;
            myBP.Addresses.Street = Supplier.BillToStreet;
            myBP.Addresses.State = B1Common.BOneCommon.GetAddressCode(Supplier.Province);
            myBP.Addresses.AddressType = BoAddressType.bo_ShipTo;
            myBP.Addresses.Add();
            //myBP.ContactPerson = Supplier.ContactPerson;
            //myBP.ContactEmployees.Delete();
            myBP.ContactEmployees.Name = Supplier.ContactPerson;
            myBP.ContactEmployees.Address = Supplier.BillToStreet;
            myBP.ContactEmployees.Fax = Supplier.FaxNumber;
            myBP.ContactEmployees.MobilePhone = Supplier.MobilePhone;
            myBP.ContactEmployees.Phone1 = Supplier.Telephone1;
            myBP.ContactEmployees.Phone2 = Supplier.Telephone2;
            myBP.ContactEmployees.CityOfBirth = Supplier.City;
            myBP.ContactEmployees.E_Mail = Supplier.Email;
            myBP.ContactEmployees.PlaceOfBirth = "CN";
            myBP.ContactEmployees.UserFields.Fields.Item("U_ZipCode").Value = Supplier.BillToZipCode;
            myBP.ContactEmployees.UserFields.Fields.Item("U_Province").Value = Supplier.Province;
            myBP.ContactEmployees.UserFields.Fields.Item("U_County").Value = Supplier.County;
            myBP.ContactEmployees.UserFields.Fields.Item("U_Website").Value = Supplier.WebSite;
            myBP.ContactEmployees.Add();

            int RntCode = 0;
            if (IsExists)
            {
                RntCode = myBP.Update();
            }
            else
            {
                RntCode = myBP.Add();
            }
            if (RntCode != 0)
            {
                result.ResultValue = ResultType.False;
                result.ResultMessage = string.Format("【{0}】供应商处理失败，ErrorCode:[{1}],ErrrMsg:[{2}];", Supplier.SupplierCode, SAP.SAPCompany.GetLastErrorCode(), SAP.SAPCompany.GetLastErrorDescription());
            }
            else
            {
                result.ResultValue = ResultType.True;
                result.ResultMessage = "【" + Supplier.SupplierCode.ToString() + "】供应商处理成功，系统数据：" + SAP.SAPCompany.GetNewObjectKey();
            }
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myBP);
            return result;
        }
    }
}
