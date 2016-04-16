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
	 M.Name, M.[Authorization]
FROM 
	Members M
INNER JOIN
	Users U 
ON
	M.account_id = U.[User_Id]
WHERE 
	U.Name = @Account AND
	M.[Enabled] = 1
END