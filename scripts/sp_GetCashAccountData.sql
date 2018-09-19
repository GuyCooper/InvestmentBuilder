SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetCashAccountData')
BEGIN
	DROP PROCEDURE sp_GetCashAccountData
END

GO

CREATE PROCEDURE sp_GetCashAccountData(@ValuationDate as DateTime, @Side as char(1), @Account as int) AS
BEGIN

SELECT 
	ca.transaction_date as TransactionDate,
	tt.[type] as TransactionType,
	ca.Parameter as Parameter,
	ca.Amount as Amount
FROM 
	CashAccount ca
INNER JOIN
	TransactionType tt
ON
	ca.type_id = tt.type_id
WHERE
	ca.valuation_date = @ValuationDate AND
	tt.side = @Side AND
	ca.account_id = @Account
	
END