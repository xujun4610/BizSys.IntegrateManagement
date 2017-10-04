using BizSys.IntegrateManagement.Repository.BOneCommon;
using BizSys.IntegrateManagement.Entity.SalesManagement.SalesDeliveryOrder;
using BizSys.IntegrateManagement.IRepository.SalesManagementService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Repository.SalesManagementService
{
    public class SalesDeliveryOrderRep : ISalesDeliveryOrderRep
    {
        public Entity.SalesManagement.SalesDeliveryOrder.SalesDeliveryOrderRootObject GetSalesDeliveryOrderByKey(string DocEntry)
        {
            SalesDeliveryOrderRootObject purchaseDeliveryRootObject = new SalesDeliveryOrderRootObject();
            purchaseDeliveryRootObject.ResultObjects = new List<ResultObjects>();
            SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            SAPbobsCOM.IRecordset resLine = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            string sql = "select * from ODLN where DocEntry = '{0}'";
            string sqlLine = "select * from DLN1 where DocEntry = '{0}'";
            string[] addrList = new string[4];
            try
            {
                res.DoQuery(string.Format(sql, Convert.ToInt32(DocEntry)));
                if (res.RecordCount != 1)
                {
                    purchaseDeliveryRootObject.ResultCode = -1;
                    purchaseDeliveryRootObject.Message = "Can not found the Sales Delivery Order in SAP.";
                }
                else
                {
                    purchaseDeliveryRootObject.ResultCode = 0;
                    purchaseDeliveryRootObject.Message = "Successful operation.";
                    ResultObjects salesOrderDelivery = new ResultObjects();
                    #region 采购交货 属性赋值
                    salesOrderDelivery.DocEntry = res.Fields.Item("DocEntry").Value;
                    salesOrderDelivery.DocumentStatus = "Released"; //全渠道的单据状态变成“下达”
                    salesOrderDelivery.BusinessPartnerCode = res.Fields.Item("CardCode").Value;
                    salesOrderDelivery.BusinessPartnerName = res.Fields.Item("CardName").Value;
                    salesOrderDelivery.DeliveryDate = res.Fields.Item("DocDueDate").Value;
                    salesOrderDelivery.PostingDate = res.Fields.Item("DocDate").Value;
                    salesOrderDelivery.DocumentDate = res.Fields.Item("DocDate").Value;
                    salesOrderDelivery.Remarks = (res.Fields.Item("Comments").Value).ToString();
                    salesOrderDelivery.BPReferenceNumber = (res.Fields.Item("BPLId").Value).ToString();
                    salesOrderDelivery.B1DocEntry = (res.Fields.Item("DocEntry").Value).ToString();
                    //salesOrderDelivery.DataSource = res.Fields.Item("U_ResouceType").Value;
                    salesOrderDelivery.DataSource = "11";
                    salesOrderDelivery.CreateDate = res.Fields.Item("CreateDate").Value;
                    //salesOrderDelivery.CreateTime = res.Fields.Item("CreateTime").Value;
                    salesOrderDelivery.UpdateDate = res.Fields.Item("UpdateDate").Value;
                    //salesOrderDelivery.UpdateTime = res.Fields.Item("UpdateTime").Value;
                    salesOrderDelivery.TaxRate = res.Fields.Item("VatPercent").Value;
                    salesOrderDelivery.TotalTax = res.Fields.Item("VatSum").Value;
                    salesOrderDelivery.DiscountForDocument = (res.Fields.Item("DiscPrcnt").Value).ToString();
                    salesOrderDelivery.TotalDiscount = (res.Fields.Item("DiscSum").Value).ToString();
                    salesOrderDelivery.DocumentCurrency = res.Fields.Item("DocCur").Value;
                    salesOrderDelivery.DocumentTotal = res.Fields.Item("DocTotal").Value;
                    salesOrderDelivery.Consignee = res.Fields.Item("U_Linkman").Value;
                    salesOrderDelivery.ContactNumber = res.Fields.Item("U_Telephone").Value;
                    salesOrderDelivery.DetailedAddress = res.Fields.Item("U_DeliveryAddress").Value;
                    if (res.Fields.Item("U_DeliveryAddress").Value.Contains("-"))
                    {
                        addrList = salesOrderDelivery.DetailedAddress.Split('-');
                        salesOrderDelivery.Province = addrList[0];
                        salesOrderDelivery.City = addrList[1];
                        salesOrderDelivery.County = addrList[2];
                        salesOrderDelivery.DetailedAddress = addrList[3];
                    }
                    salesOrderDelivery.PickingWay = res.Fields.Item("U_PickingWay").Value;
                    salesOrderDelivery.PickingAddress = res.Fields.Item("U_PickingAddress").Value;
                    salesOrderDelivery.Reference1 = res.Fields.Item("Ref1").Value;
                    salesOrderDelivery.Reference2 = res.Fields.Item("Ref2").Value;
                    salesOrderDelivery.CreateDate = res.Fields.Item("CreateDate").Value;
                    salesOrderDelivery.UpdateDate = res.Fields.Item("UpdateDate").Value;
                    salesOrderDelivery.Subtotal = res.Fields.Item("DocTotal").Value - res.Fields.Item("VatSum").Value;
                    
                    salesOrderDelivery.SalesDeliveryItems = new List<SalesDeliveryItems>();
                    #region 行数据赋值
                    resLine.DoQuery(string.Format(sqlLine, Convert.ToInt32(DocEntry)));
                    if (resLine.RecordCount == 0)
                    {
                        purchaseDeliveryRootObject.ResultCode = -1;
                        purchaseDeliveryRootObject.Message = "Can not found the Line of Sales Delivery Order in SAP.";
                    }
                    else
                    {
                        while (!resLine.EoF)
                        {
                            SalesDeliveryItems item = new SalesDeliveryItems();
                            item.DocEntry = resLine.Fields.Item("DocEntry").Value;
                            item.DiscountPerLine = resLine.Fields.Item("DiscPrcnt").Value;
                            item.ItemCode = resLine.Fields.Item("ItemCode").Value;
                            item.Quantity = resLine.Fields.Item("Quantity").Value;
                            item.UnitPrice = resLine.Fields.Item("PriceBefDi").Value;//单价
                            item.Price = resLine.Fields.Item("Price").Value;//折后价
                            item.DiscountPerLine = resLine.Fields.Item("DiscPrcnt").Value;//税率
                            //item.PriceBefDi = resLine.Fields.Item("PriceBefDi").Value;//折扣价格
                            item.LineTotal = resLine.Fields.Item("LineTotal").Value;
                            item.DistributionRule1 = resLine.Fields.Item("OcrCode").Value;
                            item.DistributionRule2 = resLine.Fields.Item("OcrCode2").Value;
                            item.DistributionRule3 = resLine.Fields.Item("OcrCode3").Value;
                            item.DistributionRule4 = resLine.Fields.Item("OcrCode4").Value;
                            item.DistributionRule5 = resLine.Fields.Item("OcrCode5").Value;
                            item.OriginalDocumentLineId = (resLine.Fields.Item("LineNum").Value).ToString();
                            item.OriginalDocumentEntry = (resLine.Fields.Item("DocEntry").Value).ToString();
                            item.LineWasClosedManually = resLine.Fields.Item("LinManClsd").Value;
                            item.DeliveredQuantity = resLine.Fields.Item("DelivrdQty").Value;
                            item.GrossTotal = resLine.Fields.Item("Gtotal").Value;
                            item.TotalTaxLine = (resLine.Fields.Item("VatSum").Value).ToString();
                            item.GrossPrice = (resLine.Fields.Item("PriceAfVAT").Value).ToString();//毛价
                            item.TaxRatePerLine = resLine.Fields.Item("VatPrcnt").Value;
                            item.Warehouse = resLine.Fields.Item("WhsCode").Value;
                            item.OpenAmount = resLine.Fields.Item("OpenSum").Value;
                            item.LineTotal = resLine.Fields.Item("LineTotal").Value;
                            item.PriceCurrency = resLine.Fields.Item("Currency").Value;
                            item.OpenQuantity = (resLine.Fields.Item("OpenQty").Value).ToString();
                            item.DeliveryDate = resLine.Fields.Item("ShipDate").Value;
                            item.ItemDescription = resLine.Fields.Item("Dscription").Value;
                            item.BaseDocumentLineId = resLine.Fields.Item("U_BaseLineNum").Value;
                            item.BaseDocumentEntry = resLine.Fields.Item("U_BaseEntry").Value;
                            item.LineStatus = "Released";//全渠道的行状态变成“下达”
                           // item.Status = "R";
                            item.BaseEntry = Common.DataConvert.ConvertToIntEx(resLine.Fields.Item("U_BaseEntry").Value);
                            item.BaseLineNum = Common.DataConvert.ConvertToIntEx(resLine.Fields.Item("U_BaseLineNum").Value);

                            salesOrderDelivery.SalesDeliveryItems.Add(item);
                            resLine.MoveNext();
                        }

                    }
                    #endregion
                    #endregion
                    purchaseDeliveryRootObject.ResultObjects.Add(salesOrderDelivery);
                }
            }
            catch (Exception ex)
            {
                purchaseDeliveryRootObject.ResultCode = -1;
                purchaseDeliveryRootObject.Message = ex.Message;
            }

            return purchaseDeliveryRootObject;
        }
    }
}
