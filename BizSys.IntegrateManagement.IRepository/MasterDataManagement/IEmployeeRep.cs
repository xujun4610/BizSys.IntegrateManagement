using BizSys.IntegrateManagement.Entity.MasterDataManagement.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.IRepository.MasterDataManagement
{
    public interface IEmployeeRep
    {
        /// <summary>
        /// 获取所有员工主数据
        /// </summary>
        /// <returns></returns>
        EmployeeRootObject GetAllEmployee();

        /// <summary>   
        /// 根据员工代码获取员工信息
        /// </summary>
        /// <param name="EmployeeID"></param>
        /// <returns></returns>
        EmployeeRootObject GetEmployeeByKey(string EmployeeID);
    }
}
