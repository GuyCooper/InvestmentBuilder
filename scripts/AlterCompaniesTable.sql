alter table Companies
add [Exchange] varchar(10) default(null) 
go

update Companies
set Exchange = 'LSE'
where Currency = 'GBP'

update Companies
set exchange = 'DAX'
where Name = 'Nokia'

update Companies
set exchange = 'MIL'
where Name = 'Enel'

 