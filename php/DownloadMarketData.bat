pushd

set outfile=%APPDATA%\InvestmentRecordBuilder\yahooMarketData.txt
php MarketDataLoader.php --o:%outfile% --a:1

pause

popd