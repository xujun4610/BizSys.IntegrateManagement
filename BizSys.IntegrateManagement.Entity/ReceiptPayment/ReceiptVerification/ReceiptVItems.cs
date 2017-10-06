using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.ReceiptPayment.ReceiptVerification
{
    public class ReceiptVItems
    {
        public string isDirty { get; set; }
        public string isDeleted { get; set; }
        public string isNew { get; set; }
        public string Bank { get; set; }
        public string BankItemCode { get; set; }
        public string Canceled { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string CreateActionId { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string CreateUserSign { get; set; }
        public string Date { get; set; }
        public string DocEntry { get; set; }
        public string LineId { get; set; }
        public string LineStatus { get; set; }
        public string LogInst { get; set; }
        public string ObjectCode { get; set; }
        public double PaymentValue { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string Status { get; set; }
        public string UpdateActionId { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateTime { get; set; }
        public string UpdateUserSign { get; set; }
        public string VisOrder { get; set; }

    }
}
