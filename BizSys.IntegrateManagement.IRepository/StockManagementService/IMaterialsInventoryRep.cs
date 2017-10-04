using BizSys.IntegrateManagement.Entity.StockManagement.MaterialsInventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.IRepository.StockManagementService
{
    public interface IMaterialsInventoryRep
    {
        /// <summary>
        /// 获取所有库存信息
        /// </summary>
        /// <returns></returns>
        MaterialsInventoryRootObject GetAllMaterialsInventory();
    }
}
