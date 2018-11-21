
/****** Object:  StoredProcedure [dbo].[sp_AuthAddTemporaryPassword]    Script Date: 04/02/2018 17:50:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_AuthAddTemporaryPassword')
BEGIN
	DROP PROCEDURE sp_AuthAddTemporaryPassword
END

GO

CREATE PROCEDURE [dbo].sp_AuthAddTemporaryPassword(@EMail NVARCHAR(256), @TemporaryPasswordHash NVARCHAR(max), @TemporarySalt NVARCHAR(max)) AS
										   
BEGIN

DECLARE @Result INT
DECLARE @UserId INT

IF(@Email = '')
BEGIN
	SET @Result = 0
END
ELSE
BEGIN
	SELECT @UserId = [Id]
	FROM UserDetails
	WHERE Email = @EMail

	SET @Result = @@ROWCOUNT
	IF(@Result <> 1)
	BEGIN
		SET @Result = 0
	END
	ELSE
	BEGIN
		BEGIN TRANSACTION
			UPDATE UserDetails
			SET PasswordHash = @TemporaryPasswordHash, PasswordCreationDate = GETDATE(), [TemporaryPassword] = 1
			FROM UserDetails
			WHERE Email = @EMail

			DELETE FROM UserSalt WHERE [User_Id] = @UserId
	
			INSERT INTO UserSalt ([User_Id], [Salt])
			VALUES (@UserId, @TemporarySalt)	

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

