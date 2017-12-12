pushd

set outfile=%APPDATA%\InvestmentRecordBuilder\yahooMarketData.txt
php MarketDataLoader.php --o:%outfile%

pause

popd