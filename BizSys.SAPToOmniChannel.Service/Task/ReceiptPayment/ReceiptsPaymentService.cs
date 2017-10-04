using BizSys.SAPToOmniChannel.Service.Service.ReceiptPaymentService;
using MagicBox.WindowsServices.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.SAPToOmniChannel.Service.Task.ReceiptPayment
{
    public class ReceiptsPaymentService:IWindowsService
    {
        public void Run()
        {
            PostReceiptService.PostReceipt();
            PostPaymentService.PostPayment(); 
        }

        public void Stop()
        {
            
        }
    }
}
