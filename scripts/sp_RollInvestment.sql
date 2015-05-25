SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_RollInvestment')
BEGIN
	DROP PROCEDURE sp_RollInvestment
END

GO

CREATE PROCEDURE [dbo].[sp_RollInvestment](@valuationDate as DATETIME, @previousDate as DATETIME, @company as VARCHAR(50), @account as VARCHAR(30)) AS
BEGIN

INSERT INTO InvestmentRecord
SELECT
	 IR.Company_id, @valuationDate, IR.Shares_Bought, IR.[Bonus_Shares issued], IR.Shares_Sold, IR.Total_Cost, 
	 IR.Selling_Price, IR.Dividends_Received, IR.account_id, IR.is_active, IR.last_bought
FROM 
	 InvestmentRecord IR 
INNER JOIN 
	Companies C ON IR.Company_id = C.Company_Id
INNER JOIN
	Users U ON IR.account_id = U.[User_Id]
WHERE 
	IR.Valuation_Date = @previousDate AND
	C.Name = @company AND
	U.Name = @account
END