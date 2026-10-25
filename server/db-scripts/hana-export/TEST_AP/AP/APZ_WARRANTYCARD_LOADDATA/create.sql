
create procedure APZ_WarrantyCard_LoadData
(
in id int,
in warrantyNo nvarchar(50),
in itemCode nvarchar(50),
in bpCode nvarchar(50),
in status char(1)
)
as
begin
if :id>0
then
	select * from APZ_OINS where "WrntyID"=:id;
	return ;
end if;
select * from APZ_OINS where 
(ifnull(:warrantyNo,'') ='' or "WrntyNum"=:warrantyNo) and
(ifnull(:status,'') ='' or "Status"=:status) and
(ifnull(:itemCode, '') = '' or "ItemCode" = :itemCode) and
(ifnull(:bpCode,'') = '' or "CardCode" = :bpCode);
end;




