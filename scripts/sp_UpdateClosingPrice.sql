CREATE PROCEDURE sp_UpdateClosingPrice(@valuationDate as DATETIME, @Investment as VARCHAR(50), @ClosingPrice as float) AS
BEGIN

UPDATE IR SET IR.Selling_Price = @ClosingPrice
FROM InvestmentRecord AS IR JOIN Companies C ON IR.Company_Id = C.Company_Id
AND C.Name = @Investment
AND IR.Valuation_Date = @ValuationDate
 
END