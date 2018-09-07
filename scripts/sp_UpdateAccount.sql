SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_UpdateAccount')
BEGIN
	DROP PROCEDURE sp_UpdateAccount
END

GO

CREATE PROCEDURE sp_UpdateAccount(@Name AS VARCHAR(30)
								  ,@Currency AS CHAR(3)
								  ,@AccountType AS VARCHAR(50)
								  ,@Enabled AS TINYINT					   
								  ,@Description AS VARCHAR(1024) = NULL
								  ,@Broker AS VARCHAR(30) = NULL
								  ) AS
BEGIN

IF EXISTS (SELECT 1 FROM Accounts WHERE Name = @Name)
BEGIN
	UPDATE dbo.Accounts
	SET 
		[Description] = @Description,
		[Currency] = @Currency,
		[Enabled] = @Enabled,
		[Broker]  = @Broker
	WHERE  Name = @Name	 
END
END
GO