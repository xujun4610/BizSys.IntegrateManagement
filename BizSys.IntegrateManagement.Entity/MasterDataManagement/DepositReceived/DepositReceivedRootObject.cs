﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.MasterDataManagement.DepositReceived
{
    /// <summary>
    /// 预收账款
    /// </summary>
    public class DepositReceivedRootObject
    {
        public string type { get; set; }
        public string Message { get; set; }
        public int ResultCode { get; set; }
        public string SignID { get; set; }
        public string Time { get; set; }
        public List<Informations> Informations { get; set; }
        public List<ResultObjects> ResultObjects { get; set; }
    }
}
