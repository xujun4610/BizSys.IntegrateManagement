using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Entity.Result
{
    public class ErrorOrderList : List<ErrorOrder>, IList<ErrorOrder>
    {
        public ErrorOrder New()
        {
            ErrorOrder order = new ErrorOrder();
            this.Add(order);
            return order;
        }
    }
}
