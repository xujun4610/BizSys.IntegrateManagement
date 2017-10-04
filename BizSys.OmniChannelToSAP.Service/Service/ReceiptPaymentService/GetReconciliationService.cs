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

namespace BizSys.OmniChannelToSAP.Service.Service.ReceiptPaymentService
{
    /// <summary>
    /// 往来核销
    /// </summary>
    public class GetReconciliationService
    {
        public async static void GetReconciliation()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetReconciliationCount"], 30);
            string guid = "ReceiptVerification-" + Guid.NewGuid();
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
                    //     //__type="Sort",
                    //     //Alias="DocEntry",
                    //     //SortType="st_Ascending"
                    //}
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
                resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.RECONCILIATION, requestJson);
            }
            catch (Exception ex)
            {
                Logger.Writer("往来核销服务-网络请求出错，错误信息：" + ex.Message);
                return;
            }
            if (string.IsNullOrEmpty(resultJson)) Logger.Writer("往来核销查询服务出错，查询结果为null。");
            #endregion
        }
    }
}
