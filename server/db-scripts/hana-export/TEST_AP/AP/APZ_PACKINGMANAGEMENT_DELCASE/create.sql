
create procedure APZ_PackingManagement_DelCase(in LineNumber int)
as
begin
	DELETE FROM APZ_DMC
	WHERE "LineNumber" = :LineNumber;
      
	DELETE FROM APZ_DMB 
	WHERE "LineNumber" = :LineNumber;
end;




