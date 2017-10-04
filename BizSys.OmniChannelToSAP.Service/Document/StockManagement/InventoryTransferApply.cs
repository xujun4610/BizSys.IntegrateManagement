using BizSys.IntegrateManagement.Entity.StockManagement.InventoryTransferApply;
using BizSys.IntegrateManagement.Entity.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizSys.OmniChannelToSAP.Service.B1Common;
using static BizSys.IntegrateManagement.Common.Enumerator;

namespace BizSys.OmniChannelToSAP.Service.Document.StockManagement
{
    public class InventoryTransferApply
    {
        public static Result CreateTransferOrder(ResultObjects order)
        {
            string B1DocEntry;
            SAPbobsCOM.StockTransfer myStockTransfer;
            if (BOneCommon.IsMainStore(order.FromWarehouse) || BOneCommon.IsMainStore(order.Warehouse))
            {
                //生成库存转储请求
                if (B1Common.BOneCommon.IsExistDocument("OWTQ", order.DocEntry.ToString(), out B1DocEntry))
                {
                    order.B1DocEntry = B1DocEntry;
                    return new Result()
                    {
                        ResultValue = ResultType.True,
                        ResultMessage = "该订单已生成到B1"
                    };
                }
                myStockTransfer = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest);

            }
            else
            {
                //生成库存转储
                if (B1Common.BOneCommon.IsExistDocument("OWTR", order.DocEntry.ToString(), out B1DocEntry))
                {
                    order.B1DocEntry = B1DocEntry;
                    return new Result()
                    {
                        ResultValue = ResultType.True,
                        ResultMessage = "该订单已生成到B1"
                    };
                }
                myStockTransfer = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oStockTransfer);

            }

            Result result = new Result();
           
            #region 主表赋值
            myStockTransfer.DocDate = order.PostingDate;
            myStockTransfer.DueDate = order.DeliveryDate;
            myStockTransfer.TaxDate = order.DocumentDate;
            myStockTransfer.Comments = order.Remarks;
            myStockTransfer.Reference1 = order.Reference1;
            myStockTransfer.Reference2 = order.Reference2;
            myStockTransfer.CardCode = order.BusinessPartnerCode;
            myStockTransfer.CardName = order.BusinessPartnerName;
            myStockTransfer.UserFields.Fields.Item("U_IM_DocEntry").Value = order.DocEntry.ToString();

            myStockTransfer.ContactPerson = order.ContactPerson;
            myStockTransfer.UserFields.Fields.Item("U_PickingWay").Value = ((emSendWay)Enum.Parse(typeof(emSendWay), order.SendWay)).GetHashCode().ToString();
            myStockTransfer.FromWarehouse = order.FromWarehouse;
            myStockTransfer.ToWarehouse = order.Warehouse;
            myStockTransfer.ContactPerson = order.ContactPerson;
            myStockTransfer.UserFields.Fields.Item("U_Telephone").Value = order.ContactNumber;
            myStockTransfer.UserFields.Fields.Item("U_DeliveryAddress").Value = order.Province + '-' + order.City + '-' + order.County + '-' + order.DetailedAddress;
            myStockTransfer.UserFields.Fields.Item("U_LotCompany").Value = order.LogisticsCompany;
            myStockTransfer.UserFields.Fields.Item("U_LotNum").Value = order.LogisticsNumber;
            myStockTransfer.UserFields.Fields.Item("U_Linkman").Value = order.Consignee.ToString();
            myStockTransfer.UserFields.Fields.Item("U_ChannalDocType").Value = "14";
            if (!string.IsNullOrEmpty(order.CustomType))
                myStockTransfer.UserFields.Fields.Item("U_ChType").Value = order.CustomType;
            #endregion
            foreach (var item in order.InventoryTransferLines)
            {
                #region 子表赋值
                myStockTransfer.Lines.ItemCode = item.ItemCode;
                myStockTransfer.Lines.ItemDescription = item.ItemDescription;
                myStockTransfer.Lines.Quantity = item.Quantity;
                myStockTransfer.Lines.FromWarehouseCode = item.FromWarehouse;
                myStockTransfer.Lines.WarehouseCode = item.Warehouse;
                var dbRule = BOneCommon.GetDistributionRule(item.ItemCode, order.DataOwner.ToString());
                myStockTransfer.Lines.DistributionRule = dbRule.OcrCode;
                myStockTransfer.Lines.DistributionRule2 = dbRule.OcrCode2;
                myStockTransfer.Lines.DistributionRule3 = dbRule.OcrCode3;
                myStockTransfer.Lines.DistributionRule4 = item.DistributionRule4;
                myStockTransfer.Lines.DistributionRule5 = item.DistributionRule5;

                myStockTransfer.Lines.Add();
                #endregion
            }

            int RntCode = myStockTransfer.Add();
            if (RntCode != 0)
            {
                result.ResultValue = ResultType.False;
                result.ResultMessage = string.Format("【{0}】库存转储申请处理失败，ErrorCode:[{1}],ErrrMsg:[{2}];", order.DocEntry, SAP.SAPCompany.GetLastErrorCode(), SAP.SAPCompany.GetLastErrorDescription());
            }
            else
            {
                result.ResultValue = ResultType.True;
                result.ResultMessage = "【" + order.DocEntry.ToString() + "】库存转储申请处理成功，系统单据：" + SAP.SAPCompany.GetNewObjectKey();
            }
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myStockTransfer);
            return result;
        }
    }
}
