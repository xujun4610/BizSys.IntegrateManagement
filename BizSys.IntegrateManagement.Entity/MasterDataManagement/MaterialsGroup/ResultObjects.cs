using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.MasterDataManagement.MaterialsGroup
{
    public  class ResultObjects
    {
        public string type { get; set; }
        public bool isDirty { get; set; }
        public bool isDeleted { get; set; }
        public bool isNew { get; set; }
        public string CreateActionId { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string CreateUserSign { get; set; }
        public string DataSource { get; set; }
        public string Deleted { get; set; }
        public int ItemsGroupCode { get; set; }
        public string ItemsGroupName { get; set; }
        public string Locked { get; set; }
        public string LogInst { get; set; }
        public string ObjectCode { get; set; }
        public string ObjectKey { get; set; }
        public string Referenced { get; set; }
        public string Series { get; set; }
        public string UpdateActionId { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateTime { get; set; }
        public string UpdateUserSign { get; set; }
    }
}
