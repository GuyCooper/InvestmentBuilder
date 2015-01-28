
alter table CashAccount
drop constraint FK_transactionType_CashAccount
go

alter table dbo.Companies
drop constraint UN_CompanyName
go

alter table dbo.InvestmentRecord
drop constraint FK_Company_Id_InvestmentRecord
go

alter table dbo.MembersCapitalAccount
drop constraint FK_Member_Id_MembersAccount
go

drop index IDX_CashAccountDate
on dbo.CashAccount

drop index IDX_MembersAccountDate
on dbo.MembersCapitalAccount

drop index IDX_Valuations_ValuationDate
on dbo.Valuations

drop table dbo.Members
go
drop table dbo.Companies
go
drop table dbo.TransactionType
go
drop table dbo.CashAccount
go
go
drop table dbo.InvestmentRecord
go
drop table dbo.MembersCapitalAccount
go
drop table dbo.Valuations
go

create table dbo.TransactionType
(
	[type_id] int identity primary key clustered,
	[type]			  varchar(20) not null,
	[side]			  char(1) not null,
)

create table dbo.CashAccount
(
	[valuation_date] datetime not null,
	[transaction_date] datetime not null,
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

create table dbo.Members
(
	[Member_Id] int identity primary key clustered,
	[Name] varchar(50)
)

create table dbo.MembersCapitalAccount
(
	[Valuation_Date] datetime not null,
	[Member_Id] int not null,
	[Units] float not null,

	constraint FK_Member_Id_MembersAccount foreign key
	([Member_Id]) references Members([Member_Id])
)


create clustered index IDX_MembersAccountDate on
dbo.MembersCapitalAccount([Valuation_Date])

create table dbo.Valuations
(
	[Valuation_Date] datetime not null,
	[Unit_Price] float not null
)

create clustered index IDX_Valuations_ValuationDate on
dbo.Valuations([Valuation_Date])


/* side: P = Payments (right hand side), R = Receipts (left hand side) */
insert into dbo.TransactionType ([type], side) values ('Admin Fee', 'P')
insert into dbo.TransactionType ([type], side) values ('Purchase', 'P')
insert into dbo.TransactionType ([type], side) values ('Redemption', 'P')
insert into dbo.TransactionType ([type], side) values ('Subscription', 'R')
insert into dbo.TransactionType ([type], side) values ('Dividend', 'R')
insert into dbo.TransactionType ([type], side) values ('Interest', 'R')
insert into dbo.TransactionType ([type], side) values ('Sale', 'R')
insert into dbo.TransactionType ([type], side) values ('BalanceInHand', 'R')
insert into dbo.TransactionType ([type], side) values ('BalanceInHandCF', 'P')

insert into dbo.Members([Name]) values('Guy Cooper')
insert into dbo.Members([Name]) values('Nigel Cooper')
insert into dbo.Members([Name]) values('James Rodgers')
insert into dbo.Members([Name]) values('Vaughan Simpson')
insert into dbo.Members([Name]) values('Tim Gallagher')