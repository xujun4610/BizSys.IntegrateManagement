using MagicBox.Log;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.OmniChannelToSAP.Service.B1Common
{
    public static class SAPCompanyPool
    {
        private static object Locked = new object();

        private static string[] CompanySign = ConfigurationManager.AppSettings["B1CompanySigns"].ToUpper().Split(',');


        private static Dictionary<string, SAPbobsCOM.Company> _AllCompany;
        public static Dictionary<string, SAPbobsCOM.Company> AllCompany()
        {
            return _AllCompany;
        }

        private static void Init()
        {

            foreach (var item in CompanySign)
            {
                Add(item);
            }
        }

        public static void Add(string CompanyKey)
        {
            if (!_AllCompany.ContainsKey(CompanyKey))
            {
                try
                {
                    lock (Locked)
                    {
                        SAPbobsCOM.Company company = ConnectB1Company(CompanyKey);
                        _AllCompany.Add(CompanyKey, company);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                if (!_AllCompany[CompanyKey].Connected || _AllCompany[CompanyKey] == null)
                {
                    _AllCompany.Remove(CompanyKey);
                    _AllCompany.Add(CompanyKey, ConnectB1Company(CompanyKey));
                }
            }
        }


        public static void Disconnect(string CompanyKey)
        {
            if (_AllCompany.ContainsKey(CompanyKey))
            {
                try
                {
                    if (_AllCompany[CompanyKey] != null || _AllCompany[CompanyKey].Connected)
                        _AllCompany[CompanyKey].Disconnect();
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(_AllCompany[CompanyKey]);

                }
                catch (Exception ex)
                {
                    System.Runtime.InteropServices.Marshal.FinalReleaseComObject(_AllCompany[CompanyKey]);
                    throw ex;
                }
                finally
                {
                    _AllCompany.Remove(CompanyKey);
                }
            }
            else
            {
                return;
            }
        }

        public static void DisconnectAll()
        {
            if (_AllCompany != null || _AllCompany.Count != 0)
                foreach (var item in CompanySign)
                {
                    try
                    {
                        Disconnect(item);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
        }


        private static SAPbobsCOM.Company ConnectB1Company(string CompanyKey)
        {
            SAPbobsCOM.Company company = null;
            NameValueCollection nvc_AppSetting = ConfigurationManager.GetSection(CompanyKey) as NameValueCollection;

            if (_AllCompany.ContainsKey(CompanyKey) && _AllCompany[CompanyKey].Connected) return _AllCompany[CompanyKey];
            company = new SAPbobsCOM.Company();

            Logger.Writer("开始连接B1账套……");
            company.DbServerType = (SAPbobsCOM.BoDataServerTypes)System.Enum.Parse(typeof(SAPbobsCOM.BoDataServerTypes), nvc_AppSetting["SAPDBServerType"]);
            company.Server = Convert.ToBoolean(nvc_AppSetting["UseHostName"]) ? nvc_AppSetting["HostName"] : nvc_AppSetting["DataSource"];
            company.language = SAPbobsCOM.BoSuppLangs.ln_Chinese;
            company.UseTrusted = Convert.ToBoolean(nvc_AppSetting["UseTrusted"]);
            company.DbUserName = nvc_AppSetting["UserID"];
            company.DbPassword = nvc_AppSetting["Password"];
            company.CompanyDB = nvc_AppSetting["InitialCatalog"];
            company.UserName = nvc_AppSetting["SAPUser"];
            company.Password = nvc_AppSetting["SAPPassword"];
            company.LicenseServer = nvc_AppSetting["SAPLicenseServer"];

            int RntCode = company.Connect();
            if (RntCode != 0)
            {
                string errMsg = string.Format("ErrorCode:[{0}],ErrrMsg:[{1}];", company.GetLastErrorCode(), company.GetLastErrorDescription());
                Logger.Writer(errMsg);
                throw new Exception(errMsg);
            }
            Logger.Writer(string.Format("已连接：{0}[{1}]", company.CompanyName, company.CompanyDB));
            return company;
        }

        public static SAPbobsCOM.Company GetSAPCompany(string CompanyKey)
        {
            if (_AllCompany == null)
            {
                _AllCompany = new Dictionary<string, SAPbobsCOM.Company>();
                Init();
            }else
            {
                Init();
            }
            if (_AllCompany.ContainsKey(CompanyKey))
            {
                return _AllCompany[CompanyKey];
            }
            else
            {
                return null;
            }


        }

    }
}


/*
public class SAPCompany
{
    private _CompanyKey { get; set; }

private SAPbobsCOM.Company _Company;
public SAPbobsCOM.Company Company
{
    get
    {
        if (_Company != null || !_Company.Connected)
        {
            ConnectB1Company();
        }
    }
}
*/


