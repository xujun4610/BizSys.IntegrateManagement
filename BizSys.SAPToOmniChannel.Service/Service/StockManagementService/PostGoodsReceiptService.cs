using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity.StockManagement.GoodsReceipt;
using BizSys.IntegrateManagement.Entity;
using MagicBox.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.SAPToOmniChannel.Service.Service.StockManagementService
{
    public class PostGoodsReceiptService
    {
        public async static void PostGoodsReceipt()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetGoodsReceipt"], 30);
            string guid = "GoodsReceipt-" + Guid.NewGuid();
            string resultJson = string.Empty;

            #region 查找条件
            Criteria cri = new Criteria()
            {
                __type = "Criteria",
                ResultCount = 1,
                isDbFieldName = false,
                BusinessObjectCode = null,
                Conditions = new List<Conditions>()
                {

                },
                Sorts = new List<Sorts>()
                {
                    new Sorts(){
                         __type="Sort",
                         Alias="ItemCode",
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
            #region 调用查询接口
            try
            {
                resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.GOODSRECEIPT, requestJson);
            }
            catch (ArgumentNullException ex)
            {
                Logger.Writer("单据类型参数不正确，" + ex.Message);
                return;
            }
            catch (HttpRequestException ex)
            {
                Logger.Writer("库存收货服务-网络请求出错，错误信息：" + ex.Message);
                return;
            }
            catch (Exception ex)
            {
                Logger.Writer("库存收货查询出错，错误信息：" + ex.Message);
                return;
            }
            if (string.IsNullOrEmpty(resultJson)) Logger.Writer("库存查询服务出错，查询结果为null。");
            #endregion

            #region 订单处理
            //反序列化
            GoodsReceiptRootObject GoodsReceipt = JsonConvert.DeserializeObject<GoodsReceiptRootObject>(resultJson);
            if (GoodsReceipt.ResultObjects.Count == 0) return;

            Logger.Writer(guid, QueueStatus.Open, "[" + GoodsReceipt.ResultObjects.Count + "]条库存收货开始处理。");
            Logger.Writer(guid, QueueStatus.Close, "库存收货信息：\r\n" + resultJson);
            //生成销售订单
            int mSuccessCount = 0;
            foreach (var item in GoodsReceipt.ResultObjects)
            {
                //item. = "001";
                //item.BusinessPartnerCode = "C0001";
                //item.BusinessPartnerName = "Allen";
                string postJson = JsonConvert.SerializeObject(item);
                try
                {
                    resultJson = await BaseHttpClient.HttpSaveAsync(DocumentType.PURCHASEDELIVERYORDER, postJson);
                }
                catch (ArgumentNullException ex)
                {
                    Logger.Writer("单据类型参数不正确，" + ex.Message);
                    return;
                }
                catch (HttpRequestException ex)
                {
                    Logger.Writer("库存收货服务-网络请求出错，错误信息：" + ex.Message);
                    return;
                }
                catch (Exception ex)
                {
                    Logger.Writer("库存收货保存出错，错误信息：" + ex.Message);
                    return;
                }

            }
            //其他数据用英文输入法下[] 标记区别
            Logger.Writer(guid, QueueStatus.Close, "[" + mSuccessCount + "]条库存收货处理成功。");
            #endregion
        }
    }
}
