SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_RecentValuationDates')
BEGIN
	DROP PROCEDURE sp_RecentValuationDates
END

GO

CREATE PROCEDURE sp_RecentValuationDates(@Account as VARCHAR(30), @DateFrom as DATETIME ) AS
BEGIN

SELECT
	 TOP 5 V.Valuation_Date 
FROM
	 Valuations V
INNER JOIN
	 Accounts A
ON	
	V.[account_id] = A.[Account_Id]
WHERE
	A.Name = @Account
AND
	v.[Valuation_Date] < @DateFrom
ORDER BY
	 Valuation_Date DESC

END