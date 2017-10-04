using BizSys.IntegrateManagement.Entity.Result;
using BizSys.IntegrateManagement.Entity.StockManagement.GoodsReceipt;
using BizSys.OmniChannelToSAP.Service.B1Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.OmniChannelToSAP.Service.Document.StockManagement
{
    public class GoodsReceipt
    {
        public static Result CreateGoodsReceipt(ResultObjects goodsReceipt)

        {
            string B1DocEntry;

            Result result = new Result();
            SAPbobsCOM.Documents myDocuments;
            //******************仓库类别 == 主仓库 生成库存收货草稿*******************//
            if (B1Common.BOneCommon.IsMainStore(goodsReceipt.GoodsReceiptLines.FirstOrDefault().Warehouse))
            {
                if (B1Common.BOneCommon.IsExistDraft("59", goodsReceipt.DocEntry.ToString(), out B1DocEntry))
                {
                    goodsReceipt.B1DocEntry = B1DocEntry;
                    return new Result()
                    {
                        ResultValue = ResultType.True,
                        ResultMessage = "该订单已生成库存收货草稿到B1"
                    };
                }
                myDocuments = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                myDocuments.DocObjectCode = SAPbobsCOM.BoObjectTypes.oInventoryGenEntry;
            }
            //******************仓库类别 <> 主仓库 生成库存收货单*******************//
            else
            {
                if (B1Common.BOneCommon.IsExistDocument("OIGN", goodsReceipt.DocEntry.ToString(), "12", out B1DocEntry))
                {
                    goodsReceipt.B1DocEntry = B1DocEntry;
                    return new Result()
                    {
                        ResultValue = ResultType.True,
                        ResultMessage = "该订单已生成库存收货到B1"
                    };
                }
                myDocuments = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenEntry);

            }


            #region 主表赋值
            myDocuments.DocDate = goodsReceipt.PostingDate;
            myDocuments.DocDueDate = goodsReceipt.DeliveryDate;
            myDocuments.TaxDate = goodsReceipt.DocumentDate;
            myDocuments.Comments = goodsReceipt.Remarks;
            myDocuments.Reference2 = goodsReceipt.Reference2;
            myDocuments.Reference1 = goodsReceipt.Reference1;
            myDocuments.CardCode = goodsReceipt.BusinessPartnerCode;
            myDocuments.CardName = goodsReceipt.BusinessPartnerName;
            myDocuments.BPL_IDAssignedToInvoice = BOneCommon.GetBranchCodeByWhsCode(goodsReceipt.GoodsReceiptLines.FirstOrDefault().Warehouse);
            //myDocuments.UserFields.Fields.Item("U_InType").Value= goodsReceipt
            myDocuments.UserFields.Fields.Item("U_IM_DocEntry").Value = goodsReceipt.DocEntry.ToString();
            myDocuments.UserFields.Fields.Item("U_ChannalDocType").Value = "12";
            myDocuments.UserFields.Fields.Item("U_InType").Value = goodsReceipt.CustomType;

            #endregion
            #region 子表赋值
            foreach (var item in goodsReceipt.GoodsReceiptLines)
            {
                myDocuments.Lines.ItemCode = item.ItemCode;
                myDocuments.Lines.ItemDescription = item.ItemDescription;
                myDocuments.Lines.WarehouseCode = item.Warehouse;
                myDocuments.Lines.Quantity = item.Quantity;
                
                myDocuments.Lines.AccountCode = B1Common.BOneCommon.GetAccountCodeByInType(goodsReceipt.CustomType);
                var dbRule = BOneCommon.GetDistributionRule(item.ItemCode, goodsReceipt.DataOwner.ToString());
                myDocuments.Lines.CostingCode = dbRule.OcrCode;
                myDocuments.Lines.CostingCode2 = dbRule.OcrCode2;
                myDocuments.Lines.CostingCode3 = dbRule.OcrCode3;
                myDocuments.Lines.CostingCode4 = item.DistributionRule4;
                myDocuments.Lines.CostingCode5 = item.DistributionRule5;
                #region 批次处理
                myDocuments.Lines.BatchNumbers.BatchNumber = DateTime.Now.Date.ToString("yyyyMMdd");
                myDocuments.Lines.BatchNumbers.Quantity = item.Quantity;
                myDocuments.Lines.BatchNumbers.Add();
                myDocuments.Lines.UnitPrice = item.Price;
                #endregion
                myDocuments.Lines.Add();
            }
            #endregion
            int RntCode = myDocuments.Add();
            if (RntCode != 0)
            {
                result.ResultValue = ResultType.False;
                result.ResultMessage = string.Format("【{0}】库存收货单处理失败，ErrorCode:[{1}],ErrrMsg:[{2}];", goodsReceipt.DocEntry, SAP.SAPCompany.GetLastErrorCode(), SAP.SAPCompany.GetLastErrorDescription());
            }
            else
            {
                goodsReceipt.B1DocEntry = SAP.SAPCompany.GetNewObjectKey();
                result.ResultValue = ResultType.True;
                result.ResultMessage = "【" + goodsReceipt.DocEntry.ToString() + "】库存发货单处理成功，系统单据：" + SAP.SAPCompany.GetNewObjectKey();
            }
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myDocuments);
            return result;
        }
    }
}
