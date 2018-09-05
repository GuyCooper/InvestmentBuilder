
--script for creating a userid table. Every valid user will have a unique id which is used 
-- for mapping the user to the members table
 
--drop table dbo.[Users]

create table dbo.[Users]
(
	[UserId] int identity primary key clustered,
	[UserName]	 nvarchar(256) not null,
	[Description] nvarchar(256) default(null)
	
	constraint UN_UserName unique([UserName])
)

go
--modify members table to have a userid column

alter table dbo.Members
add [UserId] int
go

alter table dbo.Members
add constraint FK_members_UserID_Users_UserID foreign key
	([UserId]) references [Users]([UserId])
go

--now we need to populate the new users table and modify the members table
--create a temporary stored procedure to do this

create procedure sp_tmp_UpdateUserTables(@NewUserName as nvarchar(256), @Description as nvarchar(256) )as
begin

insert into dbo.[Users] ([UserName], [Description]) values (@NewUserName, @Description)
end
go

create procedure sp_tmp_update_Members(@UserName as nvarchar(256), @Userid as int) as
begin

update dbo.Members
set 
	[UserId] = @Userid
where 
	[Name] = @UserName
end
go

exec sp_tmp_UpdateUserTables 'guy@guycooper.plus.com', 'Guy Cooper'
go
exec sp_tmp_update_Members 'Guy Cooper', @@IDENTITY
go

exec sp_tmp_UpdateUserTables 'rodgersathome@hotmail.com', 'James Rodgers'
go
exec sp_tmp_update_Members 'James Rodgers', @@IDENTITY
go

exec sp_tmp_UpdateUserTables 'nigel_cooper@talk21.com', 'Nigel Cooper'
go
exec sp_tmp_update_Members 'Nigel Cooper', @@IDENTITY
go

exec sp_tmp_UpdateUserTables 'tim_p_gallagher@yahoo.co.uk', 'Tim Gallagher'
go
exec sp_tmp_update_Members 'Tim Gallagher', @@IDENTITY
go

exec sp_tmp_UpdateUserTables 'vaughansimpson@hotmail.com', 'Vaughan Simpson'
go
exec sp_tmp_update_Members 'Vaughan Simpson', @@IDENTITY
go

exec sp_tmp_UpdateUserTables 'James Cooper', 'James Cooper'
go
exec sp_tmp_update_Members 'James Cooper', @@IDENTITY
go

exec sp_tmp_UpdateUserTables 'Naomi Cooper', 'Naomi Cooper'
go
exec sp_tmp_update_Members 'Naomi Cooper', @@IDENTITY
go

drop procedure sp_tmp_UpdateUserTables
drop procedure sp_tmp_update_Members
go

drop index dbo.Members.UN_MemberAccount

alter table dbo.Members
drop column [Name]
go

create unique index UN_MemberAccount
on Members([UserId],[account_id])
go

alter table dbo.CashAccount
alter column parameter nvarchar(256) null
go