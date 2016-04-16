/****** Object:  StoredProcedure [dbo].[sp_GetUnitValuation]    Script Date: 28/01/2015 18:20:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetStartOfYearValuation')
BEGIN
	DROP PROCEDURE sp_GetStartOfYearValuation
END

GO

CREATE PROCEDURE [dbo].sp_GetStartOfYearValuation(@valuationDate as DATETIME, @Account as VARCHAR(30)) AS
BEGIN

SELECT  top 1
	Unit_Price 
FROM
	Valuations V
INNER JOIN
	Users U
ON 
	V.account_id = U.[User_Id]
WHERE
	Valuation_Date < @valuationDate
AND
	U.Name = @Account
ORDER BY
	Valuation_Date DESC
END
GO


