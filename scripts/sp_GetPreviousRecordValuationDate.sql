
/****** Object:  StoredProcedure [dbo].[sp_GetPreviousValuationDate]    Script Date: 20/10/2014 21:43:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetPreviousRecordValuationDate')
BEGIN
	DROP PROCEDURE sp_GetPreviousRecordValuationDate
END

GO

CREATE PROCEDURE [dbo].sp_GetPreviousRecordValuationDate(@Account AS VARCHAR(30), @ValuationDate as DATETIME) AS
BEGIN

--returns the most previous valuation date in the investmentrecord table fromthe
--date specified in the parameter.this is used for calculating the monthly difference  
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
AND
	convert(date,IR.Valuation_Date)  = convert(date, @ValuationDate)
ORDER BY
	 Valuation_Date desc

END
