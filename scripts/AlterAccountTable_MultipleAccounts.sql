
alter table Accounts
drop constraint UN_AccountName
go

create index IDX_AccountName on
dbo.Accounts([Name])
go

