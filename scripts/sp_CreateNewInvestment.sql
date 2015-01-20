
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
				 @totalCost as FLOAT, @closingPrice as FLOAT, @dividend as FLOAT) AS
BEGIN

	INSERT INTO Companies (Name, Symbol, Currency, IsActive, ScalingFactor)
	VALUES (@investment, @symbol, @currency, 1, @scalingFactor)

	INSERT INTO InvestmentRecord ([Company_Id],[Valuation_Date],[Shares_Bought],[Total_Cost],[Selling_Price],[Dividends_Received])
	SELECT C.Company_Id, @valuationDate, @shares, @totalCost, @closingPrice, @dividend
	FROM Companies C WHERE C.Name = @investment 
END
GO
