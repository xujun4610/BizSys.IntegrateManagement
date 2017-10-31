using BizSys.OmniChannelToSAP.Service.Service.SalesManagementServcie;
using MagicBox.Log;
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
            Logger.Writer(string.Format("开始执行【销售交货】同步"));
            GetSalesDeliveryOrderService.GetSalesDeliveryOrder();
            GC.Collect();
        }

        public void Stop()
        {
        }
    }
}
