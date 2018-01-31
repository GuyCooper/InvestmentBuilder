/****** Object:  StoredProcedure [dbo].[sp_GetUnitValuation]    Script Date: 28/01/2015 18:20:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetUserData')
BEGIN
	DROP PROCEDURE sp_GetUserData
END

GO

CREATE PROCEDURE [dbo].sp_GetUserData(@Name as varchar(30)) AS
BEGIN

SELECT 
	 Currency, [Description] , [Broker]
FROM
	Accounts
WHERE
	Name = @Name
END
GO


