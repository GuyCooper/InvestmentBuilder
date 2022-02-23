
/****** Object:  StoredProcedure [dbo].[sp_GetPreviousValuation]    Script Date: 20/10/2014 21:43:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetLatestValuation')
BEGIN
	DROP PROCEDURE sp_GetLatestValuation
END

GO

CREATE PROCEDURE [dbo].[sp_GetLatestValuation](@Account AS INT) AS
BEGIN

--return the latest record from valuation table
SELECT 
	TOP 1 V.Valuation_Date,
		  V.Unit_Price
FROM
	Valuations V
WHERE
	V.[account_id] = @Account
ORDER BY
	 Valuation_Date desc

END
