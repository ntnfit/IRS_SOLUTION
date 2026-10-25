
create procedure APZ_PackingManagement_DelItem(in LineNum int)
as
begin
	DELETE FROM APZ_DMB 
	WHERE "LineNum" = :LineNum; 
end;




