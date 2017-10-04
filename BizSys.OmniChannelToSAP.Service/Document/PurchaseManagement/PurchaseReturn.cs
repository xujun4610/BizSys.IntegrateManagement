using BizSys.IntegrateManagement.Entity.PurchaseManagement.PurchaseReturn;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.OmniChannelToSAP.Service.B1Common;
using SAPbobsCOM;
using System;
using System.Linq;

namespace BizSys.OmniChannelToSAP.Service.Document.PurchaseManagement
{
    public class PurchaseReturn
    {
        /// <summary>
        /// 生成采购退货草稿/应付贷项凭证草稿
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static Result CreateDraftOrder(ResultObjects order)
        {
            string B1DocEntry;

            Result result = new Result();
            SAPbobsCOM.Documents documents = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
            if(string.IsNullOrEmpty(order.ReturnType))
                return new Result(){
                    ResultValue=ResultType.False,
                    ResultMessage="退货类型为空，无法判定目标单据."
                };
            if (order.ReturnType == "11")
            {
                if(B1Common.BOneCommon.IsExistDraft("21", order.DocEntry.ToString(), out B1DocEntry))
                {
                    order.B1DocEntry = B1DocEntry;
                    return new Result()
                    {
                        ResultValue = ResultType.True,
                        ResultMessage = "该订单已生成到B1"
                    };
                }
                else
                    //****************************直接退货类型 生成采购退货草稿***************************************************/
                    documents.DocObjectCode = SAPbobsCOM.BoObjectTypes.oPurchaseReturns;
            }
            else
            {
                if (B1Common.BOneCommon.IsExistDraft("19", order.DocEntry.ToString(), out B1DocEntry))
                {
                    order.B1DocEntry = B1DocEntry;
                    return new Result()
                    {
                        ResultValue = ResultType.True,
                        ResultMessage = "该订单已生成到B1"
                    };
                }
                else
                    //****************************退货退票/等价换货/不等价换货 生成应付贷项凭证草稿************************************/
                    documents.DocObjectCode = SAPbobsCOM.BoObjectTypes.oPurchaseCreditNotes;
            }

            #region 表头赋值
            documents.CardCode = order.BusinessPartnerCode;
            documents.CardName = order.BusinessPartnerName;
            documents.UserFields.Fields.Item("U_ReturnType").Value = order.ReturnType;
            documents.DocDate = order.PostingDate;//过账日期
            documents.DocDueDate = order.DeliveryDate;//交货日期
            documents.TaxDate = order.DocumentDate;//单据日期
            documents.Comments = order.Remarks;
            documents.ContactPersonCode = order.ContactPerson;
            documents.Printed = order.Printed == "Yes" ? PrintStatusEnum.psYes : PrintStatusEnum.psNo;
            documents.BPL_IDAssignedToInvoice = BOneCommon.GetBranchCodeByWhsCode(order.PurchaseReturnItems.FirstOrDefault().Warehouse);
            
            documents.UserFields.Fields.Item("U_IM_DocEntry").Value = order.DocEntry.ToString();
            #endregion
            foreach (var item in order.PurchaseReturnItems)
            {
                #region 子表赋值
                documents.Lines.ItemCode = item.ItemCode;
                documents.Lines.Quantity = double.Parse(item.Quantity);
               var dbRule= BOneCommon.GetDistributionRule(item.ItemCode, order.DataOwner.ToString());

                documents.Lines.CostingCode = dbRule.OcrCode;
                documents.Lines.CostingCode2 = dbRule.OcrCode2;
                documents.Lines.CostingCode3 = dbRule.OcrCode3;

                documents.Lines.CostingCode4 = item.DistributionRule4;
                documents.Lines.CostingCode5 = item.DistributionRule5;
                documents.Lines.BaseType = 20;
                documents.Lines.BaseEntry = Convert.ToInt32( item.BaseDocumentEntry);
                documents.Lines.BaseLine = Convert.ToInt32(item.BaseDocumentLineId);
                documents.Lines.UnitPrice = double.Parse(item.UnitPrice);
                documents.Lines.DiscountPercent = Convert.ToDouble(item.DiscountPerLine);
                documents.Lines.VatGroup = B1Common.BOneCommon.GetTaxByRate(item.TaxRatePerLine,"I");
                documents.Lines.WarehouseCode = item.Warehouse;
                documents.Lines.UserFields.Fields.Item("U_IM_DocEntry").Value = item.DocEntry;
                documents.Lines.UserFields.Fields.Item("U_IM_LineId").Value = item.LineId;
                documents.Lines.Add();
                #endregion
            }
            documents.DocTotal = order.DocumentTotal;
            int RntCode = documents.Add();
           if (RntCode != 0)
           {
               result.ResultValue = ResultType.False;
               result.ResultMessage = string.Format("【{0}】采购退货单处理失败，ErrorCode:[{1}],ErrrMsg:[{2}];", order.DocEntry, SAP.SAPCompany.GetLastErrorCode(), SAP.SAPCompany.GetLastErrorDescription());
           }
           else
           {
               //order.SAPDocEntry = SAP.SAPCompany.GetNewObjectKey();
               result.ResultValue = ResultType.True;
               result.ResultMessage = "【" + order.DocEntry.ToString() + "】采购退货单处理成功，系统单据：" + SAP.SAPCompany.GetNewObjectKey();
           }
           System.Runtime.InteropServices.Marshal.FinalReleaseComObject(documents);
           return result;

        }

       
    }
}
