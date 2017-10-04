using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity;
using MagicBox.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.OmniChannelToSAP.Service.Service.SalesOrderServiceByNiko
{
    public class GetMaterialMasterDatasService
    {
        /*
        public async static void GetMaterialMasterData()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetMaterialMasterData"], 20);
            string guid = "MaterialMasterData-" + Guid.NewGuid();
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
                     new Conditions()
                     {
                         Alias="DocumentStatus",
                        CondVal = "R",
                        Operation = "co_EQUAL",
                         Relationship="cr_AND"
                     }
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
                resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.MATERIALS, requestJson);
            }
            catch (Exception ex)
            {
                Logger.Writer("采购订单服务-网络请求出错，错误信息：" + ex.Message);
                return;
            }
            if (string.IsNullOrEmpty(resultJson)) Logger.Writer("采购订单查询服务出错，查询结果为null。");
            #endregion
            #region 订单处理
            //反序列化
            PurchaseOrderRootObject purchaseOrder = await JsonConvert.DeserializeObjectAsync<PurchaseOrderRootObject>(resultJson);
            if (purchaseOrder.ResultObjects.Count == 0) return;
            DateTime syncDateTime = DateTime.Now;
            Logger.Writer(guid, QueueStatus.Open, "[" + purchaseOrder.ResultObjects.Count + "]条采购订单开始处理。");
            Logger.Writer(guid, QueueStatus.Open, "订单信息：\r\n" + resultJson);
            //生成采购单据
            int mSuccessCount = 0;
            foreach (var item in purchaseOrder.ResultObjects)
            {
                try
                {
                    var documentResult = Document.PurchaseManagement.PurchaseOrder.CreatePurchaseOrder(item);
                    if (documentResult.ResultValue == ResultType.True)
                    {
                        string CBRequestJson = JsonObject.GetCallBackJsonString(22, item.ObjectCode, item.DocEntry.ToString(), item.B1DocEntry, syncDateTime);
                        if (await B1Common.ServiceCommon.CallBack(CBRequestJson, guid, item))
                            mSuccessCount++;
                    }
                    Logger.Writer(guid, QueueStatus.Open, documentResult.ResultMessage);
                }
                catch (Exception ex)
                {
                    Logger.Writer(guid, QueueStatus.Open, "【" + item.DocEntry + "】采购订单处理发生异常：" + ex.Message);
                }
            }
            Logger.Writer(guid, QueueStatus.Close, "[" + mSuccessCount + "]条采购订单处理成功。");
            #endregion
        }
        */
    }
}
