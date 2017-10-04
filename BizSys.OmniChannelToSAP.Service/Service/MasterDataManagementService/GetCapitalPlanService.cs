using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity.CallBack;
using BizSys.IntegrateManagement.Entity;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.CapitalPlan;
using BizSys.IntegrateManagement.Entity.Result;
using MagicBox.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.OmniChannelToSAP.Service.Service.MasterDataManagementService
{
    public class GetCapitalPlanService
    {
        /// <summary>
        /// 资金计划
        /// </summary>
        public async static void GetCapitalPlan()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetCapitalPlanCount"], 30);
            string guid = "CapitalPlan-" + Guid.NewGuid();
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
                    //new Sorts(){
                    //     __type="Sort",
                    //     Alias="DocEntry",
                    //     SortType="st_Ascending"
                    //}
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
                resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.CAPITALPLAN, requestJson);
            }
            catch (Exception ex)
            {
                Logger.Writer("资金计划服务-网络请求出错，错误信息：" + ex.Message);
                return;
            }
            if (string.IsNullOrEmpty(resultJson)) Logger.Writer("资金计划查询服务出错，查询结果为null。");
            #endregion
            #region 订单处理
            //反序列化
            CapitalPlanRootObject capitalPlan = JsonConvert.DeserializeObject<CapitalPlanRootObject>(resultJson);
            if (capitalPlan.ResultObjects.Count == 0) return;
            DateTime syncDateTime = DateTime.Now;
            Logger.Writer(guid, QueueStatus.Open, "[" + capitalPlan.ResultObjects.Count + "]条资金计划开始处理。");
            Logger.Writer(guid, QueueStatus.Open, "订单信息：\r\n" + resultJson);
            //生成资金计划
            int mSuccessCount = 0;
            foreach (var item in capitalPlan.ResultObjects)
            {
                try
                {
                    var documentResult = Document.MasterDataManagement.CapitalPlan.CreateCapitalPlan(item);
                    if (documentResult.ResultValue == ResultType.True)
                    {
                        string callBackJsonString = JsonObject.GetCallBackJsonString(item.ObjectCode, "ObjectKey", item.ObjectKey, item.ObjectKey, syncDateTime);
                        string callBackResultStr= await BaseHttpClient.HttpCallBackAsync(callBackJsonString);
                        var callBackResult = await JsonConvert.DeserializeObjectAsync<CallBackResult>(callBackResultStr);
                        if (callBackResult.ResultCode == 0)
                            mSuccessCount++;
                    }
                    Logger.Writer(guid, QueueStatus.Open, documentResult.ResultMessage);
                }
                catch (Exception ex)
                {
                    Logger.Writer(guid, QueueStatus.Open, "【" + item.ObjectKey + "】资金计划处理发生异常：" + ex.Message);
                }
            }
            Logger.Writer(guid, QueueStatus.Close, "[" + mSuccessCount + "]条资金计划处理成功。");
            #endregion
        }
        
    }
}
