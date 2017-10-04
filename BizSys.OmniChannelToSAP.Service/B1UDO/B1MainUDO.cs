using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizSys.OmniChannelToSAP.Service.B1UDO
{
    internal class B1MainUDO : IB1MainUDO
    {
        internal B1MainUDO() 
        {
            if (_ChildTables == null) _ChildTables = new List<IB1ChildTable>();
            if (_Fields == null) _Fields = new B1Fields();
        }

        private string _ObjectCode;
        public string ObjectCode
        {
            get { return _ObjectCode; }
            set { _ObjectCode = value; }
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

        private BoEnumerator.emOperateType _OperateType;
        public BoEnumerator.emOperateType OperateType
        {
            get { return _OperateType; }
            set { _OperateType = value; }
        }

        private IList<IB1ChildTable> _ChildTables;
        public IList<IB1ChildTable> ChildTables
        {
            get { return _ChildTables; }
            set { _ChildTables = value; }
        }
    }
}
