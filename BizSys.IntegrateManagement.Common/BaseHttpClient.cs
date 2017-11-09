using BizSys.IntegrateManagement.Entity.Token;
using MagicBox.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Common
{
    public class BaseHttpClient
    {
        static string BaseUrl = ConfigurationManager.AppSettings["BaseUrl"];
        static string user = ConfigurationManager.AppSettings["OMMIUser"];
        static string password = ConfigurationManager.AppSettings["OMMIPassword"];
        static string CallBackUrl = "businessdataintegration/services/json/b1CallBack?token=";
        private static string _Token;
        public static string Token
        {
            get
            {
                if (string.IsNullOrEmpty(_Token))
                {
                    var result = GetTokenAsync();
                    _Token = result.Result;
                }
                return _Token;
            }
            set { _Token = value; }
        }
        //同步token
        private static string _TokenSync;
        public static string TokenSync
        {
            get
            {
                if (string.IsNullOrEmpty(_TokenSync))
                {
                    var result = GetToken();
                    _TokenSync = result;
                }
                return _TokenSync;
            }
            set { _TokenSync = value; }
        }

        #region Token
        /// <summary>
        /// 异步获取token
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetTokenAsync()
        {
            HttpContent httpContent = new StringContent("", Encoding.UTF8, "application/json");
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            string resultJson;
            using (var http = new HttpClient(handler))
            {
                http.BaseAddress = new Uri(@BaseUrl);
                string url = $"systemcenter/services/json/userConnect?user={user}&password={password}";
                var response = http.PostAsync(url, httpContent).Result;
                response.EnsureSuccessStatusCode();
                resultJson = await response.Content.ReadAsStringAsync();
            }

            TokenRootObject token = JsonConvert.DeserializeObject<TokenRootObject>(resultJson);
            return token.UserSign;
        }
        /// <summary>
        /// 同步获取token
        /// </summary>
        /// <returns></returns>
        public static string GetToken()
        {
            string url = $"systemcenter/services/json/userConnect?user={user}&password={password}";
            string responseJson = RequestHeaderBuilder(url);

            TokenRootObject token = JsonConvert.DeserializeObject<TokenRootObject>(responseJson);
            return token.UserSign;
            /*
            HttpContent httpContent = new StringContent("", Encoding.UTF8, "application/json");
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            string resultJson;
            using (var http = new HttpClient(handler))
            {
                http.BaseAddress = new Uri(@BaseUrl);
                string url = $"systemcenter/services/json/userConnect?user={user}&password={password}";
                var response = http.PostAsync(url, httpContent).Result;
                response.EnsureSuccessStatusCode();
                resultJson = response.Content.ReadAsStringAsync().Result;
            }

            TokenRootObject token = JsonConvert.DeserializeObject<TokenRootObject>(resultJson);
            return token.UserSign;*/

        }
        #endregion

        #region Fetch
        /// <summary>
        /// 异步查询
        /// </summary>
        /// <param name="OrderType">单据类型</param>
        /// <param name="requestJson">查询条件-json格式</param>
        /// <returns></returns>
        public async static Task<string> HttpFetchAsync(DocumentType OrderType, string requestJson)
        {
            HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            string resutlJson;
            using (var http = new HttpClient(handler))
            {
                http.BaseAddress = new Uri(@BaseUrl);
                //await异步等待回应
                string url = @GetFetchOrderUrl(OrderType) + Token;
                var response = await http.PostAsync(url, httpContent);
                //确保HTTP成功状态值
                response.EnsureSuccessStatusCode();
                //if(response.StatusCode == HttpStatusCode.OK)

                resutlJson = await response.Content.ReadAsStringAsync();
            }

            return resutlJson;
        }
        /// <summary>
        /// 同步查询
        /// </summary>
        /// <param name="OrderType">单据类型</param>
        /// <param name="requestJson">查询条件-json格式</param>
        /// <returns></returns>
        public static string HttpFetch(DocumentType OrderType, string requestJson)
        {
            string url = @GetFetchOrderUrl(OrderType) + _TokenSync;
            return RequestHeaderBuilder(url, requestJson);
            /*
            HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            string resutlJson;
            using (var http = new HttpClient(handler))
            {
                http.BaseAddress = new Uri(@BaseUrl);
                //await异步等待回应
                string url = @GetFetchOrderUrl(OrderType) + Token;
                http.PostAsync(url, httpContent).Start();
                var response = http.PostAsync(url, httpContent).Result;
                //确保HTTP成功状态值
                response.EnsureSuccessStatusCode();
                //if(response.StatusCode == HttpStatusCode.OK)

                resutlJson = response.Content.ReadAsStringAsync().Result;
            }

            return resutlJson;
            */
        }
        #endregion

        #region Save
        /// <summary>
        /// 异步保存
        /// </summary>
        /// <param name="OrderType">单据类型</param>
        /// <param name="requestJson">保存数据-json格式</param>
        /// <returns></returns>
        public async static Task<string> HttpSaveAsync(DocumentType OrderType, string requestJson)
        {
            /*
            WebRequestHandler handler = new WebRequestHandler();
            X509Certificate2 certificate = GetMyX509Certificate();
            handler.ClientCertificates.Add(certificate);
            */
            HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            string jsonResult;
            using (var http = new HttpClient(handler))
            {
                http.BaseAddress = new Uri(@BaseUrl);

                HttpClient client = new HttpClient(handler);
                string url = @GetSaveOrderUrl(OrderType) + Token;
                var response = await http.PostAsync(url, httpContent);
                response.EnsureSuccessStatusCode();
                //if(response.StatusCode == HttpStatusCode.OK)
                jsonResult = await response.Content.ReadAsStringAsync();
            }
            return jsonResult;
        }
        /// <summary>
        /// 同步保存
        /// </summary>
        /// <param name="OrderType">单据类型</param>
        /// <param name="requestJson">保存数据-json格式</param>
        /// <returns></returns>
        public static string HttpSave(DocumentType OrderType, string requestJson)
        {
            HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            string jsonResult = string.Empty;
            using (var http = new HttpClient(handler))
            {
                http.BaseAddress = new Uri(@BaseUrl);
                string url = @GetSaveOrderUrl(OrderType) + Token;
                var response = http.PostAsync(url, httpContent).Result;
                response.EnsureSuccessStatusCode();//
                //if(response.StatusCode == HttpStatusCode.OK)
                jsonResult = response.Content.ReadAsStringAsync().Result;
            }
            return jsonResult;
        }
        #endregion

        #region CallBack
        /// <summary>
        /// 异步回写
        /// </summary>
        /// <param name="requestJson">回写数据-json格式</param>
        /// <returns></returns>
        public async static Task<string> HttpCallBackAsync(string requestJson)
        {
            HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            string jsonResult;
            using (var http = new HttpClient(handler))
            {
                http.BaseAddress = new Uri(@BaseUrl);
                string url = @CallBackUrl + Token;
                var response = await http.PostAsync(url, httpContent);
                response.EnsureSuccessStatusCode();
                jsonResult = await response.Content.ReadAsStringAsync();
            }
            return jsonResult;
            // string requestJson1 = "{\"ObjectId\":\"AVA_BP_CUSTOMER\",\"QueryParameters\":[{\"Key\":\"ObjectKey\",\"Text\":\"6939\"}],\"Data\":[{\"Key\":\"U_SBOSynchronization\",\"Text\":\"Y\"},{\"Key\":\"U_SBOCallbackDate\",\"Text\":\"2017/2/10 9:43:34\"},{\"Key\":\"U_SBOId\",\"Text\":\"6939\"}]}";

        }
        /// <summary>
        /// 同步回写
        /// </summary>
        /// <param name="requestJson">回写数据-json格式</param>
        /// <returns></returns>
        public static string HttpCallBack(string requestJson)
        {
            return RequestHeaderBuilder(CallBackUrl + _TokenSync, requestJson);
            /*
            HttpContent httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            string jsonResult;
            using (var http = new HttpClient(handler))
            {
                http.BaseAddress = new Uri(@BaseUrl);
                string url = @CallBackUrl + Token;
                var response = http.PostAsync(url, httpContent).Result;
                response.EnsureSuccessStatusCode();
                jsonResult = response.Content.ReadAsStringAsync().Result;
            }
            return jsonResult;
            // string requestJson1 = "{\"ObjectId\":\"AVA_BP_CUSTOMER\",\"QueryParameters\":[{\"Key\":\"ObjectKey\",\"Text\":\"6939\"}],\"Data\":[{\"Key\":\"U_SBOSynchronization\",\"Text\":\"Y\"},{\"Key\":\"U_SBOCallbackDate\",\"Text\":\"2017/2/10 9:43:34\"},{\"Key\":\"U_SBOId\",\"Text\":\"6939\"}]}";
            */
        }

        #endregion 


        public static string GetFetchOrderUrl(DocumentType OrderType)
        {
            switch (OrderType)
            {
                case DocumentType.SALESORDER:
                    return "salesmanagement/services/json/fetchSalesOrder?token=";
                case DocumentType.MATERIALS:
                    return "materialsmanagement/services/json/fetchMaterials?token=";
                case DocumentType.GOODSISSUE:
                    return "materialsmanagement/services/json/fetchGoodsIssue?token=";
                case DocumentType.GOODSRECEIPT:
                    return "materialsmanagement/services/json/fetchGoodsReceipt?token=";
                case DocumentType.ITEMGROUP:
                    return "materialsmanagement/services/json/fetchMaterialsGroup?token=";
                case DocumentType.WAREHOURSE:
                    return "materialsmanagement/services/json/fetchWarehouse?token=";
                case DocumentType.MATERIALSCATEGORY:
                    return "materialsmanagement/services/json/fetchMaterialsCategory?token=";
                case DocumentType.BUSINESSPARTNER:
                    return "businesspartner/services/json/fetchBusinessPartner?token=";
                case DocumentType.PURCHASEORDER:
                    return "purchasemanagement/services/json/fetchPurchaseOrder?token=";
                case DocumentType.RECEIPT:
                    return "receiptpayment/services/json/fetchReceipt?token=";
                case DocumentType.PAYMENT:
                    return "receiptpayment/services/json/fetchPaymentbill?token=";
                case DocumentType.RECORD:
                    return "receiptpayment/services/json/fetchRecord?token=";
                case DocumentType.RECONCILIATION:
                    return "receiptpayment/services/json/fetchReconciliation?token=";
                case DocumentType.PAYMENTAPPLY:
                    return "receiptpayment/services/json/fetchPayment?token=";
                case DocumentType.COSTREIMBURSEMENT:
                    return "receiptpayment/services/json/fetchCostReimbursement?token=";
                case DocumentType.RECEIPTVERIFICATION:
                    return "receiptpayment/services/json/fetchReceiptVerification?token=";
                case DocumentType.SALESPROMOTION:
                    return "marketingmanagement/services/json/fetchSalesPromotion?token=";
                case DocumentType.CAPITALPLAN:
                    return "budgetmanagement/services/json/fetchCapitalPlan?token=";
                case DocumentType.COSTBUDGET:
                    return "budgetmanagement/services/json/fetchCostBudget?token=";
                case DocumentType.INCOMEBUDGET:
                    return "budgetmanagement/services/json/fetchInComeBudget?token=";
                case DocumentType.SALESDELIVERYORDER:
                    return "salesmanagement/services/json/fetchSalesDelivery?token=";
                case DocumentType.INVOICE:
                    return "salesmanagement/services/json/fetchReceivable?token=";
                case DocumentType.SALESRETURNORDER:
                    return "salesmanagement/services/json/fetchSalesReturn?token=";
                case DocumentType.PURCHASEDELIVERYORDER:
                    return "purchasemanagement/services/json/fetchPurchaseDelivery?token=";
                case DocumentType.PAYABLE:
                    return "purchasemanagement/services/json/fetchPayable?token=";
                case DocumentType.PURCHASERETURN:
                    return "purchasemanagement/services/json/fetchPurchaseReturn?token=";
                case DocumentType.SUPPLIER:
                    return "businesspartner/services/json/fetchSupplier?token=";
                case DocumentType.LEAGUER:
                    return "businesspartner/services/json/fetchLeaguer?token=";
                case DocumentType.CUSTOMER:
                    return "businesspartner/services/json/fetchCustomer?token=";
                case DocumentType.PACKINGLIST:
                    return "materialsmanagement/services/json/fetchPackingList?token=";
                case DocumentType.INVENTORYUPDATE:
                    return "materialsmanagement/services/json/fetchInventoryUpdate?token=";
                case DocumentType.INVENTORYCOUNTING:
                    return "materialsmanagement/services/json/fetchInventoryCounting?token=";
                case DocumentType.INVENTORYTRANSFER:
                    return "materialsmanagement/services/json/fetchInventoryTransfer?token=";
                case DocumentType.MATERIALSINVENTORY:
                    return "materialsmanagement/services/json/fetchMaterialsInventory?token=";
                case DocumentType.EMPLOYEE:
                    return "humanresources/services/json/fetchEmployee?token=";
                case DocumentType.ORGANIZATION:
                    return "systemapplicationcenter/services/json/fetchOrganization?token=";
                case DocumentType.CUSTOMERSERVICE:
                    return "customerservicecenter/services/json/fetchApplication?token=";
            }
            throw new ArgumentNullException(string.Format("can't get url by OrderType [{0}]", OrderType));
        }

        public static string GetSaveOrderUrl(DocumentType OrderType)
        {
            switch (OrderType)
            {
                case DocumentType.SALESORDER:
                    return "salesmanagement/services/json/saveSalesOrder?token=";
                case DocumentType.MATERIALS:
                    return "materialsmanagement/services/json/saveMaterials?token=";
                case DocumentType.ITEMGROUP:
                    return "materialsmanagement/services/json/saveMaterialsGroup?token=";
                case DocumentType.WAREHOURSE:
                    return "materialsmanagement/services/json/saveWarehouse?token=";
                case DocumentType.BUSINESSPARTNER:
                    return "businesspartner/services/json/saveBusinessPartner?token=";
                case DocumentType.MATERIALSCATEGORY:
                    return "materialsmanagement/services/json/saveMaterialsCategory?token=";
                case DocumentType.PURCHASEORDER:
                    return "purchasemanagement/services/json/savePurchaseOrder?token=";
                case DocumentType.GOODSISSUE:
                    return "materialsmanagement/services/json/saveGoodsIssue?token=";
                case DocumentType.GOODSRECEIPT:
                    return "materialsmanagement/services/json/saveGoodsReceipt?token=";
                case DocumentType.RECEIPT:
                    return "receiptpayment/services/json/saveReceipt?token=";
                case DocumentType.COSTREIMBURSEMENT:
                    return "receiptpayment/services/json/saveCostReimbursement?token=";
                case DocumentType.PAYMENT:
                    return "receiptpayment/services/json/savePayment?token=";
                case DocumentType.RECORD:
                    return "receiptpayment/services/json/saveRecord?token=";
                case DocumentType.RECONCILIATION:
                    return "receiptpayment/services/json/saveReconciliation?token=";
                case DocumentType.RECEIPTVERIFICATION:
                    return "receiptpayment/services/json/saveReceiptVerification?token=";
                case DocumentType.CAPITALPLAN:
                    return "budgetmanagement/services/json/saveCapitalPlan?token=";
                case DocumentType.COSTBUDGET:
                    return "budgetmanagement/services/json/saveCostBudget?token=";
                case DocumentType.SALESPROMOTION:
                    return "marketingmanagement/services/json/saveSalesPromotion?token=";
                case DocumentType.INCOMEBUDGET:
                    return "budgetmanagement/services/json/saveInComeBudget?token=";
                case DocumentType.SALESDELIVERYORDER:
                    return "salesmanagement/services/json/saveSalesDelivery?token=";
                case DocumentType.SALESRETURNORDER:
                    return "salesmanagement/services/json/saveSalesReturn?token=";
                case DocumentType.INVOICE:
                    return "salesmanagement/services/json/saveReceivable?token=";
                case DocumentType.PURCHASEDELIVERYORDER:
                    return "purchasemanagement/services/json/savePurchaseDelivery?token=";
                case DocumentType.SUPPLIER:
                    return "businesspartner/services/json/saveSupplier?token=";
                case DocumentType.LEAGUER:
                    return "businesspartner/services/json/saveLeaguer?token=";
                case DocumentType.CUSTOMER:
                    return "businesspartner/services/json/saveCustomer?token=";
                case DocumentType.PACKINGLIST:
                    return "materialsmanagement/services/json/savePackingList?token=";
                case DocumentType.INVENTORYUPDATE:
                    return "materialsmanagement/services/json/saveInventoryUpdate?token=";
                case DocumentType.INVENTORYCOUNTING:
                    return "materialsmanagement/services/json/saveInventoryCounting?token=";
                case DocumentType.INVENTORYTRANSFER:
                    return "materialsmanagement/services/json/saveInventoryCounting?token=";
                case DocumentType.MATERIALSINVENTORY:
                    return "materialsmanagement/services/json/saveMaterialsInventory?token=";
                case DocumentType.ORGANIZATION:
                    return "systemapplicationcenter/services/json/saveOrganization?token=";
                case DocumentType.PAYABLE:
                    return "purchasemanagement/services/json/savePayable?token=";
                case DocumentType.CUSTOMERSERVICE:
                    return "customerservicecenter/services/json/saveApplication?token=";
            }
            throw new ArgumentNullException(string.Format("can't get url by OrderType [{0}]", OrderType));
        }

        /// <summary>
        /// 传统的webRequest
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <param name="requestJsonData"></param>
        /// <returns></returns>
        public static string RequestHeaderBuilder(string relativeUrl, string requestJsonData = null)
        {

            try
            {
                //相应数据
                string responseData = string.Empty;

                byte[] bs = null;
                Uri absUrl = new Uri(new Uri(BaseUrl), relativeUrl);

                HttpWebRequest wreq = WebRequest.CreateHttp(absUrl);
                wreq.ContentType = "application/json;charset=UTF-8";
                wreq.AutomaticDecompression = DecompressionMethods.GZip;
                wreq.Method = HttpMethod.Post.Method;
                wreq.Accept = "application/json, text/javascript, */*;";
                wreq.MediaType = "application/json";
                wreq.Timeout = 60 * 1000; //(1min)
                wreq.KeepAlive = true;
                if (!string.IsNullOrWhiteSpace(requestJsonData))
                {
                    bs = Encoding.UTF8.GetBytes(requestJsonData);
                    using (Stream st = wreq.GetRequestStream())
                    {
                        st.Write(bs, 0, bs.Length);
                        st.Close();
                    }
                    //wreq.ContentLength = bs == null ? 0 : bs.Length;
                }
                HttpWebResponse response = (HttpWebResponse)wreq.GetResponse();


                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseData = reader.ReadToEnd().ToString();
                        reader.Close();
                    }
                }
                //else
                //{
                //    throw new WebException(response.StatusDescription );
                //}
                return responseData;
            }
            catch (WebException ex)
            {
                string responseMsg = string.Format("[{0}]{1}", ex.Status, ex.Message);
                throw new Exception(responseMsg);
            }
            catch (Exception ex)
            {
                throw ex;
            }



        }


    }
}
