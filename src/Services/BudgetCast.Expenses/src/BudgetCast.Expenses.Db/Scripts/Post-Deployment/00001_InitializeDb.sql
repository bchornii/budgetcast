
DECLARE
@00001_Script_Name VARCHAR(250) = '00001_InitializeDb',
@00001_Task_Id VARCHAR(50) = '1',
@00001_AppliedTimes INT = 0;
 
EXEC [dbo].[spGetScriptDeploymentLog] @00001_Script_Name, @00001_AppliedTimes OUT

IF(@00001_AppliedTimes = 0)
BEGIN

	DECLARE @00001_Dt DATETIME = getdate();
	DECLARE @00001_UserId VARCHAR(20) = 'bchornii';
	DECLARE @00001_TenantId BIGINT = CAST((ABS(CHECKSUM(NEWID()))%100000+1)/10 AS INT);
	
	DECLARE @00001_CampaignName NVARCHAR(100) = 'Initial Campaign'
	DECLARE @00001_CampaignId INT = NEXT VALUE FOR CampaignIdSeq; 
	
	-- Populate Campaigns table with initial campaign
	INSERT INTO dbo.Campaigns(
		Id,
		TenantId,
		Name,
		StartsAt,
		CompletesAt,
		CreatedBy,
		CreatedOn,
		LastModifiedBy)
	VALUES (
		@00001_CampaignId,
		@00001_TenantId,
		@00001_CampaignName,
		@00001_Dt,
		DATEADD(yy, 100, @00001_Dt),
		@00001_UserId,
		@00001_Dt,
		@00001_UserId)
	
	DECLARE @00001_RandSufix VARCHAR(100);
	
	DECLARE @00001_ExpenseDescription VARCHAR(100);
	DECLARE @00001_ExpensesInitTotalPrice DECIMAL = 0;
	
	DECLARE @00001_ExpenseItemTitle VARCHAR(100);
	DECLARE @00001_ExpenseItemNote VARCHAR(100);
	DECLARE @00001_ExpenseItemPrice DECIMAL;
	DECLARE @00001_ExpenseItemQuantity INT;
	
	DECLARE @00001_TagName VARCHAR(20);
	
	DECLARE @00001_TotalExpenseInsertions INT = 10;
	
	DECLARE @00001_ExpenseId BIGINT;
	DECLARE @00001_ExpenseItemId BIGINT;
	
	WHILE @00001_TotalExpenseInsertions > 0
	BEGIN
		
		SET @00001_RandSufix = substring(cast(newid() as VARCHAR(36)), 0, 6)
	
		-- Create expense
		SET @00001_ExpenseId = NEXT VALUE FOR ExpenseIdSeq;
		SET @00001_ExpenseDescription = 'Expense for ' + CAST(@00001_ExpenseId as VARCHAR);
		INSERT INTO dbo.Expenses(
			Id, 
			TenantId,
			Description, 
			AddedAt, 
			CampaignTenantId,
			CampaignId,
			TotalPrice,
			CreatedBy,
			CreatedOn,
			LastModifiedBy)
		VALUES (
			@00001_ExpenseId,
			@00001_TenantId,
			@00001_ExpenseDescription,
			@00001_Dt,
			@00001_TenantId,
			@00001_CampaignId,
			@00001_ExpensesInitTotalPrice,
			@00001_UserId,
			@00001_Dt,
			@00001_UserId);
		
		-- Create expense items
		SET @00001_ExpenseItemId = NEXT VALUE FOR ExpenseItemIdSeq;	
		SET @00001_ExpenseItemTitle = 'Title for ' + CAST(@00001_ExpenseItemId as VARCHAR);
		SET @00001_ExpenseItemNote = 'Note for ' + CAST(@00001_ExpenseItemId as VARCHAR);
	
		SET @00001_ExpenseItemPrice = CAST((ABS(CHECKSUM(NEWID()))%11000+1)/10.0 AS decimal(18,5));
		SET @00001_ExpenseItemQuantity = CAST((ABS(CHECKSUM(NEWID()))%1000+1)/10 AS INT);
	
		INSERT INTO dbo.ExpenseItems(
			Id,
			Title,
			Note,
			Price,
			Quantity,
			ExpenseTenantId,
			ExpenseId,
			CreatedBy,
			CreatedOn,
			LastModifiedBy)
		VALUES (
			@00001_ExpenseItemId,
			@00001_ExpenseItemTitle,
			@00001_ExpenseItemNote,
			@00001_ExpenseItemPrice,
			@00001_ExpenseItemQuantity,
			@00001_TenantId,
			@00001_ExpenseId,
			@00001_UserId,
			@00001_Dt,
			@00001_UserId
		)
	
		SET @00001_ExpenseItemId = NEXT VALUE FOR ExpenseItemIdSeq;
		SET @00001_ExpenseItemPrice = CAST((ABS(CHECKSUM(NEWID()))%11000+1)/10.0 AS decimal(18,5));
		SET @00001_ExpenseItemQuantity = CAST((ABS(CHECKSUM(NEWID()))%1000+1)/10 AS INT);
	
		INSERT INTO dbo.ExpenseItems(
			Id,
			Title,
			Note,
			Price,
			Quantity,
			ExpenseTenantId,
			ExpenseId,
			CreatedBy,
			CreatedOn,
			LastModifiedBy)
		VALUES (
			@00001_ExpenseItemId,
			@00001_ExpenseItemTitle,
			@00001_ExpenseItemNote,
			@00001_ExpenseItemPrice,
			@00001_ExpenseItemQuantity,
			@00001_TenantId,
			@00001_ExpenseId,
			@00001_UserId,
			@00001_Dt,
			@00001_UserId
		)
	
		SET @00001_ExpenseItemId = NEXT VALUE FOR ExpenseItemIdSeq;
		SET @00001_ExpenseItemPrice = CAST((ABS(CHECKSUM(NEWID()))%11000+1)/10.0 AS decimal(18,5));
		SET @00001_ExpenseItemQuantity = CAST((ABS(CHECKSUM(NEWID()))%1000+1)/10 AS INT);
	
		INSERT INTO dbo.ExpenseItems(
			Id,
			Title,
			Note,
			Price,
			Quantity,
			ExpenseTenantId,
			ExpenseId,
			CreatedBy,
			CreatedOn,
			LastModifiedBy)
		VALUES (
			@00001_ExpenseItemId,
			@00001_ExpenseItemTitle,
			@00001_ExpenseItemNote,
			@00001_ExpenseItemPrice,
			@00001_ExpenseItemQuantity,
			@00001_TenantId,
			@00001_ExpenseId,
			@00001_UserId,
			@00001_Dt,
			@00001_UserId
		)
	
		-- Create tags
		SET @00001_TagName = 'Tag ' + @00001_RandSufix;
	
		INSERT INTO dbo.Tags(Name, ExpenseId, ExpenseTenantId)
		VALUES (@00001_TagName, @00001_ExpenseId, @00001_TenantId)
	
		INSERT INTO dbo.Tags(Name, ExpenseId, ExpenseTenantId)
		VALUES (@00001_TagName, @00001_ExpenseId, @00001_TenantId)
	
		INSERT INTO dbo.Tags(Name, ExpenseId, ExpenseTenantId)
		VALUES (@00001_TagName, @00001_ExpenseId, @00001_TenantId)
	
		SET @00001_TotalExpenseInsertions = @00001_TotalExpenseInsertions - 1;
	END
	
	-- Update totals for each expense based on expense items cost
	;WITH totalPrices AS (
		select ei.ExpenseId,
			   SUM(ei.Price * ei.Quantity) as Tot
		from dbo.ExpenseItems as ei
		group by ei.ExpenseId
	)
	UPDATE e
		SET e.TotalPrice = tp.Tot
	FROM dbo.Expenses as e
	JOIN totalPrices as tp on tp.ExpenseId = e.Id

	EXEC [dbo].[spSaveScriptDeploymentLog] @00001_Script_Name, @00001_Task_Id
END