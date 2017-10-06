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
            /*
            Result result = new Result();

            SAPbobsCOM.BusinessPartners myMaterial = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems);
            bool IsExists = myMaterial.GetByKey(material.ItemCode);

            myMaterial.CardType = SAPbobsCOM.BoCardTypes.cCustomer;
            myMaterial.CardCode = material.ItemCode;
            myMaterial.CardName = material.ItemDescription;
            myMaterial.GroupCode = Convert.ToInt32(material.GroupCode);
            myMaterial.CompanyPrivate = BoCardCompanyTypes.cCompany;


            myMaterial.ZipCode = material.BillToZipCode;
            myMaterial.EmailAddress = material.Email;
            myMaterial.CreditLimit = material.PaidToCredit;
            myMaterial.SalesPersonCode = -1; //销售员修改；
            myMaterial.Territory = -2; //区域修改，默认值

            myMaterial.PayTermsGrpCode = -1; //付款条件修改
            myMaterial.PriceListNum = 1; //默认价格清单修改

            myMaterial.Valid = material.Activation == "Yes" ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
            myMaterial.ValidFrom = Convert.ToDateTime(material.ActiveFrom);
            myMaterial.ValidTo = Convert.ToDateTime(material.ActiveTo);
            myMaterial.Frozen = material.Inactive == "No" ? BoYesNoEnum.tNO : BoYesNoEnum.tYES;
            myMaterial.FrozenFrom = Convert.ToDateTime(material.InactiveFrom);
            myMaterial.FrozenTo = Convert.ToDateTime(material.InactiveTo);
            myMaterial.Cellular = material.MobilePhone;
            myMaterial.Phone1 = material.Telephone1;
            myMaterial.Phone2 = material.Telephone2;
            myMaterial.Currency = material.BPCurrency;
            myMaterial.FreeText = material.Remarks;

            //if(!string.IsNullOrEmpty(material.TaxNumber))
            //myMaterial.GTSRegNo = material.TaxNumber;
            //myMaterial.GTSBillingAddrTel =  material.BillingAddress + '-' + material.BillingTelephone;
            //myMaterial.GTSBankAccountNo =  material.HouseBank + '-' + material.Account;
            foreach (var item in material.CustomerItems)
            {
                string addressName = item.City + item.County + item.BillToStreet;
                if (!string.IsNullOrEmpty(addressName))
                {
                    myMaterial.Addresses.AddressName = addressName;
                    myMaterial.Addresses.Delete();
                    myMaterial.Addresses.AddressName = addressName;
                    myMaterial.Addresses.Block = item.BillToStreet;
                    myMaterial.Addresses.City = item.City;
                    myMaterial.Addresses.County = item.County;
                    myMaterial.Addresses.Street = item.BillToStreet;
                    myMaterial.Addresses.ZipCode = item.BillToZipCode;
                    myMaterial.Addresses.State = B1Common.BOneCommon.GetAddressCode(item.Province);

                    myMaterial.Addresses.AddressType = BoAddressType.bo_ShipTo;
                    myMaterial.Addresses.Add();
                }
            }
            myMaterial.ContactPerson = material.ContactPerson;
            myMaterial.ContactEmployees.SetCurrentLine(0);
            myMaterial.ContactEmployees.Name = material.ContactPerson;
            myMaterial.ContactEmployees.Address = material.BillToStreet;
            myMaterial.ContactEmployees.Fax = material.FaxNumber;
            myMaterial.ContactEmployees.MobilePhone = material.MobilePhone;
            myMaterial.ContactEmployees.Phone1 = material.Telephone1;
            myMaterial.ContactEmployees.Phone2 = material.Telephone2;
            myMaterial.ContactEmployees.E_Mail = material.Email;
            myMaterial.ContactEmployees.CityOfBirth = material.City;

            myMaterial.ContactEmployees.Add();


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
                result.ResultMessage = string.Format("【{0}】客户处理失败，ErrorCode:[{1}],ErrrMsg:[{2}];", material.CustomerCode, SAP.SAPCompany.GetLastErrorCode(), SAP.SAPCompany.GetLastErrorDescription());
            }
            else
            {
                result.ResultValue = ResultType.True;
                result.ResultMessage = "【" + material.CustomerCode + "】客户处理成功，系统数据：" + SAP.SAPCompany.GetNewObjectKey();
            }
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myMaterial);
            return result;
            */
        }
    }
}
