SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_UpdateMemberForAccount')
BEGIN
	DROP PROCEDURE sp_UpdateMemberForAccount
END

GO

CREATE PROCEDURE sp_UpdateMemberForAccount(@Account AS VARCHAR(30), @Member AS VARCHAR(50), @Level as INT, @Add as TINYINT) AS
BEGIN

	--if user does not exist in database then add them  
	IF NOT EXISTS(SELECT
					 1 
				  FROM 
					 Members M
				  INNER JOIN
					 Accounts A
				  ON
					 M.account_id = A.[Account_Id]
				  WHERE 
					 M.Name = @Member AND
					 A.Name = @Account
				  	 )
	BEGIN
		INSERT INTO MEMBERS(Name, account_id, [Authorization])
		SELECT 
			@Member,
			[Account_Id],
			@Level
		FROM
			Accounts
		WHERE
			Name = @Account		 
	END

	--now set the enabled flag and user level
	UPDATE M
 	SET	   
		   [Enabled] = @Add,
		   [Authorization] = @Level
	FROM
		Members M
	INNER JOIN
		Accounts A
	ON
		M.account_id = A.[Account_Id]
	WHERE
		M.Name = @Member AND
		A.Name = @Account
		
END