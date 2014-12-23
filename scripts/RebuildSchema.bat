echo off
sqlcmd -S GUYANDSUE\GUYC -E -d ArgyllInvestments -i sp_AddNewShares.sql
sqlcmd -S GUYANDSUE\GUYC -E -d ArgyllInvestments -i sp_AddNewUnitValuation.sql
sqlcmd -S GUYANDSUE\GUYC -E -d ArgyllInvestments -i sp_CreateInvestment.sql
sqlcmd -S GUYANDSUE\GUYC -E -d ArgyllInvestments -i sp_CreateNewInvestment.sql
sqlcmd -S GUYANDSUE\GUYC -E -d ArgyllInvestments -i sp_GetBankBalance.sql
sqlcmd -S GUYANDSUE\GUYC -E -d ArgyllInvestments -i sp_GetDividends.sql
sqlcmd -S GUYANDSUE\GUYC -E -d ArgyllInvestments -i sp_GetInvestmentRecord.sql
sqlcmd -S GUYANDSUE\GUYC -E -d ArgyllInvestments -i sp_GetLatestInvestmentRecords.sql
sqlcmd -S GUYANDSUE\GUYC -E -d ArgyllInvestments -i sp_GetPreviousValuationDate.sql
sqlcmd -S GUYANDSUE\GUYC -E -d ArgyllInvestments -i sp_RollInvestment.sql
sqlcmd -S GUYANDSUE\GUYC -E -d ArgyllInvestments -i sp_UpdateClosingPrice.sql
sqlcmd -S GUYANDSUE\GUYC -E -d ArgyllInvestments -i sp_UpdateDividend.sql
sqlcmd -S GUYANDSUE\GUYC -E -d ArgyllInvestments -i sp_UpdateHolding.sql
sqlcmd -S GUYANDSUE\GUYC -E -d ArgyllInvestments -i sp_UpdateMembersCapitalAccount.sql
pause