using BizSys.IntegrateManagement.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.PurchaseManagement.PurchaseInvoice
{
    public class PurchaseInvoiceRootObject: IBaseRootObjects<ResultObjects>
    {
        public List<Informations> Informations { get; set; }
    }
}
