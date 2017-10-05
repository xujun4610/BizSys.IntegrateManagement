using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BizSys.IntegrateManagement.Entity;
using System.Collections.Generic;
using Newtonsoft.Json;
using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.UServiceTest;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.Customer;
using System.Threading.Tasks;
using BizSys.IntegrateManagement.Entity.Token;

namespace BizSys.IntegrateManagement.ServiceUnitTest.MasterDataManagement
{
    [TestClass]
    public class CustomerUnitTest : BaseUnitTest
    {
        [TestMethod]
        public void TestToken()
        {
            string resultJson = BaseHttpClient.GetToken();
            TokenRootObject token = JsonConvert.DeserializeObject<TokenRootObject>(resultJson);
            Assert.IsNotNull(token.UserSign);
        }
        
        [TestMethod]
        public async Task TestAsyncFetchCustomer()
        {
            this.cri.Sorts.Clear();

            string requestJson = JsonConvert.SerializeObject(cri);
            var  resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.CUSTOMER, requestJson);

            CustomerRootObject Result = JsonConvert.DeserializeObject<CustomerRootObject>(resultJson);
            Assert.AreEqual(Result.ResultCode, 0, Result.Message);
           
        }

        [TestMethod]
        public async Task TestAsyncSaveCustomer()
        {
            string requestJson = JsonConvert.SerializeObject(this.cri);
            var resultJson = await BaseHttpClient.HttpSaveAsync(DocumentType.CUSTOMER, requestJson);

            CustomerRootObject Result = JsonConvert.DeserializeObject<CustomerRootObject>(resultJson);
            Assert.AreEqual(Result.ResultCode, 0, Result.Message);
        }


        [TestMethod]
        public  void TestFetchCustomer()
        {
            string requestJson = JsonConvert.SerializeObject(this.cri);
            var resultJson =  BaseHttpClient.HttpFetch(DocumentType.CUSTOMER, requestJson);

            CustomerRootObject Result = JsonConvert.DeserializeObject<CustomerRootObject>(resultJson);
            Assert.AreEqual(Result.ResultCode, 0, Result.Message);
        }

        [TestMethod]
        public  void TestSaveCustomer()
        {
            string requestJson = JsonConvert.SerializeObject(this.cri);
            var resultJson =  BaseHttpClient.HttpSave(DocumentType.CUSTOMER, requestJson);

            CustomerRootObject Result = JsonConvert.DeserializeObject<CustomerRootObject>(resultJson);
            Assert.AreEqual(Result.ResultCode, 0, Result.Message);
        }
    }
}
