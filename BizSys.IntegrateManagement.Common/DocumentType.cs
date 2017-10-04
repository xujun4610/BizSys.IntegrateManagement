using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizSys.IntegrateManagement.Common
{
    public enum DocumentType
    {
        /**********************************************************
        枚举值前缀以模块来区分
        如：库存模块以‘1’为前缀 销售模块以2为前缀
        各个模块内部枚举值按顺序排列
        ***********************************************************/


        #region 库存模块 数字1开头
        /// <summary>
        /// 库存过账
        /// </summary>
        INVENTORYUPDATE = 11,
        /// <summary>
        /// 库存盘点
        /// </summary>
        INVENTORYCOUNTING = 12,

        /// <summary>
        /// 库存转储申请
        /// </summary>
        INVENTORYTRANSFER = 13,
        /// <summary>
        /// 库存信息
        /// </summary>
        MATERIALSINVENTORY = 14,
        /// <summary>
        /// 库存收货
        /// </summary>
        GOODSRECEIPT = 15,

        /// <summary>
        /// 库存发货
        /// </summary>
        GOODSISSUE = 16,
        #endregion
        #region 销售模块 数字2开头
        /// <summary>
        /// 销售订单
        /// </summary>
        SALESORDER = 21,

        /// <summary>
        /// 销售交货单
        /// </summary>
        SALESDELIVERYORDER = 22,

        /// <summary>
        /// 销售退货单
        /// </summary>
        SALESRETURNORDER = 23,

        /// <summary>
        /// 应收发票
        /// </summary>
        INVOICE=24,

        #endregion
        #region 采购模块 数字3开头
        /// <summary>
        /// 采购
        /// </summary>
        PURCHASEORDER = 31,


        /// <summary>
        /// 采购交货单
        /// </summary>
        PURCHASEDELIVERYORDER = 32,

        /// <summary>
        /// 采购退货单
        /// </summary>
        PURCHASERETURN = 33,

        /// <summary>
        /// 应付发票
        /// </summary>
        PAYABLE = 34,

        #endregion
        #region 收付款模块 数字4开头

        /// <summary>
        /// 收款
        /// </summary>
        RECEIPT = 41,

        /// <summary>
        /// 付款
        /// </summary>
        PAYMENT = 42,

        /// <summary>
        /// 回款核销
        /// </summary>
        RECEIPTVERIFICATION = 43,

        /// <summary>
        /// 付款申请
        /// </summary>
        PAYMENTAPPLY = 44,

        /// <summary>
        /// 往来核销
        /// </summary>
        RECONCILIATION = 45,

        /// <summary>
        /// 日记账分录
        /// </summary>
        RECORD = 46,

        /// <summary>
        /// 费用报销
        /// </summary>
        COSTREIMBURSEMENT = 47,
        #endregion
        #region 主数据模块 数字5开头
        /// <summary>
        /// 物料
        /// </summary>
        MATERIALS = 51,

        /// <summary>
        /// 供应商
        /// </summary>
        SUPPLIER = 52,

        /// <summary>
        /// 物料组
        /// </summary>
        ITEMGROUP = 53,

        /// <summary>
        /// 业务伙伴
        /// </summary>
        BUSINESSPARTNER = 54,

        /// <summary>
        /// 品类
        /// </summary>
        MATERIALSCATEGORY = 55,

        /// <summary>
        /// 拣配单
        /// </summary>
        PACKINGLIST = 56,

        /// <summary>
        /// 仓库
        /// </summary>
        WAREHOURSE = 57,

        /// <summary>
        /// 客户主数据
        /// </summary>
        CUSTOMER = 58,

        /// <summary>
        /// 会员主数据
        /// </summary>
        LEAGUER = 59,

        /// <summary>
        /// 资金计划
        /// </summary>
        CAPITALPLAN = 510,

        /// <summary>
        /// 收入预算
        /// </summary>
        INCOMEBUDGET = 511,

        /// <summary>
        /// 费用预算
        /// </summary>
        COSTBUDGET = 512,

        /// <summary>
        /// 促销活动
        /// </summary>
        SALESPROMOTION = 513,

        /// <summary>
        /// 员工主数据
        /// </summary>
        EMPLOYEE = 514,

        /// <summary>
        /// 组织部门
        /// </summary>
        ORGANIZATION = 515,
        #endregion
        #region 客户服务 数字6开头
        /// <summary>
        /// 客户服务
        /// </summary>
        CUSTOMERSERVICE = 60,
        #endregion


    }
}
