using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity.CallBack;
using BizSys.IntegrateManagement.Entity;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.Supplier;
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
    public class GetSupplierService
    {
        public async static void GetSupplier()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetSupplierCount"], 30);
            string guid = "Supplier-" + Guid.NewGuid();
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
                    //U_SBOSynchronization is Null or U_SBOSynchronization = '' or ( U_SBOCallbackDate < UpdateDate or (U_SBOCallbackDate = UpdateDate and U_SBOCallbackTime <= UpdateTime))
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
                         Alias="SupplierCode",
                         SortType="st_Descending"
                    }
                },
                //ChildCriterias = new List<ChildCriterias>(){

                //},
                NotLoadedChildren = true,
                Remarks = null
            };
            //序列化json对象
            string requestJson = await JsonConvert.SerializeObjectAsync(cri);
            #endregion
            #region 调用接口
            try
            {
                resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.SUPPLIER, requestJson);
            }
            catch (Exception ex)
            {
                Logger.Writer("供应商服务-网络请求出错，错误信息：" + ex.Message);
                return;
            }
            if (string.IsNullOrEmpty(resultJson)) Logger.Writer("供应商主数据查询服务出错，查询结果为null。");
            #endregion

            #region 订单处理
            //反序列化
            SupplierRootObject supplier = JsonConvert.DeserializeObject<SupplierRootObject>(resultJson);
            if (supplier.ResultObjects.Count == 0) return;
            DateTime syncDateTime = DateTime.Now;
            Logger.Writer(guid, QueueStatus.Open, "[" + supplier.ResultObjects.Count + "]条供应商开始处理。");
            Logger.Writer(guid, QueueStatus.Open, "供应商信息：\r\n" + resultJson);
            //生成销售订单
            int mSuccessCount = 0;
            foreach (var item in supplier.ResultObjects)
            {
                try
                {
                    var documentResult = Document.MasterDataManagement.Supplier.CreateSupplier(item);
                    if (documentResult.ResultValue == ResultType.True)
                    {
                        string callBackJsonString = JsonObject.GetCallBackJsonString(item.ObjectCode, "ObjectKey", item.ObjectKey, item.SupplierCode, syncDateTime);
                        string callBackResultStr = await BaseHttpClient.HttpCallBackAsync(callBackJsonString);
                        var callBackResult = await JsonConvert.DeserializeObjectAsync<CallBackResult>(callBackResultStr);
                        if (callBackResult.ResultCode == 0)
                            mSuccessCount++;
                        else
                            Logger.Writer(guid, QueueStatus.Open, "【" + item.ObjectKey + "】供应商回传错误:" + callBackResult.Message + "\r\n 回传内容为：" + callBackJsonString);
                    } 
                    Logger.Writer(guid, QueueStatus.Open, documentResult.ResultMessage);
                }
                catch (Exception ex)
                {
                    Logger.Writer(guid, QueueStatus.Open, "【" + item.SupplierCode + "】供应商处理发生异常：" + ex.Message);
                }
            }
            Logger.Writer(guid, QueueStatus.Close, "[" + mSuccessCount + "]条供应商处理成功。");
            #endregion
        }
    }
}
