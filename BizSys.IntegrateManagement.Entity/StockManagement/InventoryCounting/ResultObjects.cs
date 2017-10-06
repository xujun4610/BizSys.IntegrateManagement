using BizSys.IntegrateManagement.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.StockManagement.InventoryCounting
{
    public class ResultObjects: IBaseResultObjects
    {
      
        public string ApprovalStatus { get; set; }
        public string Canceled { get; set; }
        public string CountDate { get; set; }
        public string CountTime { get; set; }
        public string CountType { get; set; }
        public string CreateActionId { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateTime { get; set; }
        public int CreateUserSign { get; set; }
        public string CustomType { get; set; }
        public int DataOwner { get; set; }
        public string DataSource { get; set; }
        public string Deleted { get; set; }
        public DateTime DeliveryDate { get; set; }
       
        public string DocNum { get; set; }
        public DateTime DocumentDate { get; set; }
        public string DocumentStatus { get; set; }
        public string Handwritten { get; set; }
        public string Instance { get; set; }
        public List<InventoryCountingLines> InventoryCountingLines { get; set; }
        public string LogInst { get; set; }
      
        public string Organization { get; set; }
        public string Period { get; set; }
        public DateTime PostingDate { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string Referenced { get; set; }
        public string Remarks { get; set; }
        public string Series { get; set; }
        public string Status { get; set; }
        public string Taker1Id { get; set; }
        public string Taker1Type { get; set; }
        public string Taker2Id { get; set; }
        public string Taker2Type { get; set; }
        public string TeamMembers { get; set; }
        public string Transfered { get; set; }
        public string UpdateActionId { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateTime { get; set; }
        public string UpdateUserSign { get; set; }
        public string UserSign { get; set; }
    }
}
