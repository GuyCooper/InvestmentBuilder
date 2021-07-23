USE [UserAuthentication]
GO

/****** Object:  StoredProcedure [dbo].[sp_ValidateNewUser]    Script Date: 23/07/2021 21:58:08 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_ValidateNewUser](@Token NVARCHAR(265)) AS
BEGIN

DECLARE @UserId INT
DECLARE @Result INT

SET @Result = 0 -- result. 0 = not validated. 1 = validated

SELECT @UserId = [User_Id] FROM PasswordChange WHERE [Token] = @Token
IF(@@ROWCOUNT = 1)
BEGIN
		UPDATE UserDetails SET EmailConfirmed = 1 WHERE [Id] = @UserId
		SET @Result = 1

		DELETE FROM PasswordChange WHERE [User_Id] = @UserId
END

SELECT @Result

END

GO


