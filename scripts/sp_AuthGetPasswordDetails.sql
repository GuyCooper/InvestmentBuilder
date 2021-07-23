USE [UserAuthentication]
GO

/****** Object:  StoredProcedure [dbo].[sp_GetPasswordDetails]    Script Date: 23/07/2021 21:57:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[sp_GetPasswordDetails](@EMail NVARCHAR(256)) AS
										   
BEGIN

SELECT TemporaryPassword, PasswordCreationDate
FROM UserDetails
WHERE Email = @EMail

END

GO


