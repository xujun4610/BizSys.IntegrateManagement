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
}
