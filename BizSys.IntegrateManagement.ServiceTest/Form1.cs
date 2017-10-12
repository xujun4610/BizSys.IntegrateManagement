using BizSys.OmniChannelToSAP.Service.Task.MaterialsManagement;
using BizSys.SAPToOmniChannel.Service.Service.StockManagementService;
using System;
using System.Windows.Forms;
using BizSys.OmniChannelToSAP.Service.Service.MasterDataManagementService;
using BizSys.OmniChannelToSAP.Service.Service.StockManagementServcie;
using BizSys.SAPToOmniChannel.Service.Service.MasterDataManagementService;
using BizSys.OmniChannelToSAP.Service.Service.PurchaseManagementServcie;
using BizSys.OmniChannelToSAP.Service.Service.ReceiptPaymentService;
using BizSys.OmniChannelToSAP.Service.Service.SalesManagementServcie;
using System.Configuration;
using MagicBox.Log;
using BizSys.SAPToOmniChannel.Service.Service.SalesManagementService;
using BizSys.SAPToOmniChannel.Service.Service.PurchaseManagementService;
using BizSys.OmniChannelToSAP.Service.Task.PurchaseManagement;
using BizSys.OmniChannelToSAP.Service.Document.ReceiptPayment;
using BizSys.SAPToOmniChannel.Service.Service.ReceiptPaymentService;
using BizSys.OmniChannelToSAP.Service.Service.CustomerService;
using BizSys.OmniChannelToSAP.Service.Task.StockManagement;

namespace BizSys.IntegrateManagement.ServiceTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSalesOrder_Click(object sender, EventArgs e)
        {
            GetSalesOrderService.GetSalesOrder();

        }

        private void btnPurcaseOrder_Click(object sender, EventArgs e)
        {
            GetPurchaseOrderService.GetPurchaseOrder();
        }

        private void btnMaterials_Click(object sender, EventArgs e)
        {
            MasterDataService service = new MasterDataService();
            service.Run();
        }

        private void btnBusinessPartner_Click(object sender, EventArgs e)
        {
            GetInventoryCountingService.GetInventoryCounting();
        }

        private void btnDistributor_Click(object sender, EventArgs e)
        {
            GetSupplierService.GetSupplier();
            //service.Run();
        }
        /// <summary>
        /// 推送数据至全渠道
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            SAPToOmniChannel.Service.Service.MasterDataManagement.PostMaterialsService.PostMaterials();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            PostMaterialsInventoryService.PostMaterialsInventory();
        }

        private void btn_Customer_Click(object sender, EventArgs e)
        {
            GetCustormerService.GetCustomer();
        }

        private void btnCapitalPlan_Click(object sender, EventArgs e)
        {
            GetCapitalPlanService.GetCapitalPlan();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            GetGoodsIssueService.GetGoodsIssue();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            GetIncomeBudgetService.GetIncomeBudget();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            GetCostBudgetService.GetCostBudget();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            GetSalesPomotionService.GetSalesPomotion();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            GetGoodsReceiptService.GetGoodsReceipts();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            GetInventoryTransferApplyService.GetInventoryTransfer();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            GetCustormerService.GetCustomer();
        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            PostEmployeeService.PostEmployee();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
           
        }

        private void button12_Click(object sender, EventArgs e)
        {
            GetPurchaseReturnService.GetPurchaseReturn();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            PostOrganizationService.PostOrganization();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            GetReceiptVerificationService.GetReceiptVerification();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            (new Form2()).Show();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            GetSalesReturnOderService.GetSalesReturnOder();
        }

        private void buttonTestConnect_Click(object sender, EventArgs e)
        {
            SAPbobsCOM.Company cmy = CreateConnect();
            if (cmy.Connected)
            {
                MessageBox.Show("B1连接成功！");
            }
        }

        public static SAPbobsCOM.Company CreateConnect()
        {
            SAPbobsCOM.Company b1Company = new SAPbobsCOM.Company();
            try
            {
                Logger.Writer("开始连接B1账套……");
                b1Company.DbServerType = (SAPbobsCOM.BoDataServerTypes)System.Enum.Parse(typeof(SAPbobsCOM.BoDataServerTypes), ConfigurationManager.AppSettings["SAPDBServerType"]);
                b1Company.Server = Convert.ToBoolean(ConfigurationManager.AppSettings["UseHostName"]) ? ConfigurationManager.AppSettings["HostName"] : ConfigurationManager.AppSettings["SAPServer"];
                b1Company.language = SAPbobsCOM.BoSuppLangs.ln_Chinese;
                b1Company.UseTrusted = Convert.ToBoolean(ConfigurationManager.AppSettings["UseTrusted"]);
                b1Company.DbUserName = ConfigurationManager.AppSettings["UserID"];
                b1Company.DbPassword = ConfigurationManager.AppSettings["Password"];
                b1Company.CompanyDB = ConfigurationManager.AppSettings["SAPCompanyDB"];
                b1Company.UserName = ConfigurationManager.AppSettings["SAPUser"];
                b1Company.Password = ConfigurationManager.AppSettings["SAPPassword"];
                b1Company.LicenseServer = ConfigurationManager.AppSettings["SAPLicenseServer"];
                int RntCode = b1Company.Connect();
                if (RntCode != 0)
                {
                    string errMsg = string.Format("ErrorCode:[{0}],ErrrMsg:[{1}];", b1Company.GetLastErrorCode(), b1Company.GetLastErrorDescription());
                    Logger.Writer(errMsg);
                    throw new Exception(errMsg);
                }
                Logger.Writer("已连接 " + b1Company.CompanyName);
                return b1Company;

            }
            catch (Exception ex)
            {
                Logger.Writer(ex.Message);
            }
            return b1Company;
        }

        private void button19_Click(object sender, EventArgs e)
        {
            PostSalesDeliveryOrderService.PostSalesDeliveryOrder();
        }

        private void button20_Click(object sender, EventArgs e)
        {
            PostInvoiceService.PostInvoice();
        }

        private void button22_Click(object sender, EventArgs e)
        {
            GetSalesOrderByB1Service.HandleSalesOrder();
        }

        private void button23_Click(object sender, EventArgs e)
        {
            GetSalesDeliveryByB1Service.HandleSalesDeliveryOrder();
        }

        private void button21_Click(object sender, EventArgs e)
        {
            GetSalesDeliveryOrderService.GetSalesDeliveryOrder();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            PostPurchaseDeliveryService.PostPurchaseDelivery();
        }

        private void button24_Click(object sender, EventArgs e)
        {
            MasterDataService ms = new MasterDataService();
            ms.Run();
        }

        private void button25_Click(object sender, EventArgs e)
        {

        }

        private void button26_Click(object sender, EventArgs e)
        {
            PurchaseOrderService ps = new PurchaseOrderService();
            ps.Run();
        }

        private void button34_Click(object sender, EventArgs e)
        {
            PostWarehouseService.PostWarehouse();
        }

        private void button35_Click(object sender, EventArgs e)
        {
            PostMaterialsCategoryService.PostMaterialsCategory();//推送品类
        }

        private void button36_Click(object sender, EventArgs e)
        {
            GetPaymentApplyService.GetPayment();
        }

        private void button27_Click(object sender, EventArgs e)
        {
            GetReceiptService.GetReceipt();
        }

        private void button28_Click(object sender, EventArgs e)
        {
            PostPurchaseDeliveryService.PostPurchaseDelivery();
        }

        private void button37_Click(object sender, EventArgs e)
        {
            GetReconciliationService.GetReconciliation();
        }

        private void button39_Click(object sender, EventArgs e)
        {
            // GetRecordService.GetRecord();
            GetCostReimbursementService.GetCostReimbursement();
        }

        private void button31_Click(object sender, EventArgs e)
        {
            PostPaymentService.PostPayment();
        }

        private void button40_Click(object sender, EventArgs e)
        {
            PostReceiptService.PostReceipt();
        }

        private void button41_Click(object sender, EventArgs e)
        {
            GetCustomerServiceApplyService.GetCustomerServiceApply();
        }

        private void button42_Click(object sender, EventArgs e)
        {
            CreatePaymentByB1Invoice.HandingInvoice();
        }

        private void button43_Click(object sender, EventArgs e)
        {
            PostPurchaseInvoiceService.PostPurchaseInvoice();
        }

        private void button44_Click(object sender, EventArgs e)
        {
            GetCustomerServiceApplyService.GetCustomerServiceApply();
        }

        private void button45_Click(object sender, EventArgs e)
        {
            StockManagementService service = new StockManagementService();
            service.Run();
        }

        private void button46_Click(object sender, EventArgs e)
        {
            GetInventoryUpdateService.GetInventoryUpdate();//获取库存过账
        }

        private void button47_Click(object sender, EventArgs e)
        {
            GetCancelOrClosePurchaseOrderService.GetCancelPuchaseOrder();
        }

        private void button49_Click(object sender, EventArgs e)
        {
            GetCancelOrCloseSalesOrderService.GetCancelSalesOrder();
        }
    }
}
