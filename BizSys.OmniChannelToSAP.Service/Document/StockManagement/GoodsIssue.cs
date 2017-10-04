using BizSys.IntegrateManagement.Entity.Result;
using BizSys.IntegrateManagement.Entity.StockManagement.GoodsIssue;
using BizSys.OmniChannelToSAP.Service.B1Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.OmniChannelToSAP.Service.Document.StockManagement
{
    public class GoodsIssue
    {
        public static Result CreateGoodsIssue(ResultObjects goodsIssue)
        {
            string B1DocEntry;
            
            Result result = new Result();
            SAPbobsCOM.Documents myDocuments;
            //******************仓库类别 == 主仓库 生成库存发货草稿*******************//
            if (B1Common.BOneCommon.IsMainStore(goodsIssue.GoodsIssueLines.FirstOrDefault().Warehouse))
            {
                if (B1Common.BOneCommon.IsExistDraft("60", goodsIssue.DocEntry.ToString(), out B1DocEntry))
                {
                    goodsIssue.B1DocEntry = B1DocEntry;
                    return new Result()
                    {
                        ResultValue = ResultType.True,
                        ResultMessage = "该订单已生成到B1"
                    };
                }
                myDocuments = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
                myDocuments.DocObjectCode = SAPbobsCOM.BoObjectTypes.oInventoryGenExit;
            }
            //******************仓库类别 <> 主仓库  生成库存发货单*******************//
            else
            {
                if (B1Common.BOneCommon.IsExistDocument("OIGE", goodsIssue.DocEntry.ToString(),"11", out B1DocEntry))
                {
                    goodsIssue.B1DocEntry = B1DocEntry;
                    return new Result()
                    {
                        ResultValue = ResultType.True,
                        ResultMessage = "该订单已生成到B1"
                    };
                }
                myDocuments = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenExit);
            }

            #region 主表赋值
            myDocuments.DocDate = goodsIssue.PostingDate;
            myDocuments.DocDueDate = goodsIssue.DeliveryDate;
            myDocuments.TaxDate = goodsIssue.DocumentDate;
            myDocuments.Comments = goodsIssue.Remarks;
           // myDocuments.DocumentsOwner = B1Common.BOneCommon.GetBOneOwnerCode(goodsIssue.DataOwner);
            myDocuments.Reference1 = goodsIssue.Reference1;
            myDocuments.Reference2 = goodsIssue.Reference2;
            myDocuments.CardCode = goodsIssue.BusinessPartnerCode;
            myDocuments.CardName = goodsIssue.BusinessPartnerName;
            myDocuments.BPL_IDAssignedToInvoice = B1Common.BOneCommon.GetBranchCodeByWhsCode(goodsIssue.GoodsIssueLines.FirstOrDefault().Warehouse);
            myDocuments.UserFields.Fields.Item("U_IM_DocEntry").Value = goodsIssue.DocEntry.ToString();
            myDocuments.UserFields.Fields.Item("U_ChannalDocType").Value = "11";
            myDocuments.UserFields.Fields.Item("U_OutType").Value = goodsIssue.CustomType;

            #endregion

            #region 子表赋值
            foreach (var item in goodsIssue.GoodsIssueLines)
            {
                myDocuments.Lines.ItemCode = item.ItemCode;
                myDocuments.Lines.ItemDescription = item.ItemDescription;
                myDocuments.Lines.WarehouseCode = item.Warehouse;
                myDocuments.Lines.Quantity = item.Quantity;
                myDocuments.Lines.UnitPrice = item.Price;
                myDocuments.Lines.AccountCode = B1Common.BOneCommon.GetAccountCodeByOutType(goodsIssue.CustomType);
                var dbRule= BOneCommon.GetDistributionRule(item.ItemCode, goodsIssue.DataOwner.ToString());

                myDocuments.Lines.CostingCode = dbRule.OcrCode;
                myDocuments.Lines.CostingCode2 = dbRule.OcrCode2;
                myDocuments.Lines.CostingCode3 = dbRule.OcrCode3;
                myDocuments.Lines.CostingCode4 = item.DistributionRule4;
                myDocuments.Lines.CostingCode5 = item.DistributionRule5;
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
                myDocuments.Lines.Add();
             
            }
            
            #endregion


            int RntCode = myDocuments.Add();
            if (RntCode != 0)
            {
                result.ResultValue = ResultType.False;
                result.ResultMessage = string.Format("【{0}】库存发货处理失败，ErrorCode:[{1}],ErrrMsg:[{2}];", goodsIssue.DocEntry, SAP.SAPCompany.GetLastErrorCode(), SAP.SAPCompany.GetLastErrorDescription());
            }
            else
            {
                goodsIssue.B1DocEntry = SAP.SAPCompany.GetNewObjectKey();
                result.ResultValue = ResultType.True;
                result.ResultMessage = "【" + goodsIssue.DocEntry.ToString() + "】库存发货处理成功，系统单据：" + SAP.SAPCompany.GetNewObjectKey();
            }
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myDocuments);
            return result;
        }



    }
}

