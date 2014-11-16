alter table CashAccount
drop constraint FK_transactionType_CashAccount
go

alter table dbo.Companies
drop constraint UN_CompanyName
go

alter table dbo.InvestmentRecord
drop constraint FK_Company_Id_InvestmentRecord
go

drop index IDX_CashAccountDate
on dbo.CashAccount

drop index IDX_MembersAccountDate
on dbo.MembersCapitalAccount

drop table dbo.TransactionType
go
drop table dbo.CashAccount
go
drop table dbo.Companies
go
drop table dbo.InvestmentRecord
go
drop table dbo.MembersCapitalAccount
go

create table dbo.TransactionType
(
	[type_id] int identity primary key clustered,
	[type] varchar(20) not null
)

create table dbo.CashAccount
(
	[transaction_date] datetime not null,
	[side]             int not null,
	[type_id]		   int not null,
	[parameter]        varchar(50),
	[amount]		   float,

	constraint FK_transactionType_CashAccount foreign key
	([type_id]) references TransactionType([type_id])
)

create clustered index IDX_CashAccountDate on
dbo.CashAccount([transaction_date])

create table dbo.Companies
(
	[Company_Id] int identity primary key clustered,
	[Name] varchar(50) not null,
	[Symbol] char(10) not null,
	[Currency] char(3) not null,
	[DividendDate] datetime,
	[IsActive] tinyint not null default(1),
	[ScalingFactor] float default(1) not null

	constraint UN_CompanyName unique([Name])
)

create table dbo.InvestmentRecord
(
	[Company_id] int not null,
	[Valuation_Date] datetime not null,
	[Shares_Bought] int,	
	[Bonus_Shares issued] int default(0),
	[Shares_Sold] int default(0),
	[Total_Cost] float not null,
	[Selling_Price] float not null,
	[Dividends_Received] float default(0),

	constraint FK_Company_Id_InvestmentRecord foreign key
	([Company_Id]) references Companies([Company_Id])
)

create clustered index IDX_InvestmentRecordDate on
dbo.InvestmentRecord([Valuation_Date])

create table dbo.MembersCapitalAccount
(
	[Valuation_Date] datetime not null,
	[Member] varchar(50) not null,
	[Units] float not null
)

create clustered index IDX_MembersAccountDate on
dbo.MembersCapitalAccount([Valuation_Date])