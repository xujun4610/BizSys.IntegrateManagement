using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.CallBack
{
    public class CallBackRootObject
    {
        public string ObjectId { get; set; }
        public List<QueryParameters> QueryParameters { get; set; }
        public List<Data> Data { get; set; }
        // customed for meffert
        public List<B1_contact> b1_contact { get; set; }
    }
    public class QueryParameters
    {
        public string Key { get; set; }
        public string Text { get; set; }
    }

    public class Data
    {
        public string Key { get; set; }
        public string Text { get; set; }
    }

    #region customed for meffert

    public class B1_contact
    {
        public string Key { get; set; }
        public string Text { get; set; }
    }

    #endregion
}
