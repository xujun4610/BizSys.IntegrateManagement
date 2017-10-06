using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.CallBack
{
    public class CallBackResult
    {
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ResultCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SignID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserSign { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> Informations { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> ResultObjects { get; set; }
    }

    public class Informations
    {

    }

    public class ResultObjects
    {

    }

}
