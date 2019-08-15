
/****** Object:  StoredProcedure [dbo].[sp_GetPasswordDetails]    Script Date: 04/02/2018 17:50:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetPasswordDetails')
BEGIN
	DROP PROCEDURE sp_GetPasswordDetails
END

GO

CREATE PROCEDURE [dbo].sp_GetPasswordDetails(@EMail NVARCHAR(256)) AS
										   
BEGIN

SELECT TemporaryPassword, PasswordCreationDate
FROM UserDetails
WHERE Email = @EMail

END
GO

