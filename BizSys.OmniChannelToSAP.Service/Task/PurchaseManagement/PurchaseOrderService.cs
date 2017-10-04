using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity;
using BizSys.IntegrateManagement.Entity.PurchaseManagement.PurchaseReturn;
using BizSys.IntegrateManagement.Entity.PurchaseOrder;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.OmniChannelToSAP.Service.Service.PurchaseManagementServcie;
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

namespace BizSys.OmniChannelToSAP.Service.Task.PurchaseManagement
{
    public class PurchaseOrderService:IWindowsService
    {
        public void Run()
        {
            //采购订单
            GetPurchaseOrderService.GetPurchaseOrder();
            //采购退货单
            GetPurchaseReturnService.GetPurchaseReturn();
            //取消采购订单
            GetCancelOrClosePurchaseOrderService.GetCancelPuchaseOrder();
        }


        public void Stop()
        {
            
        }

      
       
       
    }
}
