﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.SalesManagement.SalesOrder
{
    public class SalesOrderItems
    {

        public string isDeleted { get; set; }
        public string isDirty { get; set; }
        public string isNew { get; set; }
        public string AccountCode { get; set; }
        public string Activities { get; set; }
        public string BarCode { get; set; }
        public string BaseDocumentEntry { get; set; }
        public string BaseDocumentLineId { get; set; }
        public string BaseDocumentType { get; set; }
        public double BasePriceforGrossProfit { get; set; }
        public string BaseReference { get; set; }
        public string BatchNumberManagement { get; set; }
        public string Canceled { get; set; }
        public string CreateActionId { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string CreateUserSign { get; set; }
        public string Cubage { get; set; }
        public string CurrencyRate { get; set; }
        public string DataSource { get; set; }
        public string Deleted { get; set; }
        public string DeliveredQuantity { get; set; }
        public string DeliveryDate { get; set; }
        public string DiscountPerLine { get; set; }
        public string DistributionRule1 { get; set; }
        public string DistributionRule2 { get; set; }
        public string DistributionRule3 { get; set; }
        public string DistributionRule4 { get; set; }
        public string DistributionRule5 { get; set; }
        public string DocEntry { get; set; }
        public string GoodsType { get; set; }
        public double GrossPrice { get; set; }
        public double GrossTotal { get; set; }
        public string InventoryUoM { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string ItemType { get; set; }
        public string ItmsCgrCode { get; set; }
        public string LineGrossProfit { get; set; }
        public string LineId { get; set; }
        public string LineSign { get; set; }
        public string LineStatus { get; set; }
        public double LineTotal { get; set; }
        public string LineWasClosedManually { get; set; }
        public string LogInst { get; set; }
        public string ObjectCode { get; set; }
        public string OpenAmount { get; set; }
        public string OpenQuantity { get; set; }
        public string OriginalDocumentEntry { get; set; }
        public string OriginalDocumentLineId { get; set; }
        public string OriginalDocumentType { get; set; }
        public string ParentLineSign { get; set; }
        public string ParticipateOtherPromotion { get; set; }
        public double Price { get; set; }
        public string PriceCurrency { get; set; }
        public string PriceListforGrossProfit { get; set; }
        public string ProjectCode { get; set; }
        public string PromotionCode { get; set; }
        public string Quantity { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string Referenced { get; set; }
        public string SalesCommission { get; set; }
        public string SerialNumberManagement { get; set; }
        public string ServiceNumberManagement { get; set; }
        public string Status { get; set; }
        public string TargetDocumentEntry { get; set; }
        public string TargetDocumentType { get; set; }
        public string TaxDefinition { get; set; }
        public double TaxRatePerLine { get; set; }
        public double TotalTaxLine { get; set; }
        public string TreeBasisQuantity { get; set; }
        public string TreeType { get; set; }
        public double UnitPrice { get; set; }
        public string UpdateActionId { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateTime { get; set; }
        public string UpdateUserSign { get; set; }
        public string VendorCatalogNumber { get; set; }
        public string VisOrder { get; set; }
        public string Warehouse { get; set; }
        //梅菲特定制字段
        public string GrossPriceTemp { get; set; } //原始毛价
    }
}
