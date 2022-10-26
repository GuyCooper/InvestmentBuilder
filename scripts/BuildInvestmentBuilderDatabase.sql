use master

if exists(select 1 from sysdatabases where name = '$(dbname)')
begin
	alter database $(dbname) set single_user with
	rollback immediate
	drop database $(dbname)
end

go

create database $(dbname)
go

use $(dbname)
go

create table dbo.TransactionType
(
	[type_id] int identity primary key clustered,
	[type]			  varchar(20) not null,
	[side]			  char(1) not null,
)

create table dbo.AccountTypes
(
	[Type_Id] int identity primary key clustered,
	[Type] varchar(50) not null,
	constraint UN_UserTypeName unique([Type])
)

create table dbo.[Users]
(
	[UserId] int identity primary key clustered,
	[UserName]	 nvarchar(256) not null,
	[Description] nvarchar(256) default(null)
	
	constraint UN_UserName unique([UserName])
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

	constraint FK_AccountType_User foreign key
	([Type_Id]) references AccountTypes([Type_Id])
)

create index IDX_AccountName on
dbo.Accounts([Name])
go

insert into dbo.AccountTypes ([Type]) values ('Club')
insert into dbo.AccountTypes ([Type]) values ('Personal')
go

create table Administrators
  (
	[Name] varchar(50) primary key clustered not null 
  )
go

create table dbo.CashAccount
(
	[transaction_id]   int identity(1,1),
	[valuation_date]   datetime not null,
	[transaction_date] datetime not null,
	[type_id]		   int not null,
	[parameter]        nvarchar(256),
	[amount]		   float,
	[account_id]	   int not null default(0) ,
	constraint FK_transactionType_CashAccount foreign key
	([type_id]) references TransactionType([type_id])
)
go

create clustered index IDX_CashAccountDate on
dbo.CashAccount([transaction_date])
go

create index IDX_CashAccountAccountID on
dbo.CashAccount([account_id])
go

create table dbo.Companies
(
	[Company_Id] int identity primary key clustered,
	[Name] varchar(50) not null,
	[Symbol] char(20) not null,
	[Currency] char(3) not null,
	[DividendDate] datetime,
	[IsActive] tinyint not null default(1),
	[ScalingFactor] float default(1) not null,
	[LastBoughtDate] datetime null,
	[Exchange] varchar(10) default(null), 
	constraint UN_CompanyName unique([Name])
)
go

create table dbo.InvestmentRecord
(
	[id] int identity primary key,
	[Company_id] int not null,
	[Valuation_Date] datetime not null,
	[Shares_Bought] float,	
	[Bonus_Shares issued] float default(0),
	[Shares_Sold] float default(0),
	[Total_Cost] float not null,
	[Selling_Price] float not null,
	[Dividends_Received] float default(0),
	[account_id] int not null default(0),
	[is_active] tinyint not null default(1),
	[last_bought] DATETIME,  
	constraint FK_Company_Id_InvestmentRecord foreign key
	([Company_Id]) references Companies([Company_Id])
)
go

create index IDX_InvestmentRecordDate on
dbo.InvestmentRecord([Valuation_Date])
go

create index IDX_InvestmentRecordAccountID on
dbo.InvestmentRecord([account_id])
go

create table dbo.Members
(
	[Member_Id] int identity primary key clustered,
	[UserId] int,
	[account_id] int not null default(0),
	[enabled] tinyint not null default(1),
	[Authorization] int not null default(0),

	constraint FK_members_UserID_Users_UserID foreign key
	([UserId]) references [Users]([UserId])
)
go

create index IDX_MembersAccountID on
dbo.Members([account_id])
go

create unique index UN_MemberAccount
on Members([UserId],[account_id])
go

create table dbo.MembersCapitalAccount
(
	[Valuation_Date] datetime not null,
	[Member_Id] int not null,
	[Units] float not null,

	constraint FK_Member_Id_MembersAccount foreign key
	([Member_Id]) references Members([Member_Id])
)
go

create clustered index IDX_MembersAccountDate on
dbo.MembersCapitalAccount([Valuation_Date])
go

create table dbo.Valuations
(
	[Valuation_Date] datetime not null,
	[Unit_Price] float not null,
	[account_id] int not null default(0) 
)
go

create clustered index IDX_Valuations_ValuationDate on
dbo.Valuations([Valuation_Date])
go

create index IDX_ValuationsAccountID on
dbo.Valuations([account_id])
go

create table dbo.TransactionHistory
(
	[account_id] int not null,
	[valuation_date] datetime not null,
	[transaction_date] datetime not null,
	[company_id] int not null,
	[trade_action] varchar(10) not null,
	[quantity] float not null,
	[total_cost] float not null,
	[user] varchar(50)

	constraint FK_accountid_TransactionHistory foreign key
	([account_id]) references Accounts([Account_Id]),

	constraint FK_companyid_TransactionHistory foreign key
	([company_id]) references Companies([Company_Id])
 
)

create index IDX_TransactionHistory on
dbo.TransactionHistory([account_id], [valuation_date])
go

create table dbo.Redemptions
(
	Redemption_Id int identity primary key clustered,
	[member_id] int not null,
	[transaction_date] datetime not null,
	[amount] float not null,
	[units] float null,
	[status] int not null,
	
	constraint FK_memberid_Redemptions foreign key
	([member_id]) references Members([member_id]), 

	constraint UN_MemberDate unique([member_id],[transaction_date])
)

go

create table dbo.HistoricalData
(
	[Name]			  varchar(50) not null,
	[Symbol]		  varchar(15),
	[Data]			  Text
)
go

create table dbo.HistoricalYieldData
(
	[Id] int identity primary key clustered,
	[Name]			  varchar(50) not null,
	[Year]			  int,
	[Yield]			  float,
	constraint UN_YieldAmount unique([Name],[Year])
)
go
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
insert into dbo.TransactionType ([type], side) values ('Fx Gain', 'R')
insert into dbo.TransactionType ([type], side) values ('Fx Loss', 'P')

--insert into dbo.Members([Name]) values('Guy Cooper')
--insert into dbo.Members([Name]) values('Nigel Cooper')
--insert into dbo.Members([Name]) values('James Rodgers')
--insert into dbo.Members([Name]) values('Vaughan Simpson')
--insert into dbo.Members([Name]) values('Tim Gallagher')
