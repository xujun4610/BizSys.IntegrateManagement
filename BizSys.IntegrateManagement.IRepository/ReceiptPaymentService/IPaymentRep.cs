using BizSys.IntegrateManagement.Entity.ReceiptPayment.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.IRepository.ReceiptPaymentService
{
    public interface IPaymentRep
    {
        PaymentRootObject GetReceiptsByKey(string DocEntry);
    }
}
