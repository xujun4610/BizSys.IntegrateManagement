using BizSys.IntegrateManagement.IRepository.ReceiptPaymentService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizSys.IntegrateManagement.Entity.ReceiptPayment.Payment;
using BizSys.IntegrateManagement.Repository.BOneCommon;

namespace BizSys.IntegrateManagement.Repository.ReceiptPaymentService
{
    public class PaymentRep : IPaymentRep
    {
        public PaymentRootObject GetReceiptsByKey(string DocEntry)
        {
            PaymentRootObject paymentRootObject = new PaymentRootObject();
            paymentRootObject.ResultObjects = new List<ResultObjects>();
            try
            {
                SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string sql = string.Format("select * from OVPM where DocEntry = '{0}'", DocEntry);
                res.DoQuery(sql);
                if (res.RecordCount != 1)
                    return new PaymentRootObject()
                    {
                        ResultCode = -1,
                        Message = "Can not find the Receipt in the SAP."
                    };
                ResultObjects payment = new ResultObjects();
                #region 属性赋值
                payment.DocEntry = res.Fields.Item("DocEntry").Value;
                payment.DocumentTotal = res.Fields.Item("DocTotal").Value;
               
                payment.DocumentDate = res.Fields.Item("DocDate").Value.ToString();
                payment.DeliveryDate = res.Fields.Item("DocDueDate").Value.ToString();
                payment.PostingDate = res.Fields.Item("TaxDate").Value.ToString();
                payment.BusinessPartnerCode = res.Fields.Item("CardCode").Value;
                payment.BusinessPartnerName = res.Fields.Item("CardName").Value;
                payment.B1DocEntry = res.Fields.Item("DocEntry").Value.ToString();
                payment.CreateDate = res.Fields.Item("CreateDate").Value;
                payment.UpdateDate = res.Fields.Item("UpdateDate").Value;
                payment.Handwritten = res.Fields.Item("Handwrtten").Value == "N" ? "No" : "Yes";
                payment.Remarks = res.Fields.Item("Comments").Value;
                payment.LogInst = res.Fields.Item("LogInstanc").Value + 1;
                //res.Fields.Item("U_ReceiveNum").Value;
                //res.Fields.Item("U_OvpmType").Value;
                //res.Fields.Item("U_OvpmType").Value;
                //res.Fields.Item("U_ReceiveNum").Value;

                #endregion
                paymentRootObject.ResultObjects.Add(payment);
                paymentRootObject.ResultCode = 0;
                paymentRootObject.Message = "Successful operation.";
            }
            catch (Exception ex)
            {
                paymentRootObject.ResultCode = -1;
                paymentRootObject.Message = "Failed operation." + ex.Message;
            }
            return paymentRootObject;
        }
    }
}
