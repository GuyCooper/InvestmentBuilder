CREATE PROCEDURE sp_UpdateHolding(@Holding as INT, @ValuationDate as DATETIME, @Investment as VARCHAR(50)) AS
BEGIN

UPDATE IR SET IR.Shares_Bought = @Holding
FROM InvestmentRecord AS IR JOIN Companies C ON IR.Company_Id = C.Company_Id
AND C.Name = @Investment
AND IR.Valuation_Date = @ValuationDate

END