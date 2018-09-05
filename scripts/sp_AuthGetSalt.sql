
/****** Object:  StoredProcedure [dbo].[sp_AuthGetSalt]    Script Date: 04/02/2018 17:50:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_AuthGetSalt')
BEGIN
	DROP PROCEDURE sp_AuthGetSalt
END

GO

CREATE PROCEDURE [dbo].[sp_AuthGetSalt](@EMail NVARCHAR(256)) AS
										   
BEGIN
	SELECT us.Salt
	FROM [UserDetails] ud
	INNER JOIN 
	[UserSalt] us
	ON ud.[Id] = us.[User_Id]
	WHERE
	ud.EMail = @EMail

END
GO

