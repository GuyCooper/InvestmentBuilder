USE [UserAuthentication]
GO

/****** Object:  StoredProcedure [dbo].[sp_AuthGetSalt]    Script Date: 23/07/2021 21:56:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
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

