using BizSys.SAPToOmniChannel.Service.Service.SalesManagementService;
using MagicBox.WindowsServices.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.SAPToOmniChannel.Service.Task.SalesManagement
{
    public class SalesService:IWindowsService
    {
        public void Run()
        {
            PostSalesDeliveryOrderService.PostSalesDeliveryOrder();
            PostInvoiceService.PostInvoice();
        }

        public void Stop()
        {
           
        }
    }
}
