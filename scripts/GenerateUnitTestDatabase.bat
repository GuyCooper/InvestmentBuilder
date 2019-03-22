pushd

set ServerName=DESKTOP-JJ9QOJA\SQLEXPRESS
set DBName=master

echo rebuild unit test database, server name: %ServerName%, db name: %DBName%

sqlcmd -S %ServerName% -E -d %DBName% -i BuildInvestmentBuilderUnitTestDatabase.sql

set DBName=InvestmentBuilderUnitTest1

sqlcmd -S %ServerName% -E -d %DBName% -i sp_AddCashAccountData.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_AddNewShares.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_AddNewUnitValuation.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_AddRedemption.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_AddTransactionHistory.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_CreateAccount.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_UpdateAccount.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_CreateNewInvestment.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_DeactivateCompany.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetAccountData.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetAccountMembers.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetAccountsForUser.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetActiveCompanies.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetAuthorizationLevel.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetBalanceInHand.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetBankBalance.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetCashAccountData.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetCompanyData.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetCompanyInvestmentRecords.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetDividends.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetFullInvestmentRecordData.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetInvestmentRecord.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetIssuedUnits.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetLatestInvestmentRecords.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetLatestRecordValuationDate.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetLatestValuationDate.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetMembersCapitalAccount.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetMemberSubscriptionAmount.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetPreviousRecordValuationDate.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetPreviousValuationDate.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetRedemptions.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetStartOfYearValuation.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetTradeItem.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetTransactionHistory.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetTransactionTypes.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetUnitPriceData.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetUnitValuation.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetUnitValuationRange.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetUserCompanies.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_InvestmentAccountExists.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_IsAdministrator.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_IsExistingRecordValuationDate.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_IsExistingValuationDate.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_RecentValuationDates.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_RemoveCashAccountData.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_RollbackUpdate.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_RollInvestment.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_SellShares.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_UndoLastTransaction.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_UpdateClosingPrice.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_UpdateDividend.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_UpdateHolding.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_UpdateInvesmentState.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_UpdateMemberForAccount.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_UpdateMembersCapitalAccount.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_updateRedemption.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_UpdateValuationTable.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_AddHistoricalData.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetHistoricalData.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_AddNewUser.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetUserId.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetLastTransaction.sql

popd

