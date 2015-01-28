
/****** Object:  StoredProcedure [dbo].[sp_GetMembersCapitalAccount]    Script Date: 20/10/2014 21:43:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetMembersCapitalAccount')
BEGIN
	DROP PROCEDURE sp_GetMembersCapitalAccount
END

GO

CREATE PROCEDURE [dbo].[sp_GetMembersCapitalAccount](@ValuationDate as DATETIME) AS
BEGIN

SELECT
	M.Name as Member, MCA.Units as Units
FROM 
	MembersCapitalAccount MCA
INNER JOIN	 Members M
ON MCA.Member_Id =M.Member_Id
WHERE
	Valuation_Date = @valuationDate
END
GO


