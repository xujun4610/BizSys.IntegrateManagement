using BizSys.IntegrateManagement.Entity.MasterDataManagement.MaterialsCategory;
using BizSys.IntegrateManagement.IRepository.MasterDataManagement;
using BizSys.IntegrateManagement.Repository.BOneCommon;
using System;
using System.Collections.Generic;

namespace BizSys.IntegrateManagement.Repository.MasterDataManagement
{
    public class MaterialsCategoryRep : IMaterialsCategoryRep
    {
        public Entity.MasterDataManagement.MaterialsCategory.MaterialsCategoryRootObject GetAllMaterialsCategory()
        {
            MaterialsCategoryRootObject materialsCategoryRootObject = new MaterialsCategoryRootObject();
            materialsCategoryRootObject.ResultObjects = new List<ResultObjects>();
            try
            {
                SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string sql = @"select * from AVA_OACT";
                res.DoQuery(sql);
                while (!res.EoF)
                {
                    ResultObjects materialsCategory = new ResultObjects();
                    materialsCategory.CategoryCode = res.Fields.Item("code").Value;
                    materialsCategory.CategoryName = res.Fields.Item("name").Value;
                    materialsCategoryRootObject.ResultObjects.Add(materialsCategory);
                    res.MoveNext();
                }
                materialsCategoryRootObject.ResultCode = 0;
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
            catch (Exception ex)
            {
                materialsCategoryRootObject.ResultCode = -1;
                materialsCategoryRootObject.Message = ex.Message;
            }
            
            return materialsCategoryRootObject;

        
        }

        public Entity.MasterDataManagement.MaterialsCategory.MaterialsCategoryRootObject GetMaterialsCategoryByKey(string CategoryCode)
        {
            
            MaterialsCategoryRootObject materialsCategoryRootObject = new MaterialsCategoryRootObject();
            try
            {
                SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string sql = @"select * from AVA_OACT where code = '{0}'";
                res.DoQuery(string.Format(sql, CategoryCode));
                if (res.RecordCount <= 0)
                {
                    materialsCategoryRootObject.ResultCode = -1;
                    materialsCategoryRootObject.Message = "SAP中不存在该信息。";
                }
                else
                {
                    while(!res.EoF)
                    {
                        ResultObjects materialsCategory = new ResultObjects();
                        materialsCategory.CategoryCode = res.Fields.Item("code").Value;
                        materialsCategory.CategoryName = res.Fields.Item("name").Value;
                        materialsCategoryRootObject.ResultObjects.Add(materialsCategory);
                        res.MoveNext();
                    }
                    materialsCategoryRootObject.ResultCode = 0;
                    materialsCategoryRootObject.Message = "Successful operation.";
                }
            }
            catch(Exception ex)
            {
                materialsCategoryRootObject.ResultCode = -1;
                materialsCategoryRootObject.Message = "Operation failed." + ex.Message;
            }
            return materialsCategoryRootObject;
        
        }
    }
}
