using BizSys.IntegrateManagement.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.ReceiptPayment.Reconciliation
{
    public class ResultObjects:IBaseResultObjects
    {

      
        public string ApprovalStatus { get; set; }
        public string BusinessPartnerCode { get; set; }
        public string BusinessPartnerName { get; set; }
        public string Canceled { get; set; }
        public string CreateActionId { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string CreateUserSign { get; set; }
        public string DataOwner { get; set; }
        public string DataSource { get; set; }
        public string DeliveryDate { get; set; }
     
        public string DocNum { get; set; }
        public string DocumentDate { get; set; }
        public string DocumentStatus { get; set; }
        public string Handwritten { get; set; }
        public string Instance { get; set; }
        public string LogInst { get; set; }
        public string ObjectCode { get; set; }
        public string PartnerType { get; set; }
        public string Period { get; set; }
        public string PostingDate { get; set; }
        public string RecDiff { get; set; }
        public string ReconciliationCode { get; set; }
        public string ReconciliationDate { get; set; }
        public List<ReconciliationLine1s> ReconciliationLine1s { get; set; }
        public List<ReconciliationLine2s> ReconciliationLine2s { get; set; }
        public string ReconciliationSum { get; set; }
        public string ReconciliationType { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string Remarks { get; set; }
        public string Series { get; set; }
        public string Status { get; set; }
        public string TeamMembers { get; set; }
        public string Transfered { get; set; }
        public string UpdateActionId { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateTime { get; set; }
        public string UpdateUserSign { get; set; }
        public string UserSign { get; set; }
    }
}
