alter table CashAccount
alter column amount decimal null

alter table Companies
alter column [ScalingFactor] decimal not null

alter table Companies
add constraint df_scalingfactor default(1) for [ScalingFactor]

alter table HistoricalYieldData
alter column Yield decimal null

alter table InvestmentRecord
alter column Shares_Bought decimal null

alter table InvestmentRecord
alter column [Bonus_Shares issued] decimal null

alter table InvestmentRecord
alter column Shares_Sold decimal null

alter table InvestmentRecord
alter column Total_Cost decimal not null

alter table InvestmentRecord
alter column Selling_Price decimal not null

alter table InvestmentRecord
alter column Dividends_Received decimal null

alter table InvestmentRecord
add constraint  df_Bonus_Shares_issued default(0) for [Bonus_Shares issued]

alter table InvestmentRecord
add constraint df_Shares_Sold default(0) for [Shares_Sold]

alter table InvestmentRecord
add constraint df_Dividends_Received default(0) for [Dividends_Received]

alter table InvestmentRecord
add constraint df_account_id default(0) for [account_id]

alter table InvestmentRecord
add constraint df_is_active default(1) for [is_active]

alter table MembersCapitalAccount
alter column Units decimal not null

alter table Redemptions
alter column amount decimal not null

alter table Redemptions
alter column units decimal null

alter table TransactionHistory
alter column quantity decimal not null

alter table TransactionHistory 
alter column total_cost decimal not null

alter table Valuations
alter column Unit_Price decimal not null

alter table Valuations
add constraint df_valuations_account_id default(0) for [account_id]
