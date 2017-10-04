using BizSys.IntegrateManagement.Common;
using BizSys.OmniChannelToSAP.Service.B1Common;
using MagicBox.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.OmniChannelToSAP.Service.Service.SalesManagementServcie
{
    public class CreatePaymentByB1Invoice
    {
        public static void HandingInvoice()
        {
            string guid = "SalesOrderB1-" + Guid.NewGuid();
            int successfulCount = 0;
            try
            {
                #region 获取未处理的销售订单
                int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetInvoiceCount"], 30);
                SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                SAPbobsCOM.IRecordset resLine = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                // string sql = $@"select top {resultCount} * from ODLN where U_IsSync = 'N' and U_ResouceType = '13'";
                //查询条件 
                //      1、来源Anywhere订单  where U_ResouceType = '13' 
                string sql = $@"SELECT top {resultCount} * FROM OINV";
                res.DoQuery(sql);
                if (res.RecordCount < 1) return;
                #endregion
                Logger.Writer(guid, QueueStatus.Open, $"已获取B1中销售交货单[{res.RecordCount}]");
                SAPbobsCOM.Payments myPayments;
                while (!res.EoF)
                {
                    try
                    {
                        string resourceType = res.Fields.Item("U_ResouceType").Value;
                        string PickingWay = res.Fields.Item("U_PickingWay").Value;
                        myPayments = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oIncomingPayments);

                        #region 主表  
                        int DocEntry = res.Fields.Item("DocEntry").Value;

                        myPayments.CardCode = res.Fields.Item("CardCode").Value;
                        myPayments.DocDate = res.Fields.Item("DocDate").Value;
                       // myPayments.PaymentType = res.Fields.Item("DocTotal").Value;
                        myPayments.TaxDate = res.Fields.Item("TaxDate").Value;
                        myPayments.DueDate = res.Fields.Item("DocDueDate").Value;
                        myPayments.TransferDate = DateTime.Now;
                        // myPayments.TransferAccount = order.CardNumber;
                        myPayments.TransferSum = res.Fields.Item("DocTotal").Value;
                        myPayments.DocCurrency = "RMB";//order.DocumentCurren

                        //myPayments.UserFields.Fields.Item("U_IM_DocEntry").Value = res.Fields.Item("DocEntry").Value.ToString();
                        //myPayments.UserFields.Fields.Item("U_PickingWay").Value = res.Fields.Item("U_PickingWay").Value.ToString();

                       
                        #endregion
                        resLine.DoQuery($@"select * from DLN1 where DocEntry = {DocEntry}");
                        //while (!resLine.EoF)
                        //{
                        //    #region 子表
                        //    myPayments.Lines.BaseType = 15;
                        //    myPayments.Lines.BaseEntry = resLine.Fields.Item("DocEntry").Value;
                        //    myPayments.Lines.BaseLine = resLine.Fields.Item("LineNum").Value;
                        //    myPayments.Lines.ItemCode = resLine.Fields.Item("ItemCode").Value.ToString();
                        //    myPayments.Lines.WarehouseCode = resLine.Fields.Item("WhsCode").Value.ToString();
                        //    myPayments.Lines.Quantity = resLine.Fields.Item("Quantity").Value;
                        //    myPayments.Lines.Price = resLine.Fields.Item("Price").Value;

                        //    myPayments.Lines.UserFields.Fields.Item("U_BaseEntry").Value = resLine.Fields.Item("U_IM_DocEntry").Value;
                        //    myPayments.Lines.UserFields.Fields.Item("U_BaseLineNum").Value = resLine.Fields.Item("U_IM_LineId").Value;

                        //    myPayments.Lines.Add();
                        //    resLine.MoveNext();
                        //    #endregion
                        //}
                        int resCode = myPayments.Add();
                        if (resCode == 0)
                        {
                            Logger.Writer(guid, QueueStatus.Open, $"B1应收发票单【{DocEntry}】生成成功，收款单号【{SAP.SAPCompany.GetNewObjectKey()}】。");
                            //UpdateOrder(true, DocEntry, "");
                            successfulCount++;
                        }
                        else
                        {
                            Logger.Writer(guid, QueueStatus.Open, $"B1应收发票单【{DocEntry}】生成收款失败。失败原因：{SAP.SAPCompany.GetLastErrorDescription()}");
                            //UpdateOrder(false, DocEntry, SAP.SAPCompany.GetLastErrorDescription());
                        }
                        System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myPayments);

                    }
                    catch (Exception ex)
                    {
                        Logger.Writer(guid, QueueStatus.Open, ex.Message);
                    }
                    finally
                    {
                        res.MoveNext();
                    }
                }
            }

            catch (Exception ex)
            {
                Logger.Writer(guid, QueueStatus.Close, $"处理B1中的应收发票出现异常：{ex.InnerException}");
            }
            Logger.Writer(guid, QueueStatus.Close, $"[{successfulCount}]条应收发票处理成功。");

        }
    }
}
