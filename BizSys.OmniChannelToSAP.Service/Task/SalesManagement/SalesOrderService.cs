using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.IntegrateManagement.Entity.SalesManagement.SalesDeliveryOrder;
using BizSys.IntegrateManagement.Entity.SalesManagement.SalesReturnOrder;
using BizSys.IntegrateManagement.Entity.SalesManagement.SalesOrder;
using BizSys.OmniChannelToSAP.Service.Service.SalesManagementServcie;
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
using BizSys.OmniChannelToSAP.Service.B1Common;
using System.Threading;

namespace BizSys.OmniChannelToSAP.Service.Task.SalesManagement
{
    public class SalesOrderService : IWindowsService
    {
        //private readonly static object obj = new object();
        public void Run()
        {
            //销售订单
            // for 梅菲特
            //lock (obj)
            //{
            //    GetSalesOrderService.GetSalesOrder();
            //}
            GetSalesOrderService.GetSalesOrder4Test();

            /*
            //销售退货单
            GetSalesReturnOderService.GetSalesReturnOder();

            GetSalesDeliveryOrderService.GetSalesDeliveryOrder();

            //销售订单生成销售交货/应收预留发票
            GetSalesOrderByB1Service.HandleSalesOrder();

            //销售交货单生成应收发票
            GetSalesDeliveryByB1Service.HandleSalesDeliveryOrder();
            //取消销售订单
            GetCancelOrCloseSalesOrderService.GetCancelSalesOrder();
            */
        }

        public void Stop()
        {

        }






    }
}
