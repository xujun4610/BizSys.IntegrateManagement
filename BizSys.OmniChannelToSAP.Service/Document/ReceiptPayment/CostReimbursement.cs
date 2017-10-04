using BizSys.IntegrateManagement.Entity.Result;
using BizSys.IntegrateManagement.Entity.ReceiptPayment.CostReimbursement;
using BizSys.OmniChannelToSAP.Service.B1Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.OmniChannelToSAP.Service.Document.ReceiptPayment
{
    /// <summary>
    /// 费用报销
    /// </summary>
    public class CostReimbursement
    {
        public static Result CreateJournalEntry(ResultObjects order)
        {
            string B1DocEntry;
            if (B1Common.BOneCommon.IsExistOJDT( order.DocEntry.ToString(),"F", out B1DocEntry))
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

            myJE.TaxDate = order.DocumentDate;
            myJE.DueDate = order.DocumentDate;
            myJE.ReferenceDate = order.DocumentDate;
            myJE.Memo = order.Reason;//事由
            myJE.UserFields.Fields.Item("U_DocumentType").Value = "F";//订单来源类型 费用报销
            myJE.UserFields.Fields.Item("U_IM_DocEntry").Value = order.DocEntry.ToString();
            var DistributionRule = B1Common.BOneCommon.GetDistributionRule(order.DataOwner);
            foreach (var item in order.CostReimbursementLines)
            {
                myJE.Lines.Reference1 = item.Purpose;//用途
                myJE.Lines.AccountCode = GetCostAccount(item.CostItemName, DistributionRule.OcrCode);
                myJE.Lines.Debit = item.CostMoney;
                myJE.Lines.CostingCode = DistributionRule.OcrCode;
                myJE.Lines.CostingCode2 = DistributionRule.OcrCode2;
                myJE.Lines.BPLID = Convert.ToInt32(DistributionRule.BPLId);
                myJE.Lines.Add();
                
                myJE.Lines.AccountCode = "224105";
                myJE.Lines.CostingCode = DistributionRule.OcrCode;
                myJE.Lines.CostingCode2 = DistributionRule.OcrCode2;
                myJE.Lines.Credit = item.CostMoney;
                myJE.Lines.BPLID = Convert.ToInt32(DistributionRule.BPLId);

                myJE.Lines.Add();
            }
            int RntCode = myJE.Add();
            if (RntCode != 0)
            {
                result.ResultValue = ResultType.False;
                result.ResultMessage = string.Format("【{0}】费用报销处理失败，ErrorCode:[{1}],ErrrMsg:[{2}];", order.DocEntry, SAP.SAPCompany.GetLastErrorCode(), SAP.SAPCompany.GetLastErrorDescription());
            }
            else
            {
                order.B1DocEntry = SAP.SAPCompany.GetNewObjectKey();
                result.ResultValue = ResultType.True;
                result.ResultMessage = "【" + order.DocEntry.ToString() + "】费用报销处理成功，系统单据：" + SAP.SAPCompany.GetNewObjectKey();
            }
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myJE);
            return result;
        }

        private static string GetCostAccount(string FeeCode,string DeptCode)
        {
            if (string.IsNullOrEmpty(FeeCode))
                throw new ArgumentNullException("费用类型不能为空");
            try
            {
                SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string sql = $"select U_AcctCode  from [@AVA_FEEACCT] where U_FeeCode= '{FeeCode}' and U_DeptCode='{DeptCode}'";
                res.DoQuery(sql);
                if (res.RecordCount == 0)
                    throw new ArgumentNullException("未找到对应的费用科目");
                return res.Fields.Item("U_AcctCode").Value;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
