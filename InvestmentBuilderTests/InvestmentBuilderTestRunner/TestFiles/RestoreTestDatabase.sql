--RESTORE FILELISTONLY
--FROM DISK ='C:\Data\backups\InvestmentBuilderTest'


RESTORE DATABASE InvestmentBuilderUnitTest
   FROM DISK = 'C:\Data\SQLServer\backups\InvestmentBuilderUnitTest.bak'
   WITH NORECOVERY, 
      MOVE 'InvestmentBuilderTest' TO 
'C:\Data\SQLServer\MSSQL12.SQLEXPRESS\MSSQL\InvestmentBuilder\InvestmentBuilderUnitTest.mdf', 
      MOVE 'InvestmentBuilderTest_log' 
TO 'C:\Data\SQLServer\MSSQL12.SQLEXPRESS\MSSQL\InvestmentBuilder\InvestmentBuilderUnitTest.ldf';
RESTORE LOG InvestmentBuilderUnitTest
   FROM DISK = 'C:\Data\SQLServer\backups\InvestmentBuilderUnitTest.bak'
   WITH RECOVERY;