using BizSys.OmniChannelToSAP.Service.Service.SalesManagementServcie;
using MagicBox.Log;
using MagicBox.WindowsServices.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace BizSys.OmniChannelToSAP.Service.Task.SalesManagement
{
    class SalesDeliveryService : IWindowsService
    {
        public void Run()
        {
            Logger.Writer(string.Format("[ThreadID:{0},IsBackground:{1}]开始执行【销售交货】同步",Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsBackground));
            GetSalesDeliveryOrderService.GetSalesDeliveryOrderSync();

            GC.Collect();
        }

        public void Stop()
        {
            Logger.Writer(string.Format("[ThreadID:{0},IsAlive:{1}]结束【销售交货】同步", Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsAlive));

        }
    }
}
