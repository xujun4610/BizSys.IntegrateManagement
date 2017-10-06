using BizSys.IntegrateManagement.Entity.BatchNumber;
using BizSys.IntegrateManagement.Entity.ReceiptPayment.ReceiptVerification;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.OmniChannelToSAP.Service.B1Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.OmniChannelToSAP.Service.Document.ReceiptPayment
{
    /// <summary>
    /// 回款核销
    /// </summary>
    public class ReceiptVerification
    {
        public static Result CreateJournalEntry(ResultObjects order)
        {
            string B1DocEntry;
            if (B1Common.BOneCommon.IsExistOJDT(order.ObjectKey.ToString(), "H", out B1DocEntry))
            {
                order.B1DocEntry = B1DocEntry;
                return new Result()
                {
                    ResultValue = ResultType.True,
                    ResultMessage = "该订单已生成到B1"
                };
            }
            Result result = new Result();
            SAPbobsCOM.JournalEntries myJE = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oJournalEntries);

            
            myJE.DueDate = order.CreateDate;
            myJE.ReferenceDate = order.CreateDate;
            myJE.TaxDate = order.CreateDate;
            myJE.Reference = order.Remarks;
            myJE.UserFields.Fields.Item("U_DocumentType").Value = "H";//订单来源类型 费用报销
            myJE.UserFields.Fields.Item("U_IM_DocEntry").Value = order.ObjectKey.ToString();
            //借：中转科目-112202
            //贷：应收账款 - 112201"
            int BPLId = default(int);
            DistributionRule distributionRule = B1Common.BOneCommon.GetDistributionRule(order.DataOwner);
            foreach (var item in order.ReceiptVItems)
            {
                myJE.Lines.AccountCode = "112201";
                myJE.Lines.Credit = item.PaymentValue;
                myJE.Lines.ShortName = order.BusinessPartnerCode;
                myJE.Lines.CostingCode = distributionRule.OcrCode;
                myJE.Lines.CostingCode2 = distributionRule.OcrCode2;
                myJE.Lines.BPLID = Convert.ToInt32(distributionRule.BPLId);
                myJE.Lines.Add();
                BPLId = Convert.ToInt32(distributionRule.BPLId);
            }

            myJE.Lines.AccountCode = "112202";
            myJE.Lines.ShortName = order.BusinessPartnerCode;
            myJE.Lines.BPLID = BPLId;
            myJE.Lines.Debit = order.ClearMoney;
            myJE.Lines.Add();



            int RntCode = myJE.Add();
            if (RntCode != 0)
            {
                result.ResultValue = ResultType.False;
                result.ResultMessage = string.Format("【{0}】回款核销处理失败，ErrorCode:[{1}],ErrrMsg:[{2}];", order.DocEntry, SAP.SAPCompany.GetLastErrorCode(), SAP.SAPCompany.GetLastErrorDescription());
            }
            else
            {
                order.B1DocEntry = SAP.SAPCompany.GetNewObjectKey();
                result.ResultValue = ResultType.True;
                result.ResultMessage = "【" + order.DocEntry.ToString() + "】回款核销处理成功，系统单据：" + SAP.SAPCompany.GetNewObjectKey();
            }
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myJE);
            return result;
        }
    }
}
