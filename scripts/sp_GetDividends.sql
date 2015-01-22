
/****** Object:  StoredProcedure [dbo].[sp_GetDividends]    Script Date: 20/10/2014 21:43:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetDividends')
BEGIN
	DROP PROCEDURE sp_GetDividends
END

GO

CREATE PROCEDURE [dbo].[sp_GetDividends](@valuationDate as DATETIME) AS
BEGIN

select ca.parameter as Company, ca.amount as Dividend 
from dbo.CashAccount ca
inner join TransactionType tt
on ca.type_id = tt.type_id
where tt.type = 'Dividend'
and ca.valuation_date = @valuationDate
order by ca.valuation_date desc 
 
END
GO

