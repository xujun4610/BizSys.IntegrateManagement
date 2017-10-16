using MagicBox.WindowsServices.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.OmniChannelToSAP.Service.Task.SalesManagement
{
    class SalesDeliveryService : IWindowsService
    {
        public void Run()
        {
            BizSys.OmniChannelToSAP.Service.Service.SalesManagementServcie.GetSalesDeliveryOrderService.GetSalesDeliveryOrder();
        }

        public void Stop()
        {
        }
    }
}
