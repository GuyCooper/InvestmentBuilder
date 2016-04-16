
GO

/****** Object:  StoredProcedure [dbo].[sp_GetUserCompanies]    Script Date: 19/03/2015 19:49:12 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetUserCompanies')
BEGIN
	DROP PROCEDURE sp_GetUserCompanies
END

GO

CREATE PROCEDURE [dbo].[sp_GetUserCompanies](@ValuationDate as DATETIME, @Account as VARCHAR(30)) AS
BEGIN

SELECT 
	C.Name as Name,
	IR.Selling_Price as Price
FROM 
	InvestmentRecord IR
INNER JOIN
	Companies C
ON
	IR.Company_id = C.Company_Id
INNER JOIN
	Users U
ON 
	IR.account_id = U.[User_Id]
WHERE
    IR.Valuation_Date = @ValuationDate
AND
	IR.is_active = 1
AND
	U.Name = @Account
END

GO


