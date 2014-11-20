USE [ArgyllInvestments]
GO

/****** Object:  StoredProcedure [dbo].[sp_GetPreviousValuationDate]    Script Date: 20/10/2014 21:43:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetPreviousValuationDate](@PreviousDate as DATETIME output ) AS
BEGIN

--return the latest date from valuation table
SELECT top 1 @PreviousDate = Valuation_Date FROM dbo.Valuations
order by Valuation_Date desc

end
