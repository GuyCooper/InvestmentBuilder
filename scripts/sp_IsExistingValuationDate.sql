SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_IsExistingValuationDate')
BEGIN
	DROP PROCEDURE sp_IsExistingValuationDate
END

GO

CREATE PROCEDURE sp_IsExistingValuationDate(@ValuationDate as DateTime) AS
BEGIN

SELECT 1 FROM Valuations 
WHERE Valuation_Date = @ValuationDate

END