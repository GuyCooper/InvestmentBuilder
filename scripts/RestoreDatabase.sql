--RESTORE FILELISTONLY
--FROM DISK ='C:\Data\backups\InvestmentBuilderTest'


RESTORE DATABASE InvestmentBuilderTest2
   FROM DISK = 'C:\Data\backups\InvestmentBuilderTest'
   WITH NORECOVERY, 
      MOVE 'InvestmentBuilderTest' TO 
'C:\Data\SQLEXPRESS\InvestmentBuilder\InvestmentBuilderTest2.mdf', 
      MOVE 'InvestmentBuilderTest_log' 
TO 'C:\Data\SQLEXPRESS\InvestmentBuilder\InvestmentBuilderTest2_log.ldf';
RESTORE LOG InvestmentBuilderTest2
   FROM DISK = 'C:\Data\backups\InvestmentBuilderTest'
   WITH RECOVERY;