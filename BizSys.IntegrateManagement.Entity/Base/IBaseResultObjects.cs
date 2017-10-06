using BizSys.IntegrateManagement.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.Base
{
   public class IBaseResultObjects
    {
        public string type { get; set; }
        public bool isDirty { get; set; }
        public bool isDeleted { get; set; }
        public bool isNew { get; set; }
        public int DocEntry { get; set; }
        public string B1DocEntry { get; set; }
        public string ObjectCode { get; set; }
      
        public string DataSource { get; set; }

        public List<UserFields> UserFields { get; set; }

    }
}
