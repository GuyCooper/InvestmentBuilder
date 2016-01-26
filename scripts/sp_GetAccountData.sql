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
	U.Name,
	U.[Password],
	U.[Description],
	U.Currency,
	U.[Enabled],
	U.[Broker],
	T.[Type]
FROM
	Users U
INNER JOIN
	UserTypes T
ON
	U.[Type_Id] = T.[Type_Id]
WHERE
	Name = @Account
END

GO
