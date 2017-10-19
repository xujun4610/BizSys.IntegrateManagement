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
            string b1CpySign = "BJ"; //客户主数据放北京
            Result result = new Result();

            SAPbobsCOM.BusinessPartners myBP = SAPCompanyPool.GetSAPCompany(b1CpySign).GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
            bool IsExists = myBP.GetByKey(customer.CustomerCode);

            myBP.CardType = SAPbobsCOM.BoCardTypes.cCustomer;
            myBP.CardCode = customer.CustomerCode;
            myBP.CardName = customer.CustomerName;
            myBP.GroupCode = Convert.ToInt32(customer.GroupCode);
            myBP.CompanyPrivate = BoCardCompanyTypes.cCompany;


            myBP.ZipCode = customer.BillToZipCode;
            myBP.EmailAddress = customer.Email;
            myBP.CreditLimit = customer.PaidToCredit;
            //myBP.SalesPersonCode = -1; //销售员修改；
            myBP.Territory = B1Common.BOneCommon.GetTerritoryId4MFT(b1CpySign, customer.ChannelType); //-2; //区域修改，默认值

            //myBP.PayTermsGrpCode = -1; //付款条件修改
            myBP.PriceListNum = (customer.PriceListNumber == 0) ? 1 : customer.PriceListNumber; //1; //默认价格清单修改
            myBP.DebitorAccount = "1122";

            if(customer.Activation == "Yes")
            {
                myBP.Valid = BoYesNoEnum.tYES;
                myBP.Frozen = BoYesNoEnum.tNO;
                myBP.ValidFrom = Convert.ToDateTime(customer.ActiveFrom);
                myBP.ValidTo = Convert.ToDateTime(customer.ActiveTo);
            }
            else
            {
                myBP.Valid = BoYesNoEnum.tNO;
                myBP.Frozen = BoYesNoEnum.tYES;
            }
            //myBP.Frozen = customer.Inactive == "No" ? BoYesNoEnum.tNO : BoYesNoEnum.tYES;
            //myBP.FrozenFrom = Convert.ToDateTime(customer.InactiveFrom);
            //myBP.FrozenTo = Convert.ToDateTime(customer.InactiveTo);
            myBP.Cellular = customer.MobilePhone;
            myBP.Phone1 = customer.Telephone1;
            myBP.Phone2 = customer.Telephone2;
            myBP.Currency = customer.BPCurrency;
            myBP.FreeText = customer.Remarks;

            //if(!string.IsNullOrEmpty(customer.TaxNumber))
            //myBP.GTSRegNo = customer.TaxNumber;
            //myBP.GTSBillingAddrTel =  customer.BillingAddress + '-' + customer.BillingTelephone;
            //myBP.GTSBankAccountNo =  customer.HouseBank + '-' + customer.Account;
            if (IsExists)
            {
                //update
                int index = default(int);
                int oldContractsCount = default(int); //原来的集合数量
                SAPbobsCOM.ContactEmployees ce = myBP.ContactEmployees;
                oldContractsCount = ce.Count;
                for (int i = 0; i < oldContractsCount; i++)
                {
                    ce.SetCurrentLine(i);
                    if (customer.CustomerItems.Count(ocm => ocm.ContactPerson.Equals(ce.Name)) == 0)
                    {
                        //add & disable b1
                        //item.isNew = false.ToString();
                        ce.Active = BoYesNoEnum.tNO;
                        ce.Add();
                    }
                    else
                    {
                        
                        var item = customer.CustomerItems.Where<CustomerItems>(c => c.ContactPerson.Equals(ce.Name)).FirstOrDefault(); //.isNew = false.ToString();
                        string[] addressName = { "CN", item.Province, item.City, item.County, item.Town, item.BillToStreet };
                        ce.Address = string.Concat(addressName);
                        ce.Active = BoYesNoEnum.tYES;
                        ce.Phone1 = item.Telephone1;
                        if (item.DefaltAddress.Equals("Y"))
                        {
                            myBP.ContactPerson = item.ContactPerson;
                        }
                        //存在的记录排除打标记，不是新的
                        item.isNew = false.ToString();
                    }
                }
                //add new
                if (customer.CustomerItems.Count(c=>c.isNew != false.ToString()) > 0)
                {
                    var new_OCM_items = customer.CustomerItems.Where<CustomerItems>(c => c.isNew != false.ToString());
                    foreach (var item in new_OCM_items)
                    {
                        string[] addressName = { "CN", item.Province, item.City, item.County, item.Town, item.BillToStreet };

                        //SAPbobsCOM.ContactEmployees ce = myBP.ContactEmployees;
                        ce.SetCurrentLine(ce.Count); //往后添加
                        ce.Name = item.ContactPerson;
                        ce.Address = string.Concat(addressName);
                        ce.Active = BoYesNoEnum.tYES;
                        ce.Phone1 = item.Telephone1;
                        if (item.DefaltAddress.Equals("Y"))
                        {
                            myBP.ContactPerson = item.ContactPerson;
                        }
                        ce.Add();
                    }
                }
                
            }
            else
            {
                //add
                for (int i = 0; i < customer.CustomerItems.Count; i++)
                {
                    CustomerItems item = customer.CustomerItems[i];
                    string[] addressName = { "CN", item.Province, item.City, item.County, item.Town, item.BillToStreet };
                    if (!string.IsNullOrEmpty(string.Concat(addressName)))
                    {
                        /*
                         * 这里根据梅菲特实际需求
                         * 将原来的配送地址分别放到 B1 联系人、收货地址 两个板块
                         * 更改为，统一放送到 B1 联系人。
                        myBP.Addresses.AddressName = addressName;
                        myBP.Addresses.Delete();
                        myBP.Addresses.AddressName = addressName;
                        myBP.Addresses.Block = item.BillToStreet;
                        myBP.Addresses.City = item.City;
                        myBP.Addresses.County = item.County;
                        myBP.Addresses.Street = item.BillToStreet;
                        myBP.Addresses.ZipCode = item.BillToZipCode;
                        myBP.Addresses.State = B1Common.BOneCommon.GetAddressCode(item.Province);
                        myBP.Addresses.AddressType = BoAddressType.bo_ShipTo;
                        myBP.Addresses.Add();
                        */

                        SAPbobsCOM.ContactEmployees ce = myBP.ContactEmployees;
                        ce.SetCurrentLine(i);
                        ce.Name = item.ContactPerson;
                        ce.Address = string.Concat(addressName);
                        ce.Active = BoYesNoEnum.tYES;
                        ce.Phone1 = item.Telephone1;
                        if (item.DefaltAddress.Equals("Y"))
                        {
                            myBP.ContactPerson = item.ContactPerson;
                        }
                        ce.Add();
                    }

                }
            }

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
                result.ResultMessage = string.Format("【{0}】客户处理失败，ErrorCode:[{1}],ErrrMsg:[{2}];", customer.CustomerCode, SAPCompanyPool.GetSAPCompany(b1CpySign).GetLastErrorCode(), SAPCompanyPool.GetSAPCompany(b1CpySign).GetLastErrorDescription());
            }
            else
            {
                result.ResultValue = ResultType.True;
                //回写
                result.CallBackDataList.AddRange(B1Common.BOneCommon.GetBPContractCode4MFT(b1CpySign, myBP.CardCode));
                result.ResultMessage = "【" + customer.CustomerCode + "】客户处理成功，系统数据：" + SAPCompanyPool.GetSAPCompany(b1CpySign).GetNewObjectKey();
            }
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myBP);
            SAPCompanyPool.DisconnectAll();
            return result;
        }
    }
}
