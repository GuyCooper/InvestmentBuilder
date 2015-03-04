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

	constraint UN_UserName unique([Name]),

	constraint FK_UserType_User foreign key
	([Type_Id]) references UserTypes([Type_Id])
)

insert into dbo.UserTypes ([Type]) values ('Club')
insert into dbo.Users 
select 'Argyll Investments',
	   'password',
	   'Argyll Investments',
	   'GBP',
	   [Type_Id]
from 
	dbo.UserTypes
where
	[Type] = 'Club'
