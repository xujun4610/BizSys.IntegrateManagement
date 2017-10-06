using BizSys.IntegrateManagement.Entity.ReceiptPayment.PaymentApply;
using BizSys.IntegrateManagement.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.ReceiptPayment.PaymentApply
{
    public class PaymentApplyRootObject: IBaseRootObjects<ResultObjects>
    {
        public List<Informations> Informations { get; set; }
    }
}
