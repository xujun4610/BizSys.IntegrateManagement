using BizSys.IntegrateManagement.Entity.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizSys.IntegrateManagement.Entity.ReceiptPayment;
using BizSys.IntegrateManagement.Entity.ReceiptPayment.Receipt;
using BizSys.OmniChannelToSAP.Service.B1Common;

namespace BizSys.OmniChannelToSAP.Service.Document.ReceiptPayment
{
    public class Receipt
    {
        public static Result CreateReceipt(ResultObjects order)
        {
            Result result = new Result();
            SAPbobsCOM.Payments myPayments = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oIncomingPayments);
            SAPbobsCOM.IRecordset rs = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            
            //myPayments.DocDate = order.PostingDate;
            //myPayments.Comments = order.Remarks;

            //foreach (var item in order.ReceiptItems)
            //{
            //    //myPayments.Lines.ItemCode = item.ItemCode;
            //    //myPayments.Lines.Quantity = Convert.ToDouble(item.Quantity); 
            //    //myPayments.Lines.WarehouseCode = item.WhsCode;
            //    //myPayments.Lines.Add();
            //}

            int RntCode = myPayments.Add();
           

            if (RntCode != 0)
            {
                result.ResultValue = ResultType.False;
                result.ResultMessage = string.Format("【{0}】收款单处理失败，ErrorCode:[{1}],ErrrMsg:[{2}];", order.DocEntry, SAP.SAPCompany.GetLastErrorCode(), SAP.SAPCompany.GetLastErrorDescription());
                return result;
            }
            else
            {
                order.B1DocEntry = SAP.SAPCompany.GetNewObjectKey();
                result.ResultValue = ResultType.True;
                result.ResultMessage = "【" + order.DocEntry.ToString() + "】收款单处理成功，系统单据：" + SAP.SAPCompany.GetNewObjectKey();
                return result;
            }

        }
    }
}
