using BizSys.IntegrateManagement.Entity.MasterDataManagement.MaterialsCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.IRepository.MasterDataManagement
{
   public  interface IMaterialsCategoryRep
    {
        /// <summary>
        /// 获取所有的品类
        /// </summary>
        MaterialsCategoryRootObject GetAllMaterialsCategory();


        MaterialsCategoryRootObject GetMaterialsCategoryByKey(string CategoryCode);


    }
}
