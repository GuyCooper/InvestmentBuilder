USE [UserAuthentication]
GO

/****** Object:  StoredProcedure [dbo].[sp_AuthChangePassword]    Script Date: 23/07/2021 21:55:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_AuthChangePassword](@EMail NVARCHAR(256), @Token NVARCHAR(256), @NewPasswordHash NVARCHAR(max), @NewSalt NVARCHAR(max)) AS
										   
BEGIN

DECLARE @Result INT
DECLARE @UserId INT
DECLARE @TokenDate DATETIME

SET @Result = 0

--first validate the email address and token
SELECT @TokenDate = pc.AddTime, @UserId = pc.[User_Id]
FROM PasswordChange pc
INNER JOIN UserDetails ud
ON ud.[Id] = pc.[User_Id]
WHERE ud.[EMail] = @EMail
and pc.[Token] = @Token

IF(@@ROWCOUNT = 1)
BEGIN
	--if tokendate is more than 1 hour old then do not update password
	IF (GETDATE() < DATEADD(hour, 1, @TokenDate)) 		
	BEGIN
		BEGIN TRANSACTION
		-- update userdetails table with new password
		UPDATE UserDetails
		SET PasswordHash = @NewPasswordHash, PasswordCreationDate = GETDATE(), [TemporaryPassword] = 0
		FROM UserDetails
		WHERE Email = @EMail

		--update usersalt table with new salt
		DELETE FROM UserSalt WHERE [User_Id] = @UserId
	
		INSERT INTO UserSalt ([User_Id], [Salt])
		VALUES (@UserId, @NewSalt)	

		GOTO ALLGOOD
	ON_ERROR:
			ROLLBACK TRANSACTION
	ALLGOOD:
			COMMIT TRANSACTION
			SET @Result = 1			
	END
END

--for security reasons we always remove the token from the PasswordChange table regardless of the outcome

DELETE FROM PasswordChange
WHERE [User_Id] = 
(SELECT [Id] FROM UserDetails WHERE EMail = @EMail)

SELECT @Result

END

GO

