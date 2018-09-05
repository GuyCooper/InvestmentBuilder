alter table Accounts
add [Broker] varchar(30) default(null) 
go

update Users
set [Broker] = 'ShareCentre'
where [Account_Id] = 1

update Users
set [Broker] = 'HargreavesLansdown'
where [Account_Id] = 2

update Users
set [Broker] = 'HargreavesLansdown'
where [Account_Id] = 3

update Users
set [Broker] = 'AJBell'
where [Account_Id] = 4

update Users
set [Broker] = 'AJBell'
where [Account_Id] = 5

 