USE [UserAuthentication]
GO

/****** Object:  StoredProcedure [dbo].[sp_AuthAddPasswordChangeRequest]    Script Date: 23/07/2021 21:54:06 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
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

