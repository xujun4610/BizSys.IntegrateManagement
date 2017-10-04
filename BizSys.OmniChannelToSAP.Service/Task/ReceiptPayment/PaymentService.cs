using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity;
using BizSys.IntegrateManagement.Entity.ReceiptPayment.Payment;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.IntegrateManagement.Entity.SalesOrder;
using BizSys.OmniChannelToSAP.Service.Service.ReceiptPaymentService;
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

namespace BizSys.OmniChannelToSAP.Service.Task.ReceiptPayment
{
    public class PaymentService:IWindowsService
    {

        public void Run()
        {
            //付款申请
            GetPayment();
            GetReceiptVerificationService.GetReceiptVerification();
            GetCostReimbursementService.GetCostReimbursement();
        }

        public void Stop()
        {
           
        }

        private async void GetPayment()
        {
            await GetPaymentApplyService.GetPayment();
        }

    }
}
