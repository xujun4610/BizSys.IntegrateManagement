using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.BatchNumber
{
    public class DistributionRule
    {
        /// <summary>
        /// 分支
        /// </summary>
        public string BPLId { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string OcrCode { get; set; }

        /// <summary>
        /// 人员
        /// </summary>
        public string OcrCode2 { get; set; }

        /// <summary>
        /// 品类
        /// </summary>
        public string OcrCode3 { get; set; }
    }
}
