using BizSys.IntegrateManagement.Entity.MasterDataManagement.OrganizationDepartments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.IRepository.MasterDataManagement
{
    public interface IOrganizationRep
    {
        OrganizationDepartmentsRootObject GetAllOrganization();
    }
}
