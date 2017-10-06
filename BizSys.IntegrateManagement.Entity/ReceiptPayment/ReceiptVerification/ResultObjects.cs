using BizSys.IntegrateManagement.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.ReceiptPayment.ReceiptVerification
{
    public class ResultObjects:IBaseResultObjects
    {
       
        public string Activated { get; set; }
        public string ApprovalStatus { get; set; }
        public string Bank { get; set; }
        public string BankItemCode { get; set; }
        public string BusinessPartnerCode { get; set; }
        public string DistributionRule2 { get; set; }
        public string BusinessPartnerName { get; set; }
        public  string CardCode { get; set; }
        public double ClearMoney { get; set; }
       // public string Code { get; set; }
        public string CreateActionId { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string CreateUserSign { get; set; }
        public string DataOwner { get; set; }
        public DateTime Date { get; set; }
       

      
        public double DocumentMoney { get; set; }
        public string LogInst { get; set; }
     
        public string Organization { get; set; }
        public List<ReceiptVItems> ReceiptVItems { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string Remarks { get; set; }
        public string Series { get; set; }
        public string Status { get; set; }
        public string TeamMembers { get; set; }
        public string UpdateActionId { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateTime { get; set; }
        public string UpdateUserSign { get; set; }
        public int ObjectKey { get; set; }

    }
}
