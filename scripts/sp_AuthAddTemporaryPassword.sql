USE [UserAuthentication]
GO

/****** Object:  StoredProcedure [dbo].[sp_AuthAddTemporaryPassword]    Script Date: 23/07/2021 21:54:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_AuthAddTemporaryPassword](@EMail NVARCHAR(256), @TemporaryPasswordHash NVARCHAR(max), @TemporarySalt NVARCHAR(max)) AS
										   
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

