SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_UpdateHolding')
BEGIN
	DROP PROCEDURE sp_UpdateHolding
END

GO

CREATE PROCEDURE [dbo].[sp_UpdateHolding](@holding as INT, @valuationDate as DATETIME, @investment as VARCHAR(50)) AS
BEGIN

UPDATE IR SET IR.Shares_Bought = @holding
FROM InvestmentRecord AS IR JOIN Companies C ON IR.Company_Id = C.Company_Id
AND C.Name = @investment
AND IR.Valuation_Date = @valuationDate

END