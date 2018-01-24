SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetHistoricalData')
BEGIN
	DROP PROCEDURE sp_GetHistoricalData
END

GO

CREATE PROCEDURE [dbo].[sp_GetHistoricalData](@Symbol as VARCHAR(15)) AS
BEGIN
	SELECT [Name], [Data] FROM HistoricalData WHERE [Symbol] = @Symbol
END