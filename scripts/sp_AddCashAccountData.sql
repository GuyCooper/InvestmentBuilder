SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_AddCashAccountData')
BEGIN
	DROP PROCEDURE sp_AddCashAccountData
END

GO

CREATE PROCEDURE sp_AddCashAccountData(@ValuationDate as DateTime, @TransactionDate as DateTime, @TransactionType as varchar(20),
									@Parameter as varchar(50), @Amount as float) AS
BEGIN

INSERT INTO CashAccount (valuation_date, transaction_date, [type_id], parameter, amount )
SELECT 
	@ValuationDate,
	@TransactionDate,
	[type_id],
	@Parameter,
	@Amount
FROM
	TransactionType
WHERE
	[type] = @TransactionType	
END