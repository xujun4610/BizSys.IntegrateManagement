using BizSys.IntegrateManagement.Entity.PurchaseDeliveryOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.IRepository.PurchaseManagement
{
    public interface IPurchaseDeliveryRep
    {
        PurchaseDeliveryOrderRootObject GetPurchaseDeliveryOrderByKey(string DocEntry);
    }
}
