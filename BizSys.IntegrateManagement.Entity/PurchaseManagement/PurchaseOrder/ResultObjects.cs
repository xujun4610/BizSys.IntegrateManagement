using BizSys.IntegrateManagement.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.PurchaseOrder
{
    public class ResultObjects: IBaseResultObjects
    {
      
        public string BPReferenceNumber { get; set; }
        public string ApprovalStatus { get; set; }
       
        public string BillTo { get; set; }
        public string BusinessPartnerCode { get; set; }
        public string BusinessPartnerName { get; set; }
        public string Canceled { get; set; }
        public int ContactPerson { get; set; }
        public string BillType { get; set; }
        public string CreateActionId { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string CreateUserSign { get; set; }
        public string CustomType { get; set; }
        public string DiscountPercent { get; set; }
        public string VatSum { get; set; }
        public int DataOwner { get; set; }
        public string DataSource { get; set; }
        public string Deleted { get; set; }
        public string DeliveryDate { get; set; }
        public string Discount { get; set; }
        public string DiscountForDocument { get; set; }
       
        public string DocNum { get; set; }
        public string DocumentCurrency { get; set; }
        public string DocumentDate { get; set; }
        public string DocumentRate { get; set; }
        public string DocumentStatus { get; set; }
        public string DocumentTotal { get; set; }
        public string GrossProfit { get; set; }
        public string Handwritten { get; set; }
        public string Instance { get; set; }
        public string LogInst { get; set; }
      
        public string Organization { get; set; }
        public string PaidToDate { get; set; }
        public string PaymentTermsCode { get; set; }
        public string Period { get; set; }
        public DateTime PostingDate { get; set; }
        public string PriceListforGrossProfit { get; set; }
        public string Printed { get; set; }
       
        public string ProjectCode { get; set; }
        public double PriceAfterVAT { get; set; }
        public List<PurchaseOrderItems> PurchaseOrderItems { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string Referenced { get; set; }
        public string Remarks { get; set; }
        public string Rounding { get; set; }
        public string RoundingDiffAmount { get; set; }
        public string Series { get; set; }
        public string ShipTo { get; set; }
        public string Status { get; set; }
        public string TaxRate { get; set; }
        public string TeamMembers { get; set; }
        public string TotalDiscount { get; set; }
        public string TotalNet { get; set; }
        public string TotalPaidSum { get; set; }
        public string TotalQuantity { get; set; }
        public string TotalTax { get; set; }
        public string Transfered { get; set; }
        public string UpdateActionId { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateTime { get; set; }
        public string UpdateUserSign { get; set; }
        public int UserSign { get; set; }
    }
}
