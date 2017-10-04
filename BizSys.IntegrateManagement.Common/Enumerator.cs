using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Common
{
    public class Enumerator
    {
        public enum emResourceType
        {
            Channel = 11,
            B1 = 12,
            Anywhere = 13,
            JingDong = 14,
            TianMao = 14
        }
        /// <summary>
        /// 开票类型
        /// </summary>
        public enum emBillType
        {
            Common = 11,
            AddValueTax = 12
        }
        /// <summary>
        /// 仓库类型
        /// </summary>
        public enum emWhsType
        {
            MainWarehouse = 11,
            TerminalWarehouse = 12,
            StoreWarehouse = 13,
            TemporaryWarehouse = 14
        }

        /// <summary>
        /// 付款类型
        /// </summary>
        public enum emPayType
        {
            /// <summary>
            /// 预付款
            /// </summary>
            Prepament = 13,
            /// <summary>
            /// 应付款
            /// </summary>
            Despayment = 12,
            /// <summary>
            /// 退货付款
            /// </summary>
            RtnPayment = 11
        }
        /// <summary>
        /// 付款方式
        /// </summary>
        public enum emMethods
        {
            Cash = 13,
            BankCard = 11,
            ServiceCard = 12,
            Others = 14
        }

        /// <summary>
        /// 发货方式
        /// </summary>
        public enum emSendWay
        {
            Self = 11,
            Send = 12
        }

    }
}
