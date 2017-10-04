using BizSys.SAPToOmniChannel.Service.Service.StockManagementService;
using MagicBox.WindowsServices.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.SAPToOmniChannel.Service.Task.StockManagement
{
    public class StockService:IWindowsService
    {

        public void Run()
        {
            //PostMaterialsInventoryService.PostMaterialsInventory();//推送库存信息
        }

        public void Stop()
        {
        }
    }
}
