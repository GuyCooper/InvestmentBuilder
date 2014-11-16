USE [ArgyllInvestments]
GO

/****** Object:  StoredProcedure [dbo].[sp_AddNewShares]    Script Date: 20/10/2014 21:43:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_CreateNewInvestment](@valuationDate as DATETIME, @Investment as VARCHAR(50), @Symbol as CHAR(10),
				 @Currency as CHAR(3), @ScalingFacotr as FLOAT, @Shares as INT,
				 @TotalCost as FLOAT, @ClosingPrice as FLOAT) AS
BEGIN

	INSERT INTO Companies (Name, Symbol, Currency, IsActive, ScalingFactor)
	VALUES (@Investment, @Symbol, @Currency, 1, @ScalingFacotr)

	INSERT INTO InvestmentRecord ([Company_Id],[Valuation_Date],[Shares_Bought],[Total_Cost],[Selling_Price])
	SELECT C.Company_Id, @valuationDate, @Shares, @TotalCost, @ClosingPrice
	FROM Companies C WHERE C.Name = @Investment 
END
GO
