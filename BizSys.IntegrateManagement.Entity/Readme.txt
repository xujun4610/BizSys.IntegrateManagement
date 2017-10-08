===============================================================
MasterDataManagement
	AccountReceivable			应收账款
	CapitalPlan					资金计划
	CostBudget					费用预算	
	Customer					客户
	DepositReceived				预收账款
	Employee					员工
	IncomeBudget				收入预算
	Materials					物料
	MaterialsCategory			品类
	MaterialsGroup				物料组
	OrganizationDepartments		组织部门
	SalesPomotion				促销活动
	Supplier					供应商
	Warehouse					仓库

===============================================================
PurchaseManagement
	PurchaseApplyOrder			采购申请
	PurchaseDeliveryOrder		采购交货
	PurchaseOrder				采购订单
	PurchaseReturnOrder			采购退货

=================================================================
ReceiptPayment
	InvoiceIssuable				应付发票
	InvoiceReceivable			应收发票
	Payment						付款
	Receipt						收款


===============================================================
SalesManagement
	SalesDeliveryOrder			销售交货
	SalesOrder					销售订单
	SalesReturnOrder			销售退货

=============================================================
StockManagement					
	GoodsIssue					库存发货
	GoodsReceipt				库存收货
	InventoryCounting			库存盘点
	InventoryTransferApply		库存转储
	MaterialsInventory			库存

	=============================================================
	请用有优雅的方式，修改代码！
	请注意查询条件和http web api 调用

	b1账套中，请注意添加如下字段：(B1 U开头字段不用管)
	U_IM_DocEntry
	U_ChannelDocType
	U_IM_DocLine(单据行对象)