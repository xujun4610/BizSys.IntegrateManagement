using BizSys.IntegrateManagement.Entity.StockManagement.InventoryCounting;
using BizSys.IntegrateManagement.Entity.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizSys.OmniChannelToSAP.Service.B1Common;

namespace BizSys.OmniChannelToSAP.Service.Document.StockManagement
{
    public class InventoryCounteOrder
    {
        //***********************************************************************************//
        //库存盘点生成 出库单 入库单
        //
        //***********************************************************************************//


        public static Result CreateGoodsOrder(ResultObjects order)
        {
            Result result = new Result();
            var orderDefaultItem = order.InventoryCountingLines.FirstOrDefault();
            SAPbobsCOM.Documents myDocumentGenExit = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenExit);//出库单
            SAPbobsCOM.Documents myDocumentGenEntry = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenEntry);//入库单


            #region 出库主表赋值
            myDocumentGenExit.DocDate = order.PostingDate;
            myDocumentGenExit.DocDueDate = order.DeliveryDate;
            myDocumentGenExit.TaxDate = order.DocumentDate;
            myDocumentGenExit.Comments = order.Remarks;
            myDocumentGenExit.Reference1 = order.Reference1;
            myDocumentGenExit.Reference2 = order.Reference2;
            myDocumentGenExit.BPL_IDAssignedToInvoice = B1Common.BOneCommon.GetBranchCodeByWhsCode(order.InventoryCountingLines.Where(c => c.Quantity  >0).FirstOrDefault().WarehouseCode);
            myDocumentGenExit.UserFields.Fields.Item("U_IM_DocEntry").Value = order.DocEntry.ToString() ;
            myDocumentGenExit.UserFields.Fields.Item("U_ChannalDocType").Value = "13";
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
            myDocumentGenEntry.BPL_IDAssignedToInvoice = B1Common.BOneCommon.GetBranchCodeByWhsCode(order.InventoryCountingLines.Where(c=>c.Quantity<0) .FirstOrDefault().WarehouseCode);

            #endregion

            foreach (var item in order.InventoryCountingLines)
            {
                if(item.Quantity<0)
                {
                    myDocumentGenExit.Lines.ItemCode = item.ItemCode;
                    myDocumentGenExit.Lines.ItemDescription = item.ItemDescription;
                    myDocumentGenExit.Lines.WarehouseCode = item.WarehouseCode;
                    myDocumentGenExit.Lines.Quantity = -item.Quantity;
                    myDocumentGenExit.Lines.BarCode = item.BarCode;
                    myDocumentGenExit.Lines.Add();
                }
                else if(item.Quantity >0)
                {
                    myDocumentGenEntry.Lines.ItemCode = item.ItemCode;
                    myDocumentGenEntry.Lines.ItemDescription = item.ItemDescription;
                    myDocumentGenEntry.Lines.WarehouseCode = item.WarehouseCode;
                    myDocumentGenEntry.Lines.Quantity = item.Quantity;
                    myDocumentGenEntry.Lines.BarCode = item.BarCode;
                    myDocumentGenEntry.Lines.Add();
                }   
            }
            string B1DocEntry;
            if (B1Common.BOneCommon.IsExistDocument("OIGN", order.DocEntry.ToString(), out B1DocEntry))
            {
                order.B1DocEntry = B1DocEntry;
               // B1GoodsReceiptsDocEntry = B1DocEntry;
                return new Result()
                {
                    ResultValue = ResultType.True,
                    ResultMessage = "该订单已生成到B1"
                };
            }
            int RntCodeGenExit = myDocumentGenExit.Add();

            if (RntCodeGenExit != 0)
            {
                result.ResultValue = ResultType.False;
                result.ResultMessage += string.Format("【{0}】库存盘点生成出库单失败，ErrorCode:[{1}],ErrrMsg:[{2}];", order.DocEntry, SAP.SAPCompany.GetLastErrorCode(), SAP.SAPCompany.GetLastErrorDescription());
            }
            else
            {
                result.ResultValue = ResultType.True;
                result.ResultMessage += "【" + order.DocEntry.ToString() + "】库存盘点生成出库单成功，系统单据：" + SAP.SAPCompany.GetNewObjectKey();

            }


            int RntCodeGenEntry = myDocumentGenEntry.Add();
            if (RntCodeGenEntry != 0)
            {
                result.ResultValue = ResultType.False;
                result.ResultMessage += string.Format("【{0}】库存盘点生成入库单失败，ErrorCode:[{1}],ErrrMsg:[{2}];", order.DocEntry, SAP.SAPCompany.GetLastErrorCode(), SAP.SAPCompany.GetLastErrorDescription());
            }
            else
            {
                result.ResultValue = ResultType.True;
                result.ResultMessage += "【" + order.DocEntry.ToString() + "】库存盘点生成入库单成功，系统单据：" + SAP.SAPCompany.GetNewObjectKey();
            }

           
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myDocumentGenEntry);
            return result;
        }

       

        
    }
}
