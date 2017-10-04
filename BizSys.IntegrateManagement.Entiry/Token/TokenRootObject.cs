using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.Token
{
    public class TokenRootObject
    {
        /// <summary>
        /// OperationResult
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// operation successful.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// ResultCode
        /// </summary>
        public int ResultCode { get; set; }
        /// <summary>
        /// 947a33f6-2ed0-4d9c-ad01-9ac063e5f73f
        /// </summary>
        public string SignID { get; set; }
        /// <summary>
        /// 2016-09-20T14:03:55
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// 68fc6bac014d06ad94c5734116487cff
        /// </summary>
        public string UserSign { get; set; }
        /// <summary>
        /// Informations
        /// </summary>
        public List<string> Informations { get; set; }
        /// <summary>
        /// ResultObjects
        /// </summary>
        public List<ResultObjects> ResultObjects { get; set; }
    }
}
