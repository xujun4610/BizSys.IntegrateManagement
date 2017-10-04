using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.CostBudget;


namespace BizSys.IntegrateManagement.UServiceTest.MasterDataManagement
{
    [TestClass]
    public class CostBudgetUnitTest:BaseUnitTest
    {
        
        [TestMethod]
        public async System.Threading.Tasks.Task TestFetchAsyncCostBudget()
        {
            string requestJson = JsonConvert.SerializeObject(this.cri);
            var resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.COSTBUDGET, requestJson);

            CostBudgetRootObject Result = JsonConvert.DeserializeObject<CostBudgetRootObject>(resultJson);
            Assert.AreEqual(Result.ResultCode, 0, Result.Message);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestSaveAsyncCostBudget()
        {
            ResultObjects order = new ResultObjects()
            {
                //PostingDate = DateTime.Now,


            };
            string requestJson = JsonConvert.SerializeObject(this.cri);
            var resultJson = await BaseHttpClient.HttpSaveAsync(DocumentType.COSTBUDGET, requestJson);

            CostBudgetRootObject Result = JsonConvert.DeserializeObject<CostBudgetRootObject>(resultJson);
            Assert.AreEqual(Result.ResultCode, 0, Result.Message);
        }

        [TestMethod]
        public void TestFetchCostBudget()
        {
            string requestJson = JsonConvert.SerializeObject(this.cri);
            var resultJson = BaseHttpClient.HttpFetchAsync(DocumentType.COSTBUDGET, requestJson).Result;

            CostBudgetRootObject Result = JsonConvert.DeserializeObject<CostBudgetRootObject>(resultJson);
            Assert.AreEqual(Result.ResultCode, 0, Result.Message);
        }

        [TestMethod]
        public void TestSaveCostBudget()
        {
            ResultObjects order = new ResultObjects()
            {
                //PostingDate = DateTime.Now,


            };
            string requestJson = JsonConvert.SerializeObject(this.cri);
            var resultJson = BaseHttpClient.HttpSaveAsync(DocumentType.COSTBUDGET, requestJson).Result;

            CostBudgetRootObject Result = JsonConvert.DeserializeObject<CostBudgetRootObject>(resultJson);
            Assert.AreEqual(Result.ResultCode, 0, Result.Message);
        }


       
        
    }
}
