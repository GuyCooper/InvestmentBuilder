SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_AddHistoricalData')
BEGIN
	DROP PROCEDURE sp_AddHistoricalData
END

GO

CREATE PROCEDURE [dbo].[sp_AddHistoricalData](@Name as VARCHAR(50), @Symbol as VARCHAR(15), @Data as TEXT) AS
BEGIN
	IF EXISTS(SELECT 1 FROM [HistoricalData] WHERE [Symbol] = @Symbol)
	BEGIN
		UPDATE [HistoricalData] SET [Data] = @Data WHERE [Symbol] = @Symbol
	END
	ELSE
	BEGIN
		INSERT INTO [HistoricalData] 
		VALUES(@Name, @Symbol, @Data)
	END
END