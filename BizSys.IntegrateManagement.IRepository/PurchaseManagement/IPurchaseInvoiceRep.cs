using BizSys.IntegrateManagement.Entity.PurchaseManagement.PurchaseInvoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.IRepository.PurchaseManagement
{
    public interface  IPurchaseInvoiceRep
    {
        PurchaseInvoiceRootObject GetPurchaseInvoiceByKey(string DocEntry);
    }
}
