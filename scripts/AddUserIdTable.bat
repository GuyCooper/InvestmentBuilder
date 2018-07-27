pushd

set ServerName=DESKTOP-JJ9QOJA\SQLEXPRESS
set DBName=InvestmentBuilderTest2

echo add user id table schema, server name: %ServerName%, db name: %DBName%

sqlcmd -S %ServerName% -E -d %DBName% -i CreateUserIdTable.sql.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_UpdateMemberForAccount.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetAccountMembers.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetMembersCapitalAccount.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetMemberSubscriptionAmount.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_UpdateMembersCapitalAccount.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_AddUser.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_GetUserId.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_AddRedemption.sql
popd

