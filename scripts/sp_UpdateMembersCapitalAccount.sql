SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_UpdateMembersCapitalAccount')
BEGIN
	DROP PROCEDURE sp_UpdateMembersCapitalAccount
END

GO

CREATE PROCEDURE [dbo].[sp_UpdateMembersCapitalAccount](@ValuationDate as DATETIME, @Member as varchar(50), @Units as float) AS
BEGIN
	INSERT INTO MembersCapitalAccount (Valuation_Date, Member, Units)
	VALUES (@ValuationDate, @Member, @Units)
END
