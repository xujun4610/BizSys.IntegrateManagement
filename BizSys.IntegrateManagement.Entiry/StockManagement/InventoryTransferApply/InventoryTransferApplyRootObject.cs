using BizSys.IntegrateManagement.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.StockManagement.InventoryTransferApply
{
    public class InventoryTransferApplyRootObject: IBaseRootObjects<ResultObjects>
    {
        ///**********************************库存转储申请**************************************//
        ///  public string type { get; set; }
       
        public List<Informations> Informations { get; set; }
        
    }
}
