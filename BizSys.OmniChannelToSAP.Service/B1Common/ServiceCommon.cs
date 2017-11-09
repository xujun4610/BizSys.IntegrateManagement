using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity.Base;
using BizSys.IntegrateManagement.Entity.CallBack;
using BizSys.IntegrateManagement.Entity.Task;
using BizSys.IntegrateManagement.Entity.PurchaseOrder;
using MagicBox.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.IntegrateManagement.Entity.Base;
using BizSys.IntegrateManagement.Entity;

namespace BizSys.OmniChannelToSAP.Service.B1Common
{
    public class ServiceCommon
    {
        /// <summary>
        /// callback 异步
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callBackJsonString">回写数据</param>
        /// <param name="guid"></param>
        /// <param name="Order"></param>
        /// <returns></returns>
        public async static Task<bool> CallBack<T>(string callBackJsonString, string guid, T Order) where T : IBaseResultObjects
        {
            bool isCallBackSuccessful = false;

            string callBackResultStr;
            try
            {
                callBackResultStr = await BaseHttpClient.HttpCallBackAsync(callBackJsonString);
            }
            catch (Exception ex)
            {
                Logger.Writer(guid, QueueStatus.Open, $"{guid.Substring(0, guid.IndexOf('-'))}【{Order.DocEntry}】已处理成功，在回调发生错误：{ex.Message}");
                throw ex;
            }
            var callBackResult = await JsonConvert.DeserializeObjectAsync<CallBackResult>(callBackResultStr);
            if (callBackResult.ResultCode == 0)
                isCallBackSuccessful = true;
            else
                // Logger.Writer(guid, QueueStatus.Open, "【" + Order.DocEntry + "】回传错误:" + callBackResult.Message + "\r\n 回传内容为：" + callBackJsonString);
                Logger.Writer(guid, QueueStatus.Open, $"{guid.Substring(0, guid.IndexOf('-'))}【{Order.DocEntry}】回传错误：{callBackResult.Message},回传内容为{callBackJsonString}");
            return isCallBackSuccessful;
        }
        /// <summary>
        /// callback 同步方式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callBackJsonString"></param>
        /// <param name="guid"></param>
        /// <param name="Order"></param>
        /// <returns></returns>
        public static bool CallBackSync<T>(string callBackJsonString, string guid, T Order) where T : IBaseResultObjects
        {
            bool isCallBackSuccessful = false;

            string callBackResultStr;
            try
            {
                callBackResultStr = BaseHttpClient.HttpCallBack(callBackJsonString);
            }
            catch (Exception ex)
            {
                Logger.Writer(guid, QueueStatus.Close, $"{guid.Substring(0, guid.IndexOf('-'))}【{Order.DocEntry}】已处理成功，在回调发生错误：{ex.Message}");
                throw ex;
            }
            var callBackResult = JsonConvert.DeserializeObject<CallBackResult>(callBackResultStr);
            if (callBackResult.ResultCode == 0)
                isCallBackSuccessful = true;
            else
                // Logger.Writer(guid, QueueStatus.Open, "【" + Order.DocEntry + "】回传错误:" + callBackResult.Message + "\r\n 回传内容为：" + callBackJsonString);
                Logger.Writer(guid, QueueStatus.Close, $"{guid.Substring(0, guid.IndexOf('-'))}【{Order.DocEntry}】回传错误：{callBackResult.Message},回传内容为{callBackJsonString}");
            return isCallBackSuccessful;
        }

        /// <summary>
        /// 根据userfileds集合的name找到对应的value值
        /// </summary>
        /// <param name="userfieldName"></param>
        /// <param name="userfieldsList"></param>
        public static string GetValueByNameInUserfileds(string userfieldName, List<UserFields> userfieldsList)
        {
            string userfieldValue = default(string);
            foreach (var item in userfieldsList)
            {
                if (!string.IsNullOrEmpty(item.Name) && item.Name == "U_ServiceNo")
                    userfieldValue = item.Name;
                break;
            }
            return userfieldValue;

        }


        /*
        public static Criteria CommonCris(BizObjectStructureType bizType)
        {
            switch (bizType)
            {
                case BizObjectStructureType.MASTERDATA:
                    return new Criteria()
                    {
                        __type = "Criteria",
                        ResultCount = resultCount,
                        isDbFieldName = false,
                        BusinessObjectCode = null,
                        Conditions = new List<Conditions>()
                        {
                            new Conditions {
                                Alias = "ApprovalStatus",
                                Operation = "co_EQUAL",
                                Relationship = "",
                                CondVal = "U",
                                BracketOpenNum = 1
                            },
                            new Conditions {
                                Alias = "ApprovalStatus",
                                Operation = "co_EQUAL",
                                Relationship = "cr_OR",
                                CondVal = "A",
                                BracketCloseNum = 1
                            },
                            new Conditions {
                                Alias = "DocumentStatus",
                                Operation = "co_EQUAL",
                                Relationship = "cr_AND",
                                CondVal = "R"
                            },
                            new Conditions {
                                Alias = "U_SBOSynchronization",
                                Operation = "co_IS_NULL",
                                BracketOpenNum = 1,
                                Relationship = "cr_AND"
                            },
                            new Conditions {
                                Alias = "U_SBOSynchronization",
                                CondVal = "",
                                Operation = "co_EQUAL",
                                Relationship = "cr_OR",
                                BracketCloseNum = 1
                            }
                        }
                    break;
                case BizObjectStructureType.SIMPLEOBJECT:
                    break;
                case BizObjectStructureType.DOCUMENT:
                    break;
                case BizObjectStructureType.OTHER:
                    return new Criteria();
                    break;
                default:
                    return new Criteria();
                    break;
            }
        }
        */
    }
}
