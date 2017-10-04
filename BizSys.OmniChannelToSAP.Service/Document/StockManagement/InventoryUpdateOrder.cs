using BizSys.IntegrateManagement.Entity.Result;
using System;
using BizSys.IntegrateManagement.Entity.StockManagement.InventoryUpdate;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizSys.OmniChannelToSAP.Service.B1Common;

namespace BizSys.OmniChannelToSAP.Service.Document.StockManagement
{
    public class InventoryUpdateOrder
    {
        //***********************************************************************************//
        //库存过账生成 出库单 入库单
        //
        //***********************************************************************************//


        public static Result CreateGoodsOrder(ResultObjects order)
        {
            Result result = new Result();
            var orderDefaultItem = order.InventoryUpdateLines.FirstOrDefault();
            SAPbobsCOM.Documents myDocumentGenExit = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenExit);//出库单
            SAPbobsCOM.Documents myDocumentGenEntry = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenEntry);//入库单


            #region 出库主表赋值
            myDocumentGenExit.DocDate = order.PostingDate;
            myDocumentGenExit.DocDueDate = order.DeliveryDate;
            myDocumentGenExit.TaxDate = order.DocumentDate;
            myDocumentGenExit.Comments = order.Remarks;
            myDocumentGenExit.Reference1 = order.Reference1;
            myDocumentGenExit.Reference2 = order.Reference2;
            myDocumentGenExit.BPL_IDAssignedToInvoice = B1Common.BOneCommon.GetBranchCodeByWhsCode(order.InventoryUpdateLines.Where(c => c.Quantity > 0).FirstOrDefault().WarehouseCode);
            myDocumentGenExit.UserFields.Fields.Item("U_IM_DocEntry").Value = order.DocEntry.ToString();
            myDocumentGenExit.UserFields.Fields.Item("U_ChannalDocType").Value = "13";
            myDocumentGenExit.UserFields.Fields.Item("U_OutType").Value = "C07";
            //myDocumentGenExit.SalesPersonCode = B1Common.BOneCommon.GetBOneSlpCode(order.CreateUserSign);
            #endregion
            #region 入库主表赋值
            myDocumentGenEntry.DocDate = order.PostingDate;
            myDocumentGenEntry.DocDueDate = order.DeliveryDate;
            myDocumentGenEntry.TaxDate = order.DocumentDate;
            myDocumentGenEntry.Comments = order.Remarks;
            myDocumentGenEntry.Reference1 = order.Reference1;
            myDocumentGenEntry.Reference2 = order.Reference2;
            myDocumentGenEntry.UserFields.Fields.Item("U_IM_DocEntry").Value = order.DocEntry.ToString();
            myDocumentGenEntry.UserFields.Fields.Item("U_ChannalDocType").Value = "13";
            myDocumentGenEntry.UserFields.Fields.Item("U_InType").Value = "R02";
            myDocumentGenEntry.BPL_IDAssignedToInvoice = B1Common.BOneCommon.GetBranchCodeByWhsCode(order.InventoryUpdateLines.Where(c => c.Quantity < 0).FirstOrDefault().WarehouseCode);

            #endregion
            SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            string sql = @"SELECT T0.[AvgPrice] as CostPrice FROM OITW T0 WHERE T0.[ItemCode] = '{0}' and t0.WhsCode = '{1}'";
            foreach (var item in order.InventoryUpdateLines)
            {
               
                if (item.Quantity < 0)
                {
                    myDocumentGenExit.Lines.ItemCode = item.ItemCode;
                    myDocumentGenExit.Lines.ItemDescription = item.ItemDescription;
                    myDocumentGenExit.Lines.WarehouseCode = item.WarehouseCode;
                    myDocumentGenExit.Lines.Quantity = -item.Quantity;
                    myDocumentGenExit.Lines.BarCode = item.BarCode;
                    myDocumentGenExit.Lines.CostingCode2 = order.Taker1Id;

                    myDocumentGenExit.Lines.Add();
                }
                else if (item.Quantity > 0)
                {
                    res.DoQuery(string.Format(sql, item.ItemCode, item.WarehouseCode));
                    if (res.RecordCount == 1)
                        myDocumentGenEntry.Lines.UnitPrice = res.Fields.Item("CostPrice").Value;
                    myDocumentGenEntry.Lines.ItemCode = item.ItemCode;
                    myDocumentGenEntry.Lines.ItemDescription = item.ItemDescription;
                    myDocumentGenEntry.Lines.WarehouseCode = item.WarehouseCode;
                    myDocumentGenEntry.Lines.Quantity = item.Quantity;
                    myDocumentGenEntry.Lines.BarCode = item.BarCode;
                    myDocumentGenEntry.Lines.CostingCode2 = order.Taker1Id;
                    myDocumentGenEntry.Lines.Add();
                }
            }
            string B1DocEntry;
            if (B1Common.BOneCommon.IsExistDocument("OIGE", order.DocEntry.ToString(),"13", out B1DocEntry))
            {
                result.ResultValue = ResultType.True;
                result.ResultMessage += "该订单已生成库存发货到B1;";
                
            }
            else
            {
                int RntCodeGenExit = myDocumentGenExit.Add();

                if (RntCodeGenExit != 0)
                {
                    result.ResultValue = ResultType.False;
                    result.ResultMessage += string.Format("【{0}】库存过账生成出库单失败，ErrorCode:[{1}],ErrrMsg:[{2}];", order.DocEntry, SAP.SAPCompany.GetLastErrorCode(), SAP.SAPCompany.GetLastErrorDescription());
                }
                else
                {
                    result.ResultValue = ResultType.True;
                    result.ResultMessage += "【" + order.DocEntry.ToString() + "】库存过账生成出库单成功，系统单据：" + SAP.SAPCompany.GetNewObjectKey();

                }
            }
            
            
            if (B1Common.BOneCommon.IsExistDocument("OIGN", order.DocEntry.ToString(), "13", out B1DocEntry))
            {

                result.ResultValue = ResultType.True;
                result.ResultMessage += "该过账订单已生成库存收货到B1;";
                
            }
            else
            {
                int RntCodeGenEntry = myDocumentGenEntry.Add();
                if (RntCodeGenEntry != 0)
                {
                    result.ResultValue = ResultType.False;
                    result.ResultMessage += string.Format("【{0}】库存过账生成入库单失败，ErrorCode:[{1}],ErrrMsg:[{2}];", order.DocEntry, SAP.SAPCompany.GetLastErrorCode(), SAP.SAPCompany.GetLastErrorDescription());
                }
                else
                {
                    result.ResultValue = ResultType.True;
                    result.ResultMessage += "【" + order.DocEntry.ToString() + "】库存过账生成入库单成功，系统单据：" + SAP.SAPCompany.GetNewObjectKey();
                }
            }
            


            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myDocumentGenEntry);
            return result;
        }
    }
}
