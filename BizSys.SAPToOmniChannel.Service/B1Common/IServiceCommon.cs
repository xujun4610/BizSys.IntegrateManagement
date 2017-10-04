using BizSys.IntegrateManagement.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.SAPToOmniChannel.Service.B1Common
{
    public interface IServiceCommon< IT, ID> where IT:IBaseResultObjects where ID: IBaseRootObjects<IT>
    {

    }
}
