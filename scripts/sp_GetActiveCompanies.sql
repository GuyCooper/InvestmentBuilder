SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetActiveCompanies')
BEGIN
	DROP PROCEDURE sp_GetActiveCompanies
END

GO

CREATE PROCEDURE sp_GetActiveCompanies(@Account as INT, @ValuationDate as DATETIME) AS
BEGIN

SELECT
	 C.NAME 
FROM 
	InvestmentRecord IR
INNER JOIN
	Companies C
ON 
	IR.company_id = C.Company_Id
WHERE 
	IR.is_active = 1
AND
	IR.Valuation_Date = @ValuationDate
AND
	IR.account_id = @Account
END