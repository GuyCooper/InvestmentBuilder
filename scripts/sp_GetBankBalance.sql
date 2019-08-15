
/****** Object:  StoredProcedure [dbo].[sp_GetBankBalance]    Script Date: 04/02/2016 17:47:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetBankBalance')
BEGIN
	DROP PROCEDURE sp_GetBankBalance
END

GO

CREATE PROCEDURE [dbo].[sp_GetBankBalance](@ValuationDate as DATETIME, @Account as INT) AS
BEGIN

--return the latest balance in hand amount
select sum(ca.amount) from dbo.CashAccount ca
inner join TransactionType tt
on ca.type_id = tt.type_id
and tt.type = 'BalanceInHandCF'
and ca.valuation_date = @ValuationDate
and ca.account_id = @Account
END

GO

