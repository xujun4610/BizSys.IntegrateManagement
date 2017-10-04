using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.OrganizationDepartments;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.IntegrateManagement.IRepository.MasterDataManagement;
using BizSys.IntegrateManagement.Repository.MasterDataManagement;
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
    public class PostOrganizationService
    {
        
        public async static void PostOrganization()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetOrganizationCount"], 30);
            string guid = "Organization-" + Guid.NewGuid();
            string resultJson = string.Empty;
            string postJson = string.Empty;
            OrganizationDepartmentsRootObject organizationDepartmentsRootObject = new OrganizationDepartmentsRootObject();
            IOrganizationRep organizationRep = new OrganizationRep();

            organizationDepartmentsRootObject = organizationRep.GetAllOrganization();
            if (organizationDepartmentsRootObject.ResultCode == 0 && organizationDepartmentsRootObject.ResultObjects.Count > 0)
            {
                Logger.Writer(guid, QueueStatus.Open, "获取" + organizationDepartmentsRootObject.ResultObjects.Count + "条组织部门信息.");
                foreach (var item in organizationDepartmentsRootObject.ResultObjects)
                {
                    postJson = await JsonConvert.SerializeObjectAsync(item);
                    Logger.Writer(guid, QueueStatus.Open, "获取【" + item.Code + "】组织部门信息：\r\n" + postJson);
                    try
                    {
                        resultJson = await BaseHttpClient.HttpSaveAsync(DocumentType.ORGANIZATION, postJson);
                    }
                    catch (ArgumentNullException ex)
                    {
                        Logger.Writer("单据类型参数不正确，" + ex.Message);
                        return;
                    }
                    catch (HttpRequestException ex)
                    {
                        Logger.Writer("组织部门服务-网络请求出错，错误信息：" + ex.Message);
                        return;
                    }
                    OrganizationDepartmentsRootObject DesOrganizationDepartments = JsonConvert.DeserializeObject<OrganizationDepartmentsRootObject>(resultJson);
                    if(DesOrganizationDepartments.ResultCode != 0)
                    {
                        //推送失败


                        //失败处理
                        Logger.Writer(guid, QueueStatus.Open, "组织部门【" + item.Code + "】推送失败，失败原因：" + DesOrganizationDepartments.Message);
                    }
                    else
                        Logger.Writer(guid, QueueStatus.Open, "组织部门【" + item.Code + "】推送成功." );
                }
                Logger.Writer(guid, QueueStatus.Close, "组织部门信息推送完成");
            }
            else
            {
                Logger.Writer(guid, QueueStatus.Close, "推送组织部门信息失败，无法获取组织部门或获取组织部门信息结果为空。查询返回信息:" + organizationDepartmentsRootObject.Message);
            }
            
        }
        
    }
}
