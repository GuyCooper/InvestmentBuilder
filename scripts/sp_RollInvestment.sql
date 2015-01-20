SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_RollInvestment')
BEGIN
	DROP PROCEDURE sp_RollInvestment
END

GO

CREATE PROCEDURE [dbo].[sp_RollInvestment](@valuationDate as DATETIME, @investment as VARCHAR(50)) AS
BEGIN

DECLARE @previousDate DATETIME

SELECT @previousDate = MAX(Valuation_Date) FROM InvestmentRecord

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