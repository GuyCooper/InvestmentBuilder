
GO

/****** Object:  StoredProcedure [dbo].[sp_AddCashAccountData]    Script Date: 04/02/2016 17:50:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_RemoveCashAccountData')
BEGIN
	DROP PROCEDURE sp_RemoveCashAccountData
END

GO

CREATE PROCEDURE [dbo].sp_RemoveCashAccountData(@ValuationDate as DateTime, @TransactionDate as DateTime, @TransactionType as varchar(20),
									@Parameter as varchar(50), @Account as varchar(30)) AS
BEGIN

DECLARE @Type_Id INT
DECLARE @Account_Id INT

SELECT @Type_Id = [type_id]
FROM TransactionType
WHERE [type] = @TransactionType

SELECT @Account_Id = [User_Id]
FROM Users
WHERE [Name] = @Account

DELETE FROM CashAccount
WHERE
	[valuation_date] = @ValuationDate
AND [transaction_date] = @TransactionDate
AND [type_id] = @Type_Id
AND [parameter] = @Parameter
AND [account_id] = @Account_Id

END
GO

