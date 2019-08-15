pushd

set ServerName=DESKTOP-JJ9QOJA\SQLEXPRESS
set DBName=InvestmentBuilderTest3
set dataFolder=C:\Data\SQLServer\productiondump

bcp dbo.Users in %dataFolder%\users.out -S %ServerName% -d %DBName% -T -w
bcp dbo.Accounts in %dataFolder%\accounts.out -S %ServerName% -d %DBName% -T -w
bcp dbo.Members in %dataFolder%\members.out -S %ServerName% -d %DBName% -T -w
bcp dbo.Administrators in %dataFolder%\administrators.out -S %ServerName% -d %DBName% -T -w
bcp dbo.Companies in %dataFolder%\companies.out -S %ServerName% -d %DBName% -T -w
bcp dbo.MembersCapitalAccount in %dataFolder%\memberscapitalaccount.out -S %ServerName% -d %DBName% -T -w
bcp dbo.CashAccount in %dataFolder%\cashaccount.out -S %ServerName% -d %DBName% -T -w
bcp dbo.Redemptions in %dataFolder%\redemptions.out -S %ServerName% -d %DBName% -T -w
bcp dbo.TransactionHistory in %dataFolder%\transactionhistory.out -S %ServerName% -d %DBName% -T -w
bcp dbo.HistoricalData in %dataFolder%\historicaldata.out -S %ServerName% -d %DBName% -T -w
bcp dbo.InvestmentRecord in %dataFolder%\investmentrecord.out -S %ServerName% -d %DBName% -T -w
bcp dbo.Valuations in %dataFolder%\valuations.out -S %ServerName% -d %DBName% -T -w

popd