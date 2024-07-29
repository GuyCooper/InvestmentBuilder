--drop table dbo.TransactionHistory

create table dbo.Redemptions
(
	[member_id] int not null,
	[transaction_date] datetime not null,
	[amount] decimal(18,2) not null,
	[units] decimal(18,2) null,
	[status] varchar(10) not null,
	
	constraint FK_memberid_Redemptions foreign key
	([member_id]) references Members([member_id]), 
)

create index IDX_Redemptions on
dbo.Redemptions([member_id], [transaction_date])
