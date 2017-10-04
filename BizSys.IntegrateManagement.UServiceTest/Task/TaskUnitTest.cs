using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BizSys.IntegrateManagement.IRepository.Task;
using BizSys.IntegrateManagement.Repository.Task;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.Materials;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.Warehouse;

namespace BizSys.IntegrateManagement.UServiceTest.Task
{
    [TestClass]
    public class TaskUnitTest
    {
        [TestMethod]
        public void TestGetDocument()
        {
            ITaskRep taskRep = new TaskRep();
            //MaterialsRootObject material = new MaterialsRootObject();
            //taskRep.GetDocumentWithNoSync<MaterialsRootObject>(material, 30);

            WarehouseRootObject warehouse = new WarehouseRootObject();
            taskRep.GetDocumentWithNoSync<WarehouseRootObject>(warehouse, 30);


        }
    }
}
