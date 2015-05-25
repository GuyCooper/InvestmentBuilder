if exists(select 1 from dbo.sysobjects where name = 'UN_UserName')
begin
	alter table dbo.Users
	drop constraint UN_UserName
end
go

if exists(select 1 from dbo.sysobjects where name = 'FK_UserType_User')
begin
	alter table dbo.Users
	drop constraint FK_UserType_User
end
go

if exists(select 1 from dbo.sysobjects where name = 'UN_UserTypeName')
begin
	alter table dbo.UserTypes
	drop constraint UN_UserTypeName
end
go

if exists(select 1 from dbo.sysobjects where name = 'Users')
begin
	drop table dbo.Users
end
go

if exists(select 1 from dbo.sysobjects where name = 'UserTypes')
begin
	drop table dbo.UserTypes
end
go

create table dbo.UserTypes
(
	[Type_Id] int identity primary key clustered,
	[Type] varchar(50) not null,
	constraint UN_UserTypeName unique([Type])
)

create table dbo.Users
(
	[User_Id] int identity primary key clustered,
	[Name]			  varchar(30) not null,
	[Password]		  varchar(1024) not null,
	[Description]	  varchar(1024) null,
	[Currency]		  char(3) not null,
	[Type_Id]		  int not null,
	[Enabled]		  tinyint not null

	constraint UN_UserName unique([Name]),

	constraint FK_UserType_User foreign key
	([Type_Id]) references UserTypes([Type_Id])
)

insert into dbo.UserTypes ([Type]) values ('Club')
insert into dbo.UserTypes ([Type]) values ('Personal')
go

insert into dbo.Users 
select 'Argyll Investments',
	   'password',
	   'Argyll Investments',
	   'GBP',
	   [Type_Id],
	   1
from 
	dbo.UserTypes
where
	[Type] = 'Club'

go


alter table CashAccount
add [account_id] int not null default(1) 
go

create index IDX_CashAccountAccountID on
dbo.CashAccount([account_id])

alter table InvestmentRecord
add [account_id] int not null default(1) 
go

create index IDX_InvestmentRecordAccountID on
dbo.InvestmentRecord([account_id])

alter table Members
add [account_id] int not null default(1)
go

alter table Members
add [enabled] tinyint not null default(1)
go

create index IDX_MembersAccountID on
dbo.Members([account_id])
go

alter table Valuations
add [account_id] int not null default(1) 
go

create index IDX_ValuationsAccountID on
dbo.Valuations([account_id])
go

create unique index UN_MemberAccount
on Members([Name],[account_id])

go

alter table InvestmentRecord
add [is_active] tinyint not null default(1) 
go

alter table InvestmentRecord
add [last_bought] DATETIME  
go
 
update InvestmentRecord 
set 
	is_active = C.IsActive,
	last_bought = C.LastBoughtDate
from
	InvestmentRecord IR
inner join
	Companies C
on
	IR.Company_id = C.Company_Id

go
--create table dbo.AccountCompanies
--(
--	[account_id] int not null,
--	[company_id] int not null,
--	[is_active] tinyint not null,
--	[last_bought_date] datetime not null,

--	constraint FK_Account_Id_AccountCompanies foreign key
--	([account_id]) references Users([User_Id]),

--	constraint FK_Comapny_Id_AccountCompanies foreign key
--	([company_id]) references Companies([Company_Id]),

--	constraint UN_AccountCompany unique([account_id], [company_id])
--)
--go

--insert into AccountCompanies
--select 
--	U.[User_Id],
--	C.[Company_Id],
--	1,
--	C.LastBoughtDate
--from
--	Users U, 
--	Companies C
--where
--	U.[Name] = 'Argyll Investments'
--and
--	C.IsActive = 1

--go

-- alter table Companies
-- drop column IsActive
-- go

 