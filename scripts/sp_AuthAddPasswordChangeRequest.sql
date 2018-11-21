
/****** Object:  StoredProcedure [dbo].[sp_AuthChangePassword]    Script Date: 04/02/2018 17:50:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_AuthAddPasswordChangeRequest')
BEGIN
	DROP PROCEDURE sp_AuthAddPasswordChangeRequest
END

GO

CREATE PROCEDURE [dbo].[sp_AuthAddPasswordChangeRequest](@EMail NVARCHAR(256), @Token NVARCHAR(256)) AS									   
BEGIN

DECLARE @UserId INT
DECLARE @Result INT

SET @Result = 1  -- return 1  = invalid email, 0 = success

SELECT @UserId = [Id] FROM UserDetails WHERE EMail = @EMail
IF(@@ROWCOUNT = 1)
BEGIN
	SET @Result = 0
	IF EXISTS(SELECT 1 FROM PasswordChange WHERE [User_Id] = @UserId)
	BEGIN
		UPDATE PasswordChange 
		SET [Token]  = @Token, [AddTime] = GETDATE()
		WHERE [User_Id] = @UserId
	END
	ELSE
	BEGIN
		INSERT INTO PasswordChange
		VALUES (@UserId, @Token, GETDATE())
	END
END

RETURN @Result

END
GO

