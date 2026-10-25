

CREATE PROCEDURE APZ_Territory_Search(in id nvarchar(240) default '')
as
begin
	if :id=''
	then
		select * from OTER where "descript" like '%' + :id + '%'; 
	else 
		select * from OTER; 
	end if;
end;




