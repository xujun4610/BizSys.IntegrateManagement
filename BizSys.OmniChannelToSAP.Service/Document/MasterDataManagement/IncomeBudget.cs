using BizSys.IntegrateManagement.Entity.MasterDataManagement.IncomeBudget;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.OmniChannelToSAP.Service.B1Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.OmniChannelToSAP.Service.Document.MasterDataManagement
{
    public class IncomeBudget
    {
        public static Result CreateIncomeBudget(ResultObjects incomeBudget)
        {
            Result result = new Result();
            SAPbobsCOM.BudgetDistribution myBudGet = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBudgetDistribution);
            
            foreach (var item in incomeBudget.InComeBudgetItems)
            {
              // if(myBudGet.GetByKey())
                //myBudGet.DivisionCode = 
                //myBudGet.BudgetAmount = item.LineTotal;
                    
                myBudGet.set_BudgetAmount(item.LineTotal.ToString());
                myBudGet.January = item.January;
                myBudGet.February = item.February;
                myBudGet.March = item.March;
                myBudGet.April = item.April;
                myBudGet.May = item.May;
                myBudGet.June = item.June;
                myBudGet.July = item.July;
                myBudGet.August = item.August;
                myBudGet.September = item.September;
                myBudGet.October = item.October;
                myBudGet.November = item.November;
                myBudGet.December = item.December;
                myBudGet.Add();

                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myBudGet);
            }

            return result;
        }
    }
}
