SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_UndoLastTransaction')
BEGIN
	DROP PROCEDURE sp_UndoLastTransaction
END

GO

CREATE PROCEDURE sp_UndoLastTransaction(@account as varchar(30)) AS
BEGIN

DECLARE @AccountId INT
DECLARE @ValuationDate DATETIME

SELECT @AccountId = [User_Id]
FROM [Users]
WHERE [Name] = @account

SELECT TOP 1 @ValuationDate = [Valuation_Date]
FROM 	TransactionHistory
WHERE [account_id] = @AccountId
ORDER BY [valuation_date] DESC

	DELETE FROM InvestmentRecord
	WHERE [account_id] = @AccountId
	AND [Valuation_Date] = @ValuationDate

	DELETE FROM TransactionHistory
	WHERE [account_id] = @AccountId
	AND [valuation_date] = @ValuationDate


END