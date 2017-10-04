using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.MasterDataManagement.MaterialsCategory
{
    public class ResultObjects
    {
        public string type { get; set; }
        public string isDirty { get; set; }
        public string isDeleted { get; set; }
        public string isNew { get; set; }
        public string CanHaveItem { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string CreateActionId { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string CreateUserSign { get; set; }
        public string DataSource { get; set; }
        public string Deleted { get; set; }
        public string Level { get; set; }
        public string LogInst { get; set; }
        public string ObjectCode { get; set; }
        public string ObjectKey { get; set; }
        public string ParentCode { get; set; }
        public string Referenced { get; set; }
        public string Series { get; set; }
        public string UpdateActionId { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateTime { get; set; }
        public string UpdateUserSign { get; set; }
    }
}
