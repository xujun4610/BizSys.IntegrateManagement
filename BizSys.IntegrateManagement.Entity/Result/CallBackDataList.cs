using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.Result
{
    /// <summary>
    /// Result 类 回写数据集合类
    /// </summary>
    public class CallBackDataList : List<CallBackData>, IList<CallBackData>
    {
        public CallBackData New()
        {
            CallBackData callback = new CallBackData();
            this.Add(callback);
            return callback;
        }
    }
}
