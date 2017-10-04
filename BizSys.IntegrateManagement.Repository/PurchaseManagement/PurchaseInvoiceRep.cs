using BizSys.IntegrateManagement.IRepository.PurchaseManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizSys.IntegrateManagement.Entity.PurchaseManagement.PurchaseInvoice;
using BizSys.IntegrateManagement.Repository.BOneCommon;

namespace BizSys.IntegrateManagement.Repository.PurchaseManagement
{
    public class PurchaseInvoiceRep : IPurchaseInvoiceRep
    {
        public PurchaseInvoiceRootObject GetPurchaseInvoiceByKey(string DocEntry)
        {
            PurchaseInvoiceRootObject purchaseInvoiceRootObject = new PurchaseInvoiceRootObject();
            purchaseInvoiceRootObject.ResultObjects = new List<ResultObjects>();
            SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            SAPbobsCOM.IRecordset resLine = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            string sql = "select * from OPCH where DocEntry = '{0}'";
            string sqlLine = "select * from PCH1 where DocEntry = '{0}'";
            try
            {
                res.DoQuery(string.Format(sql, Convert.ToInt32(DocEntry)));
                if (res.RecordCount != 1)
                {
                    purchaseInvoiceRootObject.ResultCode = -1;
                    purchaseInvoiceRootObject.Message = "Can not found the A/P Invoice Order in SAP.";
                }
                else
                {
                    purchaseInvoiceRootObject.ResultCode = 0;
                    purchaseInvoiceRootObject.Message = "Successful operation.";
                    ResultObjects purchaseInvoice = new ResultObjects();
                    #region 采购交货 属性赋值
                    purchaseInvoice.DocEntry = res.Fields.Item("DocEntry").Value;
                    purchaseInvoice.DocumentStatus = "Released"; //全渠道的单据状态变成“下达”
                    purchaseInvoice.CardCode = res.Fields.Item("CardCode").Value;
                    purchaseInvoice.CardName = res.Fields.Item("CardName").Value;
                    purchaseInvoice.DeliveryDate = res.Fields.Item("DocDueDate").Value;
                    purchaseInvoice.PostingDate = res.Fields.Item("DocDate").Value;
                    purchaseInvoice.DocumentDate = res.Fields.Item("TaxDate").Value;
                    //purchaseInvoice.Remarks = res.Fields.Item("Comments").Value;
                    purchaseInvoice.BPLId = (res.Fields.Item("BPLId").Value).ToString();
                    purchaseInvoice.B1DocEntry = (res.Fields.Item("DocEntry").Value).ToString();
                    purchaseInvoice.DataSource = res.Fields.Item("U_ResouceType").Value;

                    purchaseInvoice.CreateDate = res.Fields.Item("CreateDate").Value;
                    purchaseInvoice.UpdateDate = res.Fields.Item("UpdateDate").Value;
                    purchaseInvoice.Reference1 = res.Fields.Item("Ref1").Value;
                    purchaseInvoice.Reference2 = res.Fields.Item("Ref2").Value;
                    purchaseInvoice.CardCode = res.Fields.Item("CardCode").Value;
                    purchaseInvoice.CardName = res.Fields.Item("CardName").Value;
                    purchaseInvoice.DocCur = res.Fields.Item("DocCur").Value;
                    purchaseInvoice.DocTotal = res.Fields.Item("DocTotal").Value;
                    purchaseInvoice.Comments = res.Fields.Item("Comments").Value;
                    purchaseInvoice.VatSum = res.Fields.Item("VatSum").Value;
                    purchaseInvoice.DiscPrcnt = res.Fields.Item("DiscPrcnt").Value;
                    purchaseInvoice.DiscSum = res.Fields.Item("DiscSum").Value;
                    purchaseInvoice.draftKey = (res.Fields.Item("draftKey").Value).ToString();
                    purchaseInvoice.DiscPrcnt = res.Fields.Item("DiscPrcnt").Value;
                    purchaseInvoice.DiscSum = res.Fields.Item("DiscSum").Value;
                    purchaseInvoice.PayableItems = new List<PayableItems>();
                    #region 行数据赋值
                    resLine.DoQuery(string.Format(sqlLine, Convert.ToInt32(DocEntry)));
                    if (resLine.RecordCount == 0)
                    {
                        purchaseInvoiceRootObject.ResultCode = -1;
                        purchaseInvoiceRootObject.Message = "Can not found the Line of A/P invoice in SAP.";
                    }
                    else
                    {
                        while (!resLine.EoF)
                        {
                            PayableItems item = new PayableItems();
                            item.DocEntry = (resLine.Fields.Item("DocEntry").Value).ToString();
                            item.DiscPrcnt = resLine.Fields.Item("DiscPrcnt").Value;
                            item.ItemCode = resLine.Fields.Item("ItemCode").Value;
                            item.WhsCode = resLine.Fields.Item("WhsCode").Value;
                            //item = resLine.Fields.Item("ItemDescription").Value;
                            item.Quantity = resLine.Fields.Item("Quantity").Value;
                            item.Price = resLine.Fields.Item("Price").Value;
                            item.LineTotal = resLine.Fields.Item("LineTotal").Value;
                            item.BaseEntry = Common.DataConvert.ConvertToIntEx(resLine.Fields.Item("U_BaseEntry").Value);
                            item.BaseLine = Common.DataConvert.ConvertToIntEx(resLine.Fields.Item("U_BaseLineNum").Value);

                            item.Dscription = resLine.Fields.Item("Dscription").Value;
                            item.ShipDate = resLine.Fields.Item("ShipDate").Value;
                            item.Currency = resLine.Fields.Item("Currency").Value;
                            item.PriceAfVAT = resLine.Fields.Item("PriceAfVAT").Value;
                            item.GTotal = resLine.Fields.Item("GTotal").Value;
                            item.DelivrdQty = resLine.Fields.Item("DelivrdQty").Value;
                            item.OpenQty = resLine.Fields.Item("OpenQty").Value;
                            item.VatSum = resLine.Fields.Item("VatSum").Value;
                            item.OcrCode = resLine.Fields.Item("OcrCode").Value;
                            item.OcrCode2 = resLine.Fields.Item("OcrCode2").Value;
                            item.OcrCode3 = resLine.Fields.Item("OcrCode3").Value;
                            item.OcrCode4 = resLine.Fields.Item("OcrCode4").Value;
                            item.OcrCode5 = resLine.Fields.Item("OcrCode5").Value;
                            item.BsDocEntry = resLine.Fields.Item("DocEntry").Value;
                            item.BsDocLine = resLine.Fields.Item("LineNum").Value;
                            item.LineStatus = "Released";
                            purchaseInvoice.Subtotal += item.LineTotal;

                            purchaseInvoice.PayableItems.Add(item);
                            resLine.MoveNext();
                        }

                    }
                    #endregion
                    #endregion
                    purchaseInvoiceRootObject.ResultObjects.Add(purchaseInvoice);
                }
            }
            catch (Exception ex)
            {
                purchaseInvoiceRootObject.ResultCode = -1;
                purchaseInvoiceRootObject.Message = ex.Message;
            }

            return purchaseInvoiceRootObject;
        }
    }
}
