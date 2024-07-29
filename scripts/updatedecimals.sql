alter table cashaccount 
alter column amount
decimal(18,2) null


alter table HistoricalYieldData 
alter column yield
decimal(18,4) null

alter table investmentrecord
alter column shares_bought
decimal(18,2) null

alter table investmentrecord
alter column shares_sold
decimal(18,2) null

alter table investmentrecord
alter column [bonus_shares issued]
decimal(18,2) null

alter table investmentrecord
alter column Total_cost
decimal(18,2) null

alter table investmentrecord
alter column selling_price
decimal(18,2) null

alter table investmentrecord
alter column dividends_received
decimal(18,2) null

alter table memberscapitalaccount
alter column [units]
decimal(18,4)

alter table redemptions
alter column [units]
decimal(18,2) null

alter table redemptions
alter column amount
decimal(18,2) null

alter table transactionhistory
alter column quantity
decimal(18,2) null

alter table transactionhistory
alter column total_cost
decimal(18,2) null

alter table Valuations
alter column unit_price
decimal(18,4) null


