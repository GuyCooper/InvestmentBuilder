SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetLastTransaction')
BEGIN
	DROP PROCEDURE sp_GetLastTransaction
END

GO

--returns the last transactions details for this account
CREATE PROCEDURE sp_GetLastTransaction(@account as int) AS
BEGIN

SELECT TOP 1 
	th.Valuation_Date,
	th.transaction_date,
	c.Name,
	th.trade_action,
	th.quantity,
	th.total_cost
FROM 	TransactionHistory th
INNER JOIN Companies c
ON th.company_id = c.Company_Id
WHERE th.account_id = @Account
ORDER BY th.valuation_date DESC			 

END