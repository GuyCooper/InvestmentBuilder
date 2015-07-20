
/****** Object:  StoredProcedure [dbo].[sp_GetPreviousValuationDate]    Script Date: 20/10/2014 21:43:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetLatestRecordValuationDate')
BEGIN
	DROP PROCEDURE sp_GetLatestRecordValuationDate
END

GO

CREATE PROCEDURE [dbo].sp_GetLatestRecordValuationDate(@Account AS VARCHAR(30)) AS
BEGIN

--return the latest date from valuation table
SELECT 
	TOP 1 IR.Valuation_Date 
FROM
	InvestmentRecord IR
INNER JOIN 
	Users U
ON 
	IR.[account_id] = U.[User_Id]
WHERE
	U.Name = @Account
AND
	IR.is_active = 1
ORDER BY
	 Valuation_Date desc

END
