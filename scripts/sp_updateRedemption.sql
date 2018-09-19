SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_UpdateRedemption')
BEGIN
	DROP PROCEDURE sp_UpdateRedemption
END

GO

CREATE PROCEDURE sp_UpdateRedemption(@Account as int, @User as varchar(50), @TransactionDate as DateTime, @Amount as float,
						@UnitsRedeemed as float,  @Status as varchar(10)) AS
BEGIN

DECLARE @Member_ID INT

SELECT 
	@Member_ID = M.[member_id]
FROM 
	Members M
INNER JOIN
	[Users] U
ON 
	M.[UserId] = U.[UserId]
WHERE
	U.UserName = @User
AND
	M.[Account_id]= @Account

UPDATE
	 Redemptions				  
SET 
	[amount] = @Amount,
	[units] = @UnitsRedeemed,
	[status] = @Status
WHERE
	member_id = @Member_ID
AND
	transaction_date = @TransactionDate	

END