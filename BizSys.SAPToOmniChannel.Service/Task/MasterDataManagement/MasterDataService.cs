using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.MaterialsGroup;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.Materials;
using BizSys.SAPToOmniChannel.Service.B1Common;
using MagicBox.Log;
using MagicBox.WindowsServices.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BizSys.SAPToOmniChannel.Service.Service.MasterDataManagement;
using BizSys.SAPToOmniChannel.Service.Service.MasterDataManagementService;

namespace BizSys.SAPToOmniChannel.Service.Task.MasterDataManagement
{
    public class MasterDataService : IWindowsService
    {
        public void Run()
        {
            PostMaterialsGroupServcie.PostMaterialsGroup(); ;//推送物料组主数据
            PostMaterialsService.PostMaterials();//推送物料主数据
           // PostMaterialsCategoryService.PostMaterialsCategory();//推送品类
            PostWarehouseService.PostWarehouse();//仓库
            PostEmployeeService.PostEmployee();//员工
           //PostOrganizationService.PostOrganization();//组织部门
        }

        public void Stop()
        {

        }

    }
}
