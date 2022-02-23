
/****** Object:  StoredProcedure [dbo].[sp_GetAllValuations]    Script Date: 20/10/2014 21:43:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_GetAllValuations')
BEGIN
	DROP PROCEDURE sp_GetAllValuations
END

GO

CREATE PROCEDURE [dbo].sp_GetAllValuations(@Account AS INT) AS
BEGIN

--return the latest record from valuation table
SELECT 
	V.Valuation_Date,
	 V.Unit_Price
FROM
	Valuations V
WHERE
	V.[account_id] = @Account
END
