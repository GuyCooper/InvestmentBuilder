
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

CREATE PROCEDURE [dbo].sp_GetLatestRecordValuationDate(@Account AS INT) AS
BEGIN

--return the latest date from valuation table
SELECT 
	TOP 1 IR.Valuation_Date 
FROM
	InvestmentRecord IR
WHERE
	IR.[account_id] = @Account
AND
	IR.is_active = 1
ORDER BY
	 Valuation_Date desc

END
