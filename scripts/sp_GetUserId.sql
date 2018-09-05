
/****** Object:  StoredProcedure [dbo].[sp_AddNewShares]    Script Date: 20/10/2014 21:43:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetUserId')
BEGIN
	DROP PROCEDURE sp_GetUserId
END

GO

CREATE PROCEDURE [dbo].[sp_GetUserId](@UserName as nvarchar(256)) AS
BEGIN

SELECT [UserId] from dbo.[Users] where [UserName] = @UserName

END
GO
