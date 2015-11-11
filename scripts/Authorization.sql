create table Administrators
  (
	[Name] varchar(50) primary key clustered not null 
  )

  Alter table Members
  add [Authorization] int not null default(0)
  go
