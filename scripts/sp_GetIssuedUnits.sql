SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetIssuedUnits')
BEGIN
	DROP PROCEDURE sp_GetIssuedUnits
END

GO

CREATE PROCEDURE [dbo].sp_GetIssuedUnits(@ValuationDate as DateTime, @Account as int) AS
BEGIN
	SELECT 
		SUM(mca.Units)
	FROM 
		Members m
	INNER JOIN 
		MembersCapitalAccount mca
	ON 
		m.Member_Id = mca.Member_Id
	WHERE
		mca.Valuation_Date = @ValuationDate
	AND
		m.account_id = @Account
END
