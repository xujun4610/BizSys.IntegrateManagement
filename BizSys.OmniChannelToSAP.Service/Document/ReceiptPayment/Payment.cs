using BizSys.IntegrateManagement.Entity.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizSys.IntegrateManagement.Entity.ReceiptPayment.PaymentApply;
using BizSys.OmniChannelToSAP.Service.B1Common;
using SAPbobsCOM;
using static BizSys.IntegrateManagement.Common.Enumerator;

namespace BizSys.OmniChannelToSAP.Service.Document.ReceiptPayment
{
    public class Payment
    {
        /// <summary>
        /// 创建付款单草稿
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static Result CreatePaymentDraft(ResultObjects order)
        {
            string B1DocEntry = default(string);
            if (B1Common.BOneCommon.IsExistPaymentDraft(order.DocEntry.ToString(), out B1DocEntry))
            {
                order.B1DocEntry = B1DocEntry;
                return new Result()
                {
                    ResultValue = ResultType.True,
                    ResultMessage = "该订单已生成到B1"
                };
            }
            Result result = new Result();
            SAPbobsCOM.Payments myPayments = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPaymentsDrafts);
            myPayments.DocObjectCode = SAPbobsCOM.BoPaymentsObjectType.bopot_OutgoingPayments;
            SAPbobsCOM.IRecordset rs = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            var DistributionRule = B1Common.BOneCommon.GetDistributionRuleByKey(order.DataOwner.ToString());
            myPayments.CardCode = order.BusinessPartnerCode;
            myPayments.CardName = order.BusinessPartnerName;
            myPayments.TaxDate = order.DocumentDate;
            myPayments.DocDate = order.PostingDate;
            myPayments.DocType = GetDocType(order.PartnerType);
            myPayments.TransferDate = order.DocumentDate;
            myPayments.TransferSum = order.DocumentTotal;
            myPayments.Remarks = order.Remarks;
            myPayments.BPLID = Convert.ToInt32(DistributionRule.BPLId);
            myPayments.DocCurrency = "RMB";
            if (string.IsNullOrEmpty(order.PayType))
                throw new Exception("付款类型不能为空。");
            string PayType = ((emPayType)Enum.Parse(typeof(emPayType), order.PayType)).GetHashCode().ToString();
            myPayments.UserFields.Fields.Item("U_OvpmType").Value = PayType;
            myPayments.UserFields.Fields.Item("U_IM_DocEntry").Value = order.DocEntry.ToString();
            myPayments.UserFields.Fields.Item("U_BankCode").Value = order.BankItemCode;
            myPayments.UserFields.Fields.Item("U_BankDscrp").Value = order.BankItemDescription;
            myPayments.UserFields.Fields.Item("U_TailNumber").Value = order.CardNumber;
            myPayments.UserFields.Fields.Item("U_OcrCode").Value = DistributionRule.OcrCode;
            myPayments.UserFields.Fields.Item("U_OcrCode2").Value = DistributionRule.OcrCode2;
            myPayments.UserFields.Fields.Item("U_OcrCode3").Value = order.DistributionRule3;
            myPayments.UserFields.Fields.Item("U_OcrCode4").Value = order.DistributionRule4;
            myPayments.UserFields.Fields.Item("U_OcrCode5").Value = order.DistributionRule5;

            myPayments.ControlAccount = B1Common.BOneCommon.GetAccountCodeByPayType(PayType);

            myPayments.TransferAccount = GetAccountByMethod(order.ReceiptMethods);
            int RntCode = myPayments.Add();
            if (RntCode != 0)
            {
                result.ResultValue = ResultType.False;
                result.ResultMessage = string.Format("【{0}】付款单申请处理失败，ErrorCode:[{1}],ErrrMsg:[{2}];", order.DocEntry, SAP.SAPCompany.GetLastErrorCode(), SAP.SAPCompany.GetLastErrorDescription());
                return result;
            }
            else
            {
                order.B1DocEntry = SAP.SAPCompany.GetNewObjectKey();
                result.ResultValue = ResultType.True;
                result.ResultMessage = "【" + order.DocEntry.ToString() + "】付款单申请处理成功，系统单据：" + SAP.SAPCompany.GetNewObjectKey();
                return result;
            }

        }

        public static BoRcptTypes GetDocType(string docType)
        {
            switch(docType)
            {
                case "Customer":return BoRcptTypes.rCustomer;
                case "Supplier": return BoRcptTypes.rSupplier;
                case "Account":return BoRcptTypes.rAccount;
                default:throw new Exception("业务伙伴类型不能为空");
            }
        }

        

        private static string GetAccountByMethod(string payMethod)
        {
            if (string.IsNullOrEmpty(payMethod)) throw new ArgumentNullException("付款方式不能为空");
            try
            {
                string sql = @"select AcctCode from  V_AVA_ER_OPYT where TypeCode = '{0}' ";
                SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                res.DoQuery(string.Format(sql, payMethod));
                return res.Fields.Item("AcctCode").Value;
            }
            catch(Exception ex)
            {
                throw ex.InnerException;
            }
        }
    }
}
