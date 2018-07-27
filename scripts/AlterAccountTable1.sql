
alter table Accounts
drop constraint UN_UserName
go

alter table Accounts
add [Owner] varchar(50) default(null) 
go

alter table Accounts
add constraint UN_AccountName unique([Name], [Owner])
go

