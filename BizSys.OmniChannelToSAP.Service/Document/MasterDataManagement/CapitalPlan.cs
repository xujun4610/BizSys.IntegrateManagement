using BizSys.IntegrateManagement.Entity.MasterDataManagement.CapitalPlan;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.OmniChannelToSAP.Service.B1Common;
using BizSys.OmniChannelToSAP.Service.B1UDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.OmniChannelToSAP.Service.Document.MasterDataManagement
{
    public class CapitalPlan
    {
        /// <summary>
        /// 资金计划
        /// </summary>
        /// <param name="capitalPlan"></param>
        /// <returns></returns>
        public static Result CreateCapitalPlan(ResultObjects capitalPlan)
        {
            Result result = new Result();
            string sRetVal = "";
            string Period = "";
            string Sql_IsMasterDataExist = "";
            string Sql_IsLineDataExist = "";
            SAPbobsCOM.IRecordset oRs = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            SAPbobsCOM.IRecordset oRs2 = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            oRs.DoQuery(string.Format(Sql_IsMasterDataExist, Period));//校验期间是否存在
            IB1MainUDO curUDO = new B1MainUDO();
            IB1ChildTable curChild = new B1ChildTable();
            if (oRs.RecordCount <= 0)
            {
                #region MainUDO
                curUDO = new B1MainUDO();
                curUDO.ObjectCode = "AVA_OOMS";
                curUDO.Fields.Add("Code", Period);
                #endregion
                curUDO.OperateType = BoEnumerator.emOperateType.Add;
            }
            else

            {
                #region MainUDO
                curUDO = new B1MainUDO();
                curUDO.ObjectCode = "AVA_OOMS";
                curUDO.KeyField = "Code";
                curUDO.KeyValue = Period;
                #endregion
                curUDO.OperateType = BoEnumerator.emOperateType.Update;
            }
            #region ChildTable
            foreach (var item in capitalPlan.CapitalPlanItems)
            {

                //判断该行是否存在。
                oRs.DoQuery(string.Format(Sql_IsLineDataExist, Period));//校验行数据是否存在,返回行数RowCount以及LineId(不存在则返回最大的LineId)
                int iRsRowsCountr = 0;
                if (oRs.Fields.Item("RowCount").Value == 0)
                {
                    curChild = new B1ChildTable();
                    curChild.TableName = "AVA_OMS3";
                    curChild.KeyField = "LineId";
                    int NewId = oRs.Fields.Item("LineId").Value + iRsRowsCountr + 1;
                    curChild.KeyValue = NewId;
                    iRsRowsCountr++;

                    curChild.Fields.Add("U_Period", Period);
                    if (curChild.Fields.Count > 0)
                        curUDO.ChildTables.Add(curChild);
                }
                else
                {
                    curChild = new B1ChildTable();
                    curChild.TableName = "AVA_OMS3";
                    curChild.KeyField = "LineId";
                    int NewId = oRs.Fields.Item("LineId").Value;
                    curChild.KeyValue = NewId;
                    iRsRowsCountr++;

                    curChild.Fields.Add("U_Period", Period);
                    if (curChild.Fields.Count > 0)
                        curUDO.ChildTables.Add(curChild);
                }
            }
            #endregion

            B1UDOProcesser objProcesser = new B1UDOConcreteProcesser(SAP.SAPCompany, curUDO);
            objProcesser.Process();
            sRetVal = objProcesser.GetLastError();

            if (sRetVal.Length >= 0)
            {
                result.ResultValue = ResultType.False;
                result.ResultMessage = string.Format("【{0}】资金计划处理失败，ErrorCode:[{1}],ErrrMsg:[{2}];", capitalPlan.DeptId, SAP.SAPCompany.GetLastErrorCode(), SAP.SAPCompany.GetLastErrorDescription());
            }
            else
            {
                //capitalPlan.B1DocEntry = SAP.SAPCompany.GetNewObjectKey();
                result.ResultValue = ResultType.True;
                result.ResultMessage = "【" + capitalPlan.DeptId.ToString() + "】资金计划处理成功，系统单据：" + SAP.SAPCompany.GetNewObjectKey();
            }
            //System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myCp);
            return result;
        }
    }
}
