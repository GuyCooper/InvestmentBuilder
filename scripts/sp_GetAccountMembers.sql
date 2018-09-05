SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetAccountMembers')
BEGIN
	DROP PROCEDURE sp_GetAccountMembers
END

GO

CREATE PROCEDURE sp_GetAccountMembers(@Account as VARCHAR(30), @ValuationDate as DATETIME) AS
BEGIN

SELECT
	 U.UserName, M.[Authorization]
FROM 
	Members M
INNER JOIN
	Accounts A
ON
	M.account_id = A.[Account_Id]
INNER JOIN 
	[Users] U
ON U.[UserId] = M.[UserId]
WHERE 
	A.Name = @Account AND
	M.[Enabled] = 1
END