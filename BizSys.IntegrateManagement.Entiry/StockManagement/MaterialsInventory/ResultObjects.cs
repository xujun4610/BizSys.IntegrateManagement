using BizSys.IntegrateManagement.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.StockManagement.MaterialsInventory
{
    public class ResultObjects: IBaseResultObjects
    {
       
        public double AvgPrice { get; set; }
        public string CreateActionId { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateTime { get; set; }
        public string CreateUserSign { get; set; }
        public string Deleted { get; set; }
        public double IsCommited { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public double LogInst { get; set; }
   
        public int ObjectKey { get; set; }
        public double OnHand { get; set; }
        public double OnOrder { get; set; }
        public string Referenced { get; set; }
        public string Series { get; set; }
        public string UpdateActionId { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateTime { get; set; }
        public string UpdateUserSign { get; set; }
        public string WarehouseCode { get; set; }
    }
}
