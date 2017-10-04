using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.IntegrateManagement.Entity.StockManagement.InventoryUpdate;
using MagicBox.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.OmniChannelToSAP.Service.Service.StockManagementServcie
{
    public class GetInventoryUpdateService
    {
        /// <summary>
        /// 库存过账
        /// </summary>
        public async static void GetInventoryUpdate()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetInventoryUpdateCount"], 30);
            string guid = "InventoryCounting-" + Guid.NewGuid();
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
                Sorts = new List<Sorts>()
                {
                    new Sorts(){
                         __type="Sort",
                         Alias="DocEntry",
                         SortType="st_Asccending"
                    }
                },
                ChildCriterias = new List<ChildCriterias>()
                {

                },
                NotLoadedChildren = false,
                Remarks = null
            };
            //序列化json对象
            string requestJson = JsonConvert.SerializeObject(cri);
            #endregion
            #region 调用接口
            try
            {
                resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.INVENTORYUPDATE, requestJson);
            }
            catch (Exception ex)
            {
                Logger.Writer("库存过账服务-网络请求出错，错误信息：" + ex.Message);
                return;
            }
            if (string.IsNullOrEmpty(resultJson)) Logger.Writer("库存过账数据查询服务出错，查询结果为null。");
            #endregion

            #region 订单处理

            InventoryUpdateRootObject InventoryCounte = await JsonConvert.DeserializeObjectAsync<InventoryUpdateRootObject>(resultJson);
            if (InventoryCounte.ResultObjects.Count == 0) return;
            DateTime syncDateTime = DateTime.Now;
            Logger.Writer(guid, QueueStatus.Open, "[" + InventoryCounte.ResultObjects.Count + "]条库存过账开始处理。");
            Logger.Writer(guid, QueueStatus.Open, "库存过账信息：\r\n" + resultJson);

            int mSuccessCount = 0;
            foreach (var item in InventoryCounte.ResultObjects)
            {
                try
                {
                    var documentResult = Document.StockManagement.InventoryUpdateOrder.CreateGoodsOrder(item);
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
                    Logger.Writer(guid, QueueStatus.Open, "【" + item.DocEntry + "】库存过账处理发生异常：" + ex.Message);
                }
            }
            Logger.Writer(guid, QueueStatus.Close, "[" + mSuccessCount + "]条库存过账处理成功。");
            #endregion
        }
    }
}
