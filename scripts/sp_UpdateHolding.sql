SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_UpdateHolding')
BEGIN
	DROP PROCEDURE sp_UpdateHolding
END

GO

CREATE PROCEDURE [dbo].[sp_UpdateHolding](@quantity as decimal, @valuationDate as DATETIME, @company as VARCHAR(50), @account as INT) AS
BEGIN

UPDATE	
	 IR
SET
	IR.Shares_Bought = @quantity,
	IR.[Bonus_Shares issued] = 0,
	IR.Shares_Sold = 0
FROM InvestmentRecord AS IR 
INNER JOIN
	 Companies C 
ON 
	IR.Company_Id = C.Company_Id
WHERE
IR.account_id = @account
AND C.Name = @company
AND IR.Valuation_Date = @valuationDate


END