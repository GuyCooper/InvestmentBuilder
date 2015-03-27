SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_UpdateClosingPrice')
BEGIN
	DROP PROCEDURE sp_UpdateClosingPrice
END

GO

CREATE PROCEDURE [dbo].[sp_UpdateClosingPrice](@valuationDate as DATETIME, @investment as VARCHAR(50), @closingPrice as float, @account as VARCHAR(30)) AS
BEGIN

UPDATE
	 IR
SET
	 IR.Selling_Price = @closingPrice
FROM 
	 InvestmentRecord AS IR 
INNER JOIN 
	 Companies C 
ON
	 IR.Company_Id = C.Company_Id
INNER JOIN 
	Users U
ON
	IR.account_id = U.[User_Id]	 
WHERE
	 C.Name = @investment
	 AND IR.Valuation_Date = @valuationDate
	 AND U.Name = @account
END