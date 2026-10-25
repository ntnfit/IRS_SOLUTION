
create procedure APZ_InventoryCountingRequest_LoadData
(
in id int,
in startFrom timestamp,
in startTo timestamp,
in whs nvarchar(5000)
)
as
begin
if :id>0
then
	select * from APZ_OCRQ where "DocEntry"=:id;
	return ;
end if;
select * from APZ_OCRQ where 
(ifnull(:whs,'') ='' or "WhsCode" in (select Data from split(:whs,';'))) and
(ifnull(:startFrom, '') = '' or "DocDate" >= :startFrom) and
(ifnull(:startTo,'') = '' or "DocDate" <= :startTo) order by "DocEntry" DESC;
end;





