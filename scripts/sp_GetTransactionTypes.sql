SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetTransactionTypes')
BEGIN
	DROP PROCEDURE sp_GetTransactionTypes
END

GO

CREATE PROCEDURE sp_GetTransactionTypes(@side as char(1)) AS
BEGIN

SELECT [type] FROM TransactionType WHERE side = @side

END