
/****** Object:  StoredProcedure [dbo].[sp_AddNewUnitValuation]    Script Date: 20/10/2014 21:43:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_AddNewUnitValuation')
BEGIN
	DROP PROCEDURE sp_AddNewUnitValuation
END

GO

CREATE PROCEDURE [dbo].[sp_AddNewUnitValuation](@valuationDate as DATETIME, @unitValue AS decimal, @Account as INT) AS
BEGIN

IF EXISTS(SELECT [Unit_Price] FROM [Valuations] 
		WHERE [account_id] = @Account
		AND [Valuation_Date] = @valuationDate)
BEGIN
	UPDATE [Valuations] 
	SET [Unit_Price] = @unitValue
	WHERE [account_id] = @Account
	AND [Valuation_Date] = @valuationDate
END
ELSE
BEGIN
	INSERT INTO dbo.Valuations (Valuation_Date, Unit_Price, account_id)
	VALUES (@valuationDate, @unitValue, @Account)
END
				
END
