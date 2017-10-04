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
using BizSys.IntegrateManagement.MasterDataManagement.IRepository;
using BizSys.IntegrateManagement.Repository.MasterDataManagement;
using BizSys.IntegrateManagement.IRepository.Task;
using BizSys.IntegrateManagement.Repository.Task;
using BizSys.IntegrateManagement.Entity.Task;
using BizSys.SAPToOmniChannel.Service.B1Common;

namespace BizSys.SAPToOmniChannel.Service.Service.MasterDataManagement
{
    public class PostMaterialsService
    {
        public async static void PostMaterials()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetMaterialsCount"], 30);
            IMaterialsRep materialsRep = new MaterialsRep();
            string guid = "Materials-" + Guid.NewGuid();
            string resultJson = string.Empty;
            MaterialsRootObject materials = new MaterialsRootObject();

            #region 查询任务表
            ITaskRep taskRep = new TaskRep();
            
            //=====================获取任务列表中未同步的物料信息================================
            TaskRootObjects resultTasks = taskRep.GetDocumentWithNoSync<MaterialsRootObject>(materials, resultCount);

            if (resultTasks.ResultCode != 0)
            {
                Logger.Writer("从Task任务表中查询未同步物料数据出错，" + resultTasks.Message);
                return;
            }
            if (resultTasks.ResultObjects.Count == 0) return;
            #endregion
            
            #region 遍历Task结果
            Logger.Writer(guid, QueueStatus.Open, "获取未同步物料信息[" + resultTasks.ResultObjects.Count + "]条，正在同步物料信息...");
            int mSuccessCount = 0;
            foreach (var item in resultTasks.ResultObjects)
            {
                materials = materialsRep.GetMaterialsByKey(item.UniqueKey);
                
                if (materials.ResultCode == 0 && materials.ResultObjects.Count == 1)
                {
                    #region   获取推送的json数据
                    #region 查询该物料
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
                                Alias = "ItemCode",
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
                        resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.MATERIALS, requestJson);
                    }
                    catch (ArgumentNullException ex)
                    {
                        Logger.Writer("单据类型参数不正确，" + ex.Message);
                        return;
                    }
                    catch (HttpRequestException ex)
                    {
                        Logger.Writer("物料服务-网络请求出错，错误信息：" + ex.Message);
                        return;
                    }
                    catch (Exception ex)
                    {
                        Logger.Writer("物料查询出错，错误信息：" + ex.Message);
                        return;
                    }
                    if (string.IsNullOrEmpty(resultJson)) Logger.Writer("物料主数据查询服务出错，查询结果为null。");
                    #endregion
                    //反序列化查询结果
                    MaterialsRootObject materialsResult = JsonConvert.DeserializeObject<MaterialsRootObject>(resultJson);
                    #endregion
                    string postJson = string.Empty;
                    if (materialsResult.ResultCode == 0 && materialsResult.ResultObjects.Count == 1)
                    {
                        //更新
                        var material = CopyMaterials(materials.ResultObjects.FirstOrDefault(), materialsResult.ResultObjects.FirstOrDefault());
                        postJson = JsonConvert.SerializeObject(material);
                    }
                    else
                    {
                        #region 同步物料主数据-save
                        var material = materials.ResultObjects.FirstOrDefault();
                        material.isNew = true;
                        material.type = "Materials";
                        material.ObjectCode = "AVA_MM_MATERIALS";

                        postJson = JsonConvert.SerializeObject(material);
                        #endregion
                    }

                    #endregion

                    //if (await ServiceCommon<IntegrateManagement.Entity.MasterDataManagement.Materials.ResultObjects,MaterialsRootObject>.PostOrder(postJson, guid, item, DocumentType.MATERIALS))
                    //    mSuccessCount++;
                    #region MyRegion
                    try
                    {
                        Logger.Writer(guid, QueueStatus.Open, $"推送物料信息:{postJson}");
                        resultJson = await BaseHttpClient.HttpSaveAsync(DocumentType.MATERIALS, postJson);
                    }
                    catch (ArgumentNullException ex)
                    {
                        Logger.Writer("单据类型参数不正确，" + ex.Message);
                        return;
                    }
                    catch (HttpRequestException ex)
                    {
                        Logger.Writer("物料服务-网络请求出错，错误信息：" + ex.Message);
                        return;
                    }
                    catch (Exception ex)
                    {
                        Logger.Writer("物料保存出错，错误信息：" + ex.Message);
                        return;
                    }

                    //反序列化Save方法返回的的json字符串
                    MaterialsRootObject MaterialsSync = JsonConvert.DeserializeObject<MaterialsRootObject>(resultJson);
                    if (MaterialsSync.ResultCode != 0)
                    {
                        //处理同步失败的信息
                        Logger.Writer(guid, QueueStatus.Open, "任务【" + item.DocEntry + "】，物料【" + item.UniqueKey + "】信息推送全渠道失败。");

                    }
                    else
                    {
                        if (taskRep.UpdateDocumentWithSyncSucc(item.DocEntry))
                            mSuccessCount++;
                        else
                            Logger.Writer(guid, QueueStatus.Open, "任务【" + item.DocEntry + "】，物料【" + item.UniqueKey + "】信息更新状态失败。");
                    }
                    #endregion

                }
                else
                {
                    Logger.Writer(guid, QueueStatus.Open, "根据Task任务表中提供物料主键信息查询出错。");
                }
            }
            Logger.Writer(guid, QueueStatus.Close, "[" + mSuccessCount + "]条物料同步成功.");
            #endregion

        }

        public static IntegrateManagement.Entity.MasterDataManagement.Materials.ResultObjects CopyMaterials(IntegrateManagement.Entity.MasterDataManagement.Materials.ResultObjects fromMaterial, IntegrateManagement.Entity.MasterDataManagement.Materials.ResultObjects material)
        {
            #region 赋值
            material.Colour = fromMaterial.Colour;
            material.QCTemplateCode = fromMaterial.QCTemplateCode;
            material.Active = fromMaterial.Active;
            material.ActiveFrom = fromMaterial.ActiveFrom;
            material.ActiveTo = fromMaterial.ActiveTo;
            material.ApprovalStatus = fromMaterial.ApprovalStatus;
            material.Assemblied = fromMaterial.Assemblied;
            material.AssemblyItem = fromMaterial.AssemblyItem;
            material.AvgPrice = fromMaterial.AvgPrice;
            material.BarCode = fromMaterial.BarCode;
            material.Brand = fromMaterial.Brand;
            material.BatchNumberManagement = fromMaterial.BatchNumberManagement;
            material.Canceled = fromMaterial.Canceled;
            material.CategoryName = fromMaterial.CategoryName;
            material.CategoryCode = fromMaterial.CategoryCode;
            material.Colour = fromMaterial.Colour;
            material.CreateActionId = fromMaterial.CreateActionId;
            material.CreateDate = fromMaterial.CreateDate;
            material.CreateTime = fromMaterial.CreateTime;
            material.CreateUserSign = fromMaterial.CreateUserSign;
            material.CurrencyCode = fromMaterial.CurrencyCode;
            material.DataOwner = fromMaterial.DataOwner;
            material.DataSource = fromMaterial.DataSource;
            material.DefaultBOMVersion = fromMaterial.DefaultBOMVersion;
            material.DefaultWarehouse = fromMaterial.DefaultWarehouse;
            material.Deleted = fromMaterial.Deleted;
            material.DiscountForUse = fromMaterial.DiscountForUse;
            material.FaceValue = fromMaterial.FaceValue;
            material.FactoryCode = fromMaterial.FactoryCode;
            material.FixedAssets = fromMaterial.FixedAssets;
            material.ForeignDescription = fromMaterial.ForeignDescription;
            material.Height = fromMaterial.Height;
            material.InventoryItem = fromMaterial.InventoryItem;
            material.InventoryUoM = fromMaterial.InventoryUoM;
            material.MinimumInventoryLevel = fromMaterial.MinimumInventoryLevel;
            material.LogInst = material.LogInst + 1;
            material.IsCommited = fromMaterial.IsCommited;
            material.IssueMethod = fromMaterial.IssueMethod;
            material.ItemDescription = fromMaterial.ItemDescription;
            material.ItemGroup = fromMaterial.ItemGroup;
            material.ItemType = fromMaterial.ItemType;
            material.ItemCode = fromMaterial.ItemCode;
            material.Length = fromMaterial.Length;
            material.LeadTime = fromMaterial.LeadTime;
            material.MinimumOrderQuantity = fromMaterial.MinimumOrderQuantity;
            material.Model = fromMaterial.Model;
            material.NoOfItemsPerPurchaseUnit = fromMaterial.NoOfItemsPerPurchaseUnit;
            material.NoOfItemsPerSalesUnit = fromMaterial.NoOfItemsPerSalesUnit;
            material.OnHand = fromMaterial.OnHand;
            material.OnOrder = fromMaterial.OnOrder;
            material.SaleTax = fromMaterial.SaleTax;
            material.PurchaseTax = fromMaterial.PurchaseTax;
            //material.Organization = fromMaterial.Organization;
            material.PhantomItem = fromMaterial.PhantomItem;
            material.PlanningMethod = fromMaterial.PlanningMethod;
            material.Picture = fromMaterial.Picture;
            material.PreferredVendor = fromMaterial.PreferredVendor;    
            material.ProcurementMethod = fromMaterial.ProcurementMethod;
            material.PurchaseItem = fromMaterial.PurchaseItem;
            material.PurchasingUoM = fromMaterial.PurchasingUoM;
            material.Referenced = fromMaterial.Referenced;
            material.Remarks = fromMaterial.Remarks;
            material.RoutingCode = fromMaterial.RoutingCode;
            material.SalesItem = fromMaterial.SalesItem;
            material.SerialNumberManagement = fromMaterial.SerialNumberManagement;
            material.Series = fromMaterial.Series;
            material.SalesUoM = fromMaterial.SalesUoM;
            material.ServiceCardType = fromMaterial.ServiceCardType;
            material.ServiceNumberManagement = fromMaterial.ServiceNumberManagement;
            material.SerialNumberManagement = fromMaterial.SerialNumberManagement;
            material.TeamMembers = fromMaterial.TeamMembers;
            material.UpdateActionId = fromMaterial.UpdateActionId;
            material.UpdateDate = fromMaterial.UpdateDate;
            material.ValidDays = fromMaterial.ValidDays;
            material.Width = fromMaterial.Width;
            material.UpdateTime = fromMaterial.UpdateTime;
            material.UpdateUserSign = fromMaterial.UpdateUserSign;
            #endregion
            return material;
        }
    }
}
