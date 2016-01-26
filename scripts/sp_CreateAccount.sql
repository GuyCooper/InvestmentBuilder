SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_CreateAccount')
BEGIN
	DROP PROCEDURE sp_CreateAccount
END

GO

CREATE PROCEDURE sp_CreateAccount(@Name AS VARCHAR(30), @Password AS VARCHAR(1024),
								  @Description AS VARCHAR(1024), @Currency AS CHAR(3), 
								  @AccountType AS VARCHAR(50), @Enabled AS TINYINT,
								  @Broker AS VARCHAR(30) ) AS
BEGIN

IF NOT EXISTS (SELECT 1 FROM Users WHERE Name = @Name)
BEGIN
	INSERT INTO dbo.Users 
	SELECT @Name,
		   @Password,
		   @Description,
		   @Currency,
		   [Type_Id],
		   0,
		   @Broker
	FROM
		   dbo.UserTypes
	WHERE
		   [Type] = @AccountType
END

	UPDATE dbo.Users
	SET 
		[Password] = @Password,
		[Description] = @Description,
		[Currency] = @Currency,
		[Enabled] = @Enabled,
		[Broker]  = @Broker
	WHERE  Name = @Name
	 
END
GO