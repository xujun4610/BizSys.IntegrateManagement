using BizSys.IntegrateManagement.Repository.BOneCommon;
using BizSys.IntegrateManagement.Entity.PurchaseDeliveryOrder;
using BizSys.IntegrateManagement.IRepository.PurchaseManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Repository.PurchaseManagement
{
    public class PurchaseDeliveryRep : IPurchaseDeliveryRep
    {
        public Entity.PurchaseDeliveryOrder.PurchaseDeliveryOrderRootObject GetPurchaseDeliveryOrderByKey(string DocEntry)
        {
            PurchaseDeliveryOrderRootObject purchaseDeliveryRootObject = new PurchaseDeliveryOrderRootObject();
            purchaseDeliveryRootObject.ResultObjects = new List<ResultObjects>();
            SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            SAPbobsCOM.IRecordset resLine = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            string sql = "select * from OPDN where DocEntry = '{0}'";
            string sqlLine = "select * from PDN1 where DocEntry = '{0}'";
            try
            {
                res.DoQuery(string.Format(sql, Convert.ToInt32(DocEntry)));
                if (res.RecordCount != 1)
                {
                    purchaseDeliveryRootObject.ResultCode = -1;
                    purchaseDeliveryRootObject.Message = "Can not found the Purchase Delivery in SAP.";
                }
                else
                {

                    ResultObjects purchaseDelivery = new ResultObjects();

                    #region 采购交货 属性赋值

                    purchaseDelivery.DeliveryDate = res.Fields.Item("DocDueDate").Value;
                    purchaseDelivery.PostingDate = res.Fields.Item("DocDate").Value;
                    purchaseDelivery.DocumentDate = res.Fields.Item("TaxDate").Value;
                    purchaseDelivery.Remarks = res.Fields.Item("Comments").Value;
                    purchaseDelivery.B1DocEntry = (res.Fields.Item("DocEntry").Value).ToString();
                    purchaseDelivery.CreateDate = res.Fields.Item("CreateDate").Value;
                    purchaseDelivery.UpdateDate = res.Fields.Item("UpdateDate").Value;
                    purchaseDelivery.PurchaseDeliveryItems = new List<PurchaseDeliveryItems>();
                    purchaseDelivery.DocumentStatus = "Released";
                    purchaseDelivery.DataSource = res.Fields.Item("U_ResouceType").Value;
                    purchaseDelivery.PostingDate = res.Fields.Item("DocDate").Value;
                    purchaseDelivery.DocumentDate = res.Fields.Item("TaxDate").Value;
                    purchaseDelivery.BusinessPartnerCode = res.Fields.Item("CardCode").Value;
                    purchaseDelivery.BusinessPartnerName = res.Fields.Item("CardName").Value;
                    purchaseDelivery.TaxRate = res.Fields.Item("VatPerCent").Value;
                    purchaseDelivery.TotalTax = res.Fields.Item("VatSum").Value;
                    purchaseDelivery.DiscountForDocument = res.Fields.Item("DiscPrcnt").Value;
                    purchaseDelivery.TotalDiscount = res.Fields.Item("DiscSum").Value;
                    purchaseDelivery.DocumentCurrency = res.Fields.Item("DocCur").Value;
                    purchaseDelivery.DocumentTotal = res.Fields.Item("DocTotal").Value;
                    purchaseDelivery.Reference1 = res.Fields.Item("Ref1").Value;
                    purchaseDelivery.Reference2 = res.Fields.Item("Ref2").Value;
                    //purchaseDelivery.TotalNet =;
                    #region 行数据赋值
                    resLine.DoQuery(string.Format(sqlLine, Convert.ToInt32(DocEntry)));
                    if (res.RecordCount == 0)
                    {
                        purchaseDeliveryRootObject.ResultCode = -1;
                        purchaseDeliveryRootObject.Message = "Can not found the Line of Purchase Delivery Order in SAP";
                    }
                    else
                    {
                        while (!resLine.EoF)
                        {
                            //myDocument.Lines.SetCurrentLine(cuurentLine);
                            PurchaseDeliveryItems item = new PurchaseDeliveryItems();

                            item.ItemCode = resLine.Fields.Item("ItemCode").Value;
                            item.ItemDescription = resLine.Fields.Item("Dscription").Value;
                            item.Quantity = resLine.Fields.Item("Quantity").Value;
                            //item.Price = resLine.Fields.Item("price").Value;//未税单价
                            item.DiscountPerLine = resLine.Fields.Item("DiscPrcnt").Value;//行折扣率
                            item.VatSum = resLine.Fields.Item("VatSum").Value;
                            item.LineTotal = resLine.Fields.Item("LineTotal").Value;
                            item.BaseDocumentEntry = Common.DataConvert.ConvertToIntEx(resLine.Fields.Item("U_BaseEntry").Value);
                            item.BaseDocumentLineId = Common.DataConvert.ConvertToIntEx(resLine.Fields.Item("U_BaseLineNum").Value);
                            item.OriginalDocumentEntry = (resLine.Fields.Item("DocEntry").Value).ToString();
                            item.OriginalDocumentLineId = (resLine.Fields.Item("LineNum").Value).ToString();
                            item.DeliveryDate = resLine.Fields.Item("ShipDate").Value;
                            item.OpenQuantity = resLine.Fields.Item("OpenQty").Value;
                            item.Price = resLine.Fields.Item("Price").Value;
                            item.PriceCurrency = resLine.Fields.Item("Currency").Value;
                            item.OpenAmount = resLine.Fields.Item("OpenSum").Value;
                            item.Warehouse = resLine.Fields.Item("WhsCode").Value;
                            item.UnitPrice = resLine.Fields.Item("PriceBefDi").Value;
                            item.TaxRatePerLine = resLine.Fields.Item("VatPrcnt").Value;
                            item.GrossPrice = resLine.Fields.Item("PriceAfVAT").Value;
                            item.TotalTaxLine = resLine.Fields.Item("VatSum").Value;
                            item.GrossTotal = resLine.Fields.Item("Gtotal").Value;
                            item.DeliveredQuantity = resLine.Fields.Item("DelivrdQty").Value;
                            item.LineWasClosedManually = resLine.Fields.Item("LinManClsd").Value;
                            item.DistributionRule1 = resLine.Fields.Item("OcrCode").Value;
                            item.DistributionRule2 = resLine.Fields.Item("OcrCode2").Value;
                            item.DistributionRule3 = resLine.Fields.Item("OcrCode3").Value;
                            item.DistributionRule4 = resLine.Fields.Item("OcrCode4").Value;
                            item.DistributionRule5 = resLine.Fields.Item("OcrCode5").Value;
                            item.LineStatus = "Released";
                            purchaseDelivery.TotalNet += item.LineTotal;

                            purchaseDelivery.PurchaseDeliveryItems.Add(item);
                            resLine.MoveNext();
                        }
                    }
                    #endregion
                    #endregion
                    
                    purchaseDeliveryRootObject.ResultObjects.Add(purchaseDelivery);
                    purchaseDeliveryRootObject.ResultCode = 0;
                    purchaseDeliveryRootObject.Message = "Successful operation.";
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
