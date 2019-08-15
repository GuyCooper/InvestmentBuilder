SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetTransactionHistory')
BEGIN
	DROP PROCEDURE sp_GetTransactionHistory
END

GO

CREATE PROCEDURE sp_GetTransactionHistory(@dateFrom AS datetime, @dateTo AS datetime, @account as int) AS
BEGIN

SELECT
	 C.[Name], T.[trade_action],T.[transaction_date], T.[quantity], T.[total_cost]
FROM 
	TransactionHistory T
INNER JOIN 
	Companies C
ON
	T.[company_id] = C.[company_id]
WHERE
	T.[account_id] = @account
AND
	T.[transaction_date] >= @dateFrom
AND
	T.[transaction_date] <= @dateTo
END