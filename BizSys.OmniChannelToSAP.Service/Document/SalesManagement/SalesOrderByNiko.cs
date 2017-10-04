using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.OmniChannelToSAP.Service.B1Common;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BizSys.IntegrateManagement.Common.Enumerator;

namespace BizSys.OmniChannelToSAP.Service.Document.SalesManagement
{
    public class SalesOrderByNiko
    {
        //here is SalesOrder in SAP (ORDR,RDR1)
        /// <summary>
        /// 生成销售订单或者交货单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static Result CreateSalesOrder(BizSys.IntegrateManagement.Entiry.SalesManagement.SalesOrderByNiko.ResultObjects order)
        {
            string B1DocEntry;
            SAPbobsCOM.Documents myDocuments;
            //if (BOneCommon.IsMainStore(order.SalesOrderItems.FirstOrDefault().Warehouse))
            //{
            //生成销售订单
            if (B1Common.BOneCommon.IsExistDocument("ORDR", order.DocEntry.ToString(), out B1DocEntry))
            {
                order.B1DocEntry = B1DocEntry;
                return new Result()
                {
                    ResultValue = ResultType.True,
                    ResultMessage = "该订单已生成到B1"
                };
            }
            myDocuments = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);


            Result result = new Result();


            myDocuments.Series = Convert.ToInt32(order.Series);
            myDocuments.CardCode = order.BusinessPartnerCode;
            myDocuments.CardName = order.BusinessPartnerName;
            myDocuments.Reference1 = order.Reference1;
            myDocuments.Reference2 = order.Reference2;
            myDocuments.DocDate = order.PostingDate;
            myDocuments.DocDueDate = order.DeliveryDate;
            myDocuments.TaxDate = order.DocumentDate;
            myDocuments.Comments = order.Remarks;

            myDocuments.HandWritten = order.Handwritten == "Yes" ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
            //myDocuments.Address = order.DetailedAddress;


            myDocuments.UserFields.Fields.Item("U_DeliveryAddress").Value = order.Province + '-' + order.City + '-' + order.County + '-' + order.DetailedAddress;//送货地址（详细）
            //myDocuments.UserFields.Fields.Item("U_PlanDate").Value = order.DocumentDate;
            myDocuments.UserFields.Fields.Item("U_Linkman").Value = DataConvert.GetValue(order.Consignee);//联系人 收货人
            myDocuments.UserFields.Fields.Item("U_Telephone").Value = DataConvert.GetValue(order.ContactNumber);//联系电话
            myDocuments.Printed = order.Printed == "Yes" ? PrintStatusEnum.psYes : PrintStatusEnum.psNo;
            myDocuments.BPL_IDAssignedToInvoice = BOneCommon.GetBranchCodeByWhsCode(order.SalesOrderItems.FirstOrDefault().Warehouse);
            myDocuments.UserFields.Fields.Item("U_PickingWay").Value = DataConvert.GetValue(order.PickingWay);

            if (string.IsNullOrEmpty(order.BillType))
                return new Result() { ResultValue = ResultType.False, ResultMessage = "发票类型为空。" };
            myDocuments.UserFields.Fields.Item("U_InvoiceType").Value = ((emBillType)Enum.Parse(typeof(emBillType), order.BillType)).GetHashCode().ToString();
            myDocuments.UserFields.Fields.Item("U_IM_DocEntry").Value = order.DocEntry.ToString();
            if (order.DataSource == "")
                myDocuments.UserFields.Fields.Item("U_ResouceType").Value = "11";
            else
                myDocuments.UserFields.Fields.Item("U_ResouceType").Value = order.DataSource;

            foreach (var item in order.SalesOrderItems)
            {
                myDocuments.Lines.ItemCode = item.ItemCode;
                myDocuments.Lines.ItemDescription = item.ItemDescription;
                myDocuments.Lines.Quantity = Convert.ToDouble(item.Quantity);

                myDocuments.Lines.DiscountPercent = double.Parse(item.DiscountPerLine);//折扣率
                myDocuments.Lines.UnitPrice = Math.Round(item.UnitPrice, 2);
                myDocuments.Lines.VatGroup = B1Common.BOneCommon.GetTaxByRate(item.TaxRatePerLine, "O");
                myDocuments.Lines.WarehouseCode = item.Warehouse;
                myDocuments.Lines.UserFields.Fields.Item("U_Category").Value = item.ItemType;
                var dbRule = BOneCommon.GetDistributionRule(item.ItemCode, order.DataOwner.ToString());

                myDocuments.Lines.CostingCode = dbRule.OcrCode;
                myDocuments.Lines.CostingCode2 = dbRule.OcrCode2;
                myDocuments.Lines.CostingCode3 = dbRule.OcrCode3;

                myDocuments.Lines.CostingCode4 = item.DistributionRule4;
                myDocuments.Lines.CostingCode5 = item.DistributionRule5;
                myDocuments.Lines.UserFields.Fields.Item("U_IM_DocEntry").Value = item.DocEntry;
                myDocuments.Lines.UserFields.Fields.Item("U_IM_LineId").Value = item.LineId;

                myDocuments.Lines.Add();

            }
            myDocuments.DocTotal = Math.Round(order.DocumentTotal, 2);
            int RntCode = myDocuments.Add();

            if (RntCode != 0)
            {
                result.ResultValue = ResultType.False;
                result.ResultMessage = string.Format("【{0}】销售订单处理失败，ErrorCode:[{1}],ErrrMsg:[{2}];", order.DocEntry, SAP.SAPCompany.GetLastErrorCode(), SAP.SAPCompany.GetLastErrorDescription());
            }
            else
            {
                result.ResultValue = ResultType.True;
                order.B1DocEntry = SAP.SAPCompany.GetNewObjectKey();
                result.ResultMessage = "【" + order.DocEntry.ToString() + "】销售订单处理成功，系统单据：" + SAP.SAPCompany.GetNewObjectKey();
            }
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myDocuments);
            return result;
        }
    }
}
