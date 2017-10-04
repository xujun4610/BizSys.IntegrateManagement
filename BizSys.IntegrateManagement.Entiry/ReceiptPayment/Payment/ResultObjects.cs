using BizSys.IntegrateManagement.Entity.Base;
using BizSys.IntegrateManagement.Entity.ReceiptPayment.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.ReceiptPayment.Payment
{
    public class ResultObjects:IBaseResultObjects
    {

        public string ApprovalStatus { get; set; }
        public string BusinessPartnerCode { get; set; }
        public string BusinessPartnerName { get; set; }
        public string BydUUID { get; set; }
        public string Canceled { get; set; }
        public double ClosedRecSum { get; set; }
        public string CreateActionId { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string CreateUserSign { get; set; }
        public string CustomType { get; set; }
        public string DataOwner { get; set; }
        public string DataSource { get; set; }
        public string Deleted { get; set; }
        public string DeliveryDate { get; set; }
        public string DocNum { get; set; }
        public string DocumentCurrency { get; set; }
        public string DocumentDate { get; set; }
        public string DocumentRate { get; set; }
        public string DocumentStatus { get; set; }
        public double DocumentTotal { get; set; }
        public string Handwritten { get; set; }
        public string Instance { get; set; }
        public int LogInst { get; set; }
        public double OpenRecSum { get; set; }
        public List<PaymentItems> PaymentItems { get; set; }
        public string Period { get; set; }
        public string PostingDate { get; set; }
        public string RecKey { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string Referenced { get; set; }
        public string Remarks { get; set; }
        public string Series { get; set; }
        public string Status { get; set; }
        public string TeamMembers { get; set; }
        public string Transfered { get; set; }
        public string UpdateActionId { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateTime { get; set; }
        public string UpdateUserSign { get; set; }
        public string UserSign { get; set; }
    }
}
