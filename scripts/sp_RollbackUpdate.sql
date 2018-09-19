
/****** Object:  StoredProcedure [dbo].[sp_GetMemberSubscriptionAmount]    Script Date: 20/10/2014 21:43:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_RollbackUpdate')
BEGIN
	DROP PROCEDURE sp_RollbackUpdate
END

GO

CREATE PROCEDURE [dbo].sp_RollbackUpdate(@Account as int, @ValuationDate as DATETIME) AS
BEGIN

DELETE FROM
	Valuations	
WHERE
	Valuation_Date = @ValuationDate
AND
	account_id = @Account

DELETE FROM
	MembersCapitalAccount
WHERE
	Valuation_Date = @ValuationDate

END
