using BizSys.IntegrateManagement.Entity.BatchNumber;
using BizSys.IntegrateManagement.Entity.Result;
using BizSys.IntegrateManagement.Entity.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.OmniChannelToSAP.Service.B1Common
{
    public class BOneCommon
    {
        /// <summary>
        /// 根据全渠道所有者编码查询B1中所有者编码
        /// </summary>
        /// <param name="OwnerCode"></param>
        /// <returns></returns>
        public static int GetBOneOwnerCode(int OwnerCode)
        {
            int BOneOwnerCode = default(Int32);
            SAPbobsCOM.IRecordset rs = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            string sql = @"select EmpId from OHEM where U_OwnerCode = '{0}'";
            rs.DoQuery(string.Format(sql, OwnerCode));
            if (rs.RecordCount < 1) throw new ArgumentNullException("根据提供数据所有者无法找到B1中对应的员工主数据代码。");
            if (rs.RecordCount > 1) throw new ArgumentException("根据提供的数据所有者在B1找到多条员工主数据代码。");
            while (!rs.EoF)
            {
                BOneOwnerCode = rs.Fields.Item("EmpId").Value;
                rs.MoveNext();
            }
            return BOneOwnerCode;
        }

        /// <summary>
        /// 根据B1中的所有者代码获取对应的全渠道所有者代码
        /// </summary>
        /// <param name="SAPOwnerCode">SAP所有者代码</param>
        /// <returns></returns>
        public static int GetOmniChannelDataOwnerCode(int SAPOwnerCode)
        {
            int OmniChannelDataOwnerCode = default(Int32);
            SAPbobsCOM.IRecordset rs = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            string sql = @"select EmpId from OHEM where EmpID = '{0}'";
            rs.DoQuery(string.Format(sql, SAPOwnerCode));
            if (rs.RecordCount < 1) throw new ArgumentNullException("根据提供数据所有者无法找到对应的全渠道员工主数据代码。");
            if (rs.RecordCount > 1) throw new ArgumentException("根据提供的数据所有者在B1找到多条对应的全渠道员工主数据代码。");
            while (!rs.EoF)
            {
                OmniChannelDataOwnerCode = rs.Fields.Item("U_OwnerCode").Value;
                rs.MoveNext();
            }
            return OmniChannelDataOwnerCode;
        }

        /// <summary>
        /// 根据全渠道销售员工编码查询B1销售员工编码
        /// </summary>
        /// <param name="SlpCode"></param>
        /// <returns></returns>
        public static int GetBOneSlpCode(int SlpCode)
        {
            int BOneSlpCode = default(Int32);
            SAPbobsCOM.IRecordset rs = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                string sql = @"select SlpCode from OSLP where U_SlpCode = '{0}'";
                rs.DoQuery(string.Format(sql, SlpCode));
                if (rs.RecordCount < 1) throw new ArgumentNullException("根据提供数据所有者无法找到B1中对应的销售员工主数据代码。");
                if (rs.RecordCount > 1) throw new ArgumentException("根据提供的数据所有者在B1找到多条销售员工主数据代码。");
                while (!rs.EoF)
                {
                    BOneSlpCode = rs.Fields.Item("SlpCode").Value;
                    rs.MoveNext();
                }
                return BOneSlpCode;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(rs);
            }
        }

        /// <summary>
        /// 根据全渠道分支编码查询B1分支编码
        /// </summary>
        /// <param name="BPLID"></param>
        /// <returns></returns>
        public static int GetBranchCode(int BPLID)
        {
            int BOneBPID = default(Int32);
            SAPbobsCOM.IRecordset rs = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            string sql = @"select BPLID from OBPL where U_BPLID = '{0}'";
            rs.DoQuery(string.Format(sql, BPLID));
            if (rs.RecordCount < 1) throw new ArgumentNullException("根据提供分支编码无法找到B1中对应的分支代码。");
            if (rs.RecordCount > 1) throw new ArgumentException("根据提供的分支编码在B1找到多条分支代码。");
            while (!rs.EoF)
            {
                BOneBPID = rs.Fields.Item("BPLID").Value;
                rs.MoveNext();
            }
            return BOneBPID;
        }

        /// <summary>
        /// 通过仓库编码获取所属分至
        /// </summary>
        /// <param name="whsCode">仓库编码</param>
        /// <returns></returns>
        public static int GetBranchCodeByWhsCode(string whsCode)
        {
            if (string.IsNullOrEmpty(whsCode))
                throw new ArgumentNullException("仓库为空，无法确定分支.");
            int BOneBPID = default(Int32);
            SAPbobsCOM.IRecordset rs = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                string sql = @"select BPLid from OWHS where WhsCode = '{0}'";
                rs.DoQuery(string.Format(sql, whsCode));
                if (rs.RecordCount < 1) throw new ArgumentNullException("根据提供仓库编码无法找到B1中对应的分支代码。");
                if (rs.RecordCount > 1) throw new ArgumentException("根据提供的仓库编码在B1找到多条分支代码。");
                while (!rs.EoF)
                {
                    BOneBPID = rs.Fields.Item("BPLid").Value;
                    rs.MoveNext();
                }
                return BOneBPID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(rs);
            }
        }

        /// <summary>
        /// 比较两个仓库，是否有相同的所属分之
        /// 若有，则返回true
        /// </summary>
        /// <param name="fromWhs"></param>
        /// <param name="toWhs"></param>
        /// <returns></returns>
        public static bool IsTheSameBranch(string fromWhs, string toWhs)
        {
            if (string.IsNullOrEmpty(fromWhs))
                throw new ArgumentNullException("发出仓库为空，无法确定分支.");
            if (string.IsNullOrEmpty(toWhs))
                throw new ArgumentNullException("接收仓库为空，无法确定分支.");
            int fromBOneBPID = default(Int32);
            int toBOneBPID = default(Int32);
            SAPbobsCOM.IRecordset rs = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {


                string sql = @"select BPLid from OWHS where WhsCode = '{0}'";
                rs.DoQuery(string.Format(sql, fromWhs));
                if (rs.RecordCount != 1) throw new ArgumentNullException("根据提供的发出仓库编码无法找到B1中对应的分支代码。");
                fromBOneBPID = rs.Fields.Item("BPLid").Value;

                rs.DoQuery(string.Format(sql, toWhs));
                if (rs.RecordCount != 1) throw new ArgumentNullException("根据提供的接收仓库编码无法找到B1中对应的分支代码。");
                toBOneBPID = rs.Fields.Item("BPLid").Value;
                if (fromBOneBPID == toBOneBPID)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(rs);
            }
        }

        /// <summary>
        /// 判定所属仓库是否为主仓库
        /// </summary>
        /// <param name="whsCode"></param>
        /// <returns></returns>
        public static bool IsMainStore(string whsCode)
        {
            SAPbobsCOM.IRecordset rs = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                string sql = @"select U_WhsType from OWHS where WhsCode = '{0}'";
                rs.DoQuery(string.Format(sql, whsCode));
                if (rs.RecordCount < 1) throw new ArgumentNullException("在B1中找不到该仓库。");
                if (rs.Fields.Item("U_WhsType").Value == "11")
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(rs);
            }
        }

        /// <summary>
        /// 根据全渠道获取的单号判断该订单是否已处理到B1中
        /// </summary>
        /// <param name="tableName">单据表名称（主表）</param>
        /// <param name="DocEntry">全渠道单据号</param>
        /// <returns></returns>
        public static bool IsExistDocument(string tableName, string DocEntry, out string B1DocEntry)
        {
            bool IsExistDocument = false;
            B1DocEntry = default(String);
            SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            try
            {
                string sql = string.Format(@"select DocEntry from {0} where U_OCMDocEntry = '{1}'", tableName, DocEntry);
                res.DoQuery(sql);
                if (res.RecordCount >= 1)
                {
                    B1DocEntry = res.Fields.Item("DocEntry").Value.ToString();
                    IsExistDocument = true;
                }
                return IsExistDocument;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }

        /// <summary>
        /// 多账套同步,单据是否存在
        /// </summary>
        /// <param name="CompanyKey">账套标识</param>
        /// <param name="tableName"></param>
        /// <param name="DocEntry"></param>
        /// <param name="B1DocEntry"></param>
        /// <returns></returns>
        public static bool IsExistDocument4MFT(string CompanyKey, string tableName, string DocEntry, out string B1DocEntry)
        {
            bool IsExistDocument = false;
            B1DocEntry = default(String);
            SAPbobsCOM.IRecordset res = SAPCompanyPool.GetSAPCompany(CompanyKey).GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            try
            {
                string sql = string.Format(@"select DocEntry from {0} where U_OCMDocEntry = '{1}'", tableName, DocEntry);
                res.DoQuery(sql);
                if (res.RecordCount >= 1)
                {
                    B1DocEntry = res.Fields.Item("DocEntry").Value.ToString();
                    IsExistDocument = true;
                }
                return IsExistDocument;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }


        /// <summary>
        /// 根据全渠道单号判断该取消订单是否已处理到B1中 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="DocEntry"></param>
        /// <param name="B1DocEntry"></param>
        /// <returns></returns>
        public static bool IsCancelDocument(string tableName, string DocEntry, out int B1DocEntry)
        {
            bool IsExistDocument = false;
            string DocStatus = default(string);
            B1DocEntry = default(int);
            SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            try
            {
                string sql = string.Format(@"select DocEntry,DocStatus from {0} where  U_IM_DocEntry='{1}'", tableName, DocEntry);
                res.DoQuery(sql);
                if (res.RecordCount >= 1)
                {
                    DocStatus = res.Fields.Item("DocStatus").Value;
                    B1DocEntry = res.Fields.Item("DocEntry").Value;
                    if (DocStatus == "C")
                        IsExistDocument = true;
                }
                return IsExistDocument;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }


        public static bool IsExistOJDT(string DocEntry, string DocType, out string B1DocEntry)
        {
            bool IsExistDocument = false;
            B1DocEntry = default(String);
            SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            try
            {
                string sql = string.Format(@"select TransId from  OJDT where U_IM_DocEntry = '{0}' and U_DocumentType = '{1}'", DocEntry, DocType);
                res.DoQuery(sql);
                if (res.RecordCount >= 1)
                {
                    B1DocEntry = res.Fields.Item("TransId").Value.ToString();
                    IsExistDocument = true;
                }
                return IsExistDocument;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }

        public static bool IsExistDocument(string tableName, string DocEntry, string DocType, out string B1DocEntry)
        {
            bool IsExistDocument = false;
            B1DocEntry = default(String);
            SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            try
            {
                string sql = string.Format(@"select DocEntry from {0} where U_IM_DocEntry = '{1}' and U_ChannalDocType = '{2}'", tableName, DocEntry, DocType);
                res.DoQuery(sql);
                if (res.RecordCount >= 1)
                {
                    B1DocEntry = res.Fields.Item("DocEntry").Value.ToString();
                    IsExistDocument = true;
                }
                return IsExistDocument;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }


        }


        public static bool IsExistDraft(string objType, string OMNIDocEntry, out string B1DocEntry)
        {
            bool IsExistDocument = false;
            B1DocEntry = default(String);
            SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            try
            {
                string sql = string.Format(@"select DocEntry from ODRF where ObjType = '{0}' and U_IM_DocEntry = '{1}'", objType, OMNIDocEntry);
                res.DoQuery(sql);
                if (res.RecordCount >= 1)
                {
                    B1DocEntry = res.Fields.Item("DocEntry").Value.ToString();
                    IsExistDocument = true;
                }
                return IsExistDocument;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }

        /// <summary>
        /// 检查是否存在付款草稿
        /// </summary>
        /// <param name="OMNIDocEntry"></param>
        /// <param name="B1DocEntry"></param>
        /// <returns></returns>
        public static bool IsExistPaymentDraft(string OMNIDocEntry, out string B1DocEntry)
        {
            bool IsExistDocument = false;
            B1DocEntry = default(String);
            SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            try
            {
                string sql = string.Format(@"select DocEntry from OPDF where U_IM_DocEntry = '{0}'", OMNIDocEntry);
                res.DoQuery(sql);
                if (res.RecordCount >= 1)
                {
                    B1DocEntry = res.Fields.Item("DocEntry").Value.ToString();
                    IsExistDocument = true;
                }
                return IsExistDocument;
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }

        public static void CreateCallBackErrorInfo(ErrorRecord errorInfo)
        {
            SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                string sql = $@"insert into U_ErrorRecord(ObjectCode,Uniquekey,ErrorType,SBOID,CreateDate,IsSync,ErrorMsg) 
                            values('{errorInfo.ObjectCode}',
                                   '{errorInfo.UniqueKey}',
                                   {errorInfo.ErrorType},
                                   '{errorInfo.SBOID}',
                                   '{DateTime.Now}',
                                   'N',
                                   '{errorInfo.ErrorMsg}')";
                res.DoQuery(sql);
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }



        }

        public static string GetAddressCode(string ProviceName)
        {

            SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                var newProvinceName = ProviceName.Replace("省", "").Replace("特别行政区", "").Replace("自治区", "");
                string sql = $@"select Code from OCST where Name like '%{newProvinceName}%' ";
                res.DoQuery(sql);
                return res.Fields.Item("Code").Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }

        /// <summary>
        /// 根据物料和仓库获取批次信息
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="whsCode"></param>
        /// <returns></returns>
        public static List<BatchNumber> GetBatchByItemAndWhsCode(string itemCode, string whsCode)
        {
            SAPbobsCOM.IRecordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                if (string.IsNullOrEmpty(itemCode) || string.IsNullOrEmpty(whsCode))
                    throw new Exception("物料编码或仓库为空。");
                List<BatchNumber> batchList = new List<BatchNumber>();
                string sql = $@"SELECT DistNumber,T1.Quantity FROM OBTN T0 INNER JOIN OBTQ T1 ON
                                T0.ABSENTRY = T1.MdAbsEntry
                                WHERE T1.ITEMCODE = '{itemCode}' AND T1.WHSCODE = '{whsCode}'
                                ORDER BY T1.SysNumber";
                res.DoQuery(sql);
                while (!res.EoF)
                {
                    BatchNumber batch = new BatchNumber();
                    batch.BatchID = res.Fields.Item("DistNumber").Value;
                    batch.Quantity = res.Fields.Item("Quantity").Value;
                    batchList.Add(batch);
                    res.MoveNext();
                }
                return batchList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }

        public static DistributionRule GetDistributionRule(string itemCode, string userObjKey)
        {
            SAPbobsCOM.Recordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                if (string.IsNullOrEmpty(itemCode))
                    throw new ArgumentNullException("物料编码为空，无法找到品类信息。");
                if (string.IsNullOrEmpty(userObjKey))
                    throw new ArgumentNullException("用户信息为空，无法获取成本中心信息。");
                DistributionRule dbRule = new DistributionRule();
                string sql = $"select distinct(t0.UserObjKey), t1.Code as BPLId,t0.DeptId ,t0.Member from V_DEPT_USER t0 inner join [@AVA_OBPL] t1 on t0.OrgCode = t1.U_OrgCode where t0.UserObjKey = '{userObjKey}' ";
                res.DoQuery(sql);
                if (res.RecordCount > 1)
                    throw new Exception("人员属于多个部门或者分支");
                else if (res.RecordCount < 1)
                    throw new Exception("根据人员无法找到成本中心信息，请检查视图。");
                dbRule.BPLId = res.Fields.Item("BPLId").Value;
                dbRule.OcrCode = res.Fields.Item("DeptId").Value;
                dbRule.OcrCode2 = res.Fields.Item("Member").Value;
                sql = $"select U_CateCode from OITM where ItemCode = '{itemCode}'";
                res.DoQuery(sql);
                if (res.RecordCount != 1)
                    throw new Exception("根据物料信息不能找到唯一品类信息");
                dbRule.OcrCode3 = res.Fields.Item("U_CateCode").Value;
                return dbRule;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }


        /// <summary>
        /// 根据usercode获取分支
        /// </summary>
        /// <param name="userObjKey"></param>
        /// <returns></returns>
        public static DistributionRule GetDistributionRule(string userObjKey)
        {
            SAPbobsCOM.Recordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                if (string.IsNullOrEmpty(userObjKey))
                    throw new ArgumentNullException("用户信息为空，无法获取成本中心信息。");
                DistributionRule dbRule = new DistributionRule();
                string sql = $"select distinct(t0.UserObjKey), t1.Code as BPLId,t0.DeptId ,t0.Member from V_DEPT_USER t0 inner join [@AVA_OBPL] t1 on t0.OrgCode = t1.U_OrgCode where t0.UserObjKey = '{userObjKey}' ";
                res.DoQuery(sql);
                if (res.RecordCount != 1)
                    throw new Exception("根据用户信息不能找到唯一成本中心信息");
                dbRule.BPLId = res.Fields.Item("BPLId").Value;
                dbRule.OcrCode = res.Fields.Item("DeptId").Value;
                dbRule.OcrCode2 = res.Fields.Item("Member").Value;
                return dbRule;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }

        /// <summary>
        /// 根据DataOnwer获取分支
        /// </summary>
        /// <param name="userObjKey"></param>
        /// <returns></returns>
        public static DistributionRule GetDistributionRuleByKey(string userObjKey)
        {
            SAPbobsCOM.Recordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                if (string.IsNullOrEmpty(userObjKey))
                    throw new ArgumentNullException("用户信息为空，无法获取成本中心信息。");
                DistributionRule dbRule = new DistributionRule();
                string sql = $"select  distinct(t0.UserObjKey), t1.Code as BPLId,t0.DeptId ,t0.Member from V_DEPT_USER t0 inner join [@AVA_OBPL] t1 on t0.OrgCode = t1.U_OrgCode where t0.UserObjKey = '{userObjKey}' ";
                res.DoQuery(sql);
                if (res.RecordCount != 1)
                    throw new Exception("根据用户信息不能找到唯一成本中心信息");
                dbRule.BPLId = res.Fields.Item("BPLId").Value;
                dbRule.OcrCode = res.Fields.Item("DeptId").Value;
                dbRule.OcrCode2 = res.Fields.Item("Member").Value;
                return dbRule;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }


        /// <summary>
        /// 根据税率获取税码 
        /// </summary>
        /// <param name="rate"></param>
        /// <param name="TaxType">I/O</param>
        /// <returns></returns>
        public static string GetTaxByRate(double rate, string TaxType)
        {
            SAPbobsCOM.Recordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                string sql = $" select Code from OVTG where Category = '{TaxType}' and Rate = {rate}";
                res.DoQuery(sql);
                if (res.RecordCount != 1)
                    throw new Exception("根据税率无法找到税码，请检查。");
                return res.Fields.Item("Code").Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }

        /// <summary>
        /// 根据税率获取税码 
        /// </summary>
        /// <param name="CompanyKey">账套标识</param>
        /// <param name="rate"></param>
        /// <param name="TaxType">I/O</param>
        /// <returns></returns>
        public static string GetTaxByRate4MFT(string CompanyKey, double rate, string TaxType)
        {
            SAPbobsCOM.Recordset res = SAPCompanyPool.GetSAPCompany(CompanyKey).GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                string sql = $" select Code from OVTG where Category = '{TaxType}' and Rate = {rate}";
                res.DoQuery(sql);
                if (res.RecordCount != 1)
                    throw new Exception("根据税率无法找到税码，请检查。");
                return res.Fields.Item("Code").Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }

        /// <summary>
        /// 根据付款类型获取科目
        /// </summary>
        /// <param name="payType"></param>
        /// <returns></returns>
        public static string GetAccountCodeByPayType(string payType)
        {
            SAPbobsCOM.Recordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                string sql = $"select AcctCode from V_AVA_ER_OPYS where TypeCode = '{payType}'";
                res.DoQuery(sql);
                if (res.RecordCount != 1)
                    throw new Exception("根据付款类型找到对应科目，请检查。");
                return res.Fields.Item("AcctCode").Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }

        /// <summary>
        /// 根据入库类型获取科目
        /// </summary>
        /// <param name="inType"></param>
        /// <returns></returns>
        public static string GetAccountCodeByInType(string inType)
        {
            SAPbobsCOM.Recordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                string sql = $"select U_AcctCode from [@AVA_INTYPE] where Code = '{inType}'";
                res.DoQuery(sql);
                if (res.RecordCount != 1)
                    throw new Exception("根据入库类型找不到对应科目，请检查。");
                return res.Fields.Item("U_AcctCode").Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }

        /// <summary>
        /// 根据出库类型获取科目
        /// </summary>
        /// <param name="outType"></param>
        /// <returns></returns>
        public static string GetAccountCodeByOutType(string outType)
        {

            SAPbobsCOM.Recordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                string sql = $"select U_AcctCode from [@AVA_OUTTYPE]where Code = '{outType}'";
                res.DoQuery(sql);
                if (res.RecordCount != 1)
                    throw new Exception("根据出库类型找不到对应科目，请检查。");
                return res.Fields.Item("U_AcctCode").Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }


        /// <summary>
        /// 获取联系人编号
        /// </summary>
        /// <param name="CardCode"></param>
        /// <param name="ContractName"></param>
        /// <returns></returns>
        public static int GetBPContractCode(string CardCode, string ContractName)
        {
            SAPbobsCOM.Recordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                string sql = $"select Top 1 CntctCode from OCPR where CardCode = '{CardCode}' and Name = '{ContractName}' ";
                res.DoQuery(sql);
                if (res.RecordCount == 0)
                    return 0;
                else
                {
                    return res.Fields.Item(0).Value;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }

        /// <summary>
        /// 多账套获取联系人编号
        /// </summary>
        /// <param name="CardCode"></param>
        /// <param name="ContractName"></param>
        /// <returns></returns>
        public static int GetBPContractCode4MFT(string CompanyKey, string CardCode, string ContractName)
        {
            SAPbobsCOM.Recordset res = SAPCompanyPool.GetSAPCompany(CompanyKey).GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                string sql = $"select Top 1 CntctCode from OCPR where CardCode = '{CardCode}' and Name = '{ContractName}' ";
                res.DoQuery(sql);
                if (res.RecordCount == 0)
                    return 0;
                else
                {
                    return res.Fields.Item(0).Value;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }



        /// <summary>
        /// 获取联系人编号[集合] 
        /// </summary>
        /// <param name="CardCode"></param>
        /// <param name="ContractName"></param>
        /// <returns></returns>
        public static IList<CallBackData> GetBPContractCode4MFT(string CompanyKey, string CardCode)
        {
            IList<CallBackData> list = new List<CallBackData>();
            SAPbobsCOM.Recordset res = SAPCompanyPool.GetSAPCompany(CompanyKey).GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                string sql = $"select CntctCode,Name from OCPR where CardCode = N'{CardCode}' and Active = N'Y'";
                res.DoQuery(sql);
                if (res.RecordCount == 0)
                    return null;
                else
                {
                    while (!res.EoF)
                    {
                        list.Add(new CallBackData() { Value= (Convert.ToInt32(res.Fields.Item("CntctCode").Value)).ToString(), Key = res.Fields.Item("Name").Value });
                        res.MoveNext();
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }

        /// <summary>
        /// B1单据序列号
        /// </summary>
        /// <param name="B1ObjectCode"></param>
        /// <returns></returns>
        public static int GetB1DocEntrySeries(string B1ObjectCode)
        {
            SAPbobsCOM.Recordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                string sql = $"select Top 1 DfltSeries from ONNM where ObjectCode = '{B1ObjectCode}'";
                res.DoQuery(sql);
                if (res.RecordCount == 0)
                    return 0;
                else
                    return res.Fields.Item(0).Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }


        /// <summary>
        /// B1单据序列号
        /// </summary>
        /// <param name="CompanyKey">账套标识</param>
        /// <param name="B1ObjectCode"></param>
        /// <returns></returns>
        public static int GetB1DocEntrySeries4MFT(string CompanyKey, string B1ObjectCode)
        {
            SAPbobsCOM.Recordset res = SAPCompanyPool.GetSAPCompany(CompanyKey).GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                string sql = $"select Top 1 DfltSeries from ONNM where ObjectCode = '{B1ObjectCode}'";
                res.DoQuery(sql);
                if (res.RecordCount == 0)
                    return 0;
                else
                    return res.Fields.Item(0).Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }


        /// <summary>
        /// 获取业务伙伴组
        /// </summary>
        /// <param name="BusinessPartenerGroupName"></param>
        /// <returns></returns>
        public static int GetBusinessPartnerGroupObjectKey(string BusinessPartenerGroupName)
        {
            SAPbobsCOM.Recordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                string sql = $"SELECT TOP 1 GroupCode FROM OCRG WHERE GroupName = '{BusinessPartenerGroupName}'";
                res.DoQuery(sql);
                if (res.RecordCount == 0)
                    throw new Exception($"未能找到[{BusinessPartenerGroupName}]业务伙伴组编号");
                else
                    return res.Fields.Item(0).Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }
        /// <summary>
        /// 获取过账期间状态
        /// </summary>
        /// <param name="EntryFieldName">B1单据日期字段</param>
        /// <param name="EntryDate">B1单据日期</param>
        /// <returns></returns>
        public static string GetCurrentPeriodStatus(string EntryFieldName, DateTime EntryDate)
        {
            string FormatEntryDate = EntryDate.ToShortDateString();
            string str = "F_";
            string str2 = "T_";
            SAPbobsCOM.Recordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                switch (EntryFieldName)
                {
                    case "TaxDate":
                        str = str + "TaxDate";
                        str2 = str2 + "TaxDate";
                        break;

                    case "DocDueDate":
                        str = str + "DueDate";
                        str2 = str2 + "DueDate";
                        break;

                    case "DocDate":
                        str = str + "RefDate";
                        str2 = str2 + "RefDate";
                        break;

                    default:
                        str = str + "TaxDate";
                        str2 = str2 + "TaxDate";
                        break;
                }
                string queryStr = $"select Top 1 PeriodStat from [dbo].[OFPR] where  {str}  <= '{FormatEntryDate}' and  {str2}  >= '{FormatEntryDate}' ";
                res.DoQuery(queryStr);
                if (res.RecordCount == 0)
                {
                    throw new Exception($"没有该过账期间，请在B1中维护[{EntryDate}]。");
                }
                else
                {
                    return res.Fields.Item(0).Value;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }

        /// <summary>
        /// 根据税率找到客户关税
        /// </summary>
        /// <param name="CustomDutyName"></param>
        /// <returns></returns>
        public static string GetCustomDutyByRate(string CustomDutyName)
        {
            SAPbobsCOM.Recordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                string sql = $" select Top 1 CstGrpCode from OARG where CstGrpName = '{CustomDutyName}'";
                res.DoQuery(sql);
                if (res.RecordCount == 0)
                    throw new Exception("根据名称无法找到关税号码，请检查。");
                else
                    return res.Fields.Item(0).Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }

        /// <summary>
        /// 获取 业务伙伴区域编号
        /// </summary>
        /// <param name="AreaDescription">地区描述</param>
        /// <returns></returns>
        public static int GetTerritoryId(string AreaDescription)
        {
            SAPbobsCOM.Recordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                string sql = $"SELECT TOP 1 territryID FROM [OTER] WHERE descript = '{AreaDescription}'";
                res.DoQuery(sql);
                if (res.RecordCount == 0)
                    throw new Exception($"未能找到[{AreaDescription}]区域的编号");
                else
                    return res.Fields.Item(0).Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }

        /// <summary>
        /// 获取 业务伙伴区域编号
        /// </summary>
        /// <param name="CompanyKey">账套标识</param>
        /// <param name="AreaDescription">地区描述</param>
        /// <returns></returns>
        public static int GetTerritoryId4MFT(string CompanyKey,string AreaDescription)
        {
            SAPbobsCOM.Recordset res = SAPCompanyPool.GetSAPCompany(CompanyKey).GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                string sql = $"SELECT TOP 1 territryID FROM [OTER] WHERE descript = '{AreaDescription}'";
                res.DoQuery(sql);
                if (res.RecordCount == 0)
                    throw new Exception($"未能找到[{AreaDescription}]区域的编号");
                else
                    return res.Fields.Item(0).Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }

        /// <summary>
        /// 检查并获取 业务伙伴区域编号
        /// </summary>
        /// <param name="CompanyKey">账套标识</param>
        /// <param name="AreaDescription">地区描述</param>
        /// <returns></returns>
        public static int CheckTerritoryId4MFT(string CompanyKey, string AreaID)
        {
            int areaID = default(int);
            if (!Int32.TryParse(AreaID, out areaID))
            {
                throw new Exception($"该区域ID不是数值类型，请检查[{AreaID}]");
            }
            SAPbobsCOM.Recordset res = SAPCompanyPool.GetSAPCompany(CompanyKey).GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                string sql = $"SELECT TOP 1 territryID FROM [OTER] WHERE territryID = '{AreaID}'";
                res.DoQuery(sql);
                if (res.RecordCount == 0)
                    if (Convert.ToInt32(AreaID) == 0) return -2; else throw new Exception($"B1中没有此区域编号[{AreaID}]");
                else
                    return res.Fields.Item(0).Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }

        /// <summary>
        /// 查询仓库是否存在
        /// </summary>
        /// <param name="WhsCode"></param>
        /// <returns></returns>
        public static bool IsExistWarehouse(string WhsCode)
        {
            SAPbobsCOM.Warehouses whs = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oWarehouses);
            try
            {
                return whs.GetByKey(WhsCode);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(whs);
            }
        }

        /// <summary>
        /// 查询仓库是否存在
        /// </summary>
        /// <param name="CompanyKey">账套标识</param>
        /// <param name="WhsCode"></param>
        /// <returns></returns>
        public static bool IsExistWarehouse4MFT(string CompanyKey, string WhsCode)
        {
            SAPbobsCOM.Warehouses whs = SAPCompanyPool.GetSAPCompany(CompanyKey).GetBusinessObject(SAPbobsCOM.BoObjectTypes.oWarehouses);
            try
            {
                return whs.GetByKey(WhsCode);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(whs);
            }
        }

        /// <summary>
        /// 获取 销售员编号
        /// </summary>
        /// <param name="AreaDescription">地区描述</param>
        /// <returns></returns>
        public static int GetSalesPersonCode(string SalesPersonName)
        {
            SAPbobsCOM.Recordset res = SAP.SAPCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            try
            {
                string sql = $"SELECT TOP 1 SlpCode FROM [OSLP] WHERE SlpName = '{SalesPersonName}' And Active = 'Y'";
                res.DoQuery(sql);
                if (res.RecordCount == 0)
                    return -1; //无销售员工
                else
                    return res.Fields.Item(0).Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(res);
            }
        }
    }
}
