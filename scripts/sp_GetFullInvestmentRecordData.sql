
/****** Object:  StoredProcedure [dbo].[sp_GetLatestInvestmentRecords]    Script Date: 20/10/2014 21:43:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetFullInvestmentRecordData')
BEGIN
	DROP PROCEDURE sp_GetFullInvestmentRecordData
END

GO

CREATE PROCEDURE [dbo].[sp_GetFullInvestmentRecordData](@Account as VARCHAR(30)) AS
BEGIN

SELECT 
	C.Name as Name,
	IR.Valuation_Date as ValuationDate,
	IR.last_bought as LastBoughtDate,
	IR.Shares_Bought as Bought,
	IR.[Bonus_Shares issued] as Bonus,
	IR.Shares_Sold as Sold,
	IR.Total_Cost as TotalCost,
	IR.Selling_Price as Price,
	IR.Dividends_Received as Dividends	
FROM InvestmentRecord IR 
INNER JOIN Companies C
ON IR.Company_id = C.Company_Id 
INNER JOIN Accounts A
ON IR.account_id = A.[Account_Id]
WHERE A.[Name] = @Account
AND IR.is_active = 1
 
END
GO

