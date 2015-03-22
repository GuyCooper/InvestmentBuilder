
/****** Object:  StoredProcedure [dbo].[sp_GetPreviousValuationDate]    Script Date: 20/10/2014 21:43:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetLatestValuationDate')
BEGIN
	DROP PROCEDURE sp_GetLatestValuationDate
END

GO

CREATE PROCEDURE [dbo].[sp_GetLatestValuationDate](@Account AS VARCHAR(30)) AS
BEGIN

--return the latest date from valuation table
SELECT 
	TOP 1 V.Valuation_Date 
FROM
	Valuations V
INNER JOIN 
	Users U
ON 
	V.[account_id] = U.[User_Id]
WHERE
	U.Name = @Account
ORDER BY
	 Valuation_Date desc

END
