SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_IsExistingValuationDate')
BEGIN
	DROP PROCEDURE sp_IsExistingValuationDate
END

GO

CREATE PROCEDURE sp_IsExistingValuationDate(@ValuationDate as DateTime, @Account as varchar(30)) AS
BEGIN

SELECT
	 1 
FROM 
	Valuations V
INNER JOIN 
	Users U
ON 
	V.[account_id] = U.[User_Id]		 
WHERE 
	V.Valuation_Date = @ValuationDate
AND
	U.Name = @Account
END