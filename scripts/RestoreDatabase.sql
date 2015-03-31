--RESTORE FILELISTONLY
--FROM DISK ='C:\Data\backups\InvestmentBuilderTest'


RESTORE DATABASE InvestmentBuilderTest2
   FROM DISK = 'C:\Data\backups\InvestmentBuilderTest.bak'
   WITH NORECOVERY, 
      MOVE 'InvestmentBuilderTest' TO 
'C:\Data\SQLEXPRESS\InvestmentBuilder\InvestmentBuilderTest2.mdf', 
      MOVE 'InvestmentBuilderTest_log' 
TO 'C:\Data\SQLEXPRESS\InvestmentBuilder\InvestmentBuilderTest_log2.ldf';
RESTORE LOG InvestmentBuilderTest2
   FROM DISK = 'C:\Data\backups\InvestmentBuilderTest.bak'
   WITH RECOVERY;