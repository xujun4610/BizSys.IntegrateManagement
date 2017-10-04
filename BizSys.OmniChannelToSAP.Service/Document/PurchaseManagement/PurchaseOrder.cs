using BizSys.IntegrateManagement.Entity.PurchaseOrder;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.OmniChannelToSAP.Service.B1Common;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BizSys.IntegrateManagement.Common.Enumerator;

namespace BizSys.OmniChannelToSAP.Service.Document.PurchaseManagement
{
    public class PurchaseOrder
    {
        public static Result CreatePurchaseOrder(ResultObjects order)
        {
            string B1DocEntry;
            if (B1Common.BOneCommon.IsExistDocument("OPOR", order.DocEntry.ToString(), out B1DocEntry))
            {
                order.B1DocEntry = B1DocEntry;
                return new Result()
                {
                    ResultValue = ResultType.True,
                    ResultMessage = "该订单已生成到B1"
                };
            }
            Result result = new Result();
            SAPbobsCOM.Documents myDocuments = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders);
            SAPbobsCOM.IRecordset rs = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            myDocuments.CardCode = order.BusinessPartnerCode;
            myDocuments.CardName = order.BusinessPartnerName;
            myDocuments.DocDate = order.PostingDate;
            myDocuments.DocDueDate = DateTime.Parse(order.DeliveryDate);
            myDocuments.TaxDate = DateTime.Parse(order.DocumentDate);
            myDocuments.Comments = order.Remarks;
            myDocuments.Reference1 = order.Reference1;
            myDocuments.Reference2 = order.Reference2;
            myDocuments.HandWritten = order.Handwritten == "Yes" ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;
            myDocuments.UserFields.Fields.Item("U_IM_DocEntry").Value = order.DocEntry.ToString();
            myDocuments.UserFields.Fields.Item("U_InvoiceType").Value = ((emBillType)Enum.Parse(typeof(emBillType), order.BillType)).GetHashCode().ToString();
            myDocuments.Printed = order.Printed == "Yes" ? PrintStatusEnum.psYes : PrintStatusEnum.psNo;
            myDocuments.UserFields.Fields.Item("U_ResouceType").Value = "11";

            myDocuments.BPL_IDAssignedToInvoice = BOneCommon.GetBranchCodeByWhsCode(order.PurchaseOrderItems.FirstOrDefault().Warehouse);
            myDocuments.ContactPersonCode = Convert.ToInt32(order.ContactPerson);
          
            foreach (var item in order.PurchaseOrderItems)
            {
                myDocuments.Lines.ItemCode = item.ItemCode;
                myDocuments.Lines.Quantity = double.Parse(item.Quantity);
                var dbRule = BOneCommon.GetDistributionRule(item.ItemCode, order.DataOwner.ToString());
                myDocuments.Lines.CostingCode = dbRule.OcrCode;
                myDocuments.Lines.CostingCode2 = dbRule.OcrCode2;
                myDocuments.Lines.CostingCode3 = dbRule.OcrCode3;

                myDocuments.Lines.CostingCode4 = item.DistributionRule4;
                myDocuments.Lines.CostingCode5 = item.DistributionRule5;

                myDocuments.Lines.UnitPrice = Math.Round(item.UnitPrice,2);
                // myDocuments.Lines.TaxTotal = Math.Round( item.TotalTaxLine,2);
                myDocuments.Lines.PriceAfterVAT = Math.Round(myDocuments.Lines.UnitPrice * (1 + item.TaxRatePerLine));
                myDocuments.Lines.VatGroup = B1Common.BOneCommon.GetTaxByRate(item.TaxRatePerLine,"I");
                myDocuments.Lines.DiscountPercent = Convert.ToDouble(item.DiscountPerLine);
                myDocuments.Lines.WarehouseCode = item.Warehouse;
                myDocuments.Lines.UserFields.Fields.Item("U_IM_DocEntry").Value = item.DocEntry.ToString();
                myDocuments.Lines.UserFields.Fields.Item("U_IM_LineId").Value = item.LineId.ToString();
                myDocuments.Lines.UserFields.Fields.Item("U_ItemName").Value = item.Reference1;
                myDocuments.Lines.Add();
            }

            myDocuments.DocTotal =Math.Round( double.Parse(order.DocumentTotal),2);//总计
            int RntCode = myDocuments.Add();


            if (RntCode != 0)
            {
                result.ResultValue = ResultType.False;
                result.ResultMessage = string.Format("【{0}】采购订单处理失败，ErrorCode:[{1}],ErrrMsg:[{2}];", order.DocEntry, SAP.SAPCompany.GetLastErrorCode(), SAP.SAPCompany.GetLastErrorDescription());
            }
            else
            {
                order.B1DocEntry = SAP.SAPCompany.GetNewObjectKey();
                result.ResultValue = ResultType.True;
                result.ResultMessage = "【" + order.DocEntry.ToString() + "】采购订单处理成功，系统单据：" + SAP.SAPCompany.GetNewObjectKey();
            }
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myDocuments);
            return result;
        }
    }
}
