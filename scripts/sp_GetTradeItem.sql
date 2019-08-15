
/****** Object:  StoredProcedure [dbo].[sp_AddNewShares]    Script Date: 20/10/2014 21:43:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetTradeItem')
BEGIN
	DROP PROCEDURE sp_GetTradeItem
END

GO

CREATE PROCEDURE [dbo].sp_GetTradeItem(@Company as VARCHAR(50),
									   @Account as INT) AS
BEGIN

DECLARE @latestDate DATETIME
DECLARE @AccountID INT

SELECT @latestDate = MAX(Valuation_Date) FROM InvestmentRecord
WHERE account_id = @Account

IF(@@ROWCOUNT = 0)
BEGIN
	RETURN
END

SELECT 
	C.Name as Name,
	C.Symbol as Symbol,
	C.Currency as Currency,
	C.Exchange as Exchange ,
	C.ScalingFactor as ScalingFactor,
	IR.Shares_Bought,
	IR.Shares_Sold,
	IR.Total_Cost,	
	IR.last_bought as LastBoughtDate
FROM InvestmentRecord IR JOIN Companies C
ON IR.Company_id = C.Company_Id 
WHERE 
IR.Valuation_Date = @latestDate
AND IR.account_id = @Account
AND C.[Name] = @Company
AND C.IsActive = 1
 
END
GO

