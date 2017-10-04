using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.Result
{
    public class Result
    {
        public Result()
        {
            _ErrorList = new ErrorOrderList();
        }
        public ResultType ResultValue { get; set; }

        public int DocEntry { get; set; }
        public string ResultMessage { get; set; }
        private ErrorOrderList _ErrorList;
        public ErrorOrderList ErrorList
        {
            get { return _ErrorList; }
        }
    }
}
