SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetIssuedUnits')
BEGIN
	DROP PROCEDURE sp_GetIssuedUnits
END

GO

CREATE PROCEDURE [dbo].sp_GetIssuedUnits(@ValuationDate as DateTime, @AccountName as varchar(30)) AS
BEGIN
	SELECT 
		SUM(mca.Units)
	FROM 
		Members m
	INNER JOIN 
		MembersCapitalAccount mca
	ON 
		m.Member_Id = mca.Member_Id
	INNER JOIN
		Users U
	ON
		m.account_id = U.[User_Id]
	WHERE
		mca.Valuation_Date = @ValuationDate
	AND
		U.Name = @AccountName
END