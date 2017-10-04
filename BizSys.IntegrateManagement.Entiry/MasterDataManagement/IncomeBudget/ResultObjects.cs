using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.MasterDataManagement.IncomeBudget
{
    public class ResultObjects
    {
        public string type { get; set; }
        public string isDirty { get; set; }
        public string isDeleted { get; set; }
        public string isNew { get; set; }
        public string ApprovalStatus { get; set; }
        public string BeginDate { get; set; }
        public string CreateActionId { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string CreateUserSign { get; set; }
        public string DataOwner { get; set; }
        public string DataSource { get; set; }
        public string DeptId { get; set; }
        public string DeptName { get; set; }
        public string EndDate { get; set; }
        public List<InComeBudgetItems> InComeBudgetItems { get; set; }
        public string LogInst { get; set; }
        public string ObjectCode { get; set; }
        public string ObjectKey { get; set; }
        public string Organization { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string Remarks { get; set; }
        public string Series { get; set; }
        public string TeamMembers { get; set; }
        public string UpdateActionId { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateTime { get; set; }
        public string UpdateUserSign { get; set; }
        public string Year { get; set; }
    }
}
