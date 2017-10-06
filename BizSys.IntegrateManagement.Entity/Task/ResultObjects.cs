using BizSys.IntegrateManagement.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.Task
{
    public class ResultObjects:IBaseResultObjects
    {
       
        /// <summary>
        /// 单据主键
        /// </summary>
        public System.String UniqueKey
        {
            get;
            set;
        }

        /// <summary>
        /// 创建日期
        /// </summary>
        public System.DateTime CreateDate
        {
            set;
            get;
        }
        /// <summary>
        /// 创建时间
        /// </summary>
        public System.Int16 CreateTime
        {
            set;
            get;
        }
        /// <summary>
        /// 状态
        /// </summary>
        public System.String Status
        {
            set;
            get;
        }
        /// <summary>
        /// 对象
        /// </summary>
        public System.String BusinessType
        {
            set;
            get;
        }
        /// <summary>
        /// 方向
        /// </summary>
        public System.String Direction
        {
            set;
            get;
        }
        ///// <summary>
        ///// 条形码
        ///// </summary>
        //System.String CodeBars
        //{
        //    set;
        //    get;
        //}
        /// <summary>
        /// 仓库
        /// </summary>
        public System.String Warehouse
        {
            set;
            get;
        }
        /// <summary>
        /// 是否同步
        /// </summary>
        public bool IsSync
        {
            set;
            get;
        }

        /// <summary>
        /// 是否同步失败
        /// </summary>
        public bool IsSyncError
        {
            set;
            get;
        }
        /// <summary>
        /// 同时失败原因
        /// </summary>
        public System.String SyncErrorMsg
        {
            get;
            set;
        }
        /// <summary>
        /// 同步日期
        /// </summary>
        public System.DateTime SyncDate
        {
            set;
            get;
        }
        /// <summary>
        /// 同步时间
        /// </summary>
        public System.Int16 SyncTime
        {
            set;
            get;
        }

        /// <summary>
        /// 同步失败日期
        /// </summary>
        public System.DateTime SyncErrorDate
        {
            set;
            get;
        }
        /// <summary>
        /// 同步失败时间
        /// </summary>
        public System.Int16 SyncErrorTime
        {
            set;
            get;
        }


        public System.String ObjectCode
        {
            set;
            get;
        }

        
       
        /// <summary>
        /// 草稿头
        /// </summary>
        public System.String DraftObject
        {
            get;
            set;
        }

    }
}

