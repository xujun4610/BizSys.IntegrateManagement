using BizSys.IntegrateManagement.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.MasterDataManagement.Materials
{
    public class ResultObjects
    {

        public string type { get; set; }
        public bool isDirty { get; set; }
        public bool isDeleted { get; set; }
        public bool isNew { get; set; }
        public string QCTemplateCode { get; set; }
        public string Active { get; set; }
        public string ActiveFrom { get; set; }
        public string ActiveTo { get; set; }
        public string ApprovalStatus { get; set; }
        public string Assemblied { get; set; }
        public string AssemblyItem { get; set; }
        public string AvgPrice { get; set; }
        public string BarCode { get; set; }
        public string BatchNumberManagement { get; set; }
        public string Brand { get; set; }
        public string Canceled { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string Colour { get; set; }
        public string CreateActionId { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateTime { get; set; }
        public string CreateUserSign { get; set; }
        public string CurrencyCode { get; set; }
        public string DataOwner { get; set; }
        public string DataSource { get; set; }
        public string DefaultBOMVersion { get; set; }
        public string DefaultWarehouse { get; set; }
        public string Deleted { get; set; }
        public string DiscountForUse { get; set; }
        public string FaceValue { get; set; }
        public string FixedAssets { get; set; }
        public string ForeignDescription { get; set; }
        public string Height { get; set; }
        public string InventoryItem { get; set; }
        public string InventoryUoM { get; set; }
        public string IsCommited { get; set; }
        public string IssueMethod { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string ItemGroup { get; set; }
        public string ItemType { get; set; }
        public string LeadTime { get; set; }
        public string Length { get; set; }
        public int LogInst { get; set; }
        public string MinimumInventoryLevel { get; set; }
        public string MinimumOrderQuantity { get; set; }
        public string Model { get; set; }
        public string FactoryCode { get; set; }
        public double NoOfItemsPerPurchaseUnit { get; set; }
        public double NoOfItemsPerSalesUnit { get; set; }
        public string ObjectCode { get; set; }
        public string ObjectKey { get; set; }
        public string OnHand { get; set; }
        public double OnOrder { get; set; }
        public string ParentCategoryCode { get; set; }
        public string ParentCategoryName { get; set; }
        public string PhantomItem { get; set; }
        public string Picture { get; set; }
        public string PlanningMethod { get; set; }
        public string PreferredVendor { get; set; }
        public string PricePerTime { get; set; }
        public string ProcurementMethod { get; set; }
        public string PurchaseItem { get; set; }
        public double PurchaseTax { get; set; }
        public string PurchasingUoM { get; set; }
        public string Referenced { get; set; }
        public string Remarks { get; set; }
        public string RoutingCode { get; set; }
        public double SaleTax { get; set; }
        public string SalesItem { get; set; }
        public string SalesUoM { get; set; }
        public string SerialNumberManagement { get; set; }
        public string Series { get; set; }
        public string ServiceCardType { get; set; }
        public string ServiceNumberManagement { get; set; }
        public string TeamMembers { get; set; }
        public string UpdateActionId { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateTime { get; set; }
        public string UpdateUserSign { get; set; }
        public string ValidDays { get; set; }
        public string Weight { get; set; }
        public string Width { get; set; }
    }
}
