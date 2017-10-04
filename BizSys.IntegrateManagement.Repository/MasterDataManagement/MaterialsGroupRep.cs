using BizSys.IntegrateManagement.Entity.MasterDataManagement.MaterialsGroup;
using BizSys.IntegrateManagement.IRepository.MasterDataManagement;
using BizSys.IntegrateManagement.Repository.BOneCommon;
using System;
using System.Collections.Generic;

namespace BizSys.IntegrateManagement.Repository.MasterDataManagement
{
    public class MaterialsGroupRep : IMaterialsGroupRep
    {
        public Entity.MasterDataManagement.MaterialsGroup.MaterialsGroupRootObject GetAllMaterialsGroup()
        {
            MaterialsGroupRootObject materialsGroupRootObject = new MaterialsGroupRootObject();
            materialsGroupRootObject.ResultObjects = new List<ResultObjects>();
            try
            {
                SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string sql = @"select * from OITB";
                res.DoQuery(sql);
                while (!res.EoF)
                {
                    ResultObjects materialGroup = new ResultObjects();
                    materialGroup.ItemsGroupCode = res.Fields.Item("ItmsGrpCod").Value;
                    materialGroup.ItemsGroupName = res.Fields.Item("ItmsGrpNam").Value;
                    materialGroup.LogInst = res.Fields.Item("LogInstanc").Value + 1;


                    materialsGroupRootObject.ResultObjects.Add(materialGroup);
                    res.MoveNext();
                }
                materialsGroupRootObject.ResultCode = 0;
                materialsGroupRootObject.Message = "Successful opration.";
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
            catch (Exception ex)
            {
                materialsGroupRootObject.ResultCode = -1;
                materialsGroupRootObject.Message = ex.Message;
            }
            
            return materialsGroupRootObject;

        }

        public Entity.MasterDataManagement.MaterialsGroup.MaterialsGroupRootObject GetMaterialsGroupByKey(string ItmsGrpCod)
        {
            MaterialsGroupRootObject materialsGroupRootObject = new MaterialsGroupRootObject();
            materialsGroupRootObject.ResultObjects = new List<ResultObjects>();
            try
            {
                SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string sql = @"select * from OITB where ItmsGrpCod = '{0}'";
                res.DoQuery(string.Format(sql, ItmsGrpCod));
                if (res.RecordCount != 1)
                    return new MaterialsGroupRootObject()
                    {
                        ResultCode = -1,
                        Message = "Can't find the warehouse in SAP."
                    };
                materialsGroupRootObject.ResultCode = 0;
                materialsGroupRootObject.Message = "操作成功！";

                ResultObjects materialGroup = new ResultObjects();
                materialGroup.ItemsGroupCode = res.Fields.Item("ItmsGrpCod").Value;
                materialGroup.ItemsGroupName = res.Fields.Item("ItmsGrpNam").Value;
                materialGroup.LogInst = res.Fields.Item("LogInstanc").Value + 1;
                materialsGroupRootObject.ResultObjects.Add(materialGroup);

                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
            catch (Exception ex)
            {
                materialsGroupRootObject.ResultCode = -1;
                materialsGroupRootObject.Message = ex.Message;
            }

            return materialsGroupRootObject;

        }
    }
}
