SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_UpdateMembersCapitalAccount')
BEGIN
	DROP PROCEDURE sp_UpdateMembersCapitalAccount
END

GO

CREATE PROCEDURE [dbo].[sp_UpdateMembersCapitalAccount](@ValuationDate as DATETIME, @Member as varchar(50), @Units as float, @Account as VARCHAR(30)) AS
BEGIN
	DECLARE @AccountID INT
	DECLARE @MemberID INT

	SELECT
		 @AccountID = [User_Id]
	FROM 
		Users
	WHERE
		Name = @Account
	
	SELECT
		 @MemberID = [Member_Id]
	FROM 
		Members
	WHERE
		Name = @Member
		AND 
		account_id = @AccountID

	if EXISTS(SELECT [Units] FROM MembersCapitalAccount 
			  WHERE [Member_Id] = @MemberID
				AND [Valuation_Date] = @ValuationDate)
	BEGIN
		UPDATE MembersCapitalAccount 
		SET [Units] = @Units
		WHERE
			[Member_Id] = @MemberID
			AND [Valuation_Date] = @ValuationDate
	END
	ELSE
	BEGIN
		INSERT INTO MembersCapitalAccount ([Valuation_Date], [Member_Id], [Units])
		VALUES (@ValuationDate, @MemberID, @Units)
	END
END
