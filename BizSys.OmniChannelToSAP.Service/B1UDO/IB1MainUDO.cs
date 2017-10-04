using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizSys.OmniChannelToSAP.Service.B1UDO
{
    public interface IB1MainUDO
    {
        string ObjectCode { get; set; }
        string KeyField { get; set; }
        object KeyValue { get; set; }
        IB1Fields Fields { get; set; }
        BoEnumerator.emOperateType OperateType { get; set; }
        IList<IB1ChildTable> ChildTables { get; set; }
    }
}
