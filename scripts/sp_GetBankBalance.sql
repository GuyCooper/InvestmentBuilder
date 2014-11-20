USE [ArgyllInvestments]
GO

/****** Object:  StoredProcedure [dbo].[sp_GetBankBalance]    Script Date: 20/10/2014 21:43:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].sp_GetBankBalance(@valuationDate as DATETIME) AS
BEGIN

declare @BankBalance float

--return the latest balance in hand amount
select top 1 @BankBalance = ca.amount from dbo.CashAccount ca
inner join TransactionType tt
on ca.type_id = tt.type_id
and tt.type = 'Balance In Hand'
order by ca.transaction_date desc 

return @BankBalance

END
GO

