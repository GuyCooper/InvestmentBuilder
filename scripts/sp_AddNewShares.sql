SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_AddNewShares')
BEGIN
	DROP PROCEDURE sp_AddNewShares
END

GO

CREATE PROCEDURE sp_AddNewShares(@ValuationDate as DATETIME, @investment as VARCHAR(50), @shares as INT, @totalCost as float) AS
BEGIN

UPDATE IR SET IR.Shares_Bought += @shares, IR.Total_Cost += @totalCost
FROM investmentRecord AS IR JOIN Companies C ON IR.Company_Id = C.Company_Id
AND C.Name = @Investment
AND IR.Valuation_Date = @ValuationDate

UPDATE Companies SET LastBoughtDate = @ValuationDate
WHERE Name = @investment
 
END