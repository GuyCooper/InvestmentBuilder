SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_IsExistingRecordValuationDate')
BEGIN
	DROP PROCEDURE sp_IsExistingRecordValuationDate
END

GO

CREATE PROCEDURE sp_IsExistingRecordValuationDate(@ValuationDate as DateTime, @Account as varchar(30)) AS
BEGIN

SELECT
	 1 
FROM 
	InvestmentRecord IR
INNER JOIN 
	Users U
ON 
	IR.[account_id] = U.[User_Id]		 
WHERE 
	IR.Valuation_Date = @ValuationDate
AND
	U.Name = @Account
END