SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_AddRedemption')
BEGIN
	DROP PROCEDURE sp_AddRedemption
END

GO

CREATE PROCEDURE sp_AddRedemption(@Account as int, @User as nvarchar(256), @TransactionDate as DateTime, @Amount as decimal, @Status as varchar(10)) AS
BEGIN

INSERT INTO Redemptions ([member_id], [transaction_date], [amount], [status])
SELECT member_id, @TransactionDate, @Amount, @Status
FROM
	Members M
INNER JOIN [Users] U
ON U.[UserId] = M.[UserId]
WHERE
	U.[UserName] = @User
AND
	M.[account_id] = @Account

END