SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetAuthorizationLevel')
BEGIN
	DROP PROCEDURE sp_GetAuthorizationLevel
END

GO

CREATE PROCEDURE sp_GetAuthorizationLevel(@User AS VARCHAR(50), @Account AS VARCHAR(30)) AS						 
BEGIN

SELECT
	 m.[Authorization]
FROM
	Members m
INNER JOIN
	Users u
ON
	m.account_id = u.[User_Id]
WHERE
	u.[Name] = @Account
AND
	m.[Name] = @User

END
	
