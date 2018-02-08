
/****** Object:  StoredProcedure [dbo].[sp_AuthRemoveUser]    Script Date: 04/02/2018 17:50:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_AuthRemoveUser')
BEGIN
	DROP PROCEDURE sp_AuthRemoveUser
END

GO

CREATE PROCEDURE [dbo].[sp_AuthRemoveUser](@EMail NVARCHAR(256)) AS
										   
BEGIN
	DELETE FROM [UserDetails]
	WHERE EMail = @EMail
END
GO

