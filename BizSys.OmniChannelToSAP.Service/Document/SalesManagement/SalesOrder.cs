using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.IntegrateManagement.Entity.SalesManagement.SalesOrder;
using BizSys.OmniChannelToSAP.Service.B1Common;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BizSys.IntegrateManagement.Common.Enumerator;

namespace BizSys.OmniChannelToSAP.Service.Document.SalesManagement
{
    public class SalesOrder
    {
        /// <summary>
        /// 生成销售订单或者交货单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static Result CreateSalesOrder(ResultObjects order)
        {
            Result result = new Result();
            /*
             * 梅菲特奇葩要求
             * 分帐套存放salesorder
             * 根据用户 XZ，BJ， 分别存放 账套
             */
            if (string.IsNullOrWhiteSpace(order.Salesperson))
            {
                result.ResultValue = ResultType.False;
                result.ResultMessage = string.Format("OCM销售订单[{0}]，没有销售员。", order.DocEntry.ToString());
                result.DocEntry = order.DocEntry.ToString();
                return result;
            }
            if (!order.Salesperson.ToUpper().Contains("XZ") && !order.Salesperson.ToUpper().Contains("BJ"))
            {
                result.ResultValue = ResultType.False;
                result.ResultMessage = string.Format("OCM销售订单[{0}]，销售员[{1}]，无法区分所属分支。", order.DocEntry.ToString(), order.Salesperson);
                result.DocEntry = order.DocEntry.ToString();
                return result;
            }

            string B1DocEntry = string.Empty;
            string B1DlftWhsCode = "W0101"; //别的项目请修改这里
            string B1AccountCode = "6001"; //总账科目
            string B1SaleCostCode = "640101"; //销货成本
            //销售员标识
            string slpCodeSign = order.Salesperson.Substring(0, 2).ToUpper();


            //if (BOneCommon.IsMainStore(order.SalesOrderItems.FirstOrDefault().Warehouse))
            //{
            //生成销售订单

            if (B1Common.BOneCommon.IsExistDocument4MFT(slpCodeSign, "ORDR", order.DocEntry.ToString(), out B1DocEntry))
            {
                order.B1DocEntry = B1DocEntry;
                return new Result()
                {
                    ResultValue = ResultType.True,
                    ResultMessage = "该订单已生成到B1"
                };
            }

            //SAPCompanyPool.AllCompany();

            SAPbobsCOM.Documents myDocuments = SAPCompanyPool.GetSAPCompany(slpCodeSign).GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);

            myDocuments.Series = B1Common.BOneCommon.GetB1DocEntrySeries4MFT(slpCodeSign, "17");
            myDocuments.CardCode = order.BusinessPartnerCode;
            myDocuments.CardName = order.BusinessPartnerName;
            //myDocuments.Reference1 = order.Reference1;
            //myDocuments.Reference2 = order.Reference2;
            myDocuments.DocDate = order.PostingDate;
            myDocuments.DocDueDate = order.DeliveryDate;
            myDocuments.TaxDate = order.DocumentDate;
            myDocuments.Comments = order.Remarks;
            //地址
            //string[] textArray1 = new string[] { "CN", order.Province, order.City, order.County, order.Town, order.ShipTo };
            //myDocuments.Address2 = string.Concat(textArray1);
            //myDocuments.Address = order.DetailedAddress;
            myDocuments.DocCurrency = "RMB";
            //梅菲特定制区域
            myDocuments.UserFields.Fields.Item("U_101").Value = order.DocumentType; //11，12 仅限可选值 单据类型
            myDocuments.UserFields.Fields.Item("U_BillType").Value = order.BillType.ToUpper() == "COMMON" ? "11" : "12"; //11，12 仅限可选值 开票类型
            myDocuments.UserFields.Fields.Item("U_Voucher").Value = order.ServiceNumberManagementMoney; //代金券金额 ，double类型 注意
            if (!string.IsNullOrWhiteSpace(order.PrmtsContent))
                myDocuments.UserFields.Fields.Item("U_PrmtsContent").Value = order.PrmtsContent; //促销活动内容
            myDocuments.UserFields.Fields.Item("U_OCM_DocEntry").Value = order.DocEntry.ToString(); //同步过来的OCM订单编号

            foreach (var item in order.SalesOrderItems)
            {
                myDocuments.Lines.ItemCode = item.ItemCode;
                myDocuments.Lines.Quantity = Convert.ToDouble(item.Quantity);
                if (item.GoodsType.Equals("Present"))
                {
                    myDocuments.Lines.DiscountPercent = 100.0;
                }
                else
                {
                    myDocuments.Lines.DiscountPercent = double.Parse(item.DiscountPerLine);//折扣率
                }
                myDocuments.Lines.VatGroup = B1Common.BOneCommon.GetTaxByRate4MFT(slpCodeSign, item.TaxRatePerLine, "O");
                myDocuments.Lines.WarehouseCode = B1Common.BOneCommon.IsExistWarehouse4MFT(slpCodeSign, item.Warehouse) == true ? item.Warehouse : B1DlftWhsCode;
                myDocuments.Lines.UnitPrice = item.UnitPrice;
                myDocuments.Lines.PriceAfterVAT = item.GrossPrice;
                //单位？？？？(物料自动填写)
                //梅菲特 定制字段
                myDocuments.Lines.UserFields.Fields.Item("U_YSMJ").Value = item.GrossPriceTemp; //原始毛价
                myDocuments.Lines.UserFields.Fields.Item("U_001").Value = item.Reference1; //注释
                myDocuments.Lines.UserFields.Fields.Item("U_Rebate").Value = item.Reference2; //返利情况

                //其他
                myDocuments.Lines.UserFields.Fields.Item("U_OCM_DocEntry").Value = item.DocEntry.ToString();
                myDocuments.Lines.UserFields.Fields.Item("U_OCM_LineId").Value = item.LineId;

                myDocuments.Lines.Add();

            }
            myDocuments.DocTotal = Math.Round(order.DocumentTotal, 2);
            int RntCode = myDocuments.Add();

            if (RntCode != 0)
            {
                result.ResultValue = ResultType.False;
                result.ResultMessage = string.Format("【{0}】销售订单处理失败，ErrorCode:[{1}],ErrrMsg:[{2}];", order.DocEntry, SAPCompanyPool.GetSAPCompany(slpCodeSign).GetLastErrorCode(), SAPCompanyPool.GetSAPCompany(slpCodeSign).GetLastErrorDescription());
            }
            else
            {
                B1DocEntry = SAPCompanyPool.GetSAPCompany(slpCodeSign).GetNewObjectKey();
                result.ResultValue = ResultType.True;
                result.DocEntry = B1DocEntry;
                result.ResultMessage = "【" + order.DocEntry.ToString() + "】销售订单处理成功，系统单据：" + result.DocEntry;
            }
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myDocuments);
            SAPCompanyPool.DisconnectAll();

            return result;
        }
    }
}
