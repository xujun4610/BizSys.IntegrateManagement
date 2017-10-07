using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.Supplier;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.Materials;
using BizSys.IntegrateManagement.Entity.PurchaseOrder;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.IntegrateManagement.Entity.SalesManagement.SalesOrder;
using BizSys.OmniChannelToSAP.Service.Service.MasterDataManagementService;
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

namespace BizSys.OmniChannelToSAP.Service.Task.MaterialsManagement
{
    public class MasterDataService:IWindowsService
    {

        public  void Run()
        {
            GetSupplierService.GetSupplier();//供应商
            GetCustormerService.GetCustomer();//客户主数据
           //GetCapitalPlanService.GetCapitalPlan();//资金计划
            GetIncomeBudgetService.GetIncomeBudget();//收入预算
            GetCostBudgetService.GetCostBudget();//费用预算
            //GetSalesPomotionService.GetSalesPomotion();//促销活动
        }

        public void Stop()
        {
            
        }

    }
}
