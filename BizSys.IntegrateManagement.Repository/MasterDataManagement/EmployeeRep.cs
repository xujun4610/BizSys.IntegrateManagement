
using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.Employee;
using BizSys.IntegrateManagement.IRepository.MasterDataManagement;
using BizSys.IntegrateManagement.Repository.BOneCommon;
using System;
using System.Collections.Generic;

namespace BizSys.IntegrateManagement.Repository.MasterDataManagement
{
    public class EmployeeRep:IEmployeeRep
    {
        public Entity.MasterDataManagement.Employee.EmployeeRootObject GetAllEmployee()
        {
            EmployeeRootObject employeeRootObject = new EmployeeRootObject();
            employeeRootObject.ResultObjects = new List<ResultObjects>();
            try
            {
                SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string sql = @"select * from OHEM";
                res.DoQuery(sql);
                while (!res.EoF)
                {
                    ResultObjects employee = new ResultObjects();

                    employee.EmployeeID = res.Fields.Item("empID").Value;
                    employee.EmployeeType = res.Fields.Item("type").Value;
                    employee.E_Mail = res.Fields.Item("email").Value;
                    employee.MobilePhone = res.Fields.Item("mobile").Value;

                    employeeRootObject.ResultObjects.Add(employee);
                    res.MoveNext();
                }
                employeeRootObject.ResultCode = 0;
                employeeRootObject.Message = "Successful operation.";
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
            catch (Exception ex)
            {
                employeeRootObject.ResultCode = -1;
                employeeRootObject.Message = ex.Message;
            }
            return employeeRootObject;
        }

        public Entity.MasterDataManagement.Employee.EmployeeRootObject GetEmployeeByKey(string EmployeeID)
        {
            EmployeeRootObject employeeRootObject = new EmployeeRootObject();
            employeeRootObject.ResultObjects = new List<ResultObjects>();
            SAPbobsCOM.EmployeesInfo employeeInfo = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oEmployeesInfo);

            if (!employeeInfo.GetByKey(DataConvert.ConvertToIntEx(EmployeeID)))
            {
                employeeRootObject.ResultCode = -1;
                employeeRootObject.Message = "Can not found the employee in SAP.";
            }
            else {
                employeeRootObject.ResultCode = 0;
                employeeRootObject.Message = "Successful operation.";
                ResultObjects employee = new ResultObjects();

                #region 员工属性赋值
                employee.EmployeeID = employeeInfo.EmployeeID.ToString();
                employee.EmployeeType = employeeInfo.EmployeeType.ToString();
                employeeInfo.eMail = employeeInfo.eMail;
                employee.MobilePhone = employeeInfo.MobilePhone;
                #endregion

                employeeRootObject.ResultObjects.Add(employee);
            }
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(employeeRootObject);
            return employeeRootObject;
        }
    }
}
