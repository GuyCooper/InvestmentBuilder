SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetCompanyData')
BEGIN
	DROP PROCEDURE sp_GetCompanyData
END

GO

CREATE PROCEDURE sp_GetCompanyData(@Name as VARCHAR(50)) AS
BEGIN

SELECT Symbol, Currency, ScalingFactor FROM Companies where  Name = @Name
 
END