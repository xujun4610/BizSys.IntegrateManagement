using BizSys.IntegrateManagement.Entity.MasterDataManagement.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.MasterDataManagement.IRepository
{
    public interface IMaterialsRep
    {
        /// <summary>
        /// 获取SAP中所有的物料信息
        /// </summary>
        /// <returns></returns>
        MaterialsRootObject GetAllMaterials();

        /// <summary>
        /// 通过ItemCode查询一条物料信息
        /// </summary>
        /// <returns></returns>
        MaterialsRootObject GetMaterialsByKey(string ItemCode);
    }
}
