using BizSys.OmniChannelToSAP.Service.Service.CustomerService;
using MagicBox.WindowsServices.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.OmniChannelToSAP.Service.Task.CustomerService
{
    public class CustomerService : IWindowsService
    {
        public void Run()
        {
            GetCustomerServiceApplyService.GetCustomerServiceApply();
        }

        public void Stop()
        {
            
        }
    }
}
