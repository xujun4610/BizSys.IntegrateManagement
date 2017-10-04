using BizSys.OmniChannelToSAP.Service.Service.SalesOrderServiceByNiko;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.UServiceTest.SalesManagement
{
    [TestClass]
    public class UnitTest_SalesOrderByNiko2SAP
    {
        [TestMethod]
        public async System.Threading.Tasks.Task TestMethod_GetSalesOrderByNiko()
        {

            try
            {
                await GetSalesOrderByNikoService.GetSalesOrderByNiko(); 
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
