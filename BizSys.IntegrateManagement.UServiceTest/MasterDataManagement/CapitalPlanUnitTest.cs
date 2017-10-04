using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using BizSys.IntegrateManagement.Common;
using System.Threading.Tasks;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.CapitalPlan;


namespace BizSys.IntegrateManagement.UServiceTest.MasterDataManagement
{
    [TestClass]
    public class CapitalPlanUnitTest:BaseUnitTest
    {

        [TestMethod]
        public async System.Threading.Tasks.Task TestFetchAsyncCapitalPlan()
        {
            string requestJson = JsonConvert.SerializeObject(this.cri);
            var resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.CAPITALPLAN, requestJson);

            CapitalPlanRootObject Result = JsonConvert.DeserializeObject<CapitalPlanRootObject>(resultJson);
            Assert.AreEqual(Result.ResultCode, 0, Result.Message);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestSaveAsyncCapitalPlan()
        {
            ResultObjects order = new ResultObjects()
            {
                //PostingDate = DateTime.Now,


            };
            string requestJson = JsonConvert.SerializeObject(this.cri);
            var resultJson = await BaseHttpClient.HttpSaveAsync(DocumentType.CAPITALPLAN, requestJson);

            CapitalPlanRootObject Result = JsonConvert.DeserializeObject<CapitalPlanRootObject>(resultJson);
            Assert.AreEqual(Result.ResultCode, 0, Result.Message);
        }

        [TestMethod]
        public void TestFetchCapitalPlan()
        {
            string requestJson = JsonConvert.SerializeObject(this.cri);
            var resultJson = BaseHttpClient.HttpFetchAsync(DocumentType.CAPITALPLAN, requestJson).Result;

            CapitalPlanRootObject Result = JsonConvert.DeserializeObject<CapitalPlanRootObject>(resultJson);
            Assert.AreEqual(Result.ResultCode, 0, Result.Message);
        }

        [TestMethod]
        public void TestSaveCapitalPlan()
        {
            ResultObjects order = new ResultObjects()
            {
                //PostingDate = DateTime.Now,


            };
            string requestJson = JsonConvert.SerializeObject(this.cri);
            var resultJson = BaseHttpClient.HttpSaveAsync(DocumentType.CAPITALPLAN, requestJson).Result;

            CapitalPlanRootObject Result = JsonConvert.DeserializeObject<CapitalPlanRootObject>(resultJson);
            Assert.AreEqual(Result.ResultCode, 0, Result.Message);
        }


       
        


       
        
    }
}
