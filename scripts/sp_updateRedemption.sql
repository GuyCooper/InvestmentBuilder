SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_UpdateRedemption')
BEGIN
	DROP PROCEDURE sp_UpdateRedemption
END

GO

CREATE PROCEDURE sp_UpdateRedemption(@RedemptionId as int, @Amount as float,
						@UnitsRedeemed as float,  @Status as varchar(10)) AS
BEGIN

UPDATE
	 Redemptions				  
SET 
	[amount] = @Amount,
	[units] = @UnitsRedeemed,
	[status] = @Status
WHERE
	Redemption_Id = @RedemptionId

END