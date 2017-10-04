using BizSys.IntegrateManagement.Repository.BOneCommon;
using BizSys.IntegrateManagement.Entity.MasterDataManagement.OrganizationDepartments;
using BizSys.IntegrateManagement.IRepository.MasterDataManagement;
using System;
using System.Collections.Generic;

namespace BizSys.IntegrateManagement.Repository.MasterDataManagement
{
    public class OrganizationRep : IOrganizationRep
    {
        public Entity.MasterDataManagement.OrganizationDepartments.OrganizationDepartmentsRootObject GetAllOrganization()
        {
            
            OrganizationDepartmentsRootObject organizationRootObject = new OrganizationDepartmentsRootObject();
            organizationRootObject.ResultObjects = new List<ResultObjects>();
            try
            {
                SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string sql = @"select * from OUDP";
                res.DoQuery(sql);
                while (!res.EoF)
                {
                    ResultObjects organization = new ResultObjects();
                    organization.Code = res.Fields.Item("Code").Value;
                    organization.Name = res.Fields.Item("Name").Value;
                    //organization.Remarks = res.Fields.Item("Series").Value.ToString();

                    organizationRootObject.ResultObjects.Add(organization);
                    res.MoveNext();
                }
                organizationRootObject.ResultCode = 0;
                organizationRootObject.Message = "Successful operation.";
            }
            catch (Exception ex)
            {
                organizationRootObject.ResultCode = -1;
                organizationRootObject.Message = ex.Message;
            }
            return organizationRootObject;

        
        }
    }
}
