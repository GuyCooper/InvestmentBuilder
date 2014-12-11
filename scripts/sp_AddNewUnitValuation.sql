USE [ArgyllInvestments]
GO

/****** Object:  StoredProcedure [dbo].[sp_AddNewUnitValuation]    Script Date: 20/10/2014 21:43:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[sp_AddNewUnitValuation](@valuationDate as DATETIME, @unitValue AS FLOAT ) AS
BEGIN

INSERT INTO dbo.Valuations (Valuation_Date, Unit_Price)
VALUES (@valuationDate, @unitValue)
end
