using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizSys.OmniChannelToSAP.Service.B1UDO
{
    public interface IB1ChildTable
    {
        string TableName { get; set; }
        string KeyField { get; set; }
        object KeyValue { get; set; }
        IB1Fields Fields { get; set; }
    }
}
