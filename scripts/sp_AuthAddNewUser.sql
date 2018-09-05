
/****** Object:  StoredProcedure [dbo].[sp_AuthAddNewUser]    Script Date: 04/02/2018 17:50:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_AuthAddNewUser')
BEGIN
	DROP PROCEDURE sp_AuthAddNewUser
END

GO

CREATE PROCEDURE [dbo].[sp_AuthAddNewUser](@UserName NVARCHAR(256), @EMail NVARCHAR(256), @Salt NVARCHAR(max), 
										   @PasswordHash NVARCHAR(max), @PhoneNumber NVARCHAR(256), @TwoFactorEnabled BIT) AS
BEGIN

DECLARE @Result INT
DECLARE @NewId INT

IF(@EMail = '')
BEGIN
	SET @Result = 1
END
--ELSE IF(LENGTH(@PasswordHash) < 256)
--BEGIN
--	SET @Result = 2
--END
ELSE IF EXISTS(SELECT 1 FROM [UserDetails] WHERE [Email] = @Email) 
BEGIN
	SET @Result = 3
END
ELSE
BEGIN
	BEGIN TRANSACTION
	IF(@UserName = '') 
	BEGIN
		SET @UserName = @Email
	END
	
	INSERT INTO UserDetails(UserName,EMail,
							EmailConfirmed,
							PasswordHash,
							PhoneNumber,
							PhoneNumberConfirmed,
							TwoFactorEnabled,
							LoginCount, 
							AccessFailedCount,
							PasswordCreationDate,
							UserLastLogin)	
	VALUES(@UserName, @EMail, 0, @PasswordHash, @PhoneNumber, 0, @TwoFactorEnabled, 0, 0, GETDATE(), GETDATE())

	--if (@@ERROR) GOTO ON_ERROR

	SELECT @NewId = [Id]
	FROM UserDetails
	WHERE EMail = @EMail
	
	DELETE FROM UserSalt WHERE [User_Id] = @NewId
	
	INSERT INTO UserSalt ([User_Id], [Salt])
	VALUES (@NewId, @Salt)	
	SET @Result = 0
	GOTO ALLGOOD
ON_ERROR:
	ROLLBACK TRANSACTION
ALLGOOD:
	COMMIT TRANSACTION	
END

SELECT @Result

END
GO

