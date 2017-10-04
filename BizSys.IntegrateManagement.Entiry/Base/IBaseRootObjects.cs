using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.Base
{
    public class IBaseRootObjects<T> where T:IBaseResultObjects
    {
        public string type { get; set; }
        public string Message { get; set; }
        public int ResultCode { get; set; }
        public string SignID { get; set; }
        public string Time { get; set; }

        public List<T> ResultObjects { get; set; }
    }
}
