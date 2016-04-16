alter table Users
add [Broker] varchar(30) default(null) 
go

update Users
set [Broker] = 'ShareCentre'
where [User_Id] = 1

update Users
set [Broker] = 'HargreavesLansdown'
where [User_Id] = 2

update Users
set [Broker] = 'HargreavesLansdown'
where [User_Id] = 3

update Users
set [Broker] = 'AJBell'
where [User_Id] = 4

update Users
set [Broker] = 'AJBell'
where [User_Id] = 5

 