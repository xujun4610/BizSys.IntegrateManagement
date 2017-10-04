using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizSys.OmniChannelToSAP.Service.B1UDO
{
    internal class B1ChildTable : IB1ChildTable
    {
        internal B1ChildTable() 
        {
            if (_Fields == null) _Fields = new B1Fields(); 
        }

        private string _TableName;
        public string TableName
        {
            get { return _TableName; }
            set { _TableName = value; }
        }

        private string _KeyField;
        public string KeyField
        {
            get { return _KeyField; }
            set { _KeyField = value; }
        }

        private object _KeyValue;
        public object KeyValue
        {
            get { return _KeyValue; }
            set { _KeyValue = value; }
        }

        private IB1Fields _Fields;
        public IB1Fields Fields
        {
            get { return _Fields; }
            set { _Fields = value; }
        }
    }
}
