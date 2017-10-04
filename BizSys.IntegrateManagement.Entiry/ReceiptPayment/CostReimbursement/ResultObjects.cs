using BizSys.IntegrateManagement.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.ReceiptPayment.CostReimbursement
{
    public class ResultObjects : IBaseResultObjects
    {
       
        public string ApprovalStatus { get; set; }
        public string Canceled { get; set; }
        public List<CostReimbursementLines> CostReimbursementLines { get; set; }
        public string CreateActionId { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string CreateUserSign { get; set; }
        public string DataOwner { get; set; }
        public string DataSource { get; set; }
        public string DeliveryDate { get; set; }
        
        public string DocNum { get; set; }
        public DateTime DocumentDate { get; set; }
        public string DocumentStatus { get; set; }
        public string Handwritten { get; set; }
        public string Instance { get; set; }
        public string LogInst { get; set; }
        
        public string Organization { get; set; }
        public string PaymentMethod { get; set; }
        public string Period { get; set; }
        public string PostingDate { get; set; }
        public string PromotionCode { get; set; }
        public string PromotionName { get; set; }
        public string Reason { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string Referenced { get; set; }
        public string Series { get; set; }
        public string Status { get; set; }
        public string TeamMembers { get; set; }
        public double Total { get; set; }
        public string Transfered { get; set; }
        public string UpdateActionId { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateTime { get; set; }
        public string UpdateUserSign { get; set; }
        public string UserCode { get; set; }
        public string UserSign { get; set; }
    }
}
