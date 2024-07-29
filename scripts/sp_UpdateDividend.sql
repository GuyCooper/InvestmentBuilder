SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_UpdateDividend')
BEGIN
	DROP PROCEDURE sp_UpdateDividend
END

GO

CREATE PROCEDURE [dbo].[sp_UpdateDividend](@valuationDate as DATETIME, @company as VARCHAR(50), @dividend as decimal(18,2), @account as INT) AS
BEGIN

UPDATE
	IR
SET
	IR.Dividends_Received += @dividend
FROM 
	InvestmentRecord AS IR 
INNER JOIN
	 Companies C 
ON IR.Company_Id = C.Company_Id
WHERE
C.Name = @company
AND IR.Valuation_Date = @valuationDate
AND IR.account_id = @account
 
END