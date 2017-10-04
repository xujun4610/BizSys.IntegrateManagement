using BizSys.IntegrateManagement.Entity.Token;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BizSys.IntegrateManagement.ServiceTest
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            
        }

        #region 选择源数据
        
        private readonly  IList<Resource> resouceList = new List<Resource>(){
            new Resource(){ModuleName="materialsmanagement",ResourceURI="ibcp.materialsmanagement.service-1.0.0/services/json/"},
            new Resource(){ModuleName="distributorapplication",ResourceURI="ibcp.distributorapplication.service-1.0.0/services/json/"},
            new Resource(){ModuleName="businesspartner",ResourceURI="ibcp.businesspartner.service-1.0.0/services/json/"},
            new Resource(){ModuleName="purchasemanagement",ResourceURI="ibcp.purchasemanagement.service-1.0.0/services/json/"},
            new Resource(){ModuleName="salesmanagement",ResourceURI="ibcp.salesmanagement.service-1.0.0/services/json/"},
            new Resource(){ModuleName="receiptpayment",ResourceURI="ibcp.receiptpayment.service-1.0.0/services/json/"},
            new Resource(){ModuleName="marketingmanagement",ResourceURI="ibcp.marketingmanagement.service-1.0.0/services/json/"},
            new Resource(){ModuleName="budgetmanagement",ResourceURI="ibcp.budgetmanagement.service-1.0.0/services/json/"},
            new Resource(){ModuleName="humanresources",ResourceURI="ibcp.humanresources.service-1.0.0/services/json/"},
            new Resource(){ModuleName="systemapplicationcenter",ResourceURI="ibcp.systemapplicationcenter.service-1.0.0/services/json/"}
            
        };
       private readonly IList<Method> methodResouce = new List<Method>()
        {
            new Method(){ModuleName="materialsmanagement",MethodWay="fetchMaterials"},
            new Method(){ModuleName="materialsmanagement",MethodWay="fetchGoodsIssue"},
            new Method(){ModuleName="materialsmanagement",MethodWay="fetchGoodsReceipt"},
            new Method(){ModuleName="materialsmanagement",MethodWay="fetchMaterialsGroup"},
            new Method(){ModuleName="materialsmanagement",MethodWay="fetchWarehouse"},
            new Method(){ModuleName="materialsmanagement",MethodWay="fetchMaterialsCategory"},
            new Method() {ModuleName="materialsmanagement",MethodWay="fetchMaterialsInventory" },
           
            new Method(){ModuleName="distributorapplication",MethodWay="fetchmaterials"},
            
            new Method(){ModuleName="businesspartner",MethodWay="fetchSupplier"},
            new Method(){ModuleName="businesspartner",MethodWay="fetchLeaguer"},
            new Method(){ModuleName="businesspartner",MethodWay="fetchCustomer"},

            new Method(){ModuleName="purchasemanagement",MethodWay="fetchPurchaseDelivery"},
            new Method(){ModuleName="purchasemanagement",MethodWay="fetchPurchaseReturn"},
            new Method(){ModuleName="purchasemanagement",MethodWay="fetchPayable"},



            new Method(){ModuleName="salesmanagement",MethodWay="fetchSalesOrder"},
            new Method(){ModuleName="salesmanagement",MethodWay="fetchSalesDelivery"},
            new Method(){ModuleName="salesmanagement",MethodWay="fetchSalesReturn"},
            new Method(){ModuleName="salesmanagement",MethodWay="fetchReceivable"},

            new Method(){ModuleName="receiptpayment",MethodWay="fetchReceipt"},
            new Method(){ModuleName="receiptpayment",MethodWay="fetchPayment"},
            new Method(){ModuleName="receiptpayment",MethodWay="fetchRecord"},
            new Method(){ModuleName="receiptpayment",MethodWay="fetchReceiptVerification"},

            new Method(){ModuleName="marketingmanagement",MethodWay="fetchSalesPromotion"},
            new Method(){ModuleName="budgetmanagement",MethodWay="fetchCapitalPlan"},
            new Method(){ModuleName="budgetmanagement",MethodWay="fetchCostBudget"},
            new Method(){ModuleName="budgetmanagement",MethodWay="fetchInComeBudget"},

            new Method(){ModuleName="humanresources",MethodWay="fetchmaterials"},
           
            new Method(){ModuleName="systemapplicationcenter",MethodWay="fetchOrganization"}
        };
        #endregion

        private void Form2_Load(object sender, EventArgs e)
        {
            this.txtEndPoint.Text = ConfigurationManager.AppSettings["BaseUrl"];
            this.txtCondition.Text = @"{""__type"":""Criteria"",
                                        ""ResultCount"":30,
                                        ""Conditions"":[],
                                        ""isDbFieldName"":false,
                                        ""BusinessObjectCode"":null,
                                        ""Sorts"":[{
                                                ""__type"":""Sort"",
                                                ""Alias"":""ObjectKey"",
                                                ""SortType"":""st_Descending""}],
                                        ""ChildCriterias"":[],
                                        ""NotLoadedChildren"":true,
                                        ""Remarks"":null}";
            LoadResouce(null);
            LoadParameters();

        }
        private void LoadResouce(string key)
        {
            this.cmbResource.Items.Clear();
            if(!string.IsNullOrEmpty(key))
            {
                foreach (var item in resouceList.Where(c => c.ResourceURI.Contains(key)))
                {
                    this.cmbResource.Items.Add(item.ResourceURI);
                }
            }
            else
            {
                foreach (var item in resouceList)
                {
                    this.cmbResource.Items.Add(item.ResourceURI);
                }
            }
            this.cmbResource.SelectionStart = this.cmbResource.Text.Length;
            
        }

        private void LoadMethodsSource()
        {
            if (string.IsNullOrEmpty(this.cmbResource.Text))
                return;
            var moduleList = this.resouceList.Where(c => c.ResourceURI == this.cmbResource.Text);
            if (moduleList.Count() == 0)
                return;
            if (string.IsNullOrEmpty(moduleList.FirstOrDefault().ModuleName))
                return;
            string methodName = moduleList.FirstOrDefault().ModuleName;
            this.cmbMethods.Items.Clear();
            foreach (var item in this.methodResouce.Where(c => c.ModuleName == methodName))
            {
                this.cmbMethods.Items.Add(item.MethodWay);
            }
        }

        private async void LoadParameters()
        {
            HttpContent httpContent = new StringContent("", Encoding.UTF8, "application/json");
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            string resutlJson;
            using (var http = new HttpClient(handler))
            {
                http.BaseAddress = new Uri(this.txtEndPoint.Text);
                string url = "/systemcenter/services/json/userConnect?user=admin&password=1q2w3e";
                var response = http.PostAsync(url, httpContent).Result;
                response.EnsureSuccessStatusCode();
                resutlJson = await response.Content.ReadAsStringAsync();
            }

            TokenRootObject token = JsonConvert.DeserializeObject<TokenRootObject>(resutlJson);
            this.txtParameters.Text = "?token=" + token.UserSign;
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            HttpContent httpContent = new StringContent(this.txtCondition.Text, Encoding.UTF8, "application/json");
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            string resutlJson;
            using (var http = new HttpClient(handler))
            {
                http.BaseAddress = new Uri(this.txtEndPoint.Text);
                string url = this.cmbResource.Text + this.cmbMethods.Text+this.txtParameters.Text;
                var response = await http.PostAsync(url, httpContent);
                response.EnsureSuccessStatusCode();
                resutlJson = await response.Content.ReadAsStringAsync();
            }
            
            this.txtResult.Text = resutlJson;
        }

        private void TextUpdate(object sender, EventArgs e)
        {
            LoadResouce(this.cmbResource.Text);
            this.cmbResource.DroppedDown = true;
            LoadMethodsSource();
        }

        private void SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMethodsSource();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(this.txtResult.Text))
            {
                Clipboard.SetDataObject(this.txtResult.Text);
            }
           
        }

        private void cmbMethods_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
