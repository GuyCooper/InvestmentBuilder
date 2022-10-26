
drop index Redemptions.IDX_Redemptions

alter table dbo.Redemptions
add constraint UN_MemberDate unique([member_id],[transaction_date])
go

alter table dbo.Redemptions
drop column [status]
go

alter table dbo.Redemptions
add Redemption_Id int identity primary key clustered
go

alter table dbo.Redemptions
add [status] int null
go


update dbo.Redemptions
set [status] = 0
