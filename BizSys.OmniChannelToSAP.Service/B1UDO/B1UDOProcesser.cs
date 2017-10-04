using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizSys.OmniChannelToSAP.Service.B1UDO
{
    internal abstract class B1UDOProcesser
    {
        protected IB1MainUDO myCurMainUDO;
        protected SAPbobsCOM.Company myCompany;

        internal B1UDOProcesser(SAPbobsCOM.Company SBOCompany, IB1MainUDO b1MainUDO)
        {
            if (SBOCompany == null) throw new Exception("This Company instance is invalid!");
            else myCompany = SBOCompany;
            if (b1MainUDO == null) throw new Exception("This B1MainUDO instance is invalid!");
            else myCurMainUDO = b1MainUDO;
            _LastErrorMsg = string.Empty;
        }

        protected string _LastErrorMsg;
        internal string GetLastError()
        {
            return _LastErrorMsg;
        }

        internal void Process()
        {
            try
            {
                if (myCurMainUDO.OperateType == BoEnumerator.emOperateType.Add) Add();
                else Update();
                _LastErrorMsg = string.Empty;
            }
            catch (Exception ex) { _LastErrorMsg = ex.Message.ToString(); }
        }

        protected virtual void Add()
        {
            SAPbobsCOM.CompanyService oCmpService = myCompany.GetCompanyService();
            SAPbobsCOM.GeneralService oGenService = default(SAPbobsCOM.GeneralService);
            SAPbobsCOM.GeneralData oGenData = default(SAPbobsCOM.GeneralData);
            SAPbobsCOM.GeneralDataParams oGenDataParams = default(SAPbobsCOM.GeneralDataParams);
            SAPbobsCOM.GeneralDataCollection oChildren = default(SAPbobsCOM.GeneralDataCollection);
            SAPbobsCOM.GeneralData oChild = default(SAPbobsCOM.GeneralData);

            try
            {
                oGenService = oCmpService.GetGeneralService(myCurMainUDO.ObjectCode);
                oGenData = oGenService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);
                oGenDataParams = oGenService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams);
                IList<string> HasChildTableNames = new List<string>();
                foreach (var curItem in myCurMainUDO.Fields)
                {
                    oGenData.SetProperty(curItem.FieldName, curItem.FieldValue);
                }
                foreach (var curChild in myCurMainUDO.ChildTables)
                {
                    oChildren = oGenData.Child(curChild.TableName);
                    oChild = oChildren.Add();
                    foreach (var curFielValues in curChild.Fields)
                    {
                        oChild.SetProperty(curFielValues.FieldName, curFielValues.FieldValue);
                    }
                }
                oGenService.Add(oGenData);
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                if (oChild != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oChild);
                if (oChildren != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oChildren);
                if (oGenDataParams != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oGenDataParams);
                if (oGenData != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oGenData);
                if (oGenService != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oGenService);
                if (oCmpService != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oCmpService);
            }
        }

        protected virtual void Update()
        {
            SAPbobsCOM.CompanyService oCmpService = myCompany.GetCompanyService();
            SAPbobsCOM.GeneralService oGenService = default(SAPbobsCOM.GeneralService);
            SAPbobsCOM.GeneralData oGenData = default(SAPbobsCOM.GeneralData);
            SAPbobsCOM.GeneralDataParams oGenDataParams = default(SAPbobsCOM.GeneralDataParams);
            SAPbobsCOM.GeneralDataCollection oChildren = default(SAPbobsCOM.GeneralDataCollection);
            SAPbobsCOM.GeneralData oChild = default(SAPbobsCOM.GeneralData);

            try
            {
                oGenService = oCmpService.GetGeneralService(myCurMainUDO.ObjectCode);
                oGenData = oGenService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);
                oGenDataParams = oGenService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams);
                oGenDataParams.SetProperty(myCurMainUDO.KeyField, myCurMainUDO.KeyValue);
                oGenData = oGenService.GetByParams(oGenDataParams);
                foreach (var curItem in myCurMainUDO.Fields)
                {
                    oGenData.SetProperty(curItem.FieldName, curItem.FieldValue);
                }
                foreach (var curChild in myCurMainUDO.ChildTables)
                {
                    oChildren = oGenData.Child(curChild.TableName);
                    bool IsExists = false;
                    if (oChildren.Count > 0)
                    {
                        for (int iRowCounter = 0; iRowCounter < oChildren.Count; iRowCounter++)
                        {
                            oChild = oChildren.Item(iRowCounter);
                            if (curChild.KeyValue.Equals(oChild.GetProperty(curChild.KeyField)))
                            {
                                IsExists = true;
                                foreach (var curFielValues in curChild.Fields)
                                {
                                    oChild.SetProperty(curFielValues.FieldName, curFielValues.FieldValue);
                                }
                            }
                        }
                    }
                    if (!IsExists)
                    {
                        oChild = oChildren.Add();
                        foreach (var curFielValues in curChild.Fields)
                        {
                            oChild.SetProperty(curFielValues.FieldName, curFielValues.FieldValue);
                        }
                    }
                }
                oGenService.Update(oGenData);
            }
            catch (Exception ex) { throw ex; }
            finally
            {
                if (oChild != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oChild);
                if (oChildren != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oChildren);
                if (oGenDataParams != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oGenDataParams);
                if (oGenData != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oGenData);
                if (oGenService != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oGenService);
                if (oCmpService != null)
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(oCmpService);
            }
        }
    }
}
