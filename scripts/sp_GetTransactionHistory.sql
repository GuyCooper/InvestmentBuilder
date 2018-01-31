SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetTransactionHistory')
BEGIN
	DROP PROCEDURE sp_GetTransactionHistory
END

GO

CREATE PROCEDURE sp_GetTransactionHistory(@dateFrom AS datetime, @dateTo AS datetime, @account as varchar(30)) AS
BEGIN

SELECT
	 C.[Name], T.[trade_action],T.[transaction_date], T.[quantity], T.[total_cost]
FROM 
	TransactionHistory T
INNER JOIN 
	Companies C
ON
	T.[company_id] = C.[company_id]
INNER JOIN
	Accounts A
ON
	T.[account_id] = A.[Account_id]
WHERE
	A.[Name] = @account
AND
	T.[transaction_date] >= @dateFrom
AND
	T.[transaction_date] <= @dateTo
END