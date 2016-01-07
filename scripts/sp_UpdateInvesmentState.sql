SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_UpdateInvesmentState')
BEGIN
	DROP PROCEDURE sp_UpdateInvesmentState
END

GO

CREATE PROCEDURE sp_UpdateInvesmentState(@ValuationDate as DATETIME, @company as VARCHAR(50), @account as varchar(30)) AS
BEGIN

UPDATE 
	IR
SET
	IR.is_active = 0
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