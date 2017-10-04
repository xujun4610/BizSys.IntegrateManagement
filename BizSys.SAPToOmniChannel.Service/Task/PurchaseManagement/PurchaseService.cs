using BizSys.SAPToOmniChannel.Service.Service.PurchaseManagementService;
using MagicBox.WindowsServices.Common;

namespace BizSys.SAPToOmniChannel.Service.Task.PurchaseManagement
{
    public class PurchaseService : IWindowsService
    {
        public void Run()
        {
            //推送采购交货
            PostPurchaseDeliveryService.PostPurchaseDelivery();
            PostPurchaseInvoiceService.PostPurchaseInvoice();
        }



        public void Stop()
        {
            
        }


    }
}
