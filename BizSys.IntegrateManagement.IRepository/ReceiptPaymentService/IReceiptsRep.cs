using BizSys.IntegrateManagement.Entity.ReceiptPayment.Receipt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.IRepository.ReceiptPaymentService
{
    public interface IReceiptsRep
    {
        ReceiptRootObject GetReceiptsByKey(string DocEntry);
    }
}
