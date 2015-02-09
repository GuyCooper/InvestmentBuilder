SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetBalanceInHand')
BEGIN
	DROP PROCEDURE sp_GetBalanceInHand
END

GO

CREATE PROCEDURE sp_GetBalanceInHand(@ValuationDate as DATETIME) AS
BEGIN

SELECT ca.amount FROM 
CashAccount ca
INNER JOIN
TransactionType tt
ON ca.type_id = tt.type_id
WHERE ca.valuation_date = @ValuationDate
AND tt.[type] = 'BalanceInHandCF'
END