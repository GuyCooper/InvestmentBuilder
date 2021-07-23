
set ServerName=LAPTOP-D6H2KOAE\SQLEXPRESS01
set DBName=UserAuthentication

echo rebuild unit test database, server name: %ServerName%, db name: %DBName%

sqlcmd -S %ServerName% -b -E -d master -i CreateAuthDatabase.sql -v dbname = %DBName%

sqlcmd -S %ServerName% -E -d %DBName% -i sp_AuthAddNewUser.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_AuthAddPasswordChangeRequest.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_AuthAddTemporaryPassword.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_AuthChangePassword.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_AuthenticateUser.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_AuthGetPasswordDetails.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_AuthGetSalt.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_AuthRemoveUser.sql
sqlcmd -S %ServerName% -E -d %DBName% -i sp_AuthValidateNewUser.sql