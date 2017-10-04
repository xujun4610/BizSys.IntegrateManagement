using BizSys.IntegrateManagement.Entity.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.IRepository.Task
{
    public interface ITaskRep
    {
        /// <summary>
        /// 获取任务表中未同步的物料
        /// </summary>
        /// <param name="materialsCount">查询数量</param>
        /// <returns></returns>
        TaskRootObjects GetDocumentWithNoSync<T>(T documentObject,int materialsCount);

        bool UpdateDocumentWithSyncSucc(int DocEntry);

        void UpdateDocumentOrderNo(string TableName, string DocEntry, string IMDocEntry);

        
    }
}
