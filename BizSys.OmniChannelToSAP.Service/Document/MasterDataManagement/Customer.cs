using BizSys.IntegrateManagement.Entity.MasterDataManagement.Customer;
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
    public class Customer
    {
        public static Result CreateCustomer(ResultObjects customer)
        {
            Result result = new Result();

            SAPbobsCOM.BusinessPartners myBP = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
            bool IsExists = myBP.GetByKey(customer.CustomerCode);

            myBP.CardType = SAPbobsCOM.BoCardTypes.cCustomer;
            myBP.CardCode = customer.CustomerCode;
            myBP.CardName = customer.CustomerName;

            //myBP.GroupCode = customer.GroupCode;
            myBP.ZipCode = customer.BillToZipCode;
            myBP.EmailAddress = customer.Email;
            myBP.CreditLimit = customer.PaidToCredit;
            myBP.UserFields.Fields.Item("U_SysGen").Value = "01";
            myBP.Valid = customer.Activation == "Yes" ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
            myBP.ValidFrom = Convert.ToDateTime(customer.ActiveFrom);
            myBP.ValidTo = Convert.ToDateTime(customer.ActiveTo);
            myBP.Frozen = customer.Inactive == "No" ? BoYesNoEnum.tNO : BoYesNoEnum.tYES;
            myBP.FrozenFrom = Convert.ToDateTime(customer.InactiveFrom);
            myBP.FrozenTo = Convert.ToDateTime(customer.InactiveTo);
            myBP.Cellular = customer.MobilePhone;
            myBP.Phone1 = customer.Telephone1;
            myBP.Phone2 = customer.Telephone2;
            myBP.Currency = customer.BPCurrency;
            myBP.FreeText = customer.Remarks;

            if (!string.IsNullOrEmpty(customer.InvoiceRecipient))
                myBP.UserFields.Fields.Item("U_InvoiceRecipient").Value = customer.InvoiceRecipient;
            if (!string.IsNullOrEmpty(customer.BusinessPartnerNature))
                myBP.UserFields.Fields.Item("U_CustomerType").Value = customer.BusinessPartnerNature;
            if (!string.IsNullOrEmpty(customer.CustomerLevel))
                myBP.UserFields.Fields.Item("U_CustLevel").Value = customer.CustomerLevel;
            if(!string.IsNullOrEmpty(customer.TaxNumber))
                myBP.GTSRegNo = customer.TaxNumber;
            myBP.GTSBillingAddrTel =  customer.BillingAddress + '-' + customer.BillingTelephone;
            myBP.GTSBankAccountNo =  customer.HouseBank + '-' + customer.Account;
            foreach (var item in customer.CustomerItems)
            {
                string addressName = item.City+item.County+ item.BillToStreet;
                if (!string.IsNullOrEmpty(addressName))
                {
                    myBP.Addresses.AddressName = addressName;
                    myBP.Addresses.Delete();
                    myBP.Addresses.AddressName = addressName;
                    myBP.Addresses.Block = item.BillToStreet;
                    myBP.Addresses.City = item.City;
                    myBP.Addresses.County = item.County;
                    myBP.Addresses.Street = item.BillToStreet;
                    myBP.Addresses.UserFields.Fields.Item("U_Phone1").Value = item.Telephone1;
                    myBP.Addresses.UserFields.Fields.Item("U_Cellular").Value = item.MobilePhone;
                    myBP.Addresses.UserFields.Fields.Item("U_CntctPrsn").Value = item.ContactPerson;
                    myBP.Addresses.ZipCode = item.BillToZipCode;
                    myBP.Addresses.State = B1Common.BOneCommon.GetAddressCode(item.Province);

                    myBP.Addresses.AddressType = BoAddressType.bo_ShipTo;
                    myBP.Addresses.Add();
                }
            }
            myBP.ContactPerson = customer.ContactPerson;
            myBP.ContactEmployees.SetCurrentLine(0);
            myBP.ContactEmployees.Name = customer.ContactPerson;
            myBP.ContactEmployees.Address = customer.BillToStreet;
            myBP.ContactEmployees.Fax = customer.FaxNumber;
            myBP.ContactEmployees.MobilePhone = customer.MobilePhone;
            myBP.ContactEmployees.Phone1 = customer.Telephone1;
            myBP.ContactEmployees.Phone2 = customer.Telephone2;
            myBP.ContactEmployees.UserFields.Fields.Item("U_ZipCode").Value = customer.BillToZipCode;
            myBP.ContactEmployees.E_Mail = customer.Email;
            myBP.ContactEmployees.UserFields.Fields.Item("U_Website").Value = customer.Website;
            myBP.ContactEmployees.UserFields.Fields.Item("U_Province").Value = customer.Province;
            myBP.ContactEmployees.CityOfBirth = customer.City;
            myBP.ContactEmployees.UserFields.Fields.Item("U_County").Value = customer.County;

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
                result.ResultMessage = string.Format("【{0}】客户处理失败，ErrorCode:[{1}],ErrrMsg:[{2}];", customer.CustomerCode, SAP.SAPCompany.GetLastErrorCode(), SAP.SAPCompany.GetLastErrorDescription());
            }
            else
            {
                result.ResultValue = ResultType.True;
                result.ResultMessage = "【" + customer.CustomerCode + "】客户处理成功，系统数据：" + SAP.SAPCompany.GetNewObjectKey();
            }
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myBP);
            return result;
        }
    }
}
