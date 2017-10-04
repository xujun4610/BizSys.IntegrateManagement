using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.PurchaseManagement.PurchaseInvoice
{
    public  class PayableItems
    {
        public string isDirty { get; set; }
        public string isDeleted { get; set; }
        public string isNew { get; set; }
        public double GTotal { get; set; }
        public int BaseEntry { get; set; }
        public int BaseLine { get; set; }
        public string BaseOpnQty { get; set; }
        public string BaseQty { get; set; }
        public string BaseRef { get; set; }
        public string BaseType { get; set; }
        public int BsDocEntry { get; set; }
        public int BsDocLine { get; set; }
        public string BsDocType { get; set; }
        public string Canceled { get; set; }
        public string CreateActionId { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string CreateUserSign { get; set; }
        public string Currency { get; set; }
        public string DataSource { get; set; }
        public string Deleted { get; set; }
        public double DelivrdQty { get; set; }
        public double DiscPrcnt { get; set; }
        public string DocDate { get; set; }
        public string DocEntry { get; set; }
        public string Dscription { get; set; }
        public string FreeTxt { get; set; }
        public string IsCommited { get; set; }
        public string ItemCode { get; set; }
        public string LineId { get; set; }
        public string LineNum { get; set; }
        public string LineStatus { get; set; }
        public double LineTotal { get; set; }
        public string LogInst { get; set; }
        public string ObjType { get; set; }
        public string ObjectCode { get; set; }
        public string OcrCode { get; set; }
        public string OcrCode2 { get; set; }
        public string OcrCode3 { get; set; }
        public string OcrCode4 { get; set; }
        public string OcrCode5 { get; set; }
        public string OnHand { get; set; }
        public string OnOrder { get; set; }
        public double OpenQty { get; set; }
        public string OrderedQty { get; set; }
        public string OwnerCode { get; set; }
        public string PickIdNo { get; set; }
        public string PickOty { get; set; }
        public string PickStatus { get; set; }
        public double Price { get; set; }
        public double PriceAfVAT { get; set; }
        public string QtyToShip { get; set; }
        public double Quantity { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string Referenced { get; set; }
        public DateTime ShipDate { get; set; }
        public string SlpCode { get; set; }
        public string Status { get; set; }
        public string TargetType { get; set; }
        public string Text { get; set; }
        public string TrgetEntry { get; set; }
        public string UpdateActionId { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateTime { get; set; }
        public string UpdateUserSign { get; set; }
        public double VatSum { get; set; }
        public string VisOrder { get; set; }
        public string WhsCode { get; set; }
    }
}
