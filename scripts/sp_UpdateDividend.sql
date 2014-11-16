CREATE PROCEDURE sp_UpdateDividend(@valuationDate as DATETIME, @Investment as VARCHAR(50), @Dividend as float) AS
BEGIN

UPDATE IR SET IR.Dividends_Received += @Dividend
FROM InvestmentRecord AS IR JOIN Companies C ON IR.Company_Id = C.Company_Id
AND C.Name = @Investment
AND IR.Valuation_Date = @ValuationDate
 
END