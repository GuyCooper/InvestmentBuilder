
GO

/****** Object:  StoredProcedure [dbo].[sp_AddCashAccountData]    Script Date: 04/02/2016 17:50:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_RemoveCashAccountData')
BEGIN
	DROP PROCEDURE sp_RemoveCashAccountData
END

GO

CREATE PROCEDURE [dbo].sp_RemoveCashAccountData(@TransactionID AS INT) AS
BEGIN

DELETE FROM CashAccount
WHERE [transaction_id] = @TransactionID

END
GO

