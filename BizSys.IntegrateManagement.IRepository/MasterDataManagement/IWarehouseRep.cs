using BizSys.IntegrateManagement.Entity.MasterDataManagement.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.IRepository.MasterDataManagement
{
    public interface IWarehouseRep
    {
        /// <summary>
        /// 获取所有仓库信息
        /// </summary>
        /// <returns></returns>
        WarehouseRootObject GetAllWarehourse();

        /// <summary>
        /// 根据仓库代码获取仓库信息
        /// </summary>
        /// <param name="whsCode"></param>
        /// <returns></returns>
        WarehouseRootObject GetWhsByKey(string whsCode);
    }
}
