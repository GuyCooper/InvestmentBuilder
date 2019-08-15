--RESTORE FILELISTONLY
--FROM DISK ='C:\Data\backups\InvestmentBuilderTest'

RESTORE DATABASE InvestmentBuilderTest2
   FROM DISK = 'C:\Data\SQLServer\backups\InvestmentBuilderTest.bak'
   WITH NORECOVERY, 
      MOVE 'InvestmentBuilderTest' TO 
'C:\Data\SQLServer\MSSQL12.SQLEXPRESS\MSSQL\InvestmentBuilder\InvestmentBuilderTest2.mdf', 
      MOVE 'InvestmentBuilderTest_log' 
TO 'C:\Data\SQLServer\MSSQL12.SQLEXPRESS\MSSQL\InvestmentBuilder\InvestmentBuilderTest2_log.ldf';
RESTORE LOG InvestmentBuilderTest2
   FROM DISK = 'C:\Data\SQLServer\backups\InvestmentBuilderTest.bak'
   WITH RECOVERY;