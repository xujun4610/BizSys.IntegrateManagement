using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace BizSys.OmniChannelToSAP.Service.B1UDO
{
    public class BoEnumerator 
    {
        #region 基本枚举类型
        /// <summary>
        /// Yes ; No
        /// </summary>
        public enum emOperateType
        {
            [DefaultValue(0), Description("添加")]
            Add,
            [DefaultValue(1), Description("更新")]
            Update
        }
        #endregion
    }
}
