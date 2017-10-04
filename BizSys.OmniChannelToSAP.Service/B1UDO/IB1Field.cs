using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizSys.OmniChannelToSAP.Service.B1UDO
{
    public interface IB1Field
    {
        string FieldName { get; set; }
        object FieldValue { get; set; }
    }
}
