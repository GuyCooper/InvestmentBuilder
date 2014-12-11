CREATE PROCEDURE [dbo].[sp_UpdateMembersCapitalAccount](@valuationDate as DATETIME, @previousDate as DATETIME, @previousUnitValue as float) AS
BEGIN

--select user and the amount invested this month
SELECT ca.parameter as member, ca.amount 
INTO #ThisMonthsUsers
FROM 
CashAccount ca
INNER JOIN TransactionType tt
ON
ca.type_id = tt.type_id
WHERE transaction_date > @previousDate
and tt.[type] = 'Subscription'

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