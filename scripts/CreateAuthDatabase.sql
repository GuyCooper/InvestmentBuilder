use master

if exists(select 1 from sysdatabases where name = 'UserAuthentication')
begin
	drop database UserAuthentication
end

go

create database UserAuthentication
go

use UserAuthentication

create table dbo.UserDetails
(
	[Id] int identity primary key clustered,
	[UserName] nvarchar(256) not null,
	[EMail] nvarchar(256) not null,
	[EmailConfirmed] bit not null,
	[PasswordHash] nvarchar(max) not null,
	[PhoneNumber] nvarchar(256) null,
	[PhoneNumberConfirmed] bit not null,
	[TwoFactorEnabled] bit not null,
	[LoginCount] int not null,
	[AccessFailedCount] int not null,
	[PasswordCreationDate] datetime not null,
	[UserLastLogin] datetime not null,
	[TemporaryPassword] bit not null  default(0) 
	constraint UN_EmailName unique([Email])
)

go

create table dbo.UserSalt
(
	[User_Id] int not null,
	[Salt] nvarchar(max) not null,
	constraint FK_UserId_User foreign key
	([User_Id]) references UserDetails([Id]) on delete cascade
)
go

create table dbo.PasswordChange
(
	[User_Id] int primary key not null,
	[Token] nvarchar(256) not null,
	[AddTime] datetime  not null,

	constraint FK_UserDetails_Email foreign key
	([User_Id]) references UserDetails([Id])

)

go