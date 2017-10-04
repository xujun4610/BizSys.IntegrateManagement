using BizSys.IntegrateManagement.Common;
using BizSys.OmniChannelToSAP.Service.B1Common;
using MagicBox.Log;
using SAPbobsCOM;
using System;
using System.Configuration;

namespace BizSys.OmniChannelToSAP.Service.Service.SalesManagementServcie
{
    public class GetSalesOrderByB1Service
    {
        /// <summary>
        /// 销售订单生成交货 或预付发票
        /// </summary>
        public static void HandleSalesOrder()
        {
            string guid = "SalesOrderB1-" + Guid.NewGuid();
            int successfulCount = 0;
            try
            {
                #region 获取未处理的销售订单
                int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetSalesOrderCount"], 30);
                SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                SAPbobsCOM.IRecordset resLine = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                string sql = $@"select top {resultCount} * from AVA_VIEW_CZ_ORDR";

                res.DoQuery(sql);
                if (res.RecordCount < 1) return;
                #endregion
                Logger.Writer(guid, QueueStatus.Open, $"已获取[{res.RecordCount}]条B1中销售订单");
                SAPbobsCOM.Documents myDocuments;

                while (!res.EoF)
                {
                    try
                    {
                        string resourceType = res.Fields.Item("U_ResouceType").Value;
                        string PickingWay = res.Fields.Item("U_PickingWay").Value;
                        //来源Anywhere 提货方式 送货
                        if (resourceType == "13" && PickingWay == "1")
                        {
                            myDocuments = SAP.SAPCompany.GetBusinessObject(BoObjectTypes.oInvoices);
                            myDocuments.ReserveInvoice = BoYesNoEnum.tYES;//应收预留发票
                        }
                        else
                            myDocuments = SAP.SAPCompany.GetBusinessObject(BoObjectTypes.oDeliveryNotes);


                        /*---------------1 来源Anywhere 需要判断提货方式-----------------------*/
                        /*---------------2 来源全渠道 直接生成销售交货-----------------------*/
                        #region 主表  
                        int DocEntry = res.Fields.Item("DocEntry").Value;

                        myDocuments.CardCode = res.Fields.Item("CardCode").Value;
                        myDocuments.DocDate = res.Fields.Item("DocDate").Value;
                        myDocuments.DocTotal = res.Fields.Item("DocTotal").Value;
                        myDocuments.TaxDate = res.Fields.Item("TaxDate").Value;
                        myDocuments.DocDueDate = res.Fields.Item("DocDueDate").Value;
                        myDocuments.UserFields.Fields.Item("U_IM_DocEntry").Value = DocEntry.ToString();
                        myDocuments.UserFields.Fields.Item("U_PickingWay").Value = PickingWay;
                        myDocuments.UserFields.Fields.Item("U_ResouceType").Value = res.Fields.Item("U_ResouceType").Value;
                        myDocuments.BPL_IDAssignedToInvoice = res.Fields.Item("BPLId").Value;
                        myDocuments.UserFields.Fields.Item("U_ErrorMsg").Value = "";
                        #endregion
                        resLine.DoQuery($@"select * from RDR1 where DocEntry = {DocEntry}");

                        while (!resLine.EoF)
                        {

                            //判断是否有主仓库的行
                            myDocuments.Lines.WarehouseCode = resLine.Fields.Item("WhsCode").Value.ToString();
                            if (!BOneCommon.IsMainStore(myDocuments.Lines.WarehouseCode))
                            {
                                #region 子表
                                myDocuments.Lines.BaseType = 17;
                                myDocuments.Lines.BaseEntry = resLine.Fields.Item("DocEntry").Value;
                                myDocuments.Lines.BaseLine = resLine.Fields.Item("LineNum").Value;
                                myDocuments.Lines.ItemCode = resLine.Fields.Item("ItemCode").Value.ToString();

                                myDocuments.Lines.Quantity = resLine.Fields.Item("Quantity").Value;
                                myDocuments.Lines.Price = resLine.Fields.Item("Price").Value;
                                myDocuments.Lines.UserFields.Fields.Item("U_BaseEntry").Value = resLine.Fields.Item("U_IM_DocEntry").Value;
                                myDocuments.Lines.UserFields.Fields.Item("U_BaseLineNum").Value = resLine.Fields.Item("U_IM_LineId").Value;

                                #region 批次处理
                                //获取该物料该仓库下的所有批次信息 按创建时间排序
                                var ListBatchNumber = B1Common.BOneCommon.GetBatchByItemAndWhsCode(myDocuments.Lines.ItemCode, myDocuments.Lines.WarehouseCode);
                                double batchQuantitySum = 0;//0行~n行的批次总数量
                                double hasDistributedQuantitySum = 0;//已分配的批次数量
                                foreach (var batch in ListBatchNumber)
                                {
                                    myDocuments.Lines.BatchNumbers.BatchNumber = batch.BatchID;
                                    batchQuantitySum += batch.Quantity;
                                    myDocuments.Lines.BatchNumbers.Quantity = (batchQuantitySum - hasDistributedQuantitySum) >= (myDocuments.Lines.Quantity - hasDistributedQuantitySum) ?
                                        (myDocuments.Lines.Quantity - hasDistributedQuantitySum) : (batchQuantitySum - hasDistributedQuantitySum);//该行分配的批次数量为 =当前行批次的总数量-已分配好的数量
                                    hasDistributedQuantitySum += myDocuments.Lines.BatchNumbers.Quantity;
                                    myDocuments.Lines.BatchNumbers.Add();

                                    //该物料的数量<= 当前行的批次总数量，说明批次数量已够分配并已成功分配，跳出循环。
                                    if (myDocuments.Lines.Quantity <= batchQuantitySum)
                                        break;

                                }
                                #endregion
                                myDocuments.Lines.Add();
                                resLine.MoveNext();
                                #endregion
                                int resCode = myDocuments.Add();
                                if (resCode == 0)
                                {
                                    Logger.Writer(guid, QueueStatus.Open, $"B1销售订单【{DocEntry}】生成成功。");
                                    UpdateOrder(true, DocEntry, "");
                                    successfulCount++;
                                }
                                else
                                {
                                    Logger.Writer(guid, QueueStatus.Open, $"B1销售订单【{DocEntry}】生成失败。失败原因：{SAP.SAPCompany.GetLastErrorDescription()}");
                                    UpdateOrder(false, DocEntry, SAP.SAPCompany.GetLastErrorDescription());
                                }
                                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(myDocuments);

                            }
                            else { break; }
                        }

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
                string sql = $"update ORDR set U_IsSync = '{isSync}',U_ErrorMsg='{message}' where DocEntry = '{DocEntry}'";
                res.DoQuery(sql);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }


        }
    }
}
