using BizSys.IntegrateManagement.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.ReceiptPayment.InvoiceIssuable
{
    /// <summary>
    /// 应付发票
    /// </summary>
    public class InvoiceIssuableRootObject: IBaseRootObjects<ResultObjects>
    {
     
        public List<Informations> Informations { get; set; }
        
    }
}
