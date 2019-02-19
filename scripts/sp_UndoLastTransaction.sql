SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_UndoLastTransaction')
BEGIN
	DROP PROCEDURE sp_UndoLastTransaction
END

GO

CREATE PROCEDURE sp_UndoLastTransaction(@account as int, @fromValuationDate as DATETIME) AS
BEGIN

DECLARE @AccountId INT
DECLARE @ValuationDate DATETIME

SELECT TOP 1 @ValuationDate = [Valuation_Date]
FROM 	TransactionHistory
WHERE [account_id] = @Account
AND valuation_date > @fromValuationDate
ORDER BY [valuation_date] DESC

	DELETE FROM InvestmentRecord
	WHERE [account_id] = @Account
	AND [Valuation_Date] = @ValuationDate

	DELETE FROM TransactionHistory
	WHERE [account_id] = @Account
	AND [valuation_date] = @ValuationDate


END