using BizSys.IntegrateManagement.Entity.MasterDataManagement.MaterialsGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.IRepository.MasterDataManagement
{
    public interface IMaterialsGroupRep
    {
        /// <summary>
        /// 获取所有物料组数据
        /// </summary>
        /// <returns></returns>
        MaterialsGroupRootObject GetAllMaterialsGroup();

        MaterialsGroupRootObject GetMaterialsGroupByKey(string ItmsGrpCod);
    }
}
