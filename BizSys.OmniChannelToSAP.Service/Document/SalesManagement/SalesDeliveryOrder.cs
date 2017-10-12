using BizSys.IntegrateManagement.Entity.BatchNumber;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.IntegrateManagement.Entity.SalesManagement.SalesDeliveryOrder;
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
    public class SalesDeliveryOrder
    {
        #region 前奏：销售订单
        /// <summary>
        /// 来源于Anywhere的交货单  先生成销售订单，基于销售订单生成销售交货单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static Result CreateSalesOrder(ResultObjects order)
        {
            string B1DocEntry;
            string B1DlftWhsCode = "01";
            string B1DlftSaleCostCode = string.Empty;
            string B1DlftFIAccount = string.Empty;
            if (B1Common.BOneCommon.IsExistDocument("ORDR", order.DocEntry.ToString(), out B1DocEntry))
            {
                order.B1DocEntry = B1DocEntry;
                return new Result()
                {
                    ResultValue = ResultType.True,
                    ResultMessage = "该订单已生成到B1"
                };
            }
            Result result = new Result();
            SAPbobsCOM.Documents myDocuments = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);

            myDocuments.Series = BOneCommon.GetB1DocEntrySeries("15");
            //myDocuments.HandWritten = order.Handwritten == "Yes" ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;

            //myDocuments.Reference1 = order.Reference1;
            //myDocuments.Reference2 = order.Reference2;
            myDocuments.DocDate = order.PostingDate;
            myDocuments.TaxDate = order.DocumentDate;
            myDocuments.DocDueDate = order.DeliveryDate;
            myDocuments.Comments = order.Remarks;
            myDocuments.CardCode = order.BusinessPartnerCode;
            myDocuments.CardName = order.BusinessPartnerName;
            myDocuments.ContactPersonCode = BOneCommon.GetBPContractCode(order.BusinessPartnerCode, order.ContactPerson);
            //收货地址
            string[] textArray1 = new string[] { "CN", order.Province, order.City, order.County, order.Town, order.ShipTo };
            myDocuments.Address2 = string.Concat(textArray1);
            //myDocuments.BPL_IDAssignedToInvoice = BOneCommon.GetBranchCodeByWhsCode(order.SalesDeliveryItems.FirstOrDefault().Warehouse);
            //if (string.IsNullOrEmpty(order.BillType))
            //    return new Result() { ResultValue = ResultType.False, ResultMessage = "发票类型为空。" };
            //myDocuments.UserFields.Fields.Item("U_InvoiceType").Value = ((emBillType)Enum.Parse(typeof(emBillType), order.BillType)).GetHashCode().ToString();
            //myDocuments.UserFields.Fields.Item("U_IM_DocEntry").Value = order.DocEntry.ToString();
            //myDocuments.UserFields.Fields.Item("U_ResouceType").Value = order.DataSource;
            //myDocuments.UserFields.Fields.Item("U_DeliveryAddrerss").Value = order.Status + '-' + order.County + '-' + order.City;
            myDocuments.DiscountPercent = Convert.ToDouble(order.ProductDiscountRate);
            foreach (var item in order.SalesDeliveryItems)
            {
                myDocuments.Lines.ItemCode = item.ItemCode;
                myDocuments.Lines.Quantity = Convert.ToDouble(item.Quantity);
                myDocuments.Lines.UnitPrice = item.UnitPrice;
                myDocuments.Lines.DiscountPercent = Convert.ToDouble(item.DiscountPerLine);
                myDocuments.Lines.WarehouseCode = B1Common.BOneCommon.IsExistWarehouse(item.Warehouse) == true ? item.Warehouse : B1DlftWhsCode;
                var dbRule = BOneCommon.GetDistributionRule(item.ItemCode, order.DataOwner.ToString());
                myDocuments.Lines.CostingCode = dbRule.OcrCode;
                myDocuments.Lines.CostingCode2 = dbRule.OcrCode2;
                myDocuments.Lines.CostingCode3 = dbRule.OcrCode3;
                //myDocuments.Lines.UserFields.Fields.Item("U_IM_DocEntry").Value = item.DocEntry.ToString();
                //myDocuments.Lines.UserFields.Fields.Item("U_IM_LineId").Value = item.LineId.ToString();
                myDocuments.Lines.Add();
            }
            myDocuments.DocTotal = order.DocumentTotal;
            int RntCode = myDocuments.Add();

            if (RntCode != 0)
            {
                result.ResultValue = ResultType.False;
                result.ResultMessage = string.Format("【{0}】销售交货订单处理失败，ErrorCode:[{1}],ErrrMsg:[{2}];", order.DocEntry, SAP.SAPCompany.GetLastErrorCode(), SAP.SAPCompany.GetLastErrorDescription());
            }
            else
            {
                order.B1DocEntry = SAP.SAPCompany.GetNewObjectKey();
                result.ResultValue = ResultType.True;
                result.ResultMessage = "【" + order.DocEntry.ToString() + "】销售交货订单处理成功，系统单据：" + SAP.SAPCompany.GetNewObjectKey();
            }
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myDocuments);

            return result;
        }
        #endregion

        #region 交货单 本体
        /// <summary>
        /// 生成交货单
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static Result CreateSalesDeliveryOrder(ResultObjects order)
        {
            string B1DocEntry;
            string B1DlftWhsCode = "01";
            string B1DlftSaleCostCode = string.Empty;
            string B1DlftFIAccount = string.Empty;
            /*
            if (B1Common.BOneCommon.IsExistDocument("ODLN", order.DocEntry.ToString(), out B1DocEntry))
            {
                order.B1DocEntry = B1DocEntry;
                return new Result()
                {
                    ResultValue = ResultType.True,
                    ResultMessage = "该订单已生成到B1"
                };
            }
            */
            Result result = new Result();

            //没有物料，就加物料
            foreach (var item in order.SalesDeliveryItems)
            {
                SAPbobsCOM.Items b1Material = SAP.SAPCompany.GetBusinessObject(BoObjectTypes.oItems);
                if (!b1Material.GetByKey(item.ItemCode))
                {
                    //go to sysnc material
                    BizSys.OmniChannelToSAP.Service.Service.MasterDataManagementService.GetMatarialService.GetMaterial(item.ItemCode);
                }
            }

            SAPbobsCOM.Documents myDocuments = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDeliveryNotes);

            myDocuments.Series = B1Common.BOneCommon.GetB1DocEntrySeries("15");
            //myDocuments.HandWritten = order.Handwritten == "Yes" ? BoYesNoEnum.tYES : BoYesNoEnum.tNO;

            //myDocuments.Reference1 = order.Reference1;
            //myDocuments.Reference2 = order.Reference2;
            myDocuments.DocDate = Convert.ToDateTime(order.PostingDate);
            myDocuments.TaxDate = Convert.ToDateTime(order.DocumentDate);
            myDocuments.DocDueDate = Convert.ToDateTime(order.DeliveryDate);
            myDocuments.Comments = order.Remarks;
            myDocuments.CardCode = order.BusinessPartnerCode;
            myDocuments.CardName = order.BusinessPartnerName;
            myDocuments.ContactPersonCode = B1Common.BOneCommon.GetBPContractCode(order.BusinessPartnerCode, order.ContactPerson);
            //收货地址
            string[] textArray1 = new string[] { "CN", order.Province, order.City, order.County, order.Town, order.DetailedAddress };
            myDocuments.Address2 = string.Concat(textArray1);
            //myDocuments.BPL_IDAssignedToInvoice = BOneCommon.GetBranchCodeByWhsCode(order.SalesDeliveryItems.FirstOrDefault().Warehouse);
            //if (string.IsNullOrEmpty(order.BillType))
            //    return new Result() { ResultValue = ResultType.False, ResultMessage = "发票类型为空。" };
            //myDocuments.UserFields.Fields.Item("U_InvoiceType").Value = ((emBillType)Enum.Parse(typeof(emBillType), order.BillType)).GetHashCode().ToString();
            //myDocuments.UserFields.Fields.Item("U_IM_DocEntry").Value = order.DocEntry.ToString();
            //myDocuments.UserFields.Fields.Item("U_ResouceType").Value = order.DataSource;
            //myDocuments.UserFields.Fields.Item("U_DeliveryAddrerss").Value = order.Status + '-' + order.County + '-' + order.City;

            myDocuments.DiscountPercent = double.Parse(order.DiscountForDocument);
            foreach (var item in order.SalesDeliveryItems)
            {
                //SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(BoObjectTypes.BoRecordset);
                //string sql = $@"select DocEntry,LineNum from RDR1 where U_IM_DocEntry = '{item.DocEntry}' and U_IM_LineId = '{item.LineId}'";
                //res.DoQuery(sql);
                //if (res.RecordCount != 1)
                //    throw new Exception("基于Anywhere销售订单生成交货单无法确定唯一的销售订单行，请检查");
                myDocuments.Lines.ItemCode = item.ItemCode;
                myDocuments.Lines.WarehouseCode = B1Common.BOneCommon.IsExistWarehouse(item.Warehouse) == true ? item.Warehouse : B1DlftWhsCode;
                myDocuments.Lines.Quantity = Convert.ToDouble(item.Quantity);
                myDocuments.Lines.DiscountPercent = item.DiscountPerLine;
                myDocuments.Lines.PriceAfterVAT = item.GrossPrice;
                myDocuments.Lines.VatGroup = B1Common.BOneCommon.GetTaxByRate(item.TaxRatePerLine, "O");
                //myDocuments.Lines.UnitPrice = item.UnitPrice;
                //myDocuments.Lines.COGSAccountCode 销货成本科目
                //myDocuments.Lines.COGSCostingCode 成本分配
                //myDocuments.Lines.AccountCode  总账科目
                myDocuments.Lines.COGSCostingCode = "D012";
                myDocuments.Lines.UserFields.Fields.Item("U_JHLX").Value = "正常";

                //myDocuments.Lines.BaseEntry = res.Fields.Item("DocEntry").Value;
                //myDocuments.Lines.BaseLine = res.Fields.Item("LineNum").Value;
                myDocuments.Lines.Add();
            }
            myDocuments.DocTotal = Math.Round(order.DocumentTotal, 2);
            int RntCode = myDocuments.Add();

            if (RntCode != 0)
            {
                result.ResultValue = ResultType.False;
                result.ResultMessage = string.Format("【{0}】销售交货订单处理失败，ErrorCode:[{1}],ErrrMsg:[{2}];", order.DocEntry, SAP.SAPCompany.GetLastErrorCode(), SAP.SAPCompany.GetLastErrorDescription());
            }
            else
            {
                B1DocEntry = SAP.SAPCompany.GetNewObjectKey();
                result.ResultValue = ResultType.True;
                result.DocEntry = B1DocEntry;
                result.ResultMessage = "【" + order.DocEntry.ToString() + "】销售交货订单处理成功，系统单据：" + SAP.SAPCompany.GetNewObjectKey();
            }
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myDocuments);

            return result;
        }

        #endregion
    }
}
