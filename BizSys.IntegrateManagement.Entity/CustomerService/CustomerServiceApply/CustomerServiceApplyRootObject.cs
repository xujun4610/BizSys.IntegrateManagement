﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.CustomerService.CustomerServiceApply
{
    public class CustomerServiceApplyRootObject
    {
        public string type { get; set; }
        public string Message { get; set; }
        public string ResultCode { get; set; }
        public string SignID { get; set; }
        public string Time { get; set; }
        public List<Informations> Informations { get; set; }
        public List<ResultObjects> ResultObjects { get; set; }
    }
}
