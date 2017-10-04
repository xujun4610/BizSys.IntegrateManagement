using BizSys.IntegrateManagement.Entity.SalesManagement.InvoiceOrder;
using BizSys.IntegrateManagement.IRepository.SalesManagementService;
using BizSys.IntegrateManagement.Repository.BOneCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Repository.SalesManagementService
{
    public class InvoiceOrderRep : IInvoiceOrderRep
    {
        public InvoiceOrderRootObject GetInvoiceByKey(int DocEntry)
        {
            InvoiceOrderRootObject invoiceOrderRootObject = new InvoiceOrderRootObject();
            invoiceOrderRootObject.ResultObjects = new List<ResultObjects>();
            SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            SAPbobsCOM.IRecordset resLine = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            string sql = "select * from OINV where DocEntry ='{0}'";
            string sqlLine = "select * from INV1 where DocEntry ='{0}'";
            try
            {
                res.DoQuery(string.Format(sql, Convert.ToInt32(DocEntry)));

                if (res.RecordCount != 1)
                {
                    invoiceOrderRootObject.ResultCode = -1;
                    invoiceOrderRootObject.Message = "Can not found Invoice order in SAP.";
                }
                else
                {
                    invoiceOrderRootObject.ResultCode = 0;
                    invoiceOrderRootObject.Message = "Operation successful.";
                    ResultObjects invoice = new ResultObjects();
                    invoice.ReceivableItems = new List<ReceivableItems>();
                    #region 属性赋值

                    invoice.DocEntry = res.Fields.Item("DocEntry").Value;
                    invoice.BPLId = res.Fields.Item("BPLId").Value;
                    invoice.CardCode = res.Fields.Item("CardCode").Value;
                    invoice.CardName = res.Fields.Item("CardName").Value;
                    invoice.UpdateDate = res.Fields.Item("UpdateDate").Value;
                    invoice.DeliveryDate = res.Fields.Item("DocDueDate").Value;
                    invoice.PostingDate = res.Fields.Item("DocDate").Value;
                    invoice.DocumentDate = res.Fields.Item("TaxDate").Value;
                    invoice.DocTotal = res.Fields.Item("CntctCode").Value;
                    invoice.B1DocEntry = (res.Fields.Item("DocEntry").Value).ToString();
                    //invoice.DataSource = res.Fields.Item("U_ResouceType").Value;
                    invoice.DataSource = "13";
                    invoice.CreateDate = res.Fields.Item("CreateDate").Value;
                    //invoice.CreateTime = res.Fields.Item("CreateTime").Value;
                    //invoice.UpdateTime = res.Fields.Item("UpdateTime").Value;
                   // invoice.BPLName = (res.Fields.Item("VatSum").Value).ToString();
                    invoice.OwnerCode = res.Fields.Item("DiscPrcnt").Value;
                    //invoice.VatSum = res.Fields.Item("DiscSum").Value;
                    invoice.DiscPrcnt = res.Fields.Item("DOcCur").Value;
                    invoice.Reference1 = res.Fields.Item("Ref1").Value;
                    invoice.Reference2 = res.Fields.Item("Ref2").Value;
                    invoice.DocTotal = res.Fields.Item("DocTotal").Value;
                    invoice.VatSum = res.Fields.Item("VatSum").Value;
                    invoice.DiscPrcnt = res.Fields.Item("DiscPrcnt").Value.ToString();
                    invoice.DiscSum = res.Fields.Item("DiscSum").Value;
                    invoice.DocumentStatus = "Released";
                    //invoice.B1DocEntry = Common.DataConvert.ConvertToIntEx(res.Fields.Item("DocEntry").Value);
                    #region 行数据赋值
                    resLine.DoQuery(string.Format(sqlLine, Convert.ToInt32(DocEntry)));
                    if (resLine.RecordCount == 0)
                    {
                        invoiceOrderRootObject.ResultCode = -1;
                        invoiceOrderRootObject.Message = "Can not found Invoice order in SAP.";
                    }
                    else
                    {
                        while (!resLine.EoF)
                        {
                            //myDocument.Lines.SetCurrentLine(cuurentLine);
                            ReceivableItems item = new ReceivableItems();
                            item.DocEntry = resLine.Fields.Item("DocEntry").Value;
                            item.LineId = resLine.Fields.Item("LineNum").Value;
                            item.Quantity = resLine.Fields.Item("Quantity").Value;
                            item.Price = resLine.Fields.Item("PriceBefDi").Value;//不含税单价
                            item.LineTotal = resLine.Fields.Item("LineTotal").Value;
                            item.ItemCode = resLine.Fields.Item("ItemCode").Value;
                            item.BaseEntry = Common.DataConvert.ConvertToIntEx(resLine.Fields.Item("U_BaseEntry").Value);
                            item.BaseLine = Common.DataConvert.ConvertToIntEx(resLine.Fields.Item("U_BaseLineNum").Value);
                            //item.Reference1 = resLine.Fields.Item("Ref1").Value;
                            //item.Reference2 = resLine.Fields.Item("ref2").Value;
                            item.ItemCode = resLine.Fields.Item("ItemCode").Value;
                            item.Dscription = resLine.Fields.Item("Dscription").Value;
                            item.ShipDate = resLine.Fields.Item("ShipDate").Value;
                            item.Currency = resLine.Fields.Item("Currency").Value;
                            item.WhsCode = resLine.Fields.Item("WhsCode").Value;
                            item.PriceAfVAT = resLine.Fields.Item("PriceAfVAT").Value;
                            item.DocDate = resLine.Fields.Item("DocDate").Value;
                            item.GTotal = resLine.Fields.Item("GTotal").Value;
                            item.DelivrdQty = resLine.Fields.Item("DelivrdQty").Value;
                            item.OpenQty = resLine.Fields.Item("OpenQty").Value;
                            item.DiscPrcnt = resLine.Fields.Item("DiscPrcnt").Value;
                            item.VatSum = resLine.Fields.Item("vatSum").Value;
                            item.OcrCode = resLine.Fields.Item("OcrCode").Value;
                            item.OcrCode2 = resLine.Fields.Item("OcrCode2").Value;
                            item.OcrCode3 = resLine.Fields.Item("OcrCode3").Value;
                            item.OcrCode4 = resLine.Fields.Item("OcrCode4").Value;
                            item.OcrCode5 = resLine.Fields.Item("OcrCode5").Value;
                            item.BsDocEntry = (resLine.Fields.Item("DocEntry").Value).ToString();
                            item.BsDocLine = (resLine.Fields.Item("LineNum").Value).ToString();
                            item.LineStatus = "Released";
                            invoice.Subtotal += item.LineTotal;

                            invoice.ReceivableItems.Add(item);
                            resLine.MoveNext();
                        }
                    }
                    #endregion

                    #endregion
                    invoiceOrderRootObject.ResultObjects.Add(invoice);
                }
            }
            catch (Exception ex)
            {
                invoiceOrderRootObject.ResultCode = -1;
                invoiceOrderRootObject.Message = ex.Message;
            }
            return invoiceOrderRootObject;

        }
    }
}
