
/****** Object:  StoredProcedure [dbo].[sp_AuthGetPasswordHash]    Script Date: 11/05/2024 17:50:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_AuthGetPasswordHash')
BEGIN
	DROP PROCEDURE sp_AuthGetPasswordHash
END

GO

CREATE PROCEDURE [dbo].[sp_AuthGetPasswordHash](@Email NVARCHAR(256)) AS
										   
BEGIN

SELECT PasswordHash FROM UserDetails WHERE Email = @Email

END
GO

