using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.MasterDataManagement.Customer
{
    public class CustomerItems
    {
        public string isDeleted { get; set; }
        public string isDirty { get; set; }
        public string isNew { get; set; }
        public int AddressCode { get; set; }
        public string BillToStreet { get; set; }
        public string BillToZipCode { get; set; }
        public string City { get; set; }
        public string ContactPerson { get; set; }
        public string County { get; set; }
        public string CreateActionId { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateTime { get; set; }
        public int CreateUserSign { get; set; }
        public string DataSource { get; set; }
        public string DefaltAddress { get; set; }
        public string Deleted { get; set; }
        public string Email { get; set; }
        public string FaxNumber { get; set; }
        public int LineId { get; set; }
        public int LogInst { get; set; }
        public string MobilePhone { get; set; }
        public string ObjectCode { get; set; }
        public int ObjectKey { get; set; }
        public string Province { get; set; }
        public string Reference1 { get; set; }
        public string Reference2 { get; set; }
        public string Referenced { get; set; }
        public string Remarks { get; set; }
        public string Telephone1 { get; set; }
        public string Telephone2 { get; set; }
        public string Town { get; set; }
        public string UpdateActionId { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateTime { get; set; }
        public int UpdateUserSign { get; set; }
    }
}
