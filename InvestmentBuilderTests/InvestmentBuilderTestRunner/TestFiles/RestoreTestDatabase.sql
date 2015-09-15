--RESTORE FILELISTONLY
--FROM DISK ='C:\Data\backups\InvestmentBuilderTest'


RESTORE DATABASE InvestmentBuilderUnitTest
   FROM DISK = 'C:\Data\backups\InvestmentBuilderUnitTest.bak'
   WITH NORECOVERY, 
      MOVE 'InvestmentBuilderTest' TO 
'C:\Data\SQLEXPRESS\InvestmentBuilder\InvestmentBuilderUnitTest.mdf', 
      MOVE 'InvestmentBuilderTest_log' 
TO 'C:\Data\SQLEXPRESS\InvestmentBuilder\InvestmentBuilderUnitTest.ldf';
RESTORE LOG InvestmentBuilderUnitTest
   FROM DISK = 'C:\Data\backups\InvestmentBuilderUnitTest.bak'
   WITH RECOVERY;