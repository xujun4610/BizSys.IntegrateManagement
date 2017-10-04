using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.Warehouse;
using BizSys.IntegrateManagement.IRepository.MasterDataManagement;
using BizSys.IntegrateManagement.IRepository.Task;
using BizSys.IntegrateManagement.Repository.MasterDataManagement;
using BizSys.IntegrateManagement.Repository.Task;
using MagicBox.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.SAPToOmniChannel.Service.Service.MasterDataManagementService
{
    public class PostWarehouseService
    {
         public async static void  PostWarehouse()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetWarehouseCount"], 30);
            string guid = "Warehouse-" + Guid.NewGuid();
            string resultJson = string.Empty;
            int mSuccessCount = 0;
            WarehouseRootObject warehouseResultObject = new WarehouseRootObject();
            /*-----------------------
              *1、从Tasks表中查询出未同步的仓库信息
              *2、遍历获取到的Tasks集合， 根据每条记录的Uniquekey值  查询仓库表OWHS
              *3、根据查询得到的仓库信息，序列化后推送全渠道
              *4、依据推送返回的结果处理仓库信息，推送失败的将信息主键保存至失败列表
              * ------------------------- */
            #region 查询Task任务表
            ITaskRep taskRep = new TaskRep();
            /*---------------------从Tasks表中查询出未同步的仓库信息----------------------------*/
            var resultWarehouse = taskRep.GetDocumentWithNoSync<WarehouseRootObject>(warehouseResultObject, resultCount);
            if (resultWarehouse.ResultCode != 0)
            {
                Logger.Writer("从Task任务表中查询未同步仓库数据出错，" + resultWarehouse.Message);
                return;
            }
            if (resultWarehouse.ResultObjects.Count == 0) return;
            
            #endregion
            #region 遍历Task信息
             Logger.Writer(guid,QueueStatus.Open,"获取未同步仓库信息["+resultWarehouse.ResultObjects.Count+"]条，正在同步仓库信息...");
		    foreach(var item in resultWarehouse.ResultObjects)
            {
                IWarehouseRep warehouseRep = new WarehouseRep();
                //根据Task任务表中提供的仓库编码 查询OWHS表 获取仓库信息
                warehouseResultObject = warehouseRep.GetWhsByKey(item.UniqueKey);
                if (warehouseResultObject.ResultCode == 0 && warehouseResultObject.ResultObjects.Count == 1)
                {
                    #region 查询仓库
                    #region 查询条件
                    Criteria cri = new Criteria()
                    {
                        __type = "Criteria",
                        ResultCount = 1,
                        isDbFieldName = false,
                        BusinessObjectCode = null,
                        Conditions = new List<Conditions>()
                        {
                            new Conditions(){
                                Alias = "WarehouseCode",
                                CondVal = item.UniqueKey,
                                Operation = "co_EQUAL"
                              }
                        },

                        Sorts = new List<Sorts>()
                        {
                            new Sorts(){
                                 __type="Sort",
                                 Alias="WarehouseCode",
                                 SortType="st_Asccending"
                            }
                        },
                        ChildCriterias = new List<ChildCriterias>()
                        {

                        },
                        NotLoadedChildren = false,
                        Remarks = null
                    };
                    #endregion
                    //序列化json对象
                    string requestJson = await JsonConvert.SerializeObjectAsync(cri);
                    #region 调用查询接口
                    try
                    {
                        resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.WAREHOURSE, requestJson);
                    }
                    catch (ArgumentNullException ex)
                    {
                        Logger.Writer("单据类型参数不正确，" + ex.Message);
                        return;
                    }
                    catch (HttpRequestException ex)
                    {
                        Logger.Writer("仓库服务-网络请求出错，错误信息：" + ex.Message);
                        return;
                    }
                    catch (Exception ex)
                    {
                        Logger.Writer("仓库查询出错，错误信息：" + ex.Message);
                        return;
                    }
                    if (string.IsNullOrEmpty(resultJson)) Logger.Writer("仓库主数据查询服务出错，查询结果为null。");
                    #endregion
                    #endregion
                    string postJson = string.Empty;
                    WarehouseRootObject warehouseResult = await JsonConvert.DeserializeObjectAsync<WarehouseRootObject>(resultJson);
                    if(warehouseResult.ResultCode==0&&warehouseResult.ResultObjects.Count==1)
                    {
                        //更新仓库信息
                        var warehouse = warehouseResult.ResultObjects.FirstOrDefault();
                        var warehouseFrom = warehouseResultObject.ResultObjects.FirstOrDefault();
                        #region 赋值
                        warehouse.Organization = warehouseFrom.Organization;
                        warehouse.LogInst = warehouse.LogInst+1;
                        warehouse.Activated = warehouseFrom.Activated;
                        warehouse.ApprovalStatus = warehouseFrom.ApprovalStatus;
                        warehouse.CreateActionId = warehouseFrom.CreateActionId;
                        warehouse.DataOwner = warehouseFrom.DataOwner;
                        warehouse.DataSource = warehouseFrom.DataSource;
                        warehouse.EstimateWorkDays = warehouseFrom.EstimateWorkDays;
                        warehouse.Referenced = warehouseFrom.Referenced;
                        warehouse.Series = warehouseFrom.Series;
                        warehouse.TeamMembers = warehouseFrom.TeamMembers;
                        warehouse.UpdateActionId = warehouseFrom.UpdateActionId;
                        warehouse.UpdateDate = warehouseFrom.UpdateDate;
                        warehouse.UpdateTime = warehouseFrom.UpdateTime;
                        warehouse.UpdateUserSign = warehouseFrom.UpdateUserSign;
                        warehouse.WarehouseName = warehouseFrom.WarehouseName;
                        warehouse.WhsType = Enum.GetName(typeof(Enumerator.emWhsType),DataConvert.ConvertToIntEx(warehouseFrom.WhsType)) ;
                        warehouse.Workload = warehouseFrom.Workload;
                        #endregion
                        postJson = JsonConvert.SerializeObject(warehouse);
                    }
                    else
                    {
                        //将仓库对象序列号json字符串
                        var warehouse = warehouseResultObject.ResultObjects.FirstOrDefault();
                        warehouse.type = "Warehouse";
                        warehouse.ObjectCode = "AVA_MM_WAREHOUSE";
                        warehouse.isNew = true;
                        postJson = JsonConvert.SerializeObject(warehouse);
                    }
                    #region 同步仓库信息
                    
                    #region 调用Save方法更新主数据信息
                    try
                    {
                        Logger.Writer(guid, QueueStatus.Open, $"推送仓库信息:{postJson}");
                        resultJson = await BaseHttpClient.HttpSaveAsync(DocumentType.WAREHOURSE, postJson);
                    }
                    catch (ArgumentNullException ex)
                    {
                        Logger.Writer("单据类型参数不正确，" + ex.Message);
                        return;
                    }
                    catch (HttpRequestException ex)
                    {
                        Logger.Writer("仓库服务-网络请求出错，错误信息：" + ex.Message);
                        return;
                    }
                    catch (Exception ex)
                    {
                        Logger.Writer("仓库同步出错，错误信息：" + ex.Message);
                        return;
                    }
                    #endregion
                    //反序列化Save方法返回的的json字符串
                    WarehouseRootObject warehouseSync = JsonConvert.DeserializeObject<WarehouseRootObject>(resultJson);
                    if (warehouseSync.ResultCode != 0)
                    {
                        //处理同步失败的信息
                        Logger.Writer(guid, QueueStatus.Open, "任务【" + item.DocEntry + "】，仓库【" + item.UniqueKey + "】信息同步失败，失败原因：" + warehouseSync.Message + "。");

                    }
                    else
                    {
                        if (taskRep.UpdateDocumentWithSyncSucc(item.DocEntry))
                            mSuccessCount++;
                        else
                            Logger.Writer(guid, QueueStatus.Open, "任务【" + item.DocEntry + "】，仓库【" + item.UniqueKey + "】信息更新状态失败。");
                    }
                    #endregion
                }
                else
                {
                    Logger.Writer(guid, QueueStatus.Open, "根据Task任务表中提供仓库主键信息查询出错。");
                }
            }
            
	        #endregion

            //其他数据用英文输入法下[]标记区别
            Logger.Writer(guid, QueueStatus.Close, "[" + mSuccessCount + "]条仓库同步成功。");
           
        }

       
    }
}
