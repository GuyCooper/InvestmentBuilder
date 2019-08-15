SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_CreateAccount')
BEGIN
	DROP PROCEDURE sp_CreateAccount
END

GO

CREATE PROCEDURE sp_CreateAccount(@Name AS VARCHAR(30)
								  ,@Currency AS CHAR(3)
								  ,@AccountType AS VARCHAR(50)
								  ,@Enabled AS TINYINT					   
								  ,@Description AS VARCHAR(1024) = NULL
								  ,@Broker AS VARCHAR(30) = NULL
								  ) AS
BEGIN

	INSERT INTO dbo.Accounts 
	SELECT @Name,
		   @Description,
		   @Currency,
		   [Type_Id],
		   @Enabled,
		   @Broker
	FROM
		   dbo.AccountTypes
	WHERE
		   [Type] = @AccountType

	SELECT @@IDENTITY
END

GO