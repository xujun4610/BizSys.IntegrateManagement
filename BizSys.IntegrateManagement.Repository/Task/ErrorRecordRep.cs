using BizSys.IntegrateManagement.IRepository.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizSys.IntegrateManagement.Entity.Task;

namespace BizSys.IntegrateManagement.Repository.Task
{
    public class ErrorRecordRep : IErrorRecordRep
    {
        public void CreateErrorInfo(ErrorRecord errorInfo)
        {
            try
            {
                SAPbobsCOM.IRecordset res = BOneCommon.SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string sql = $@"insert into U_ErrorRecord(ObjectCode,Uniquekey,ErrorType,SBOID,CreateDate,IsSync,ErrorMsg) 
                            values('{errorInfo.ObjectCode}',
                                   '{errorInfo.UniqueKey}',
                                   {errorInfo.ErrorType},
                                   '{errorInfo.SBOID}',
                                   '{DateTime.Now}',
                                   'N',
                                   '{errorInfo.ErrorMsg}')";
                res.DoQuery(sql);

            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public IList<ErrorRecord> GetErrorInfo(int mCount)
        {
            IList<ErrorRecord> errorInfoList = new List<ErrorRecord>();
            try
            {
                SAPbobsCOM.IRecordset res = BOneCommon.SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string sql = $@"select top {mCount} * from U_ErrorRecord where IsSync = 'N'";
                res.DoQuery(sql);
                while(!res.EoF)
                {
                    ErrorRecord errorInfo = new ErrorRecord();
                    errorInfo.ObjectCode = res.Fields.Item("ObjectCode").Value;
                    errorInfo.UniqueKey = res.Fields.Item("UniqueKey").Value;
                    errorInfo.CreateDate = res.Fields.Item("CreateDate").Value;
                    errorInfo.ErrorMsg = res.Fields.Item("ErrorMsg").Value;
                    errorInfoList.Add(errorInfo);
                }
                return errorInfoList;
            }
            catch(Exception ex)
            {
                throw ex.InnerException;
            }
        }


    }
}
