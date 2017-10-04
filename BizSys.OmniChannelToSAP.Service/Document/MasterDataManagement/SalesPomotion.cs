using BizSys.OmniChannelToSAP.Service.B1UDO;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.SalesPomotion;
using BizSys.IntegrateManagement.Entity.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizSys.OmniChannelToSAP.Service.B1Common;

namespace BizSys.OmniChannelToSAP.Service.Document.MasterDataManagement
{
    /// <summary>
    /// 促销活动
    /// </summary>
    public class SalesPomotion
    {
        public static Result CreateSalesPomotion(ResultObjects SalesPomotion)
        {
            Result result = new Result();
            string sRetVal = "";
            string Sql_IsExistMasterData = "";
            string Sql_IsExistLineData = "";
            SAPbobsCOM.IRecordset oRs1 = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            SAPbobsCOM.IRecordset oRs2 = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            #region MainUDO
            oRs1.DoQuery(Sql_IsExistMasterData);
            IB1MainUDO SP_UDO = new B1MainUDO();
            if (oRs1.RecordCount <= 0)
            {
                SP_UDO = new B1MainUDO();
                SP_UDO.ObjectCode = "AVA_OACP";
                SP_UDO.Fields.Add("U_PromitNum", SalesPomotion.PromotionCode);
                SP_UDO.Fields.Add("U_Zodiac", SalesPomotion.StartDate);
                SP_UDO.Fields.Add("U_Probation", SalesPomotion.EndDate);
                SP_UDO.Fields.Add("U_EmployeeType", SalesPomotion.PromType);
                SP_UDO.OperateType = BoEnumerator.emOperateType.Add;
            }
            else
            {
                SP_UDO = new B1MainUDO();
                SP_UDO.ObjectCode = "AVA_OACP";
                SP_UDO.Fields.Add("U_PromitNum", SalesPomotion.PromotionCode);
                SP_UDO.Fields.Add("U_Zodiac", SalesPomotion.StartDate);
                SP_UDO.Fields.Add("U_Probation", SalesPomotion.EndDate);
                SP_UDO.Fields.Add("U_EmployeeType", SalesPomotion.PromType);
                SP_UDO.OperateType = BoEnumerator.emOperateType.Update;
            }
            #endregion

            #region ChildTable
            IB1ChildTable SP1_Child = new B1ChildTable();

            foreach (var sp1item in SalesPomotion.SalesPromotionRegulationss)
            {
                oRs2.DoQuery(Sql_IsExistLineData);
                int iRowCount = 0;
                if (oRs2.Fields.Item("RowCount").Value == 0)
                {
                    SP1_Child = new B1ChildTable();
                    SP1_Child.TableName = "AVA_ACP1";
                    SP1_Child.KeyField = "LineId";
                    SP1_Child.KeyValue = oRs2.Fields.Item("LineId").Value + iRowCount + 1;
                    iRowCount++;
                    SP1_Child.Fields.Add("U_AreaCode", sp1item.ObjectCode);
                    SP1_Child.Fields.Add("U_CardType", sp1item.ProCondition);
                    SP1_Child.Fields.Add("U_CardCode", sp1item.ProCondition);
                    SP1_Child.Fields.Add("U_CardName", sp1item.ProCondition);
                    if (SP1_Child.Fields.Count > 0) SP_UDO.ChildTables.Add(SP1_Child);
                }
                else
                {
                    SP1_Child = new B1ChildTable();
                    SP1_Child.TableName = "AVA_ACP1";
                    SP1_Child.KeyField = "LineId";
                    SP1_Child.KeyValue = oRs2.Fields.Item("LineId").Value; ;
                    iRowCount++;
                    SP1_Child.Fields.Add("U_AreaCode", sp1item.ObjectCode);
                    SP1_Child.Fields.Add("U_CardType", sp1item.ProCondition);
                    SP1_Child.Fields.Add("U_CardCode", sp1item.ProCondition);
                    SP1_Child.Fields.Add("U_CardName", sp1item.ProCondition);
                    if (SP1_Child.Fields.Count > 0) SP_UDO.ChildTables.Add(SP1_Child);
                }
            }

            #endregion

            B1UDOProcesser objProcesser = new B1UDOConcreteProcesser(SAP.SAPCompany, SP_UDO);
            objProcesser.Process();
            sRetVal = objProcesser.GetLastError();
            if (sRetVal.Length != 0)
            {
                result.ResultValue = ResultType.False;
                result.ResultMessage = string.Format("【{0}】促销活动处理失败，ErrorCode:[{1}],ErrrMsg:[{2}];", SP_UDO.ObjectCode, SAP.SAPCompany.GetLastErrorCode(), SAP.SAPCompany.GetLastErrorDescription());
            }
            else
            {
                
                result.ResultValue = ResultType.True;
                result.ResultMessage = "【" + SP_UDO.ObjectCode.ToString() + "】促销活动处理成功，系统单据：" + SAP.SAPCompany.GetNewObjectKey();
            }

            return result;
        }
    }
}
