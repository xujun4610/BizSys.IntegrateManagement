using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.Task
{
    public class ErrorRecord
    {
        /// <summary>
        /// 单据类型/单据对象号
        /// </summary>
        public string ObjectCode { get; set; }

        /// <summary>
        /// 关键字（主键）
        /// </summary>
        public string UniqueKey { get; set; }

        /// <summary>
        /// 错误类型 回调错误 0/推送错误 1
        /// </summary>
        public Int16 ErrorType { get; set; }

        /// <summary>
        /// B1中单号/主键（回调错误类型才有）
        /// </summary>
        public string SBOID { get; set; }

        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 是否成功推送/回调至全渠道   默认为N
        /// </summary>
        public string IsSync { get; set; }

        public DateTime SyncDate { get; set; }

        public string ErrorMsg { get; set; }
    }
}
