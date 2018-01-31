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

CREATE PROCEDURE [dbo].[sp_GetUnitValuationRange](@Account as VARCHAR(30), @dateFrom as DATETIME, @dateTo as DATETIME) AS
BEGIN

SELECT  
	Unit_Price 
FROM
	Valuations V
INNER JOIN
	Accounts A
ON 
	V.account_id = A.[Account_Id]
WHERE
	Valuation_Date >= @dateFrom
	and
	Valuation_Date <= @dateTo
AND
	A.Name = @Account
END
GO


