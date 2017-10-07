using BizSys.IntegrateManagement.Entity.SalesManagement.SalesOrder;
using BizSys.IntegrateManagement.Entity.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizSys.OmniChannelToSAP.Service.B1Common;

namespace BizSys.OmniChannelToSAP.Service.Document.SalesManagement
{
    public class CancelOrCloseSalesOrder
    {
        public static Result CreateCancelSalesOrder(ResultObjects order)
        {
            int B1DocEntry;
            SAPbobsCOM.Documents myDocuments;
            myDocuments = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);

            /// 判断该采购单是否取消 若取消了 返回成功；否则取消该订单            
            //1、判断订单是否取消   思路： 取消订单的DocStatus的值为 C
            if (B1Common.BOneCommon.IsCancelDocument("ORDR", order.DocEntry.ToString(), out B1DocEntry))
            {
                order.B1DocEntry = B1DocEntry.ToString();
                return new Result()
                {
                    ResultValue = ResultType.True,
                    ResultMessage = "该取消订单已生成到B1"
                };
            }
            Result result = new Result();

            //取消订单   思路：查看SDK 查看Document对象关于Cancel方法的用法
            //2、通过getByKey方法检测是否存在该单据，调用Cancel（）方法。
            if(myDocuments.GetByKey(B1DocEntry))
            {
                int RntCode = myDocuments.Cancel();

                if (RntCode != 0)
                {
                    result.ResultValue = ResultType.False;
                    result.ResultMessage = string.Format("【{0}】取消销售订单处理失败，ErrorCode:[{1}],ErrrMsg:[{2}];", order.DocEntry, SAP.SAPCompany.GetLastErrorCode(), SAP.SAPCompany.GetLastErrorDescription());
                }
                else
                {
                    result.ResultValue = ResultType.True;
                    order.B1DocEntry = SAP.SAPCompany.GetNewObjectKey();
                    result.ResultMessage = "【" + order.DocEntry.ToString() + "】销售订单处理成功，系统单据：" + order.B1DocEntry;
                }
            }
            else
            {
                result.ResultValue = ResultType.False;
                result.ResultMessage = string.Format("【{0}】取消销售订单处理失败，B1中未找到该订单【" + B1DocEntry + "】");
            }
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myDocuments);
            return result;
        }

        public static Result CreateCloseSalesOrder(ResultObjects order)
        {
            Result result = new Result();
            /// 判断该采购单是否关闭 若已关闭 返回成功；否则关闭该订单
            //判断订单是否关闭 思路：关闭订单的DocStatus的值为 C



            //关闭订单  思路：查看SDK 查看Document对象关于Close方法的用法


            return result;
        }
    }
}
