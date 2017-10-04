using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.Employee;
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
using BizSys.IntegrateManagement.IRepository.Task;
using BizSys.IntegrateManagement.Repository.Task;
using BizSys.IntegrateManagement.Entity.Task;
using BizSys.IntegrateManagement.IRepository.MasterDataManagement;
using BizSys.IntegrateManagement.Repository.MasterDataManagement;

namespace BizSys.SAPToOmniChannel.Service.Service.MasterDataManagementService
{
    public class PostEmployeeService
    {
        /// <summary>
        /// 推送员工主数据
        /// </summary>
        public async static void PostEmployee()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetEmployeeCount"], 30);
            string guid = "GetEmployeeCount-" + Guid.NewGuid();
            string resultJson = string.Empty;
            EmployeeRootObject Employee = new EmployeeRootObject();
            IEmployeeRep employeeRep = new EmployeeRep();

            #region 查询任务表
            ITaskRep taskRep = new TaskRep();

            //=====================获取任务列表中未同步的员工信息================================
            TaskRootObjects resultTasks = taskRep.GetDocumentWithNoSync<EmployeeRootObject>(Employee, resultCount);

            if (resultTasks.ResultCode != 0)
            {
                Logger.Writer("从Task任务表中查询未同步员工数据出错，" + resultTasks.Message);
                return;
            }
            if (resultTasks.ResultObjects.Count == 0) return;
            #endregion

            #region 遍历Task结果
            Logger.Writer(guid, QueueStatus.Open, "获取未同步员工信息[" + resultTasks.ResultObjects.Count + "]条，正在同步员工信息...");
            int mSuccessCount = 0;
            foreach (var item in resultTasks.ResultObjects)
            {
                Employee = employeeRep.GetEmployeeByKey(item.UniqueKey);
                if (Employee.ResultCode == 0 && Employee.ResultObjects.Count == 1)
                {
                    #region 查询该员工
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
                        resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.EMPLOYEE, requestJson);
                    }
                    catch (ArgumentNullException ex)
                    {
                        Logger.Writer("单据类型参数不正确，" + ex.Message);
                        return;
                    }
                    catch (HttpRequestException ex)
                    {
                        Logger.Writer("员工服务-网络请求出错，错误信息：" + ex.Message);
                        return;
                    }
                    catch (Exception ex)
                    {
                        Logger.Writer("员工查询出错，错误信息：" + ex.Message);
                        return;
                    }
                    if (string.IsNullOrEmpty(resultJson)) Logger.Writer("员工主数据查询服务出错，查询结果为null。");
                    #endregion
                    //反序列化查询结果
                    EmployeeRootObject EmployeeResult = JsonConvert.DeserializeObject<EmployeeRootObject>(resultJson);
                    #endregion
                    if (EmployeeResult.ResultCode == 0 && EmployeeResult.ResultObjects.Count == 1)
                    {
                        //更新
                    }
                    else
                    {
                        #region 同步员工主数据-save
                        string postJson = JsonConvert.SerializeObject(Employee.ResultObjects);
                        try
                        {
                            resultJson = await BaseHttpClient.HttpSaveAsync(DocumentType.EMPLOYEE, postJson);
                        }
                        catch (ArgumentNullException ex)
                        {
                            Logger.Writer("单据类型参数不正确，" + ex.Message);
                            return;
                        }
                        catch (HttpRequestException ex)
                        {
                            Logger.Writer("员工服务-网络请求出错，错误信息：" + ex.Message);
                            return;
                        }
                        catch (Exception ex)
                        {
                            Logger.Writer("员工保存出错，错误信息：" + ex.Message);
                            return;
                        }

                        //反序列化Save方法返回的的json字符串
                        EmployeeRootObject EmployeeSync = JsonConvert.DeserializeObject<EmployeeRootObject>(resultJson);
                        if (EmployeeSync.ResultCode != 0)
                        {
                            //处理同步失败的信息

                        }
                        else
                        {
                            if (taskRep.UpdateDocumentWithSyncSucc(item.DocEntry))
                                mSuccessCount++;
                            else
                                Logger.Writer(guid, QueueStatus.Open, "任务【" + item.DocEntry + "】，员工【" + item.UniqueKey + "】信息更新状态失败。");
                        }
                        #endregion
                    }

                }
                else
                {
                    Logger.Writer(guid, QueueStatus.Open, "根据Task任务表中提供员工主键信息查询出错。");
                }
            }
            #endregion
            
        }
    }
}
