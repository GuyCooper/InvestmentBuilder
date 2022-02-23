SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetCashTransactions')
BEGIN
	DROP PROCEDURE sp_GetCashTransactions
END

GO

CREATE PROCEDURE sp_GetCashTransactions(@Account as int, @TransactionType as varchar(20)) AS
BEGIN

SELECT 
	ca.valuation_date, 
	tt.[type], 
	SUM(ca.amount) as amount
FROM 
	CashAccount ca
INNER JOIN
	TransactionType tt
ON 
	tt.[type_id] = ca.[type_id]
WHERE
	ca.account_id = 1
	and tt.[type]  = @TransactionType
GROUP BY
	ca.valuation_date, tt.[type] 
ORDER BY 
	ca.valuation_date desc

END