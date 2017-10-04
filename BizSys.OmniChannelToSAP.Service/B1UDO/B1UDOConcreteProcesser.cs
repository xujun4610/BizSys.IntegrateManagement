using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizSys.OmniChannelToSAP.Service.B1UDO
{
    internal class B1UDOConcreteProcesser : B1UDOProcesser
    {
        internal B1UDOConcreteProcesser(SAPbobsCOM.Company SBOCompany, IB1MainUDO b1MainUDO) : base(SBOCompany, b1MainUDO) 
        { }
    }
}
