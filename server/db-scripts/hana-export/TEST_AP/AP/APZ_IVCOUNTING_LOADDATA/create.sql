
create procedure APZ_IVCounting_LoadData
(
in id int default 0,
in startFrom timestamp default '',
in startTo timestamp default '',
in whs nvarchar(50) default '',
in status char(1) default ''
)
as
begin
if :id>0
then
	select * from APZ_OINC where "DocEntry"=:id;
	return ;
end if;
select * from APZ_OINC where 
(ifnull(:whs,'') ='' or "WhsCode"=:whs) and
(ifnull(:startFrom, '') = '' or "DocDate" >= :startFrom) and
(ifnull(:startTo,'') = '' or "DocDate" <= :startTo)and
(ifnull(:status,'') ='' or "DocStatus"=:status)
 order by "DocEntry" DESC;
end;




