using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.MaterialsCategory;
using BizSys.IntegrateManagement.Entity.Task;
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
    public class PostMaterialsCategoryService
    {
        /// <summary>
        /// 推送品类
        /// </summary>
        public async static void PostMaterialsCategory()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetMaterialsCategoryCount"], 30);
            string guid = "MaterialsCategory-" + Guid.NewGuid();
            string resultJson = string.Empty;
            MaterialsCategoryRootObject materialCategory = new MaterialsCategoryRootObject();
            IMaterialsCategoryRep MaterialsCategoryRep = new MaterialsCategoryRep();
            #region 查询任务表
            ITaskRep taskRep = new TaskRep();

            //=====================获取任务列表中未同步的品类信息================================
            TaskRootObjects resultTasks = taskRep.GetDocumentWithNoSync<MaterialsCategoryRootObject>(materialCategory, resultCount);

            if (resultTasks.ResultCode != 0)
            {
                Logger.Writer("从Task任务表中查询未同步品类数据出错，" + resultTasks.Message);
                return;
            }
            if (resultTasks.ResultObjects.Count == 0) return;
            #endregion


            #region 遍历Task结果
            Logger.Writer(guid, QueueStatus.Open, "获取未同步品类信息[" + resultTasks.ResultObjects.Count + "]条，正在同步品类信息...");
            int mSuccessCount = 0;
            foreach (var item in resultTasks.ResultObjects)
            {
                materialCategory = MaterialsCategoryRep.GetMaterialsCategoryByKey(item.UniqueKey);
                if (materialCategory.ResultCode == 0 && materialCategory.ResultObjects.Count == 1)
                {
                    #region 查询该品类
                    #region 查找条件
                    Criteria cri = new Criteria()
                    {
                        __type = "Criteria",
                        ResultCount = 1,
                        isDbFieldName = false,
                        BusinessObjectCode = null,
                        Conditions = new List<Conditions>()
                        {
                            new Conditions(){
                                Alias = "ObjectKey",
                                CondVal = item.UniqueKey,
                                Operation = "co_EQUAL"
                              }
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
                        resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.MATERIALSCATEGORY, requestJson);
                    }
                    catch (ArgumentNullException ex)
                    {
                        Logger.Writer("单据类型参数不正确，" + ex.Message);
                        return;
                    }
                    catch (HttpRequestException ex)
                    {
                        Logger.Writer("品类服务-网络请求出错，错误信息：" + ex.Message);
                        return;
                    }
                    catch (Exception ex)
                    {
                        Logger.Writer("品类查询出错，错误信息：" + ex.Message);
                        return;
                    }
                    if (string.IsNullOrEmpty(resultJson)) Logger.Writer("品类主数据查询服务出错，查询结果为null。");
                    #endregion
                    //反序列化查询结果
                    MaterialsCategoryRootObject MaterialsCategoryResult = JsonConvert.DeserializeObject<MaterialsCategoryRootObject>(resultJson);
                    #endregion
                    if (MaterialsCategoryResult.ResultCode == 0 && MaterialsCategoryResult.ResultObjects.Count == 1)
                    {
                        //更新
                    }
                    else
                    {
                        #region 同步品类主数据-save
                        string postJson = JsonConvert.SerializeObject(materialCategory.ResultObjects);
                        try
                        {
                            resultJson = await BaseHttpClient.HttpSaveAsync(DocumentType.MATERIALSCATEGORY, postJson);
                        }
                        catch (ArgumentNullException ex)
                        {
                            Logger.Writer("单据类型参数不正确，" + ex.Message);
                            return;
                        }
                        catch (HttpRequestException ex)
                        {
                            Logger.Writer("品类服务-网络请求出错，错误信息：" + ex.Message);
                            return;
                        }
                        catch (Exception ex)
                        {
                            Logger.Writer("品类保存出错，错误信息：" + ex.Message);
                            return;
                        }

                        //反序列化Save方法返回的的json字符串
                        MaterialsCategoryRootObject MaterialsCategorySync = JsonConvert.DeserializeObject<MaterialsCategoryRootObject>(resultJson);
                        if (MaterialsCategorySync.ResultCode != 0)
                        {
                            //处理同步失败的信息

                        }
                        else
                        {
                            if (taskRep.UpdateDocumentWithSyncSucc(item.DocEntry))
                                mSuccessCount++;
                            else
                                Logger.Writer(guid, QueueStatus.Open, "任务【" + item.DocEntry + "】，品类【" + item.UniqueKey + "】信息更新状态失败。");
                        }
                        #endregion
                    }

                }
                else
                {
                    Logger.Writer(guid, QueueStatus.Open, "根据Task任务表中提供品类主键信息查询出错。");
                }
            }
            #endregion
        }
    }
}
