SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_RemoveRedemption')
BEGIN
	DROP PROCEDURE sp_RemoveRedemption
END

GO

CREATE PROCEDURE sp_RemoveRedemption(@RedemptionId as int) AS
BEGIN

DELETE Redemptions
FROM Redemptions r 
JOIN Members m
on m.Member_Id = r.member_id
WHERE r.Redemption_Id = @RedemptionId
AND r.[status] = 0

select @@ROWCOUNT

END