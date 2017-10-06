using BizSys.IntegrateManagement.Entity.Base;
using BizSys.IntegrateManagement.Entity.PurchaseDeliveryOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.PurchaseManagement.PurchaseReturn
{
   public class PurchaseReturnRootObject: IBaseRootObjects<ResultObjects>
    {
      
        public List<Informations> Informations { get; set; }
        
    }
}
