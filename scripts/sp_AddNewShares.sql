SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_AddNewShares')
BEGIN
	DROP PROCEDURE sp_AddNewShares
END

GO

CREATE PROCEDURE sp_AddNewShares(@ValuationDate as DATETIME, @company as VARCHAR(50), @shares as decimal(18,2), @totalCost as decimal(18,2), @account as int) AS
BEGIN

UPDATE 
	IR
SET
	IR.Shares_Bought += @shares, 
	IR.Total_Cost += @totalCost,
	IR.last_bought = @ValuationDate
FROM 
	investmentRecord AS IR
INNER JOIN 
	Companies C 
ON IR.Company_Id = C.Company_Id
WHERE C.Name = @company
AND IR.Valuation_Date = @ValuationDate
AND IR.account_id = @account

END