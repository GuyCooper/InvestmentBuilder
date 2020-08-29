SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_AddTransactionHistory')
BEGIN
	DROP PROCEDURE sp_AddTransactionHistory
END

GO

CREATE PROCEDURE sp_AddTransactionHistory(@valuationDate AS datetime, @transactionDate AS datetime, @company as varchar(50), 
										  @action as varchar(10), @quantity as FLOAT,
										  @total_cost as float, @account as int, @user as varchar(50)) AS
BEGIN

DECLARE @Company_Id INT

SELECT @Company_Id = [Company_Id]
FROM Companies
WHERE [Name] = @company

INSERT INTO TransactionHistory 
	([account_id],[valuation_date], [transaction_date], [company_id], [trade_action], [quantity], [total_cost], [user])
VALUES
	(@Account, @valuationDate, @transactionDate, @Company_Id, @action, @quantity, @total_cost, @user)
END