
/****** Object:  StoredProcedure [dbo].[sp_AddNewShares]    Script Date: 20/10/2014 21:43:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_CreateNewInvestment')
BEGIN
	DROP PROCEDURE sp_CreateNewInvestment
END

GO

CREATE PROCEDURE [dbo].[sp_CreateNewInvestment](@valuationDate as DATETIME, @investment as VARCHAR(50), @symbol as CHAR(10),
				 @currency as CHAR(3), @scalingFactor as FLOAT, @shares as INT,
				 @totalCost as FLOAT, @closingPrice as FLOAT,
				 @account as INT, @exchange as VARCHAR(10)) AS
BEGIN

	IF NOT EXISTS (SELECT 1 FROM Companies WHERE Name = @investment)
	BEGIN
		INSERT INTO Companies (Name, Symbol, Currency, ScalingFactor, Exchange)
		VALUES (@investment, @symbol, @currency, @scalingFactor, @exchange )
	END

	DECLARE @CompanyId INT

	SELECT @CompanyId = Company_Id
	FROM Companies
	WHERE Name = @investment

	INSERT INTO InvestmentRecord
	 ([Company_Id],[Valuation_Date],[Shares_Bought],[Total_Cost],[Selling_Price],[Dividends_Received], [account_id], [is_active], [last_bought])
	SELECT 
		@CompanyId, @valuationDate, @shares, @totalCost, @closingPrice, 0,
		@Account, 1, @valuationDate
	
	return @@ROWCOUNT		 
END
GO
