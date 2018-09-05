
/****** Object:  StoredProcedure [dbo].[sp_AuthAddNewUser]    Script Date: 04/02/2018 17:50:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_AuthenticateUser')
BEGIN
	DROP PROCEDURE sp_AuthenticateUser
END

GO

CREATE PROCEDURE [dbo].[sp_AuthenticateUser](@EMail NVARCHAR(256), @PasswordHash NVARCHAR(max)) AS
										   
BEGIN

DECLARE @Result INT

IF(@Email = '' OR @PasswordHash = '')
BEGIN
	SET @Result = 0
END
ELSE
BEGIN
	SELECT @Result = COUNT(*) 
	FROM UserDetails
	WHERE Email = @EMail
	AND PasswordHash = @PasswordHash

	IF(@Result = 0)
	BEGIN
		UPDATE UserDetails
		SET AccessFailedCount = AccessFailedCount+1
		FROM UserDetails
		WHERE Email = @EMail
	END
	ELSE
	BEGIN
		UPDATE UserDetails
		SET LoginCount = LoginCount+1,
		UserLastLogin = GETDATE()
		FROM UserDetails
		WHERE Email = @EMail
	END
END

SELECT @Result

END
GO

