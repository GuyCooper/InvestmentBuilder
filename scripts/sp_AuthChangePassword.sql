
/****** Object:  StoredProcedure [dbo].[sp_AuthChangePassword]    Script Date: 04/02/2018 17:50:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_AuthChangePassword')
BEGIN
	DROP PROCEDURE sp_AuthChangePassword
END

GO

CREATE PROCEDURE [dbo].[sp_AuthChangePassword](@EMail NVARCHAR(256), @OldPasswordHash NVARCHAR(max), @NewPasswordHash NVARCHAR(max), @NewSalt NVARCHAR(max)) AS
										   
BEGIN

DECLARE @Result INT
DECLARE @NewId INT

IF(@Email = '' OR @OldPasswordHash = '' OR @NewPasswordHash = '')
BEGIN
	SET @Result = 0
END
ELSE
BEGIN
	SELECT @NewId = [Id]
	FROM UserDetails
	WHERE Email = @EMail
	AND PasswordHash = @OldPasswordHash

	SET @Result = @@ROWCOUNT
	IF(@Result <> 1)
	BEGIN
		SET @Result = 0
	END
	ELSE
	BEGIN
		BEGIN TRANSACTION
			UPDATE UserDetails
			SET PasswordHash = @NewPasswordHash, PasswordCreationDate = GETDATE()
			FROM UserDetails
			WHERE Email = @EMail

			DELETE FROM UserSalt WHERE [User_Id] = @NewId
	
			INSERT INTO UserSalt ([User_Id], [Salt])
			VALUES (@NewId, @NewSalt)	

			GOTO ALLGOOD
ON_ERROR:
		ROLLBACK TRANSACTION
ALLGOOD:
		COMMIT TRANSACTION			
	END
END

SELECT @Result

END
GO

