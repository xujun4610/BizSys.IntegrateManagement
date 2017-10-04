using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity;
using BizSys.IntegrateManagement.Entity.CallBack;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.IntegrateManagement.Entity.StockManagement.GoodsIssue;
using MagicBox.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace BizSys.OmniChannelToSAP.Service.Service.StockManagementServcie
{
    public class GetGoodsIssueService
    {
        
        public async static void GetGoodsIssue()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetGoodsIssueCount"], 30);
            string guid = "GoodsIssue-" + Guid.NewGuid();
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
            string requestJson = JsonConvert.SerializeObject(cri);
            #endregion
            #region 调用接口
            try
            {
                resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.GOODSISSUE, requestJson);
            }
            catch (Exception ex)
            {
                Logger.Writer("库存发货服务-网络请求出错，错误信息：" + ex.Message);
                return;
            }
            if (string.IsNullOrEmpty(resultJson)) Logger.Writer("库存发货查询服务出错，查询结果为null。");
            #endregion
            #region 订单处理
            //反序列化
            
            GoodsIssueRootObject goodsIssueOrder = JsonConvert.DeserializeObject<GoodsIssueRootObject>(resultJson);
            if (goodsIssueOrder.ResultObjects.Count == 0) return;
            DateTime syncDateTime = DateTime.Now;
            Logger.Writer(guid, QueueStatus.Open, "[" + goodsIssueOrder.ResultObjects.Count + "]条库存发货开始处理。");
            Logger.Writer(guid, QueueStatus.Open, "订单信息：\r\n" + resultJson);
            //生成库存发货
            int mSuccessCount = 0;
            foreach (var item in goodsIssueOrder.ResultObjects)
            {
                try
                {
                    var documentResult = Document.StockManagement.GoodsIssue.CreateGoodsIssue(item);
                    if (documentResult.ResultValue == ResultType.True)
                    {
                        string callBackJsonString = JsonObject.GetCallBackJsonString(item.ObjectCode, item.DocEntry.ToString(), item.B1DocEntry, syncDateTime);
                        if(await B1Common.ServiceCommon.CallBack(callBackJsonString,guid,item))
                            mSuccessCount++;
                        
                    }
                    Logger.Writer(guid, QueueStatus.Open, documentResult.ResultMessage);
                }
                catch (Exception ex)
                {
                    //单据编号用中文输入法下【】标记
                    Logger.Writer(guid, QueueStatus.Open, "【" + item.DocEntry + "】库存发货处理发生异常：" + ex.Message);
                }
            }
            //其他数据用英文输入法下[]标记区别
            Logger.Writer(guid, QueueStatus.Close, "[" + mSuccessCount + "]条库存发货处理成功。");
            #endregion
        }
        
    }
}
