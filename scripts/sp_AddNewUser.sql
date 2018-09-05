
/****** Object:  StoredProcedure [dbo].[sp_AddNewUser]    Script Date: 20/10/2014 21:43:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_AddNewUser')
BEGIN
	DROP PROCEDURE sp_AddNewUser
END

GO

CREATE PROCEDURE [dbo].[sp_AddNewUser](@UserName AS NVARCHAR(256), @Description AS NVARCHAR(256)) AS
BEGIN

IF NOT EXISTS(SELECT 1 FROM [Users] WHERE [UserName] = @UserName)
BEGIN

INSERT INTO [Users] ([UserName],[Description])
VALUES (@UserName, @Description)
END

END
GO
