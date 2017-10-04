using BizSys.IntegrateManagement.Common;
using BizSys.OmniChannelToSAP.Service.B1Common;
using MagicBox.Log;
using System;
using System.Configuration;

namespace BizSys.OmniChannelToSAP.Service.Service.SalesManagementServcie
{
    public class GetSalesDeliveryByB1Service
    {
        /// <summary>
        /// 销售交货生成发票
        /// </summary>
        public static void  HandleSalesDeliveryOrder()
        {
            string guid = "SalesOrderB1-" + Guid.NewGuid();
            int successfulCount = 0;
            try
            {
                #region 获取未处理的销售订单
                int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetSalesDeliveryOrderCount"], 30);
                SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                SAPbobsCOM.IRecordset resLine = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                // string sql = $@"select top {resultCount} * from ODLN where U_IsSync = 'N' and U_ResouceType = '13'";
                //查询条件 
                //      1、来源Anywhere订单 提货方式自提 未生成
                //      2、来源全渠道订单 非主仓   未生成
                string sql = $@"SELECT top {resultCount} * from AVA_VIEW_CZ_ODLN";
                res.DoQuery(sql);
                if (res.RecordCount < 1) return;
                #endregion
                Logger.Writer(guid, QueueStatus.Open, $"已获取B1中销售交货单[{res.RecordCount}]");
                SAPbobsCOM.Documents myDocuments;
                while (!res.EoF)
                {
                    try
                    {
                        string resourceType = res.Fields.Item("U_ResouceType").Value;
                        string PickingWay = res.Fields.Item("U_PickingWay").Value;
                        myDocuments = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);

                        #region 主表  
                        int DocEntry = res.Fields.Item("DocEntry").Value;

                        myDocuments.CardCode = res.Fields.Item("CardCode").Value;
                        myDocuments.DocDate = res.Fields.Item("DocDate").Value;
                        myDocuments.DocTotal = res.Fields.Item("DocTotal").Value;
                        myDocuments.TaxDate = res.Fields.Item("TaxDate").Value;
                        myDocuments.DocDueDate = res.Fields.Item("DocDueDate").Value;
                        myDocuments.UserFields.Fields.Item("U_IM_DocEntry").Value = DocEntry.ToString();
                        myDocuments.UserFields.Fields.Item("U_PickingWay").Value = PickingWay;
                        myDocuments.BPL_IDAssignedToInvoice = res.Fields.Item("BPLId").Value;
                        myDocuments.UserFields.Fields.Item("U_ResouceType").Value = res.Fields.Item("U_ResouceType").Value;
                        myDocuments.UserFields.Fields.Item("U_ErrorMsg").Value = "";
                        #endregion
                        resLine.DoQuery($@"select * from DLN1 where DocEntry = {DocEntry}");
                        while (!resLine.EoF)
                        {
                            #region 子表
                            myDocuments.Lines.BaseType = 15;
                            myDocuments.Lines.BaseEntry = resLine.Fields.Item("DocEntry").Value;
                            myDocuments.Lines.BaseLine = resLine.Fields.Item("LineNum").Value;
                            myDocuments.Lines.ItemCode = resLine.Fields.Item("ItemCode").Value.ToString();
                            myDocuments.Lines.WarehouseCode = resLine.Fields.Item("WhsCode").Value.ToString();
                            myDocuments.Lines.Quantity = resLine.Fields.Item("Quantity").Value;
                            myDocuments.Lines.Price = resLine.Fields.Item("Price").Value;
                            myDocuments.Lines.BatchNumbers.BatchNumber = "1";
                            myDocuments.Lines.BatchNumbers.Quantity = myDocuments.Lines.Quantity;
                            myDocuments.Lines.BatchNumbers.Add();

                            myDocuments.Lines.UserFields.Fields.Item("U_BaseEntry").Value = resLine.Fields.Item("U_IM_DocEntry").Value;
                            myDocuments.Lines.UserFields.Fields.Item("U_BaseLineNum").Value = resLine.Fields.Item("U_IM_LineId").Value;
                            
                            myDocuments.Lines.Add();
                            resLine.MoveNext();
                            #endregion
                        }
                        int resCode = myDocuments.Add();
                        if (resCode == 0)
                        {
                            Logger.Writer(guid, QueueStatus.Open, $"B1销售交货单【{DocEntry}】生成成功，应收发票单号【{SAP.SAPCompany.GetNewObjectKey()}】。");
                            UpdateOrder(true, DocEntry, "");
                            successfulCount++;
                        }
                        else
                        {
                            Logger.Writer(guid, QueueStatus.Open, $"B1销售交货单【{DocEntry}】生成发票失败。失败原因：{SAP.SAPCompany.GetLastErrorDescription()}");
                            UpdateOrder(false, DocEntry, SAP.SAPCompany.GetLastErrorDescription());
                        }
                        System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myDocuments);
                        
                    }
                    catch(Exception ex)
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
                Logger.Writer(guid, QueueStatus.Close, $"处理B1中的销售订单出现异常：{ex.InnerException}");
            }
            Logger.Writer(guid, QueueStatus.Close, $"[{successfulCount}]条销售订单处理成功。");


        }

        public static void UpdateOrder(bool IsSucessful, int DocEntry, string message)
        {
            try
            {
                SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string isSync = IsSucessful == true ? "Y" : "N";
                string sql = $"update ODLN set U_IsSync = '{isSync}',U_ErrorMsg='{message}' where DocEntry = '{DocEntry}'";
                res.DoQuery(sql);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }


        }
    }
}
