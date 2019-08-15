/****** Object:  StoredProcedure [dbo].[sp_GetUnitValuation]    Script Date: 28/01/2015 18:20:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetUnitValuationRange')
BEGIN
	DROP PROCEDURE sp_GetUnitValuationRange
END

GO

CREATE PROCEDURE [dbo].[sp_GetUnitValuationRange](@Account as INT, @dateFrom as DATETIME, @dateTo as DATETIME) AS
BEGIN

SELECT  
	Unit_Price 
FROM
	Valuations V
WHERE
	Valuation_Date >= @dateFrom
AND
	Valuation_Date <= @dateTo
AND
	V.account_id = @Account
END
GO


