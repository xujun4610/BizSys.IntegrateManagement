using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using BizSys.IntegrateManagement.Common;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.UServiceTest.StockManagement
{
    [TestClass]
    public class GoodsIssueUnitTest:BaseUnitTest
    {
        
        [TestMethod]
        public async System.Threading.Tasks.Task TestFetchAsyncGoodsIssue()
        {
            string requestJson = JsonConvert.SerializeObject(this.cri);
            var resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.GOODSISSUE, requestJson);

            //RootObject Result = JsonConvert.DeserializeObject<            //RootObject>(resultJson);
            //Assert.AreEqual(Result.ResultCode, 0, Result.Message);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestSaveAsyncGoodsIssue()
        {
            //ResultObjects order = new ResultObjects()
            //{
            //    //PostingDate = DateTime.Now,


            //};
            //string requestJson = JsonConvert.SerializeObject(this.cri);
            //var resultJson = await BaseHttpClient.HttpSaveAsync(DocumentType.GOODSISSUE, requestJson);

            //            //RootObject Result = JsonConvert.DeserializeObject<            //RootObject>(resultJson);
            //Assert.AreEqual(Result.ResultCode, 0, Result.Message);
        }

        [TestMethod]
        public void TestFetchGoodsIssue()
        {
            string requestJson = JsonConvert.SerializeObject(this.cri);
            var resultJson = BaseHttpClient.HttpFetchAsync(DocumentType.GOODSISSUE, requestJson).Result;

                        //RootObject Result = JsonConvert.DeserializeObject<            //RootObject>(resultJson);
           // Assert.AreEqual(Result.ResultCode, 0, Result.Message);
        }

        [TestMethod]
        public void TestSaveGoodsIssue()
        {
            //ResultObjects order = new ResultObjects()
            //{
            //    //PostingDate = DateTime.Now,


            //};
            //string requestJson = JsonConvert.SerializeObject(this.cri);
            //var resultJson = BaseHttpClient.HttpSaveAsync(DocumentType.GOODSISSUE, requestJson).Result;

            //            //RootObject Result = JsonConvert.DeserializeObject<            //RootObject>(resultJson);
            //Assert.AreEqual(Result.ResultCode, 0, Result.Message);
        }


       
        
    }
}
