
rmdir C:\Projects\InvestmentBuilder\InvestmentBuilderWebClient\dist\Reports /S /Q
xcopy "C:\Users\guy\Documents\Guy\InvestmentBuilder\Argyll Investments_1\*.pdf"  "C:\Projects\InvestmentBuilder\InvestmentBuilderWebClient\dist\Reports\Argyll Investments_1\" /-Y /R
xcopy "C:\Users\guy\Documents\Guy\InvestmentBuilder\Guy SIPP_2\*.pdf"  "C:\Projects\InvestmentBuilder\InvestmentBuilderWebClient\dist\Reports\Guy SIPP_2\" /-Y /R
xcopy "C:\Users\guy\Documents\Guy\InvestmentBuilder\Guy ISA_3\*.pdf"  "C:\Projects\InvestmentBuilder\InvestmentBuilderWebClient\dist\Reports\Guy ISA_3\" /-Y /R
xcopy "C:\Users\guy\Documents\Guy\InvestmentBuilder\Naomi JISA_4\*.pdf"  "C:\Projects\InvestmentBuilder\InvestmentBuilderWebClient\dist\Reports\Naomi JISA_4\" /-Y /R
xcopy "C:\Users\guy\Documents\Guy\InvestmentBuilder\James ISA_5\*.pdf"  "C:\Projects\InvestmentBuilder\InvestmentBuilderWebClient\dist\Reports\James ISA_5\" /-Y /R
"C:\Program Files\IIS Express\iisexpress.exe" /path:"C:\Projects\InvestmentBuilder\InvestmentBuilderWebClient\dist" /port:8091

