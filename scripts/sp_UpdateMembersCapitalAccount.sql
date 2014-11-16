CREATE PROCEDURE sp_UpdateMembersCapitalAccount(@valuationDate as DATETIME, @previousUnitValue as float) AS
BEGIN

DECLARE @previousDate DATETIME

SELECT @previousDate = MAX(Valuation_Date) FROM MembersCapitalAccount

--select user and the amount invested this month
SELECT parameter as member, amount 
INTO #ThisMonthsUsers
FROM 
CashAccount
WHERE transaction_date > @previousDate

--select previous months units for each user
SELECT member, units
INTO #PreviousUnits 
FROM
MembersCapitalAccount
WHERE
Valuation_Date = @previousDate

--add the value of the previous months units to the new calculated amount for each member
INSERT INTO MembersCapitalAccount
SELECT 
@valuationDate,
tm.member,
pu.Units + tm.amount * (1 /  @previousUnitValue)
FROM 
#PreviousUnits pu,
#ThisMonthsUsers tm
WHERE
pu.Member = tm.member

SELECT 
	SUM(Units)
FROM
	MembersCapitalAccount
WHERE
	Valuation_Date = @valuationDate

END