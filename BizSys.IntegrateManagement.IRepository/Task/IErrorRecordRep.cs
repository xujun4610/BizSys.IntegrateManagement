using BizSys.IntegrateManagement.Entity.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.IRepository.Task
{
    public interface IErrorRecordRep
    {
        IList<ErrorRecord> GetErrorInfo(int mCount);

        void CreateErrorInfo(ErrorRecord errorInfo);
    }
}
