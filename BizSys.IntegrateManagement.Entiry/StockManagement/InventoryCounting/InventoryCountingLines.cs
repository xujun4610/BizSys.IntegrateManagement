using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.StockManagement.InventoryCounting
{
    public class InventoryCountingLines
    {
        public string isDirty { get; set; }
        public string isDeleted { get; set; }
        public string isNew { get; set; }
        public string BarCode { get; set; }
        public string BatchNumberManagement { get; set; }
        public string Canceled { get; set; }
        public string CountDate { get; set; }
        public string CountDirection { get; set; }
        public string CountTime { get; set; }
        public string Counted { get; set; }
        public string CountedQuantity { get; set; }
        public string Counter1CountedQuantity { get; set; }
        public string Counter2CountedQty { get; set; }
        public string CreateActionId { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string CreateUserSign { get; set; }
        public string DataSource { get; set; }
        public string Deleted { get; set; }
        public string DifferencePercentage { get; set; }
        public string DocEntry { get; set; }
        public string InWhsQtyOnCountDate { get; set; }
        public string InventoryUoM { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string ItemFreezeStatus { get; set; }
        public string LineId { get; set; }
        public string LineStatus { get; set; }
        public string LogInst { get; set; }
        public string ObjectCode { get; set; }
        public double Quantity { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string Referenced { get; set; }
        public string SerialNumberManagement { get; set; }
        public string Status { get; set; }
        public string TakerMaster { get; set; }
        public string UpdateActionId { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateTime { get; set; }
        public string UpdateUserSign { get; set; }
        public string VisOrder { get; set; }
        public string WarehouseCode { get; set; }
    }
}
