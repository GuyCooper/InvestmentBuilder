SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetActiveCompanies')
BEGIN
	DROP PROCEDURE sp_GetActiveCompanies
END

GO

CREATE PROCEDURE sp_GetActiveCompanies(@Account as VARCHAR(30), @ValuationDate as DATETIME) AS
BEGIN

SELECT
	 C.NAME 
FROM 
	InvestmentRecord IR
INNER JOIN
	Companies C
ON 
	IR.company_id = C.Company_Id
INNER JOIN
	Users U
ON
	IR.account_id = U.[User_Id]
WHERE 
	IR.is_active = 1
AND
	IR.Valuation_Date = @ValuationDate
AND
	U.Name = @Account
END