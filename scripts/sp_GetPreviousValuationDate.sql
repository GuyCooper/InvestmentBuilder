
/****** Object:  StoredProcedure [dbo].[sp_GetPreviousValuationDate]    Script Date: 20/10/2014 21:43:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetPreviousValuationDate')
BEGIN
	DROP PROCEDURE sp_GetPreviousValuationDate
END

GO

CREATE PROCEDURE [dbo].[sp_GetPreviousValuationDate](@ValuationDate as DATETIME, @Account AS INT) AS
BEGIN

--return the previous date from valuation table
SELECT 
	TOP 1 V.Valuation_Date 
FROM
	Valuations V
WHERE
	V.[account_id] = @Account
AND
	V.Valuation_Date < @ValuationDate
ORDER BY
	 Valuation_Date Desc

END
