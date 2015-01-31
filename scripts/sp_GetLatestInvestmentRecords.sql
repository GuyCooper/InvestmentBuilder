
/****** Object:  StoredProcedure [dbo].[sp_GetLatestInvestmentRecords]    Script Date: 20/10/2014 21:43:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetLatestInvestmentRecords')
BEGIN
	DROP PROCEDURE sp_GetLatestInvestmentRecords
END

GO

CREATE PROCEDURE [dbo].[sp_GetLatestInvestmentRecords](@valuationDate as DATETIME) AS
BEGIN

SELECT 
	C.Name as Name,
	C.LastBoughtDate as LastBoughtDate,
	IR.Shares_Bought as Bought,
	IR.[Bonus_Shares issued] as Bonus,
	IR.Shares_Sold as Sold,
	IR.Total_Cost as TotalCost,
	IR.Selling_Price as Price,
	IR.Dividends_Received as Dividends	
FROM InvestmentRecord IR JOIN Companies C
ON IR.Company_id = C.Company_Id 
WHERE IR.Valuation_Date = @valuationDate
AND C.IsActive = 1
ORDER BY LastBoughtDate ASC
 
END
GO

