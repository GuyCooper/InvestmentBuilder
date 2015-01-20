SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_UpdateValuationTable')
BEGIN
	DROP PROCEDURE sp_UpdateValuationTable
END

GO

CREATE PROCEDURE [dbo].[sp_UpdateValuationTable](@ValuationDate as DATETIME, @UnitPrice as float) AS
BEGIN
	INSERT INTO dbo.Valuations (Valuation_Date,  Unit_Price)
	VALUES (@ValuationDate,@UnitPrice)
END
