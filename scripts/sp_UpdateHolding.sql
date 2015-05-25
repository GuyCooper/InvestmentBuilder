SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_UpdateHolding')
BEGIN
	DROP PROCEDURE sp_UpdateHolding
END

GO

CREATE PROCEDURE [dbo].[sp_UpdateHolding](@holding as INT, @valuationDate as DATETIME, @company as VARCHAR(50), @account as VARCHAR(30)) AS
BEGIN

UPDATE	
	 IR
SET
	IR.Shares_Bought = @holding
FROM InvestmentRecord AS IR 
INNER JOIN
	 Companies C 
ON 
	IR.Company_Id = C.Company_Id
INNER JOIN
	Users U
ON IR.account_id = U.[User_Id]	
AND C.Name = @company
AND IR.Valuation_Date = @valuationDate
AND U.Name = @account

END