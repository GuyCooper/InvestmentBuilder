--RESTORE FILELISTONLY
--FROM DISK ='C:\Data\backups\InvestmentBuilderTest'

RESTORE DATABASE InvestmentBuilderTest99
   FROM DISK = 'C:\Projects\Data\backups\InvestmentBuilderTest2.bak'
   WITH NORECOVERY, 
      MOVE 'InvestmentBuilderTest' TO 
'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS01\MSSQL\DATA\InvestmentBuilderTest99.mdf', 
      MOVE 'InvestmentBuilderTest_log' 
TO 'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS01\MSSQL\DATA\InvestmentBuilderTest99_log.ldf';
RESTORE LOG InvestmentBuilderTest99
   FROM DISK = 'C:\Projects\Data\backups\InvestmentBuilderTest2.bak'
   WITH RECOVERY;