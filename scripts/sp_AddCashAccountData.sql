
/****** Object:  StoredProcedure [dbo].[sp_AddCashAccountData]    Script Date: 04/02/2016 17:50:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_AddCashAccountData')
BEGIN
	DROP PROCEDURE sp_AddCashAccountData
END

GO

CREATE PROCEDURE [dbo].[sp_AddCashAccountData](@ValuationDate as DateTime, @TransactionDate as DateTime, @TransactionType as varchar(20),
									@Parameter as varchar(50), @Amount as float, @Account as varchar(30)) AS
BEGIN

INSERT INTO CashAccount (valuation_date, transaction_date, [type_id], parameter, amount, [account_id])
SELECT 
	@ValuationDate,
	@TransactionDate,
	t.[type_id],
	@Parameter,
	@Amount,
	u.[user_id]
FROM
	TransactionType t, Users u 
WHERE
	t.[type] = @TransactionType 
AND
	u.Name = @Account		



END
GO

