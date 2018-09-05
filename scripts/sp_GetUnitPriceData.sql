
/****** Object:  StoredProcedure [dbo].[sp_GetPreviousValuationDate]    Script Date: 20/10/2014 21:43:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetUnitPriceData')
BEGIN
	DROP PROCEDURE sp_GetUnitPriceData
END

GO

CREATE PROCEDURE [dbo].[sp_GetUnitPriceData](@Account AS VARCHAR(30)) AS
BEGIN

--return the latest date from valuation table
SELECT 
	 V.Valuation_Date as Valuation_Date,
	 V.Unit_Price as Unit_Price 
FROM
	Valuations V
INNER JOIN 
	Accounts A
ON 
	V.[account_id] = A.[Account_Id]
WHERE
	A.Name = @Account
ORDER BY
	 Valuation_Date ASC

END
