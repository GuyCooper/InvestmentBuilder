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
	Name,
	[Password],
	[Description],
	Currency,
	[Enabled]
FROM
	Users
WHERE
	Name = @Account
END

GO
