SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_UpdateClosingPrice')
BEGIN
	DROP PROCEDURE sp_UpdateClosingPrice
END

GO

CREATE PROCEDURE [dbo].[sp_UpdateClosingPrice](@valuationDate as DATETIME, @investment as VARCHAR(50), @closingPrice as float) AS
BEGIN

UPDATE IR SET IR.Selling_Price = @closingPrice
FROM InvestmentRecord AS IR JOIN Companies C ON IR.Company_Id = C.Company_Id
AND C.Name = @investment
AND IR.Valuation_Date = @valuationDate
 
END