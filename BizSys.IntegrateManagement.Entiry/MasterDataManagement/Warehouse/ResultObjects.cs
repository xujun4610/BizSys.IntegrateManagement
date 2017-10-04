using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.MasterDataManagement.Warehouse
{
    public class ResultObjects
    {
        public string type { get; set; }
        public bool isDirty { get; set; }
        public bool isDeleted { get; set; }
        public bool isNew { get; set; }
        public string Activated { get; set; }
        public string ApprovalStatus { get; set; }
        public string CreateActionId { get; set; }
        public string CreateDate { get; set; }
        public int CreateTime { get; set; }
        public string CreateUserSign { get; set; }
        public string DataOwner { get; set; }
        public string DataSource { get; set; }
        public string Deleted { get; set; }
        public string EstimateWorkDays { get; set; }
        public int LogInst { get; set; }
        public string ObjectCode { get; set; }
        public string ObjectKey { get; set; }
        public string Organization { get; set; }
        public string Referenced { get; set; }
        public string Series { get; set; }
        public string TeamMembers { get; set; }
        public string UpdateActionId { get; set; }
        public string UpdateDate { get; set; }
        public int UpdateTime { get; set; }
        public string UpdateUserSign { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public string WhsType { get; set; }
        public string Workload { get; set; }
    }
}
