--RESTORE FILELISTONLY
--FROM DISK ='C:\Data\backups\InvestmentBuilderTest'


RESTORE DATABASE InvestmentBuilderTest3
   FROM DISK = 'C:\Data\backups\InvestmentBuilderTest'
   WITH NORECOVERY, 
      MOVE 'InvestmentBuilderTest' TO 
'C:\Data\SQLEXPRESS\InvestmentBuilder\InvestmentBuilderTest3.mdf', 
      MOVE 'InvestmentBuilderTest_log' 
TO 'C:\Data\SQLEXPRESS\InvestmentBuilder\InvestmentBuilderTest3_log.ldf';
RESTORE LOG InvestmentBuilderTest3
   FROM DISK = 'C:\Data\backups\InvestmentBuilderTest'
   WITH RECOVERY;