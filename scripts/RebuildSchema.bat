
set ServerName=%1
set DBName=%2

echo rebuild schema, server name: %ServerName%, db name: %DBName%

pause

sqlcmd -S %ServerName% -E -d %DBName% -i CreateTables.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_AddNewShares.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_AddNewUnitValuation.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_CreateInvestment.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_CreateNewInvestment.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetBalanceInHand.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetBankBalance.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetCashAccountData.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetCompanyData.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetDividends.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetInvestmentRecord.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetLatestInvestmentRecords.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetMembersCapitalAccount.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetMemberSubscriptionAmount.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetPreviousValuationDate.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetTransactionTypes.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetUnitValuation.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_RollInvestment.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_UpdateClosingPrice.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_UpdateDividend.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_UpdateHolding.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_UpdateMembersCapitalAccount.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_UpdateValuationTable.sql

pause