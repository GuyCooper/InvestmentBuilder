pushd

set ServerName=DESKTOP-JJ9QOJA\SQLEXPRESS
set DBName=InvestmentBuilderTest2
set dataFolder=C:\Data\SQLServer\backups

bcp dbo.Users out %dataFolder%\users.out -S %ServerName% -d %DBName% -T -w
bcp dbo.Accounts out %dataFolder%\accounts.out -S %ServerName% -d %DBName% -T -w
bcp dbo.Members out %dataFolder%\members.out -S %ServerName% -d %DBName% -T -w
bcp dbo.Administrators out %dataFolder%\administrators.out -S %ServerName% -d %DBName% -T -w
bcp dbo.Companies out %dataFolder%\companies.out -S %ServerName% -d %DBName% -T -w
bcp dbo.MembersCapitalAccount out %dataFolder%\memberscapitalaccount.out -S %ServerName% -d %DBName% -T -w
bcp dbo.CashAccount out %dataFolder%\cashaccount.out -S %ServerName% -d %DBName% -T -w
bcp dbo.Redemptions out %dataFolder%\redemptions.out -S %ServerName% -d %DBName% -T -w
bcp dbo.TransactionHistory out %dataFolder%\transactionhistory.out -S %ServerName% -d %DBName% -T -w
bcp dbo.HistoricalData out %dataFolder%\historicaldata.out -S %ServerName% -d %DBName% -T -w
bcp dbo.InvestmentRecord out %dataFolder%\investmentrecord.out -S %ServerName% -d %DBName% -T -w
bcp dbo.Valuations out %dataFolder%\valuations.out -S %ServerName% -d %DBName% -T -w

popd