using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity;
using BizSys.IntegrateManagement.Entity.CallBack;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.Materials;
using BizSys.IntegrateManagement.Entity.Result;
using MagicBox.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.OmniChannelToSAP.Service.Service.MasterDataManagementService
{
    public class GetMatarialService
    {
        public async static void GetMaterial(string ItemCode = null)
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["ResultCount"], 30);
            string guid = "Material-" + Guid.NewGuid();
            string resultJson = string.Empty;
            #region 查找条件
            Criteria cri = null;
            if (string.IsNullOrWhiteSpace(ItemCode))
            {
                cri = new Criteria()
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
                 new Conditions(){
                     Alias="U_SBOCallbackDate",
                     Operation = "co_LESS_THAN",
                     ComparedAlias = "UpdateDate",
                     Relationship="cr_OR",
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
                     Alias="ItemCode",
                     SortType="st_Descending"
                }
            },
                    ChildCriterias = new List<ChildCriterias>()
                    {

                    },
                    NotLoadedChildren = false,
                    Remarks = null
                };
            }
            else
            {
                cri = new Criteria()
                {
                    __type = "Criteria",
                    ResultCount = resultCount,
                    isDbFieldName = false,
                    BusinessObjectCode = null,
                    Conditions = new List<Conditions>()
                    {
                        new Conditions()
                        {
                            Alias = "ItemCode",
                            CondVal = ItemCode,
                            Operation = "co_EQUAL",
                            Relationship = "cr_AND"
                        }
                    },
                    ChildCriterias = new List<ChildCriterias>()
                    {

                    }
                };
            }

            //序列化json对象
            string requestJson = JsonConvert.SerializeObject(cri);
            #endregion
            #region 调用接口
            try
            {
                resultJson = BaseHttpClient.HttpFetch(DocumentType.MATERIALS, requestJson);
            }
            catch (Exception ex)
            {
                Logger.Writer("物料主数据服务-网络请求出错，错误信息：" + ex.Message);
                return;
            }
            if (string.IsNullOrEmpty(resultJson)) Logger.Writer("物料主数据查询服务出错，查询结果为null。");
            #endregion
            #region 物料处理
            MaterialsRootObject DesOrderName = JsonConvert.DeserializeObject<MaterialsRootObject>(resultJson);
            if (DesOrderName.ResultObjects.Count == 0) return;
            DateTime syncDateTime = DateTime.Now;
            Logger.Writer(guid, QueueStatus.Open, "[" + DesOrderName.ResultObjects.Count + "]条物料主数据开始处理。");
            Logger.Writer(guid, QueueStatus.Open, "物料信息：\r\n" + resultJson);
            int mSuccessCount = 0;
            foreach (var item in DesOrderName.ResultObjects)
            {
                try
                {
                    var documentResult = Document.MasterDataManagement.Material.CreateMaterial(item);
                    if (documentResult.ResultValue == ResultType.True)
                    {
                        string callBackJsonString = JsonObject.GetCallBackJsonString(item.ObjectCode, "ObjectKey", item.ObjectKey, item.ObjectKey, syncDateTime);
                        string callBackResultStr = await BaseHttpClient.HttpCallBackAsync(callBackJsonString);
                        var callBackResult = JsonConvert.DeserializeObject<CallBackResult>(callBackResultStr);
                        if (callBackResult.ResultCode == 0)
                            mSuccessCount++;
                        else
                            Logger.Writer(guid, QueueStatus.Open, "【" + item.ObjectKey + "】物料回传错误:" + callBackResult.Message + "\r\n 回传内容为：" + callBackJsonString);
                    }
                    Logger.Writer(guid, QueueStatus.Open, documentResult.ResultMessage);
                }
                catch (Exception ex)
                {
                    Logger.Writer(guid, QueueStatus.Open, "【" + item.ItemCode + "】物料主数据处理发生异常：" + ex.Message);
                }
            }
            Logger.Writer(guid, QueueStatus.Close, "[" + mSuccessCount + "]条物料主数据处理成功。");
            #endregion
        }
    }
}
