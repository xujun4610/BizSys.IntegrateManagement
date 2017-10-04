using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizSys.OmniChannelToSAP.Service.B1UDO
{
    public interface IB1Fields: IList<IB1Field>
    {
        void Add(string FieldName,object FieldValue);
    }
}
