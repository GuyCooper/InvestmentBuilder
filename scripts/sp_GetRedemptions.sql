SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetRedemptions')
BEGIN
	DROP PROCEDURE sp_GetRedemptions
END

GO

CREATE PROCEDURE sp_GetRedemptions(@Account as varchar(30), @TransactionDate as DateTime) AS
BEGIN

SELECT M.[Name], R.[amount], R.[transaction_date], R.[status]
FROM
	Redemptions R
INNER JOIN	Members M
ON
	R.[member_id] = M.[Member_Id]
INNER JOIN
	Users U
ON
	M.account_id = U.[User_Id]
WHERE
	U.[Name] = @Account					  
AND
	R.[transaction_date] > @TransactionDate 
END