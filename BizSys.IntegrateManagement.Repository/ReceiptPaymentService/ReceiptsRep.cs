using BizSys.IntegrateManagement.Entity.ReceiptPayment.Receipt;
using BizSys.IntegrateManagement.IRepository.ReceiptPaymentService;
using BizSys.IntegrateManagement.Repository.BOneCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Repository.ReceiptPaymentService
{
   public class ReceiptsRep : IReceiptsRep
    {
        public ReceiptRootObject GetReceiptsByKey(string DocEntry)
        {
            ReceiptRootObject receiptRootObject = new ReceiptRootObject();
            receiptRootObject.ResultObjects = new List<ResultObjects>();
            try
            {
                SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string sql = string.Format("select * from ORCT where DocEntry = '{0}'", DocEntry);
                res.DoQuery(sql);
                if (res.RecordCount != 1)
                    return new ReceiptRootObject()
                    {
                        ResultCode = -1,
                        Message = "Can not find the Receipt in the SAP."
                    };
                ResultObjects receipts = new ResultObjects();
                receiptRootObject.ResultCode = 0;
                receiptRootObject.Message = "Successful operation.";
                #region 属性赋值
                receipts.DocEntry = res.Fields.Item("DocEntry").Value;
                receipts.DocumentTotal = res.Fields.Item("DocTotal").Value;
               // receipts.TotalPaidSum = res.Fields.Item("DocTotalSy").Value;
                receipts.DocumentDate = res.Fields.Item("DocDate").Value;
                receipts.DeliveryDate = res.Fields.Item("DocDueDate").Value;
                receipts.PostingDate = res.Fields.Item("TaxDate").Value;
                receipts.BusinessPartnerCode = res.Fields.Item("CardCode").Value;
                receipts.BusinessPartnerName = res.Fields.Item("CardName").Value;
                receipts.B1DocEntry= res.Fields.Item("DocEntry").Value.ToString();
                receipts.Handwritten = res.Fields.Item("Handwrtten").Value == "N" ? "No" : "Yes";
                receipts.Remarks = res.Fields.Item("Comments").Value;
               
                receipts.BusinessPartnerCode = res.Fields.Item("CardCode").Value;
                //res.Fields.Item("U_ReceiveNum").Value;
                //res.Fields.Item("U_OvpmType").Value;
                //res.Fields.Item("U_OvpmType").Value;
                //res.Fields.Item("U_ReceiveNum").Value;

                #endregion
                receiptRootObject.ResultObjects.Add(receipts);
            }
            catch(Exception ex)
            {
                receiptRootObject.ResultCode = -1;
                receiptRootObject.Message = "Failed operation." + ex.Message;
            }
            return receiptRootObject;
        }
    }
}
