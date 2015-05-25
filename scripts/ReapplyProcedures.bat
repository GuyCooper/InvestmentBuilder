
set ServerName=%1
set DBName=%2

echo rebuild schema, server name: %ServerName%, db name: %DBName%

pause

sqlcmd -S %ServerName% -E -d %DBName% -i sp_AddCashAccountData.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_AddNewShares.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_AddNewUnitValuation.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_CreateAccount.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_CreateNewInvestment.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_DeactivateCompany.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetAccountData.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetAccountMembers.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetActiveCompanies.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetBalanceInHand.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetBankBalance.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetCashAccountData.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetCompanyData.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetDividends.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetInvestmentRecord.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetIssuedUnits.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetLatestInvestmentRecords.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetLatestValuationDate.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetMembersCapitalAccount.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetMemberSubscriptionAmount.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetPreviousValuationDate.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetTransactionTypes.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetUnitPriceData.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetUnitValuation.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetUserCompanies.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetUserData.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_IsExistingValuationDate.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_RecentValuationDates.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_RollbackUpdate.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_RollInvestment.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_SellShares.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_UpdateClosingPrice.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_UpdateDividend.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_UpdateHolding.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_UpdateMemberForAccount.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_UpdateMembersCapitalAccount.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_UpdateValuationTable.sql

pause