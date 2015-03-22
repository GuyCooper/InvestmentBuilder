
/****** Object:  StoredProcedure [dbo].[sp_GetMemberSubscriptionAmount]    Script Date: 20/10/2014 21:43:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetMemberSubscriptionAmount')
BEGIN
	DROP PROCEDURE sp_GetMemberSubscriptionAmount
END

GO

CREATE PROCEDURE [dbo].[sp_GetMemberSubscriptionAmount](@Member as varchar(50), @ValuationDate as DATETIME, @Account as VARCHAR(30)) AS
BEGIN

SELECT ca.amount 
FROM 
CashAccount ca
INNER JOIN TransactionType tt
ON
ca.type_id = tt.type_id
INNER JOIN Users U
ON
ca.account_id = U.[User_Id]
WHERE ca.valuation_date = @ValuationDate
and ca.parameter = @Member
and tt.[type] = 'Subscription'
and U.Name = @Account
END
GO

