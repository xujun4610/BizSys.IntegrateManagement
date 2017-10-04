USE [SBO_CZ]
GO
/****** Object:  StoredProcedure [dbo].[Tasks]    Script Date: 2017/1/9 13:27:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER Procedure [dbo].[Tasks]
	@object_type nvarchar(20), 	-- SBO Object Type
	@transaction_type nchar(1),-- [A]dd, [U]pdate, [D]elete, [C]ancel, C[L]ose
	@num_of_cols_in_key int,
	@list_of_key_cols_tab_del nvarchar(255),
	@list_of_cols_val_tab_del nvarchar(255),			
	@error  int Output,		-- Result (0 for no error)
	@error_message nvarchar (200) Output		-- Error string to be displayed
As
Begin
	Declare @DocEntry int
	Declare @Objtype nvarchar(30)
	Declare @WhsCode nvarchar(30)
	Declare @DocStatus nvarchar(30)
	Declare @AutoKey int 
	Declare @DraftStatus nchar(1)
	declare @BaseEntry int

	SET @AutoKey = 1;
	if exists(SELECT 1 FROM AVA_CZ_TASKLIST)
	begin
		SELECT @AutoKey = MAX(DocEntry) FROM AVA_CZ_TASKLIST
	end

--单据添加	
	IF @transaction_type = 'A'
	Begin
	
		--物料 价格清单
		IF @object_type = '4' 
		Begin		
			INSERT INTO [AVA_CZ_TASKLIST] (DocEntry,BusinessType,Direction,Uniquekey,Status,CreateDate)
			SELECT ROW_NUMBER() Over(Order by ObjType) + @AutoKey,ObjType,null,ItemCode,'A',GETDATE()
			FROM OITM Where itemtype = 'I' and ItemCode = @list_of_cols_val_tab_del
		End
		
		--应收发票
		IF @object_type = '13'
		Begin
			INSERT INTO [AVA_CZ_TASKLIST] (DocEntry,BusinessType,Direction,Uniquekey,Status,CreateDate)
			SELECT ROW_NUMBER() Over(Order by ObjType) + @AutoKey,ObjType,'I',DocEntry,'A',GETDATE()
			FROM OINV
			Where 
				U_ResouceType = '11'
				And DocEntry = @list_of_cols_val_tab_del
				And Exists(
					SELECT T0.DocEntry,t1.DocEntry,t2.ItmsGrpCod,T2.ItmsGrpNam
					FROM INV1 T0
						INNER JOIN OITM T1 ON T0.ItemCode = T1.ItemCode
						INNER JOIN OITB T2 ON T1.ItmsGrpCod = T2.ItmsGrpCod
					Where T0.DocEntry = @list_of_cols_val_tab_del
				)
		End

		--销售交货
		IF @object_type='15'
		Begin
			INSERT INTO [AVA_CZ_TASKLIST] (DocEntry,BusinessType,Direction,Uniquekey,Status,CreateDate)
			SELECT ROW_NUMBER() Over(Order by ObjType) + @AutoKey,ObjType,'I',DocEntry,'A',GETDATE()
			FROM ODLN
			Where 
				U_ResouceType = '11'
				And DocEntry = @list_of_cols_val_tab_del
				And Exists(
					SELECT T0.DocEntry,t1.DocEntry,t2.ItmsGrpCod,T2.ItmsGrpNam
					FROM DLN1 T0
						INNER JOIN OITM T1 ON T0.ItemCode = T1.ItemCode
						INNER JOIN OITB T2 ON T1.ItmsGrpCod = T2.ItmsGrpCod
					Where T0.DocEntry = @list_of_cols_val_tab_del
				)
		End

		--应付发票
		IF @object_type = '18'
		Begin
			INSERT INTO [AVA_CZ_TASKLIST] (DocEntry,BusinessType,Direction,Uniquekey,Status,CreateDate)
			SELECT ROW_NUMBER() Over(Order by ObjType) + @AutoKey,ObjType,'I',DocEntry,'A',GETDATE()
			FROM OPCH
			Where 
				DocEntry = @list_of_cols_val_tab_del
				
		End

		--采购收货
		IF @object_type = '20'
		Begin
			INSERT INTO [AVA_CZ_TASKLIST] (DocEntry,BusinessType,Direction,Uniquekey,Status,CreateDate)
			SELECT ROW_NUMBER() Over(Order by ObjType) + @AutoKey,ObjType,'I',DocEntry,'A',GETDATE()
			FROM OPDN
			Where 
				DocEntry = @list_of_cols_val_tab_del
				And Exists(
					SELECT T0.DocEntry,t1.DocEntry,t2.ItmsGrpCod,T2.ItmsGrpNam
					FROM PDN1 T0
						INNER JOIN OITM T1 ON T0.ItemCode = T1.ItemCode
						INNER JOIN OITB T2 ON T1.ItmsGrpCod = T2.ItmsGrpCod
					Where T0.DocEntry = @list_of_cols_val_tab_del
				)
		End

		--收款单
		IF @object_type='24'
		Begin
			INSERT INTO [AVA_CZ_TASKLIST] (DocEntry,BusinessType,Direction,Uniquekey,Status,CreateDate)
			SELECT ROW_NUMBER() Over(Order by ObjType) + @AutoKey,ObjType,'I',DocEntry,'A',GETDATE()
			FROM ORCT Where DocEntry = @list_of_cols_val_tab_del
		End

		--收款单
		IF @object_type='46'
		Begin
			INSERT INTO [AVA_CZ_TASKLIST] (DocEntry,BusinessType,Direction,Uniquekey,Status,CreateDate)
			SELECT ROW_NUMBER() Over(Order by ObjType) + @AutoKey,ObjType,'I',DocEntry,'A',GETDATE()
			FROM OVPM Where DocEntry = @list_of_cols_val_tab_del
		End

		--物料组
		IF @object_type='52'
		Begin
			INSERT INTO [AVA_CZ_TASKLIST] (DocEntry,BusinessType,Direction,Uniquekey,Status,CreateDate)
			SELECT ROW_NUMBER() Over(Order by Object) + @AutoKey,Object,null,ItmsGrpCod,'A',GETDATE()
			FROM OITB Where ItmsGrpCod = @list_of_cols_val_tab_del
		End

		--仓库
		IF @object_type = '64'
		Begin
			INSERT INTO [AVA_CZ_TASKLIST] (DocEntry,BusinessType,Direction,Uniquekey,Status,CreateDate)
			SELECT ROW_NUMBER() Over(Order by ObjType) + @AutoKey,ObjType,null,WhsCode,'A',GETDATE()
			FROM OWHS Where WhsCode = @list_of_cols_val_tab_del
		End

		--员工主数据
		IF @object_type = '171'
		Begin
			INSERT INTO [AVA_CZ_TASKLIST] (DocEntry,BusinessType,Direction,Uniquekey,Status,CreateDate)
			SELECT ROW_NUMBER() Over(Order by empID) + @AutoKey,empID,null,empID,'A',GETDATE()
			FROM OHEM Where empID = @list_of_cols_val_tab_del
		End
		
	End
	--更新单据
	IF @transaction_type = 'U'
	Begin
		--物料 价格清单
		IF @object_type = '4'
		Begin
			--判断任务列表里的数据是否同步，未同步则不用更新。
			IF Exists(
					SELECT * 
					FROM [AVA_CZ_TASKLIST] 
					Where ISNULL(IsSync,'N') = 'Y' 
						And BusinessType = @object_type 
						And UniqueKey = @list_of_cols_val_tab_del)
			Begin	--未完成同步
				IF Not Exists(
						SELECT * 
						FROM [AVA_CZ_TASKLIST] 
						Where BusinessType = @object_type 
							And UniqueKey = @list_of_cols_val_tab_del and ISNULL(IsSync,'N') = 'N')
				Begin	
					update 	[AVA_CZ_TASKLIST] set IsSync='Y'
					where BusinessType = @object_type And UniqueKey = @list_of_cols_val_tab_del	
					and ISNULL(IsSync,'N') = 'N'	
					INSERT INTO [AVA_CZ_TASKLIST] (DocEntry,BusinessType,Direction,Uniquekey,Status,CreateDate)
					SELECT ROW_NUMBER() Over(Order by ObjType) + @AutoKey,ObjType,null,ItemCode,'A',GETDATE()
					FROM OITM Where itemtype = 'I' and ItemCode = @list_of_cols_val_tab_del				
				End
			End	
			else
			IF Not Exists(
						SELECT * 
						FROM [AVA_CZ_TASKLIST] 
						Where ISNULL(IsSync,'N') = 'N' 
							And BusinessType = @object_type 
							And UniqueKey = @list_of_cols_val_tab_del)		
							begin
									update 	[AVA_CZ_TASKLIST] set IsSync='Y'
										where BusinessType = @object_type And UniqueKey = @list_of_cols_val_tab_del	
										and ISNULL(IsSync,'N') = 'N' 
									INSERT INTO [AVA_CZ_TASKLIST] (DocEntry,BusinessType,Direction,Uniquekey,Status,CreateDate)
									SELECT ROW_NUMBER() Over(Order by ObjType) + @AutoKey,ObjType,null,ItemCode,'A',GETDATE()
									FROM OITM Where itemtype = 'I' and ItemCode = @list_of_cols_val_tab_del	
							end
				
		End
		
		--仓库
		IF @object_type = '64'
		Begin
			--判断任务列表里的数据是否同步，未同步则不用更新。
			IF Exists(
					SELECT * 
					FROM [AVA_CZ_TASKLIST] 
					Where ISNULL(IsSync,'N') = 'Y'
						And BusinessType = @object_type 
						And UniqueKey = @list_of_cols_val_tab_del)
			Begin	--未完成同步
				IF Not Exists(
						SELECT * 
						FROM [AVA_CZ_TASKLIST] 
						Where BusinessType = @object_type 
							And UniqueKey = @list_of_cols_val_tab_del and ISNULL(IsSync,'N') = 'N')		
				Begin	
					update 	[AVA_CZ_TASKLIST] set IsSync='Y'
					where BusinessType = @object_type And UniqueKey = @list_of_cols_val_tab_del	
					and ISNULL(IsSync,'N') = 'N' 	
					INSERT INTO [AVA_CZ_TASKLIST] (DocEntry,BusinessType,Direction,Uniquekey,Status,CreateDate)
					SELECT ROW_NUMBER() Over(Order by ObjType) + @AutoKey,ObjType,null,WhsCode,'A',GETDATE()
					FROM OWHS Where WhsCode = @list_of_cols_val_tab_del				
				End
			End	
			else
			IF Not Exists(
						SELECT * 
						FROM [AVA_CZ_TASKLIST] 
						Where ISNULL(IsSync,'N') = 'N' 
							And BusinessType = @object_type 
							And UniqueKey = @list_of_cols_val_tab_del)		
							begin
									update 	[AVA_CZ_TASKLIST] set IsSync='Y'
										where BusinessType = @object_type And UniqueKey = @list_of_cols_val_tab_del	
										and ISNULL(IsSync,'N') = 'N' 
									INSERT INTO [AVA_CZ_TASKLIST] (DocEntry,BusinessType,Direction,Uniquekey,Status,CreateDate)
									SELECT ROW_NUMBER() Over(Order by ObjType) + @AutoKey,ObjType,null,WhsCode,'A',GETDATE()
									FROM OWHS Where WhsCode = @list_of_cols_val_tab_del	
							end
				
		End
	
	End
	----单据删除
	--IF @transaction_type = 'D'
	--Begin

		

	--End
	--单据取消、单据关闭
	--IF @transaction_type = 'C' OR @transaction_type = 'L'
	--Begin
		
	--End
	-- Select the return values
	select @error, @error_message


End