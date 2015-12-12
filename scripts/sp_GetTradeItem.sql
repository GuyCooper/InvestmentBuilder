
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
									   @Account as VARCHAR(30)) AS
BEGIN

DECLARE @latestDate DATETIME
DECLARE @AccountID INT

SELECT @AccountID = [USER_ID]
FROM [Users]
WHERE [Name] = @Account

SELECT @latestDate = MAX(Valuation_Date) FROM InvestmentRecord
WHERE account_id = @AccountID

IF(@@ROWCOUNT = 0)
BEGIN
	RETURN
END

SELECT 
	C.Name,
	C.Symbol,
	C.Currency,
	C.Exchange,
	C.LastBoughtDate,
	C.ScalingFactor,
	IR.Shares_Bought,
	IR.Shares_Sold,
	IR.Total_Cost	
FROM InvestmentRecord IR JOIN Companies C
ON IR.Company_id = C.Company_Id 
WHERE 
IR.Valuation_Date = @latestDate
AND IR.account_id = @AccountID
AND C.[Name] = @Company
AND C.IsActive = 1
 
END
GO

