SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_UpdateMembersCapitalAccount')
BEGIN
	DROP PROCEDURE sp_UpdateMembersCapitalAccount
END

GO

CREATE PROCEDURE [dbo].[sp_UpdateMembersCapitalAccount](@ValuationDate as DATETIME, @Member as nvarchar(256), @Units as decimal(18,4), @Account as INT) AS
BEGIN
	DECLARE @MemberID INT

	SELECT
		 @MemberID = M.[Member_Id]
	FROM 
		Members M
	INNER JOIN 
		[Users] U
    ON M.[UserId] = U.[UserId]
	WHERE
		U.UserName = @Member
		AND 
		M.account_id = @Account

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
