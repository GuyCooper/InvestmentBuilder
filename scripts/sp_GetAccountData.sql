SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetAccountData')
BEGIN
	DROP PROCEDURE sp_GetAccountData
END

GO

CREATE PROCEDURE sp_GetAccountData(@Account AS VARCHAR(30)) AS						 
BEGIN

SELECT
	A.Name,
	A.[Description],
	A.Currency,
	A.[Enabled],
	A.[Broker],
	T.[Type]
FROM
	Accounts A
INNER JOIN
	AccountTypes T
ON
	A.[Type_Id] = T.[Type_Id]
WHERE
	Name = @Account
END

GO
