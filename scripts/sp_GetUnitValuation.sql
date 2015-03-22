/****** Object:  StoredProcedure [dbo].[sp_GetUnitValuation]    Script Date: 28/01/2015 18:20:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetUnitValuation')
BEGIN
	DROP PROCEDURE sp_GetUnitValuation
END

GO

CREATE PROCEDURE [dbo].[sp_GetUnitValuation](@valuationDate as DATETIME, @Account as VARCHAR(30)) AS
BEGIN

SELECT  
	Unit_Price 
FROM
	Valuations V
INNER JOIN
	Users U
ON 
	V.account_id = U.[User_Id]
WHERE
	Valuation_Date = @valuationDate
AND
	U.Name = @Account
END
GO


