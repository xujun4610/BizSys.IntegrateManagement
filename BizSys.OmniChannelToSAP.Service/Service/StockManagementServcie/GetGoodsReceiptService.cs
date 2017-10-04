using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity;
using BizSys.IntegrateManagement.Entity.CallBack;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.IntegrateManagement.Entity.StockManagement.GoodsReceipt;
using MagicBox.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;

namespace BizSys.OmniChannelToSAP.Service.Service.StockManagementServcie
{
    public class GetGoodsReceiptService
    {

        public async static void GetGoodsReceipts()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetGoodsReceiptsCount"], 30);
            string guid = "GoodsReceipts-" + Guid.NewGuid();
            string resultJson = string.Empty;
            #region 查找条件
            Criteria cri = new Criteria()
            {
                __type = "Criteria",
                ResultCount = resultCount,
                isDbFieldName = false,
                BusinessObjectCode = null,
                Conditions = new List<Conditions>()
                {
                     new Conditions()
                     {
                         Alias="U_SBOSynchronization",
                         Operation = "co_IS_NULL",
                         BracketOpenNum = 1
                     },
                     new Conditions()
                     {
                         Alias="U_SBOSynchronization",
                         CondVal="",
                        Operation = "co_EQUAL",
                        Relationship = "cr_OR",
                         BracketCloseNum = 1
                     },
                },
                Sorts = new List<Sorts>(){
                    new Sorts(){
                         __type="Sort",
                         Alias="DocEntry",
                         SortType="st_Ascending"
                    }
                },
                ChildCriterias = new List<ChildCriterias>()
                {

                },
                NotLoadedChildren = false,
                Remarks = null
            };
            //序列化json对象
            string requestJson = await JsonConvert.SerializeObjectAsync(cri);
            #endregion
            #region 调用接口
            try
            {
                resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.GOODSRECEIPT, requestJson);
            }
            catch (Exception ex)
            {
                Logger.Writer("库存收货服务-网络请求出错，错误信息：" + ex.Message);
                return;
            }
            if (string.IsNullOrEmpty(resultJson)) Logger.Writer("库存收货查询服务出错，查询结果为null。");
            #endregion
            #region 订单处理
            //反序列化
            GoodsReceiptRootObject goodsReceipt = JsonConvert.DeserializeObject<GoodsReceiptRootObject>(resultJson);
            if (goodsReceipt.ResultObjects.Count == 0) return;
            DateTime syncDateTime = DateTime.Now;
            Logger.Writer(guid, QueueStatus.Open, "[" + goodsReceipt.ResultObjects.Count + "]条库存收货开始处理。");
            Logger.Writer(guid, QueueStatus.Open, "订单信息：\r\n" + resultJson);
            //生成库存收货
            int mSuccessCount = 0;
            foreach (var item in goodsReceipt.ResultObjects)
            {
                try
                {
                    var documentResult = Document.StockManagement.GoodsReceipt.CreateGoodsReceipt(item);
                    if (documentResult.ResultValue == ResultType.True)
                    {
                        string callBackJsonString = JsonObject.GetCallBackJsonString(item.ObjectCode, item.DocEntry.ToString(), item.B1DocEntry, syncDateTime);
                        if (await B1Common.ServiceCommon.CallBack(callBackJsonString, guid, item))
                            mSuccessCount++;
                    }
                    Logger.Writer(guid, QueueStatus.Open, documentResult.ResultMessage);

                }
                catch (Exception ex)
                {
                    Logger.Writer(guid, QueueStatus.Open, "【" + item.DocEntry + "】库存收货处理发生异常：" + ex.Message);
                }
            }
            Logger.Writer(guid, QueueStatus.Close, "[" + mSuccessCount + "]条库存收货处理成功。");
            #endregion
        }

    }
}
