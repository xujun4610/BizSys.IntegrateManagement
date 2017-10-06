using BizSys.IntegrateManagement.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.SalesManagement.InvoiceOrder
{
    public class ResultObjects : IBaseResultObjects
    {


        public int BPLId { get; set; }
        public string ApprovalStatus { get; set; }

        public string Canceled { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string ClosedRecSum { get; set; }
        public string CntctCode { get; set; }
        public string CreateActionId { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string CreateUserSign { get; set; }
        public string DataOwner { get; set; }
        public string Deleted { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string DiscPrcnt { get; set; }
        public double DiscSum { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string DocNum { get; set; }
        public double DocTotal { get; set; }
        public DateTime DocumentDate { get; set; }
        public string DocumentStatus { get; set; }
        public string draftKey { get; set; }
        public string Handwritten { get; set; }
        public string Instance { get; set; }
        public string LogInst { get; set; }

        public string OpenRecSum { get; set; }
        public double OwnerCode { get; set; }
        public string Period { get; set; }
        public string Pick { get; set; }
        public string PickStatus { get; set; }
        public DateTime PostingDate { get; set; }
        public List<ReceivableItems> ReceivableItems { get; set; }
        public string Referenced { get; set; }
        public string Series { get; set; }
        public string SlpCode { get; set; }
        public string Status { get; set; }
        public string Transfered { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateTime { get; set; }
        public string UpdateUserSign { get; set; }
        public string UserSign { get; set; }
        public double VatSum { get; set; }
        public string BPLName { get; set; }
        public double Subtotal { get; set; }
    }
}
