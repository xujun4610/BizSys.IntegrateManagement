using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.MaterialsGroup;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.Materials;
using MagicBox.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BizSys.IntegrateManagement.IRepository.Task;
using BizSys.IntegrateManagement.Entity.Task;
using BizSys.IntegrateManagement.Repository.Task;
using BizSys.IntegrateManagement.Repository.MasterDataManagement;
using BizSys.IntegrateManagement.IRepository.MasterDataManagement;

namespace BizSys.SAPToOmniChannel.Service.Service.MasterDataManagement
{
    public class PostMaterialsGroupServcie
    {
        public async static void PostMaterialsGroup()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetMaterialsGroupCount"], 30);
            string guid = "MaterialsGroup-" + Guid.NewGuid();
            string resultJson = string.Empty;
            MaterialsGroupRootObject materialGroups = new MaterialsGroupRootObject();

            #region 查询任务表
            ITaskRep taskRep = new TaskRep();

            //=====================获取任务列表中未同步的物料组信息================================
            TaskRootObjects resultTasks = taskRep.GetDocumentWithNoSync<MaterialsGroupRootObject>(materialGroups, resultCount);

            if (resultTasks.ResultCode != 0)
            {
                Logger.Writer("从Task任务表中查询未同步物料组数据出错，" + resultTasks.Message);
                return;
            }
            if (resultTasks.ResultObjects.Count == 0) return;
            #endregion
            
            #region 遍历Task任务
             Logger.Writer(guid,QueueStatus.Open,"获取未同步物料组信息["+resultTasks.ResultObjects.Count+"]条，正在同步物料组信息...");
		    int mSuccessCount = 0;
            foreach (var item in resultTasks.ResultObjects)
            {
                IMaterialsGroupRep materialsGroupRep = new MaterialsGroupRep();
                //根据Task任务表中提供的物料组
                materialGroups = materialsGroupRep.GetMaterialsGroupByKey(item.UniqueKey);
                if (materialGroups.ResultCode == 0 && materialGroups.ResultObjects.Count == 1)
                {
                    #region 查询物料组
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
                                Alias = "ItemsGroupCode",
                                CondVal = item.UniqueKey,
                                Operation = "co_EQUAL"
                              }
                        },

                        Sorts = new List<Sorts>()
                        {
                            new Sorts(){
                                 __type="Sort",
                                 Alias="ItemsGroupCode",
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
                    #region 调用查询方法
                    try
                    {
                        resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.ITEMGROUP, requestJson);
                    }
                    catch (ArgumentNullException ex)
                    {
                        Logger.Writer("单据类型参数不正确，" + ex.Message);
                        return;
                    }
                    catch (HttpRequestException ex)
                    {
                        Logger.Writer("物料组服务-网络请求出错，错误信息：" + ex.Message);
                        return;
                    }
                    catch (Exception ex)
                    {
                        Logger.Writer("物料组查询出错，错误信息：" + ex.Message);
                        return;
                    }
                    if (string.IsNullOrEmpty(resultJson)) Logger.Writer("物料组主数据查询服务出错，查询结果为null。");
                    #endregion
                    #endregion
                    string postJson = string.Empty;
                    MaterialsGroupRootObject materialsGroupResult = await JsonConvert.DeserializeObjectAsync<MaterialsGroupRootObject>(resultJson);
                    if(materialsGroupResult.ResultCode == 0 && materialsGroupResult.ResultObjects.Count == 1)
                    {
                        
                        var materialGroup = materialGroups.ResultObjects.FirstOrDefault();
                        var materialGroupFrom = materialsGroupResult.ResultObjects.FirstOrDefault();
                        #region 赋值
                        materialGroup.DataSource = materialGroupFrom.DataSource;
                        materialGroup.Deleted = materialGroupFrom.Deleted;
                        materialGroup.isDeleted = materialGroupFrom.isDeleted;
                        materialGroup.ItemsGroupName = materialGroupFrom.ItemsGroupName;
                        materialGroup.Locked = materialGroupFrom.Locked;
                        materialGroup.LogInst = materialGroupFrom.LogInst + 1;
                        materialGroup.Referenced = materialGroupFrom.Referenced;
                        materialGroup.Series = materialGroupFrom.Series;
                        materialGroup.UpdateActionId = materialGroupFrom.UpdateActionId;
                        materialGroup.UpdateDate = materialGroupFrom.UpdateDate;
                        materialGroup.UpdateTime = materialGroupFrom.UpdateTime;
                        materialGroup.UpdateUserSign = materialGroupFrom.UpdateUserSign;
                        
                        #endregion
                        postJson = JsonConvert.SerializeObject(materialGroup);
                    }
                    else
                    {
                        //将仓库对象序列号json字符串
                        var materialGroup = materialGroups.ResultObjects.FirstOrDefault();
                        materialGroup.type = "MATERIALGROUP";
                        materialGroup.isNew = true;
                        materialGroup.ObjectCode = "AVA_MM_OITB";
                        postJson = JsonConvert.SerializeObject(materialGroup);

                    }
                    #region 同步仓库信息
                    
                    #region 调用Save方法更新主数据信息
                    try
                    {
                        Logger.Writer(guid, QueueStatus.Open, $"推送物料组信息:{postJson}");
                        resultJson = await BaseHttpClient.HttpSaveAsync(DocumentType.ITEMGROUP, postJson);
                    }
                    catch (ArgumentNullException ex)
                    {
                        Logger.Writer("单据类型参数不正确，" + ex.Message);
                        return;
                    }
                    catch (HttpRequestException ex)
                    {
                        Logger.Writer("物料组服务-网络请求出错，错误信息：" + ex.Message);
                        return;
                    }
                    catch (Exception ex)
                    {
                        Logger.Writer("物料组保存出错，错误信息：" + ex.Message);
                        return;
                    }
                    #endregion
                    //反序列化Save方法返回的的json字符串
                    MaterialsGroupRootObject materialsGroupSync = JsonConvert.DeserializeObject<MaterialsGroupRootObject>(resultJson);
                    if (materialsGroupSync.ResultCode != 0)
                    {
                        //处理同步失败的信息
                        Logger.Writer(guid, QueueStatus.Open, "任务【" + item.DocEntry + "】，物料组数据【" + item.UniqueKey + "】信息同步失败。");
                    }
                    else
                    {
                        if (taskRep.UpdateDocumentWithSyncSucc(item.DocEntry))
                            mSuccessCount++;
                        else
                            Logger.Writer(guid, QueueStatus.Open, "任务【" + item.DocEntry + "】，物料组数据【" + item.UniqueKey + "】信息更新状态失败。");
                    }
                    #endregion
                }
                else
                {
                    Logger.Writer(guid, QueueStatus.Open, "根据Task任务表中提供物料组数据主键信息查询出错。");
                }
            }
	       

            //其他数据用英文输入法下[]标记区别
            Logger.Writer(guid, QueueStatus.Close, "[" + mSuccessCount + "]条物料组主数据同步成功。");
            #endregion


        
        }
    }
}
