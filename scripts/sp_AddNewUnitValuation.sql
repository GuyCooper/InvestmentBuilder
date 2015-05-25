
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

CREATE PROCEDURE [dbo].[sp_AddNewUnitValuation](@valuationDate as DATETIME, @unitValue AS FLOAT, @Account as VARCHAR(30)) AS
BEGIN

INSERT INTO dbo.Valuations (Valuation_Date, Unit_Price, account_id)
SELECT
	@valuationDate, @unitValue, U.[User_Id]
FROM
	Users U
WHERE
	U.Name = @Account
END
