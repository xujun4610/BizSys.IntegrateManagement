using System;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.Materials;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.SAPToOmniChannel.Service.B1Common
{
    public class OrderCopy
    {
        public static ResultObjects CopyMaterials(ResultObjects fromMaterial, ResultObjects material)
        {
            #region 赋值

            material.QCTemplateCode = fromMaterial.QCTemplateCode;
            material.Active = fromMaterial.Active;
            material.ActiveFrom = fromMaterial.ActiveFrom;
            material.ActiveTo = fromMaterial.ActiveTo;
            material.ApprovalStatus = fromMaterial.ApprovalStatus;
            material.Assemblied = fromMaterial.Assemblied;
            material.AssemblyItem = fromMaterial.AssemblyItem;
            material.AvgPrice = fromMaterial.AvgPrice;
            material.BarCode = fromMaterial.BarCode;
            material.BatchNumberManagement = fromMaterial.BatchNumberManagement;
            material.Canceled = fromMaterial.Canceled;
            material.CategoryCode = fromMaterial.CategoryCode;
            material.CreateActionId = fromMaterial.CreateActionId;
            material.CreateDate = fromMaterial.CreateDate;
            material.CreateTime = fromMaterial.CreateTime;
            material.CreateUserSign = fromMaterial.CreateUserSign;
            material.CurrencyCode = fromMaterial.CurrencyCode;
            material.DataOwner = fromMaterial.DataOwner;
            material.DataSource = fromMaterial.DataSource;
            material.DefaultBOMVersion = fromMaterial.DefaultBOMVersion;
            material.DefaultWarehouse = fromMaterial.DefaultWarehouse;
            material.Deleted = fromMaterial.Deleted;
            material.DiscountForUse = fromMaterial.DiscountForUse;
            material.FaceValue = fromMaterial.FaceValue;
            material.FixedAssets = fromMaterial.FixedAssets;
            material.ForeignDescription = fromMaterial.ForeignDescription;
            material.Height = fromMaterial.Height;
            material.InventoryItem = fromMaterial.InventoryItem;
            material.InventoryUoM = fromMaterial.InventoryUoM;
            material.MinimumInventoryLevel = fromMaterial.MinimumInventoryLevel;
            material.LogInst = fromMaterial.LogInst;
            material.IsCommited = fromMaterial.IsCommited;
            material.IssueMethod = fromMaterial.IssueMethod;
            material.ItemDescription = fromMaterial.ItemDescription;
            material.ItemGroup = fromMaterial.ItemGroup;
            material.ItemType = fromMaterial.ItemType;
            material.Length = fromMaterial.Length;
            material.MinimumOrderQuantity = fromMaterial.MinimumOrderQuantity;
            material.NoOfItemsPerPurchaseUnit = fromMaterial.NoOfItemsPerPurchaseUnit;
            material.NoOfItemsPerSalesUnit = fromMaterial.NoOfItemsPerSalesUnit;
            material.OnHand = fromMaterial.OnHand;
            material.OnOrder = fromMaterial.OnOrder;
           // material.Organization = fromMaterial.Organization;
            material.PhantomItem = fromMaterial.PhantomItem;
            material.Picture = fromMaterial.Picture;
            material.ProcurementMethod = fromMaterial.ProcurementMethod;
            material.PurchaseItem = fromMaterial.PurchaseItem;
            material.PurchasingUoM = fromMaterial.PurchasingUoM;
            material.Referenced = fromMaterial.Referenced;
            material.Remarks = fromMaterial.Remarks;
            material.RoutingCode = fromMaterial.RoutingCode;
            material.SalesItem = fromMaterial.SalesItem;
            material.SerialNumberManagement = fromMaterial.SerialNumberManagement;
            material.Series = fromMaterial.Series;
            material.SalesUoM = fromMaterial.SalesUoM;
            material.ServiceCardType = fromMaterial.ServiceCardType;
            material.ServiceNumberManagement = fromMaterial.ServiceNumberManagement;
            material.TeamMembers = fromMaterial.TeamMembers;
            material.UpdateActionId = fromMaterial.UpdateActionId;
            material.UpdateDate = fromMaterial.UpdateDate;
            material.ValidDays = fromMaterial.ValidDays;
            material.Width = fromMaterial.Width;
            material.UpdateTime = fromMaterial.UpdateTime;
            material.UpdateUserSign = fromMaterial.UpdateUserSign;
            #endregion
            return material;
        }

    }
}
