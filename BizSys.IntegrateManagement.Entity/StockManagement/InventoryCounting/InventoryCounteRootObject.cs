using BizSys.IntegrateManagement.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.StockManagement.InventoryCounting
{
    /// <summary>
    /// 库存盘点
    /// </summary>
    public class InventoryCounteRootObject: IBaseRootObjects<ResultObjects>
    {
        public List<Informations> Informations { get; set; }
        
    }
}
