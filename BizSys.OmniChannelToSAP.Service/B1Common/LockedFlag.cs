using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.OmniChannelToSAP.Service.B1Common
{
    internal class LockedFlag
    {
        /// <summary>
        /// 锁定类标记
        /// </summary>
        public static readonly object obj = new object();
    }
}
