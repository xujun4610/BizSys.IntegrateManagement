using BizSys.IntegrateManagement.Entity.MasterDataManagement.SalesPomotion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.MasterDataManagement.SalesPomotion
{
    public class ResultObjects
    {
        public string type { get; set; }
        public string isDirty { get; set; }
        public string isDeleted { get; set; }
        public string isNew { get; set; }
        public string Activated { get; set; }
        public string AmountInvested { get; set; }
        public string ApprovalStatus { get; set; }
        public string CreateActionId { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string CreateUserSign { get; set; }
        public string DataOwner { get; set; }
        public string DebitService { get; set; }
        public string DebitServiceNumber { get; set; }
        public string Deleted { get; set; }
        public string EndDate { get; set; }
        public string ImageFileName { get; set; }
        public string ItemGroupActivated { get; set; }
        public string ItemGroupCode { get; set; }
        public string ItemGroupPrice { get; set; }
        public string ItemGroupPriceListNumber { get; set; }
        public string LogInst { get; set; }
        public string ObjectCode { get; set; }
        public string ObjectKey { get; set; }
        public string Organization { get; set; }
        public string PromMoney { get; set; }
        public string PromStatus { get; set; }
        public string PromType { get; set; }
        public string PromotionCode { get; set; }
        public string PromotionName { get; set; }
        public string PromotionProductsBySP { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string Referenced { get; set; }
        public string Remarks { get; set; }
        public List<SalesPromotionJoinConditions> SalesPromotionJoinConditions { get; set; }
        public List<SalesPromotionRegulationss> SalesPromotionRegulationss { get; set; }
        public string Series { get; set; }
        public string StartDate { get; set; }
        public string TeamMembers { get; set; }
        public string U_SBOSynchronization { get; set; }
        public string UpdateActionId { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateTime { get; set; }
        public string UpdateUserSign { get; set; }
    }
}
