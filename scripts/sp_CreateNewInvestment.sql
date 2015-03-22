
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
				 @totalCost as FLOAT, @closingPrice as FLOAT, @dividend as FLOAT,
				 @account as VARCHAR(30)) AS
BEGIN

	INSERT INTO Companies (Name, Symbol, Currency, ScalingFactor)
	VALUES (@investment, @symbol, @currency, @scalingFactor )

	INSERT INTO InvestmentRecord
	 ([Company_Id],[Valuation_Date],[Shares_Bought],[Total_Cost],[Selling_Price],[Dividends_Received], [account_id], [is_active], [last_bought])
	SELECT 
		C.Company_Id, @valuationDate, @shares, @totalCost, @closingPrice, @dividend,
		U.[User_Id], 1, @valuationDate
	FROM 
		InvestmentRecord IR
	INNER JOIN
		Companies C
	ON IR.Company_id = C.Company_Id
	INNER JOIN
		Users U
	ON IR.account_id = U.[User_Id]		
	WHERE
		C.Name = @investment
	AND
		U.Name = @account	 
END
GO
