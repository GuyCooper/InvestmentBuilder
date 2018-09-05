
/****** Object:  StoredProcedure [dbo].[sp_GetLatestInvestmentRecords]    Script Date: 20/10/2014 21:43:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_DeactivateCompany')
BEGIN
	DROP PROCEDURE sp_DeactivateCompany
END

GO

CREATE PROCEDURE [dbo].[sp_DeactivateCompany](@Name VARCHAR(50), @Account as VARCHAR(30)) AS
BEGIN

UPDATE
	 IR
SET
	 is_active = 0 
FROM 
	InvestmentRecord IR
INNER JOIN
	Companies C
ON
	IR.Company_id = C.Company_Id
INNER JOIN
	Accounts A
ON 
	IR.account_id = A.[Account_Id]
WHERE
    C.Name = @Name
AND
	A.Name = @Account
END
GO

