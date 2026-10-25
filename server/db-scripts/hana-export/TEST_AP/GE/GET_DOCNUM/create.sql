create procedure Get_DocNum()
as
begin
	select "DocNum","ObjType" as "DocType" from OWOR
	union ALL
	select "DocNum","ObjType" as "DocType" from opdn
	;
end;

