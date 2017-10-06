using BizSys.IntegrateManagement.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.ReceiptPayment.Receipt
{
    public class ResultObjects:IBaseResultObjects
    {

      
       
        public string ApprovalStatus { get; set; }
     
        public string BankItemCode { get; set; }
        public string BankItemDescription { get; set; }
        public string BusinessPartnerCode { get; set; }
        public string BusinessPartnerName { get; set; }
        public string BydUUID { get; set; }
        public string Canceled { get; set; }
        public string CardNumber { get; set; }
        public string ClosedRecSum { get; set; }
        public string CreateActionId { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string CreateUserSign { get; set; }
        public string CustomType { get; set; }
        public string DataOwner { get; set; }
        public string DataSource { get; set; }
        public string Deleted { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string DistributionRule1 { get; set; }
        public string DistributionRule2 { get; set; }
        public string DistributionRule3 { get; set; }
        public string DistributionRule4 { get; set; }
        public string DistributionRule5 { get; set; }
      
        public string DocEntryAnywhere { get; set; }
        public string DocNum { get; set; }
        public string DocumentCurrency { get; set; }
        public DateTime DocumentDate { get; set; }
        public string DocumentRate { get; set; }
        public string DocumentStatus { get; set; }
        public double DocumentTotal { get; set; }
        public string Handwritten { get; set; }
        public string Instance { get; set; }
        public string LogInst { get; set; }
     
        public string OpenRecSum { get; set; }
        public string PartnerType { get; set; }
        public string PayClosedRecSum { get; set; }
        public string PayType { get; set; }
        public string Period { get; set; }
        public DateTime PostingDate { get; set; }
        public string RecKey { get; set; }
        public List<ReceiptItems> ReceiptItems { get; set; }
        public string ReceiptMethods { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string Referenced { get; set; }
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
