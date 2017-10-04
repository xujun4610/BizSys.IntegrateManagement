--销售交货视图（用于自动生成发票） 
--视图条件 
--1、来源Anywhere订单 提货方式自提 未生成
--2、来源全渠道订单 非主仓   未生成
create view AVA_VIEW_CZ_ODLN
as
SELECT  distinct(T0.DocEntry),CardCode,t0.DocDate,DocTotal,t0.TaxDate,
		t0.DocDueDate,t0.U_IM_DocEntry,t0.U_PickingWay,
		t0.BPLId,t0.U_ResouceType
			FROM ODLN T0 inner JOIN DLN1 T1 ON T0.DOCENTRY = T1.DOCENTRY
			inner JOIN OWHS T2 ON T1.WhsCode = T2.WhsCode
				WHERE T0.DocStatus = 'O' and T0.U_ResouceType = '13' and U_IsSync = 'N' and T0.U_PickingWay = '11'