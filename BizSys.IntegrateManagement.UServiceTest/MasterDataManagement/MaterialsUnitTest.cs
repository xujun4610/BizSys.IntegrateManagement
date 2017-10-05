using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.Materials;
using Newtonsoft.Json;
using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.UServiceTest.MasterDataManagement
{
    [TestClass]
    public class MaterialsUnitTest:BaseUnitTest
    {
        Sorts sort = new Sorts()
        {
            __type = "Sort",
            Alias = "ItemCode",
            SortType = "st_Ascending"
        };

        [TestMethod]
        public async System.Threading.Tasks.Task TestGetToken()
        {
            string token = await BaseHttpClient.GetTokenAsync();
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestAsyncFetchMaterials()
        {
            this.cri.Sorts.Clear();
            this.cri.Sorts.Add(sort);

            string requestJson = JsonConvert.SerializeObject(this.cri);
            var resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.MATERIALS, requestJson);

            MaterialsRootObject Result = await JsonConvert.DeserializeObjectAsync<MaterialsRootObject>(resultJson);
            Assert.AreEqual(Result.ResultCode, 0, Result.Message);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestAsyncSaveMaterials()
        {
            ResultObjects order = new ResultObjects()
            {
                //PostingDate = DateTime.Now,


            };
            string requestJson = JsonConvert.SerializeObject(this.cri);
            var resultJson =  await BaseHttpClient.HttpSaveAsync(DocumentType.MATERIALS, requestJson);

            MaterialsRootObject Result = JsonConvert.DeserializeObject<MaterialsRootObject>(resultJson);
            Assert.AreEqual(Result.ResultCode, 0, Result.Message);
        }


        [TestMethod]
        public void TestFetchMaterials()
        {
            this.cri.Sorts.Clear();
            this.cri.Sorts.Add(sort);
            string requestJson = JsonConvert.SerializeObject(this.cri);
            var resultJson = BaseHttpClient.HttpFetch(DocumentType.MATERIALS, requestJson);

            MaterialsRootObject Result = JsonConvert.DeserializeObject<MaterialsRootObject>(resultJson);
            Assert.AreEqual(Result.ResultCode, 0, Result.Message);
        }

        [TestMethod]
        public void TestSaveMaterials()
        {
            ResultObjects order = new ResultObjects()
           {
               //PostingDate = DateTime.Now,


           };
            string requestJson = JsonConvert.SerializeObject(order);
            var resultJson = BaseHttpClient.HttpSave(DocumentType.MATERIALS, requestJson);

            MaterialsRootObject saveResult = JsonConvert.DeserializeObject<MaterialsRootObject>(resultJson);
            Assert.AreEqual(saveResult.ResultCode, 0, saveResult.Message);
        }
        
    }
}
