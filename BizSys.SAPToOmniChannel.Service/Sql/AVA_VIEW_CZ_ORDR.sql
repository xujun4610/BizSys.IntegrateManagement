USE [SBO_CZ]
GO

/****** Object:  View [dbo].[AVA_VIEW_CZ_ORDR]    Script Date: 2017/4/10 11:09:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

alter view [dbo].[AVA_VIEW_CZ_ORDR] 
as 
select distinct(t0.DocEntry),U_PickingWay,CardCode,t0.DocDate,DocTotal,
TaxDate,DocDueDate,t0.U_IM_DocEntry,U_ResouceType,t0.BPLId
  from ORDR t0 left join RDR1 t1 on t0.DocEntry = t1.DocEntry 
left join OWHS t2 on t1.WhsCode = t2.WhsCode
where  t0.DocStatus = 'O' and ((U_ResouceType = '13' and U_IsSync = 'N') 
or(t0.U_ResouceType = '11' and U_IsSync = 'N' and t2.U_WhsType <> '11'))
GO


