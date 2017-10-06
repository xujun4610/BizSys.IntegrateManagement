using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.Token
{
    public class ResultObjects
    {
        /// <summary>
        /// User
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// IsDirty
        /// </summary>
        public bool isDirty { get; set; }
        /// <summary>
        /// IsDeleted
        /// </summary>
        public bool isDeleted { get; set; }
        /// <summary>
        /// IsNew
        /// </summary>
        public bool isNew { get; set; }
        /// <summary>
        /// Yes
        /// </summary>
        public string Activated { get; set; }
        /// <summary>
        /// CreateTime
        /// </summary>
        public int CreateTime { get; set; }
        /// <summary>
        /// CreateUserSign
        /// </summary>
        public int CreateUserSign { get; set; }
        /// <summary>
        /// DataOwner
        /// </summary>
        public int DataOwner { get; set; }
        /// <summary>
        /// LogInst
        /// </summary>
        public int LogInst { get; set; }
        /// <summary>
        /// AVA_SYS_USER
        /// </summary>
        public string ObjectCode { get; set; }
        /// <summary>
        /// ObjectKey
        /// </summary>
        public int ObjectKey { get; set; }
        /// <summary>
        /// Series
        /// </summary>
        public int Series { get; set; }
        /// <summary>
        /// Yes
        /// </summary>
        public string SupperUser { get; set; }
        /// <summary>
        /// UpdateTime
        /// </summary>
        public int UpdateTime { get; set; }
        /// <summary>
        /// UpdateUserSign
        /// </summary>
        public int UpdateUserSign { get; set; }
        /// <summary>
        /// admin
        /// </summary>
        public string UserCode { get; set; }
        /// <summary>
        /// adminitrator
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 3fde6bb0541387e4ebdadf7c2ff31123=
        /// </summary>
        public string UserPassword { get; set; }
    }
}
