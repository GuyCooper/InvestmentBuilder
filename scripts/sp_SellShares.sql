SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT 1 FROM sys.procedures WHERE Name = 'sp_SellShares')
BEGIN
	DROP PROCEDURE sp_SellShares
END

GO

CREATE PROCEDURE sp_SellShares(@ValuationDate as DATETIME, @company as VARCHAR(50), @shares as INT, @account as varchar(30)) AS
BEGIN

DECLARE @CompanyId INT
DECLARE @AccountID INT
DECLARE @Shares_Bought INT
DECLARE @Active TINYINT

--get the company id
SELECT @CompanyId = [Company_Id]
FROM Companies
WHERE [Name] = @company

--get the accountid
SELECT @AccountID = [Account_Id]
FROM [Accounts]
WHERE [Name] = @account

--get the current quantity of shares
SELECT @Shares_Bought = Shares_Bought
FROM InvestmentRecord
WHERE [Company_id] = @CompanyId
AND [account_id] = @AccountID
AND [Valuation_Date] = @ValuationDate

--if current quantity is less than or equal to the number of shares being sold
--then set the is_active flag to false for this investment
IF(@Shares_Bought <= @shares)
	SET @Active = 0
ELSE
	SET @Active = 1

--now update the investment record table
UPDATE 
	InvestmentRecord
SET
	Shares_Sold += @shares,
	is_active = @Active
FROM 
	investmentRecord
WHERE [Company_id] = @CompanyId
	AND [account_id] = @AccountID
	AND [Valuation_Date] = @ValuationDate

END