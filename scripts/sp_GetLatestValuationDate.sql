USE [ArgyllInvestments]
GO

/****** Object:  StoredProcedure [dbo].[sp_AddNewShares]    Script Date: 20/10/2014 21:43:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetLatestValuationDate](@valuationDate as DATETIME out) AS
BEGIN

DECLARE @latestDate DATETIME

SELECT @latestDate = MAX(Valuation_Date) FROM InvestmentRecord

end
