using BizSys.IntegrateManagement.Common;
using BizSys.IntegrateManagement.Entity;
using BizSys.IntegrateManagement.Entity.StockManagement.MaterialsInventory;
using BizSys.IntegrateManagement.IRepository.StockManagementService;
using BizSys.IntegrateManagement.Repository.StockManagementService;
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
    public class PostMaterialsInventoryService
    {
        public async static void PostMaterialsInventory()
        {
            int resultCount = DataConvert.ConvertToIntEx(ConfigurationManager.AppSettings["GetMaterialsInventory"], 30);
            string guid = "MaterialsInventory-" + Guid.NewGuid();
            string resultJson = string.Empty;
            string requestJson = string.Empty;
            MaterialsInventoryRootObject materialsInventoryRootObject = new MaterialsInventoryRootObject();
            IMaterialsInventoryRep materialsInventoryRep = new MaterialsInventoryRep();
            materialsInventoryRootObject = materialsInventoryRep.GetAllMaterialsInventory();
            if (materialsInventoryRootObject.ResultCode == 0 && materialsInventoryRootObject.ResultObjects.Count != 0)
            {
                Logger.Writer(guid, QueueStatus.Open, "获取库存信息" + materialsInventoryRootObject.ResultObjects.Count + "条.\r\n");
                
                foreach(var item in materialsInventoryRootObject.ResultObjects)
                {
                    #region 查询库存
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
                                CondVal = item.WarehouseCode,
                                Operation = "co_EQUAL"
                              },
                            new Conditions(){
                                Alias = "ItemCode",
                                CondVal = item.ItemCode,
                                Operation = "co_EQUAL",
                                 Relationship="cr_AND"
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
                    string questJson = await JsonConvert.SerializeObjectAsync(cri);
                    #region 调用查询接口
                    try
                    {
                        resultJson = await BaseHttpClient.HttpFetchAsync(DocumentType.MATERIALSINVENTORY, questJson);
                    }
                    catch (Exception ex)
                    {
                        Logger.Writer("库存查询出错，错误信息：" + ex.Message);
                        return;
                    }
                    if (string.IsNullOrEmpty(resultJson)) Logger.Writer("库存主数据查询服务出错，查询结果为null。");
                    #endregion
                    #endregion
                    string postJson = string.Empty;
                    MaterialsInventoryRootObject MaterialsInventoryAsync = await JsonConvert.DeserializeObjectAsync<MaterialsInventoryRootObject>(resultJson);
                    if (MaterialsInventoryAsync.ResultCode == 0 && MaterialsInventoryAsync.ResultObjects.Count == 1)
                    {
                        //更新库存信息
                        var materialsInventory = MaterialsInventoryAsync.ResultObjects.FirstOrDefault();
                        #region 赋值
                        materialsInventory.LogInst += 1;
                        materialsInventory.OnHand = item.OnHand;
                        materialsInventory.OnOrder = item.OnOrder;
                        materialsInventory.IsCommited = item.IsCommited;
                        materialsInventory.UpdateDate = item.UpdateDate;
                        materialsInventory.UpdateUserSign = item.UpdateUserSign;

                        #endregion
                        postJson = JsonConvert.SerializeObject(materialsInventory);
                    }
                    else
                    {
                        item.type = "MaterialsInventory";
                        item.isNew = true;
                        item.ObjectCode = "AVA_MM_INVENTORY";
                        item.LogInst += 1;
                        item.isDirty = true;
                        postJson = await JsonConvert.SerializeObjectAsync(item);

                    }
                    Logger.Writer(guid, QueueStatus.Open, "推送库存信息：" + postJson);
                    try
                    {
                        resultJson = await BaseHttpClient.HttpSaveAsync(DocumentType.MATERIALSINVENTORY, postJson);
                    }
                    catch (Exception ex)
                    {
                        Logger.Writer("库存信息保存出错，错误信息：" + ex.Message);
                        return;
                    }
                   

                }
                Logger.Writer(guid, QueueStatus.Close, "推送库存信息完成。");
            }
            else
            {
                Logger.Writer(guid, QueueStatus.Close, "推送库存失败，库存信息为空，或在获取库存时发生错误。获取库存返回消息:" + materialsInventoryRootObject.Message);
            }
          
        }
    }
}
