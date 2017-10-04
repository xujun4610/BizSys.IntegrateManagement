using BizSys.IntegrateManagement.Entity.SalesManagement.SalesDeliveryOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.IRepository.SalesManagementService
{
    public interface ISalesDeliveryOrderRep
    {
       
        SalesDeliveryOrderRootObject GetSalesDeliveryOrderByKey(string DocEntry);
    }
}
