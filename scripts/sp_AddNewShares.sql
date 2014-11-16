CREATE PROCEDURE sp_AddNewShares(@valuationDate as DATETIME, @Investment as VARCHAR(50), @Shares as INT, @TotalCost as float) AS
BEGIN

UPDATE IR SET IR.Shares_Bought += @Shares, IR.Total_Cost += @TotalCost
FROM InvestmentRecord AS IR JOIN Companies C ON IR.Company_Id = C.Company_Id
AND C.Name = @Investment
AND IR.Valuation_Date = @ValuationDate
 
END