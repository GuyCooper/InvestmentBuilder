SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_SellShares')
BEGIN
	DROP PROCEDURE sp_SellShares
END

GO

CREATE PROCEDURE sp_SellShares(@ValuationDate as DATETIME, @company as VARCHAR(50), @shares as INT, @account as varchar(30)) AS
BEGIN

UPDATE 
	IR
SET
	IR.Shares_Sold += @shares
FROM 
	investmentRecord AS IR
INNER JOIN 
	Companies C 
ON IR.Company_Id = C.Company_Id
INNER JOIN Users U
ON IR.account_id = U.[User_Id]
AND C.Name = @company
AND IR.Valuation_Date = @ValuationDate
AND U.Name = @account

END