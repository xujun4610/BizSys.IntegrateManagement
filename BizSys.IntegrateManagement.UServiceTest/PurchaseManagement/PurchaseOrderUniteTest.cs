using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity;
using System.Collections.Generic;
using BizSys.IntegrateManagement.Entity.PurchaseOrder;
using System.Threading.Tasks;
using BizSys.OmniChannelToSAP.Service.Service.PurchaseManagementServcie;

namespace BizSys.IntegrateManagement.UServiceTest.PurchaseManagement
{
    [TestClass]
    public class PurchaseOrderUniteTest: BaseUnitTest
    {
        Sorts sorts = new Sorts(){
                    __type="Sort",
                    Alias="DocEntry",
                    SortType="st_Ascending"
            };
            


        [TestMethod]
        public async System.Threading.Tasks.Task TestAsyncFetchPurchaseOrder()
        {
            string requestJson = JsonConvert.SerializeObject(this.cri);
            var resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.PURCHASEORDER, requestJson);
            PurchaseOrderRootObject purchaseOrder = await JsonConvert.DeserializeObjectAsync<PurchaseOrderRootObject>(resultJson);
            Assert.AreEqual(purchaseOrder.ResultCode, "0", purchaseOrder.Message);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestAsyncSavePurchaseOrder()
        {
            ResultObjects order = new ResultObjects()
            {
                PostingDate = DateTime.Now,

                PurchaseOrderItems = new List<PurchaseOrderItems>()
                {

                }
            };
            string requestJson = JsonConvert.SerializeObject(order);
            var resultJson = await BaseHttpClient.HttpSaveAsync(DocumentType.PURCHASEORDER, requestJson);
            PurchaseOrderRootObject saveResult = JsonConvert.DeserializeObject<PurchaseOrderRootObject>(resultJson);
            Assert.AreEqual(saveResult.ResultCode, "0", saveResult.Message);
        }


        [TestMethod]
        public void TestFetchPurchaseOrder()
        {
            cri.Sorts.Clear();
            cri.Sorts.Add(sorts);

            string requestJson = JsonConvert.SerializeObject(this.cri);
            var resultJson = BaseHttpClient.HttpFetch(DocumentType.PURCHASEORDER, requestJson);
            PurchaseOrderRootObject purchaseOrder = JsonConvert.DeserializeObject<PurchaseOrderRootObject>(resultJson);
            Assert.AreEqual(purchaseOrder.ResultCode, "0", purchaseOrder.Message);
        }

        [TestMethod]
        public void TestSavePurchaseOrder()
        {
            ResultObjects order = new ResultObjects()
            {
                PostingDate = DateTime.Now,
                
                PurchaseOrderItems = new List<PurchaseOrderItems>()
                {
                    
                }
            };
            string requestJson = JsonConvert.SerializeObject(order);
            var resultJson = BaseHttpClient.HttpSave(DocumentType.PURCHASEORDER, requestJson);
            PurchaseOrderRootObject saveResult = JsonConvert.DeserializeObject<PurchaseOrderRootObject>(resultJson);
            Assert.AreEqual(saveResult.ResultCode, "0", saveResult.Message);
        }
        
    }
}
