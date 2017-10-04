using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity;
using BizSys.IntegrateManagement.Entity.PurchaseOrder;
using BizSys.IntegrateManagement.Entity.Result;
using MagicBox.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.OmniChannelToSAP.Service.Service.PurchaseManagementServcie
{
    public  class GetCancelOrClosePurchaseOrderService
    {
        public async static void GetCancelPuchaseOrder()
        {
            #region 获取取消采购订单
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetPurchaseOrderCount"], 30);
            string guid = "PurchaseOrderCancel-" + Guid.NewGuid();
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
                    //Canceled = 'Y' and (U_SBOCallbackDate < UpdateDate or (U_SBOCallbackDate = UpdateDate and U_SBOCallbackTime <= UpdateTime) )
                         new Conditions(){
                         Alias="Canceled",
                         Operation = "co_EQUAL",
                         CondVal = "Y"
                    },
                    new Conditions(){
                         Alias="U_SBOCallbackDate",
                         Operation = "co_LESS_THAN",
                         ComparedAlias = "UpdateDate",
                         Relationship="cr_AND",
                         BracketOpenNum = 1
                    },
                    new Conditions(){
                         Alias="UpdateDate",
                         Operation = "co_EQUAL",
                         ComparedAlias = "U_SBOCallbackDate",
                         Relationship="cr_OR",
                          BracketOpenNum = 1
                    },
                    new Conditions(){
                         Alias="U_SBOCallbackTime",
                         Operation = "co_LESS_EQUAL",
                         ComparedAlias = "UpdateTime",
                         Relationship="cr_AND",
                         BracketCloseNum = 2
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
                resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.PURCHASEORDER, requestJson);
            }
            catch (Exception ex)
            {
                Logger.Writer("采购订单取消服务-网络请求出错，错误信息：" + ex.Message);
                return;
            }
            if (string.IsNullOrEmpty(resultJson)) Logger.Writer("采购订单取消查询服务出错，查询结果为null。");
            #endregion
            #endregion
            #region 订单处理
            //反序列化
            PurchaseOrderRootObject purchaseOrder = await JsonConvert.DeserializeObjectAsync<PurchaseOrderRootObject>(resultJson);
            if (purchaseOrder.ResultObjects.Count == 0) return;
            DateTime syncDateTime = DateTime.Now;
            Logger.Writer(guid, QueueStatus.Open, "[" + purchaseOrder.ResultObjects.Count + "]条取消采购订单开始处理。");
            Logger.Writer(guid, QueueStatus.Open, "订单信息：\r\n" + resultJson);
            //生成采购单据
            int mSuccessCount = 0;
            foreach (var item in purchaseOrder.ResultObjects)
            {
                try
                {
                    var documentResult = Document.PurchaseManagement.CancelOrClosePurchaseOrder.CreateCancelPurchaseOrder(item);
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
                    Logger.Writer(guid, QueueStatus.Open, "【" + item.DocEntry + "】取消采购订单处理发生异常：" + ex.Message);
                }
            }
            Logger.Writer(guid, QueueStatus.Close, "[" + mSuccessCount + "]条取消采购订单处理成功。");
            #endregion
        }

        public async static void GetClosePuchaseOrder()
        {
            #region 获取关闭采购订单

            #endregion
        }
    }
}
