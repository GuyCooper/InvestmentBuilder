SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetAccountsForUser')
BEGIN
	DROP PROCEDURE sp_GetAccountsForUser
END

GO

CREATE PROCEDURE sp_GetAccountsForUser(@User AS VARCHAR(50), @CheckAdmin AS BIT) AS						 
BEGIN

IF @CheckAdmin = 1 AND EXISTS (SELECT [Name] FROM [Administrators] WHERE Name = @User)
	SELECT Name FROM [Accounts] WHERE [Enabled] = 1
ELSE
BEGIN
	SELECT
		 A.[Name]
	FROM
		 [Accounts] A
	INNER JOIN
		 [Members] M
	ON
		A.[Account_id] = M.[Account_Id]
	WHERE
		M.[Name] = @User
	AND
		A.[Enabled] = 1
END
END
	
