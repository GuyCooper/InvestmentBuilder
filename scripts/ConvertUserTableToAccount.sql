create table dbo.AccountTypes
(
	[Type_Id] int identity primary key clustered,
	[Type] varchar(50) not null,
	constraint UN_AccountTypeName unique([Type])
)

create table dbo.Accounts
(
	[Account_Id] int identity primary key clustered,
	[Name]			  varchar(30) not null,
	[Description]	  varchar(1024) null,
	[Currency]		  char(3) not null,
	[Type_Id]		  int not null,
	[Enabled]		  tinyint not null,
	[Broker] varchar(30) default(null), 

	constraint UN_AccountName unique([Name]),

	constraint FK_AccountType_Account foreign key
	([Type_Id]) references AccountTypes([Type_Id])
)

insert into dbo.AccountTypes ([Type]) values ('Club')
insert into dbo.AccountTypes ([Type]) values ('Personal')
go

insert into Accounts ([Name], [Description], [Currency], [Type_Id], [Enabled], [Broker])
select [Name], [Description], [Currency], [Type_Id], [Enabled], [Broker]
from Users
go

--sp_help [Users]

alter table [TransactionHistory]
drop constraint [FK_accountid_TransactionHistory]
go

alter table [Users]
drop constraint [PK__Users__206D9170265ABCD3]
go

alter table [Users]
drop constraint [UN_UserName]
go

drop table [Users]
go

drop table [UserTypes]
go