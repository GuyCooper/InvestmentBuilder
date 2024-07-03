 
create table dbo.CashAccountTemp
(
	[transaction_id]   int identity(1,1),
	[valuation_date]   datetime not null,
	[transaction_date] datetime not null,
	[type_id]		   int not null,
	[parameter]        nvarchar(256),
	[amount]		   decimal null,
	[account_id]	   int not null default(0) ,
	constraint FK_transactionType_CashAccount1 foreign key
	([type_id]) references TransactionType([type_id])
)

set identity_insert CashAccountTemp on

insert into CashAccountTemp(

[transaction_id], 
[valuation_date],
[transaction_date],
[type_id],
[parameter],
[amount],
[account_id])
select 
[transaction_id], 
[valuation_date],
[transaction_date],
[type_id],
[parameter],
[amount],
[account_id]
from CashAccount

set identity_insert CashAccountTemp off

alter table CashAccount
drop constraint if exists FK_transactionType_CashAccount

drop table CashAccount

create table dbo.CashAccount
(
	[transaction_id]   int identity(1,1),
	[valuation_date]   datetime not null,
	[transaction_date] datetime not null,
	[type_id]		   int not null,
	[parameter]        nvarchar(256),
	[amount]		   decimal null,
	[account_id]	   int not null default(0) ,
	constraint FK_transactionType_CashAccount foreign key
	([type_id]) references TransactionType([type_id])
)

set identity_insert CashAccount on

insert into CashAccount(

[transaction_id], 
[valuation_date],
[transaction_date],
[type_id],
[parameter],
[amount],
[account_id])
select 
[transaction_id], 
[valuation_date],
[transaction_date],
[type_id],
[parameter],
[amount],
[account_id]
from CashAccountTemp

set identity_insert CashAccountTemp off

alter table CashAccountTemp
drop constraint if exists FK_transactionType_CashAccount

drop table CashAccountTemp