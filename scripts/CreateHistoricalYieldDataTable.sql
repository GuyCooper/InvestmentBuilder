drop table HistoricalYieldData

create table dbo.HistoricalYieldData
(
	[Id] int identity primary key clustered,
	[Name]			  varchar(50) not null,
	[Year]			  int,
	[Yield]			  float,
	constraint UN_YieldAmount unique([Name],[Year])
)

go