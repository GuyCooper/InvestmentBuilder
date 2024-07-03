
/****** Object:  StoredProcedure [dbo].[sp_AuthUpdatePassword]    Script Date: 11/05/2024 17:50:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_AuthUpdatePassword')
BEGIN
	DROP PROCEDURE sp_AuthUpdatePassword
END

GO

CREATE PROCEDURE [dbo].[sp_AuthUpdatePassword](@Email NVARCHAR(256),  @Salt NVARCHAR(max), @PasswordHash NVARCHAR(max)) AS
										   
BEGIN

DECLARE @Result INT

IF EXISTS(SELECT 1 FROM [UserDetails] WHERE [Email] = @Email) 
BEGIN
	BEGIN TRANSACTION
	UPDATE [UserDetails] SET PasswordHash = @PasswordHash
	WHERE EMail = @Email

	DELETE FROM UserSalt
	WHERE User_Id IN (SELECT Id FROM UserDetails WHERE EMail = @Email)

	INSERT INTO UserSalt
	SELECT Id, @Salt
	FROM UserDetails
	WHERE EMail = @Email

	SET @Result = 0
	GOTO ALLGOOD
ON_ERROR:
	ROLLBACK TRANSACTION
ALLGOOD:
	COMMIT TRANSACTION	
END
ELSE
BEGIN
	SET @Result = 1
END

SELECT @Result

END
GO

