SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_IsExistingRecordValuationDate')
BEGIN
	DROP PROCEDURE sp_IsExistingRecordValuationDate
END

GO

CREATE PROCEDURE sp_IsExistingRecordValuationDate(@ValuationDate as DateTime, @Account as INT) AS
BEGIN

SELECT
	 1 
FROM 
	InvestmentRecord IR
WHERE 
	IR.Valuation_Date = @ValuationDate
AND
	IR.[account_id] = @Account
END