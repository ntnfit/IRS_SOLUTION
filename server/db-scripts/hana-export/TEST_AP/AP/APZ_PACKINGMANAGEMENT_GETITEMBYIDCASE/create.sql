
create procedure APZ_PackingManagement_GetItemByIdCase(in LineNumber int)
as
begin
	SELECT *
	From APZ_DMB
	Where "LineNumber" = :LineNumber
	ORDER BY "LineNum" DESC;
end;




