using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizSys.OmniChannelToSAP.Service.B1UDO
{
    internal class B1Fields : List<IB1Field>, IB1Fields
    {
        public void Add(string FieldName,object FieldValue)
        {
            IB1Field field = new B1Field();
            field.FieldName = FieldName;
            field.FieldValue = FieldValue;
            base.Add(field);
        }
    }
}
