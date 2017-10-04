using BizSys.IntegrateManagement.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.ReceiptPayment.InvoiceReceivable
{
    /// <summary>
    /// 应收发票
    /// </summary>
    public class InvoiceReceivableRootObject: IBaseRootObjects<ResultObjects>
    {
     
        public List<Informations> Informations { get; set; }
    }
}
