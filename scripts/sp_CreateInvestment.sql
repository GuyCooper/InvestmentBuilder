CREATE PROCEDURE dbo.[sp_CreateInvestment](@valuationDate as DATETIME, @previousDate as DATETIME, @investment as VARCHAR(50)) AS
BEGIN

INSERT INTO InvestmentRecord
SELECT
	 IR.Company_id, @valuationDate, IR.Shares_Bought, IR.[Bonus_Shares issued], IR.Shares_Sold, IR.Total_Cost, 
	 IR.Selling_Price, IR.Dividends_Received
FROM 
	 InvestmentRecord IR INNER JOIN Companies C ON IR.Company_id = C.Company_Id
WHERE 
	IR.Valuation_Date = @previousDate AND
	C.Name = @investment
END