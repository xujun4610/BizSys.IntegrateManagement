using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.IntegrateManagement.Entity.SalesOrder;
using MagicBox.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.OmniChannelToSAP.Service.Service.SalesManagementServcie
{
    public class GetCancelOrCloseSalesOrderService
    {
        /// <summary>
        /// 获取取消销售订单
        /// </summary>
        public async static void GetCancelSalesOrder()
        {
            #region 查询取消销售订单
            //获取条件： 更新时间大于回传时间 订单状态为取消
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetSalesOrderCount"], 30);
            string guid = "SalesOrderCancel-" + Guid.NewGuid();
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
                     //DataSource = '' and Canceled= 'Y' and (U_SBOCallbackDate < UpdateDate or (UpdateDate = U_SBOCallbackDate and U_SBOCallbackTime < UpdateTime))
                      new Conditions()
                     {
                        Alias="DataSource",
                        CondVal="",
                        Operation = "co_EQUAL"
                     },
                        new Conditions(){
                         Alias="Canceled",
                         Operation = "co_EQUAL",
                         CondVal = "Y",
                        Relationship = "cr_AND"
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
            string requestJson = await JsonConvert.SerializeObjectAsync(cri);
            #endregion
            #region 调用接口
            try
            {
                resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.SALESORDER, requestJson);
            }
            catch (Exception ex)
            {
                Logger.Writer("取消销售订单服务-网络请求出错，错误信息：" + ex.Message);
                return;
            }
            if (string.IsNullOrEmpty(resultJson)) Logger.Writer("取消销售订单查询服务出错，查询结果为null。");
            #endregion

            #endregion

            #region 生成取消订单
            //反序列化
            SalesOrderRootObject salesOrder = await JsonConvert.DeserializeObjectAsync<SalesOrderRootObject>(resultJson);
            if (salesOrder.ResultObjects.Count == 0) return;
            DateTime syncDateTime = DateTime.Now;
            Logger.Writer(guid, QueueStatus.Open, "[" + salesOrder.ResultObjects.Count + "]条取消销售订单开始处理。");
            Logger.Writer(guid, QueueStatus.Open, "订单信息：\r\n" + resultJson);
            //生成销售订单
            int mSuccessCount = 0;
            foreach (var item in salesOrder.ResultObjects)
            {
                try
                {
                    var documentResult = Document.SalesManagement.CancelOrCloseSalesOrder.CreateCancelSalesOrder(item);
                    if (documentResult.ResultValue == ResultType.True)
                    {
                        var callBackJsonString = JsonObject.GetCallBackJsonString(item.ObjectCode, item.DocEntry.ToString(), item.B1DocEntry, syncDateTime);
                        if (await B1Common.ServiceCommon.CallBack(callBackJsonString, guid, item))
                            mSuccessCount++;
                    }
                    Logger.Writer(guid, QueueStatus.Open, documentResult.ResultMessage);
                }
                catch (Exception ex)
                {
                    Logger.Writer(guid, QueueStatus.Open, "【" + item.DocEntry + "】取消销售订单处理发生异常：" + ex.Message);
                }
            }
            Logger.Writer(guid, QueueStatus.Close, "[" + mSuccessCount + "]条取消销售订单处理成功。");
            #endregion




        }

        /// <summary>
        /// 获取关闭销售订单
        /// </summary>
        public async static void GetCloseSalesOrder()
        {
            #region 获取关闭销售订单
            //获取条件： 更新时间大于回传时间 订单状态为取消



            #endregion

            #region 生成关闭订单
            //

            #endregion

        }

    }
}
