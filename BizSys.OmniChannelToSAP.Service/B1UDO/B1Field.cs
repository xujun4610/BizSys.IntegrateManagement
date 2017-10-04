using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizSys.OmniChannelToSAP.Service.B1UDO
{
    internal class B1Field : IB1Field
    {
        internal B1Field() { }

        private string _FieldName;
        public string FieldName 
        {
            get { return _FieldName; }
            set { _FieldName = value; }
        }

        private object _FieldValue;
        public object FieldValue 
        {
            get { return _FieldValue; }
            set { _FieldValue = value; }
        }
    }
}
