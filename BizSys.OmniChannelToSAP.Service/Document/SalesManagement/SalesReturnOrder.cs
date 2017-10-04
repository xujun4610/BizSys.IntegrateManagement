using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.IntegrateManagement.Entity.SalesManagement.SalesReturnOrder;
using BizSys.OmniChannelToSAP.Service.B1Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.OmniChannelToSAP.Service.Document.SalesManagement
{
    public class SalesReturnOrder
    {
        public static Result CreateSalesReturnOrderOrDraft(ResultObjects order)
        {
            string B1DocEntry;
            Result result = new Result();
            bool isReturnOrder = false;
            bool isCredits = false;
            SAPbobsCOM.Documents myDocuments;
            string whsCode = order.SalesReturnItems.FirstOrDefault().Warehouse;
            if (order.ReturnType == "11" && B1Common.BOneCommon.IsMainStore(whsCode))
            {
                //创建销售退货草稿
                if (BOneCommon.IsExistDraft("16", order.DocEntry.ToString(), out B1DocEntry))
                {
                    order.B1DocEntry = B1DocEntry;
                    return new Result()
                    {
                        ResultValue = ResultType.True,
                        ResultMessage = "该订单已创建销售退货草稿到B1"
                    };
                }
                myDocuments = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                myDocuments.DocObjectCode = SAPbobsCOM.BoObjectTypes.oReturns;
            }
            else if (order.ReturnType == "11" && !B1Common.BOneCommon.IsMainStore(whsCode))
            {
                //创建销售退货单
                isReturnOrder = true;
                if (BOneCommon.IsExistDocument("ORDN", order.DocEntry.ToString(), out B1DocEntry))
                {
                    order.B1DocEntry = B1DocEntry;
                    return new Result()
                    {
                        ResultValue = ResultType.True,
                        ResultMessage = "该订单已创建销售退货单到B1"
                    };
                }
                myDocuments = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oReturns);

            }
            else if (order.ReturnType != "11" && B1Common.BOneCommon.IsMainStore(whsCode))
            {
                //创建应收贷项凭证草稿
                if (BOneCommon.IsExistDraft("14", order.DocEntry.ToString(), out B1DocEntry))
                {
                    order.B1DocEntry = B1DocEntry;
                    return new Result()
                    {
                        ResultValue = ResultType.True,
                        ResultMessage = "该订单已创建销售退货草稿到B1"
                    };
                }
                myDocuments = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                myDocuments.DocObjectCode = SAPbobsCOM.BoObjectTypes.oCreditNotes;
            }
            else if (order.ReturnType != "11" && !B1Common.BOneCommon.IsMainStore(whsCode))
            {
                //创建应收贷项凭证
                if (BOneCommon.IsExistDocument("ORIN", order.DocEntry.ToString(), out B1DocEntry))
                {
                    order.B1DocEntry = B1DocEntry;
                    return new Result()
                    {
                        ResultValue = ResultType.True,
                        ResultMessage = "该订单已创建应收贷项凭证到B1"
                    };
                }
                isCredits = true;
                myDocuments = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oCreditNotes);

            }
            else
                return new Result() { ResultValue = ResultType.False, ResultMessage = "根据提供的退货类型和仓库无法确认目标单据类型." };


            #region 主表赋值
            ResultObjects salesOrderDelivery = new ResultObjects();
            myDocuments.CardCode = order.BusinessPartnerCode;
            //myDocuments.CardCode = "C0000001";
            myDocuments.CardName = order.BusinessPartnerName;
            myDocuments.DocDate = order.PostingDate;
            myDocuments.DocDueDate = order.DeliveryDate;
            myDocuments.TaxDate = order.DocumentDate;
            myDocuments.Comments = order.Remarks;
            myDocuments.Reference1 = order.Reference1;
            myDocuments.Reference2 = order.Reference2;
            myDocuments.Comments = order.Remarks;
            myDocuments.UserFields.Fields.Item("U_ReturnType").Value = order.ReturnType;
            myDocuments.UserFields.Fields.Item("U_BackReason").Value = order.ReturnReason;
            myDocuments.BPL_IDAssignedToInvoice = BOneCommon.GetBranchCodeByWhsCode(order.SalesReturnItems.FirstOrDefault().Warehouse);
            myDocuments.UserFields.Fields.Item("U_IM_DocEntry").Value = order.DocEntry.ToString();
            myDocuments.UserFields.Fields.Item("U_Linkman").Value = DataConvert.GetValue(order.Consignee);//联系人 收货人
            myDocuments.UserFields.Fields.Item("U_Telephone").Value = order.ContactNumber;
            myDocuments.UserFields.Fields.Item("U_LotCompany").Value = order.LogisticsCompany;
            myDocuments.UserFields.Fields.Item("U_LotNum").Value = order.LogisticsNumber;
            myDocuments.UserFields.Fields.Item("U_DeliveryAddress").Value = order.Province + '-' + order.City + '-' + order.County + '-' + order.DetailedAddress;

            foreach (var item in order.UserFields)
            {
                if (!string.IsNullOrEmpty(item.Name) && item.Name == "U_ServiceNo")
                {
                    if(!string.IsNullOrEmpty(item.Value))
                    myDocuments.UserFields.Fields.Item("U_ServiceEntry").Value = item.Value;
                    break;
                }
            }
            #endregion

            foreach (var item in order.SalesReturnItems)
            {
                #region 子表
                myDocuments.Lines.ItemCode = item.ItemCode;
                myDocuments.Lines.ItemDescription = item.ItemDescription;
                myDocuments.Lines.Quantity = item.Quantity;
                myDocuments.Lines.UnitPrice = item.UnitPrice;
                myDocuments.Lines.DiscountPercent = Convert.ToDouble(item.DiscountPerLine);
                myDocuments.Lines.VatGroup = B1Common.BOneCommon.GetTaxByRate(item.TaxRatePerLine, "O");
                myDocuments.Lines.WarehouseCode = item.Warehouse;
                var dbRule = BOneCommon.GetDistributionRule(item.ItemCode, order.DataOwner.ToString());
                myDocuments.Lines.BaseType = 15;
                myDocuments.Lines.BaseEntry = Convert.ToInt32(item.BaseDocumentEntry);
                myDocuments.Lines.BaseLine = Convert.ToInt32(item.BaseDocumentLineId);

                myDocuments.Lines.CostingCode = dbRule.OcrCode;
                myDocuments.Lines.CostingCode2 = dbRule.OcrCode2;
                myDocuments.Lines.CostingCode3 = dbRule.OcrCode3;

                myDocuments.Lines.CostingCode4 = item.DistributionRule4;
                myDocuments.Lines.CostingCode5 = item.DistributionRule5;
                myDocuments.Lines.UserFields.Fields.Item("U_IM_DocEntry").Value = item.DocEntry;
                myDocuments.Lines.UserFields.Fields.Item("U_IM_LineId").Value = item.LineId;
                if (isReturnOrder)
                {
                    myDocuments.Lines.BatchNumbers.BatchNumber = DateTime.Now.Date.ToString("yyyyMMdd");
                    myDocuments.Lines.BatchNumbers.Quantity = item.Quantity;
                    myDocuments.Lines.BatchNumbers.Add();
                }
                if (isCredits)
                {
                    #region 批次处理
                    //获取该物料该仓库下的所有批次信息 按创建时间排序
                    var ListBatchNumber = B1Common.BOneCommon.GetBatchByItemAndWhsCode(item.ItemCode, item.Warehouse);
                    double batchQuantitySum = 0;//0行~n行的批次总数量
                    double hasDistributedQuantitySum = 0;//已分配的批次数量
                    foreach (var batch in ListBatchNumber)
                    {
                        myDocuments.Lines.BatchNumbers.BatchNumber = batch.BatchID;
                        batchQuantitySum += batch.Quantity;
                        myDocuments.Lines.BatchNumbers.Quantity = (batchQuantitySum - hasDistributedQuantitySum) >= (item.Quantity - hasDistributedQuantitySum) ?
                            (item.Quantity - hasDistributedQuantitySum) : (batchQuantitySum - hasDistributedQuantitySum);//该行分配的批次数量为 =当前行批次的总数量-已分配好的数量
                        hasDistributedQuantitySum += myDocuments.Lines.BatchNumbers.Quantity;
                        myDocuments.Lines.BatchNumbers.Add();

                        //该物料的数量<= 当前行的批次总数量，说明批次数量已够分配并已成功分配，跳出循环。
                        if (item.Quantity <= batchQuantitySum)
                            break;

                    }
                    #endregion
                }
                myDocuments.Lines.Add();
                #endregion
            }

            int RntCode = myDocuments.Add();
            if (RntCode != 0)
            {
                result.ResultValue = ResultType.False;
                result.ResultMessage = string.Format("【{0}】销售退货单处理失败，ErrorCode:[{1}],ErrrMsg:[{2}];", order.DocEntry, SAP.SAPCompany.GetLastErrorCode(), SAP.SAPCompany.GetLastErrorDescription());
            }
            else
            {
                order.B1DocEntry = SAP.SAPCompany.GetNewObjectKey();
                result.ResultValue = ResultType.True;
                result.ResultMessage = "【" + order.DocEntry.ToString() + "】销售退货单处理成功，系统单据：" + SAP.SAPCompany.GetNewObjectKey();
            }
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myDocuments);
            return result;
        }
    }
}
