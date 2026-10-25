
create procedure APZ_PackingManagement_selectMaxLinenum
as
begin
	select top 1 "LineNum" from APZ_DMB order by "LineNum" DESC;
end;




