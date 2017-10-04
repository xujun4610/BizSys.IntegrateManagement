using BizSys.IntegrateManagement.Entity.CustomerService.CustomerServiceApply;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.OmniChannelToSAP.Service.B1Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.OmniChannelToSAP.Service.Document.CustomerService
{
    public class CustomerServiceApply
    {
        public static Result CreateCustomerServiceApply(ResultObjects customerServiceApply)
        {
            Result result = new Result();
            try
            {
                SAPbobsCOM.CustomerEquipmentCards myCard = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oCustomerEquipmentCards);
                SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string sql = @"select  insID from OINS where internalSN = '{0}'";
                int rstCode = 0;
                int InsDocEntry;
                bool UpdateFlag = false;
                res.DoQuery(string.Format(sql, customerServiceApply.ProductCode));
                if (res.RecordCount == 1)
                {
                    InsDocEntry = res.Fields.Item("insID").Value;
                    UpdateFlag = myCard.GetByKey(InsDocEntry);
                }

                myCard.InternalSerialNum = customerServiceApply.ProductCode;
                myCard.ItemCode = customerServiceApply.ItemCode;
                myCard.ItemDescription = customerServiceApply.ItemName;
                myCard.CustomerCode = customerServiceApply.CardCode;
                myCard.CustomerName = customerServiceApply.CardName;
                myCard.UserFields.Fields.Item("U_Description").Value = customerServiceApply.Description;
                myCard.UserFields.Fields.Item("U_DocEntry").Value = customerServiceApply.DocEntry;

                string serviceTimes = myCard.UserFields.Fields.Item("U_ServiceTimes").Value;
                if (string.IsNullOrEmpty(serviceTimes))
                {
                    serviceTimes = "1";
                }
                else
                {
                    serviceTimes = (Convert.ToInt32(serviceTimes) + 1).ToString();
                }
                myCard.UserFields.Fields.Item("U_ServiceTimes").Value = serviceTimes;

                if (UpdateFlag)
                    rstCode = myCard.Update();
                else
                    rstCode = myCard.Add();
                if (rstCode != 0)
                {
                    result.ResultValue = ResultType.False;
                    result.ResultMessage = string.Format("【{0}】服务申请处理失败，ErrorCode:[{1}],ErrrMsg:[{2}];", customerServiceApply.DocEntry, SAP.SAPCompany.GetLastErrorCode(), SAP.SAPCompany.GetLastErrorDescription());
                }
                else
                {
                    customerServiceApply.B1DocEntry = SAP.SAPCompany.GetNewObjectKey();
                    result.ResultValue = ResultType.True;
                    result.ResultMessage = "【" + customerServiceApply.DocEntry.ToString() + "】服务申请处理成功，系统单据：" + SAP.SAPCompany.GetNewObjectKey();
                }
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myCard);
                return result;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }



        }
    }
}
