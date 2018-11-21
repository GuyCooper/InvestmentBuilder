
create table dbo.PasswordChange
(
	[User_Id] int primary key not null,
	[Token] nvarchar(256) not null,
	[AddTime] datetime  not null,

	constraint FK_UserDetails_Email foreign key
	([User_Id]) references UserDetails([Id])

)
