SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetBalanceInHand')
BEGIN
	DROP PROCEDURE sp_GetBalanceInHand
END

GO

CREATE PROCEDURE sp_GetBalanceInHand(@ValuationDate as DATETIME, @Account AS VARCHAR(30)) AS
BEGIN

SELECT
	 ca.amount
FROM 
	CashAccount ca
INNER JOIN
	TransactionType tt
ON 
	ca.[type_id] = tt.[type_id]
INNER JOIN
	Users u
ON
	ca.[account_id] = u.[User_Id]
WHERE 
	ca.valuation_date = @ValuationDate
AND 
	tt.[type] = 'BalanceInHandCF'
AND
	u.Name = @Account
END