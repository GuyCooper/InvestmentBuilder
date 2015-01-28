/****** Object:  StoredProcedure [dbo].[sp_GetBankBalance]    Script Date: 20/10/2014 21:43:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetBankBalance')
BEGIN
	DROP PROCEDURE sp_GetBankBalance
END

GO

CREATE PROCEDURE [dbo].[sp_GetBankBalance](@valuationDate as DATETIME) AS
BEGIN

--return the latest balance in hand amount
select top 1 ca.amount from dbo.CashAccount ca
inner join TransactionType tt
on ca.type_id = tt.type_id
and tt.type = 'BalanceInHandCF'
and ca.valuation_date = @valuationDate

END
GO

