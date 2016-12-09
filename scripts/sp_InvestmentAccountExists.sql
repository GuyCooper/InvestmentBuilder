SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_InvestmentAccountExists')
BEGIN
	DROP PROCEDURE sp_InvestmentAccountExists
END

GO

CREATE PROCEDURE sp_InvestmentAccountExists(@Name VARCHAR(30)) AS
BEGIN

SELECT
	 1 
FROM 
	Users
WHERE
	[Name] = @Name
END