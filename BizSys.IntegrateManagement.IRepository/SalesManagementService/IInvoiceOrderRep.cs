using BizSys.IntegrateManagement.Entity.SalesManagement.InvoiceOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.IRepository.SalesManagementService
{
    public interface IInvoiceOrderRep
    {
        InvoiceOrderRootObject GetInvoiceByKey(int DocEntry);
    }
}
