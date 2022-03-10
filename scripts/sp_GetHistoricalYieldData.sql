SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetHistoricalYieldData')
BEGIN
	DROP PROCEDURE sp_GetHistoricalYieldData
END

GO

CREATE PROCEDURE [dbo].[sp_GetHistoricalYieldData] AS
BEGIN
	SELECT [Name], [Year], [Yield] FROM HistoricalYieldData 
END