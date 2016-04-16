--drop table dbo.TransactionHistory

create table dbo.TransactionHistory
(
	[account_id] int not null,
	[valuation_date] datetime not null,
	[transaction_date] datetime not null,
	[company_id] int not null,
	[trade_action] varchar(10) not null,
	[quantity] int not null,
	[total_cost] float not null,
	[user] varchar(50)

	constraint FK_accountid_TransactionHistory foreign key
	([account_id]) references Users([User_Id]),

	constraint FK_companyid_TransactionHistory foreign key
	([company_id]) references Companies([Company_Id])
 
)

create index IDX_TransactionHistory on
dbo.TransactionHistory([account_id], [valuation_date])