USE [UserAuthentication]
GO

/****** Object:  StoredProcedure [dbo].[sp_ChangePassword]    Script Date: 23/07/2021 21:57:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_ChangePassword](@EMail NVARCHAR(256), @OldPasswordHash NVARCHAR(max), @NewPasswordHash NVARCHAR(max), @NewSalt NVARCHAR(max)) AS
										   
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

