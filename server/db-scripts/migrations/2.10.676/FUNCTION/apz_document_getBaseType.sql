
drop function apz_document_getBaseType;

GO


create function apz_document_getBaseType(in objType int)
returns arrBase int ARRAY
as
begin
	
	declare arrInt int ARRAY;
	
	if :objType = 17 then
		arrInt = array(23);
	elseif :objType = 15 then
		arrInt = array(23,17,16,13,15);
	elseif :objType = 13 then
		arrInt = array(23,17,15,13);
	elseif :objType = 234000031 then
		arrInt = array(15,13);
	elseif :objType = 16 then
		arrInt = array(15,234000031,16);
	elseif :objType = 540000006 then
		arrInt = array(1470000113);
	elseif :objType = 22 then
		arrInt = array(1470000113,540000006);
	elseif :objType = 20 then
		arrInt = array(22,540000006,20);
	elseif :objType = 18 then
		arrInt = array(22,20,540000006,18);
	elseif :objType = 21 then
		arrInt = array(20,234000032,21);
	elseif :objType = 234000032 then
		arrInt = array(20,18);
	elseif :objType = 67 then
		arrInt = array(1250000001);
	end if;
	
	arrBase = :arrInt;
	
end