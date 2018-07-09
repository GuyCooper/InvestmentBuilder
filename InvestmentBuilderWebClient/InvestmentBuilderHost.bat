
rmdir C:\Projects\InvestmentBuilder\InvestmentBuilderWebClient\dist\Reports /S /Q
xcopy "C:\Users\guy\Documents\Guy\InvestmentBuilder\Argyll Investments\*.pdf"  "C:\Projects\InvestmentBuilder\InvestmentBuilderWebClient\dist\Reports\Argyll Investments\" /-Y /R
xcopy "C:\Users\guy\Documents\Guy\InvestmentBuilder\Guy ISA\*.pdf"  "C:\Projects\InvestmentBuilder\InvestmentBuilderWebClient\dist\Reports\Guy ISA\" /-Y /R
xcopy "C:\Users\guy\Documents\Guy\InvestmentBuilder\Guy SIPP\*.pdf"  "C:\Projects\InvestmentBuilder\InvestmentBuilderWebClient\dist\Reports\Guy SIPP\" /-Y /R
xcopy "C:\Users\guy\Documents\Guy\InvestmentBuilder\James ISA\*.pdf"  "C:\Projects\InvestmentBuilder\InvestmentBuilderWebClient\dist\Reports\James ISA\" /-Y /R
xcopy "C:\Users\guy\Documents\Guy\InvestmentBuilder\Naomi JISA\*.pdf"  "C:\Projects\InvestmentBuilder\InvestmentBuilderWebClient\dist\Reports\Naomi JISA\" /-Y /R
"C:\Program Files\IIS Express\iisexpress.exe" /path:"C:\Projects\InvestmentBuilder\InvestmentBuilderWebClient\dist" /port:8091

