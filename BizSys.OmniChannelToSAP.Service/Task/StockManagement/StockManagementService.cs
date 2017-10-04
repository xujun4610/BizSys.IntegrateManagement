using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity;
using BizSys.OmniChannelToSAP.Service.Service.StockManagementServcie;
using MagicBox.Log;
using MagicBox.WindowsServices.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.OmniChannelToSAP.Service.Task.StockManagement
{
    public class StockManagementService:IWindowsService
    {
        public void Run()
        {
            GetInventoryTransferApplyService.GetInventoryTransfer();//获取库存转储申请单
            //GetInventoryUpdateService.GetInventoryUpdate();//获取库存过账
            GetGoodsIssueService.GetGoodsIssue();//库存发货
            GetGoodsReceiptService.GetGoodsReceipts();//库存收货
        }

        public void Stop()
        {
            
        }
    }
}
