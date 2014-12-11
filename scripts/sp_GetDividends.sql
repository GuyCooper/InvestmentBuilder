USE [ArgyllInvestments]
GO

/****** Object:  StoredProcedure [dbo].[sp_GetDividends]    Script Date: 20/10/2014 21:43:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_GetDividends](@previousValuationDate as DATETIME) AS
BEGIN

select ca.parameter as Company, ca.amount as Dividend 
from dbo.CashAccount ca
inner join TransactionType tt
on ca.type_id = tt.type_id
where tt.type = 'Dividend'
and ca.transaction_date > @previousValuationDate
order by ca.transaction_date desc 
 
END
GO

