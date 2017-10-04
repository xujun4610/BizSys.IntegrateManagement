using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.ReceiptPayment.CostReimbursement
{
    public class CostReimbursementLines
    {
        public string isDirty { get; set; }
        public string isDeleted { get; set; }
        public string isNew { get; set; }
        public string AcctCode { get; set; }
        public string Canceled { get; set; }
        public string CostItemName { get; set; }
        public double CostMoney { get; set; }
        public string CostName { get; set; }
        public string CreateActionId { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string CreateUserSign { get; set; }
        public string CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string DataSource { get; set; }
        public string DistributionRule1 { get; set; }
        public string DistributionRule2 { get; set; }
        public string DistributionRule3 { get; set; }
        public string DistributionRule4 { get; set; }
        public string DistributionRule5 { get; set; }
        public string DocEntry { get; set; }
        public string LineId { get; set; }
        public string LineStatus { get; set; }
        public string LogInst { get; set; }
        public string ObjectCode { get; set; }
        public string Purpose { get; set; }
        public string RateId { get; set; }
        public string RateName { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public string UpdateActionId { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateTime { get; set; }
        public string UpdateUserSign { get; set; }
        public string UseDate { get; set; }
        public string VisOrder { get; set; }
    }
}
