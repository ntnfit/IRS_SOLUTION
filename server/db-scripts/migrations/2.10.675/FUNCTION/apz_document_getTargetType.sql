
drop function apz_document_getTargetType;

GO


create function apz_document_getTargetType(in objType int)
returns arrTarget int ARRAY
as
begin
	
	declare arrInt int ARRAY;
	
	if :objType = 23 then
		arrInt = array(17,15,13);
	elseif :objType = 13 then
		arrInt = array(15,13);
	elseif :objType = 15 then
		arrInt = array(234000031,16,13,15);
	elseif :objType = 18 then
		arrInt = array(234000032,19,18);
	elseif :objType = 17 then
		arrInt = array(15,13);
	elseif :objType = 540000006 then
		arrInt = array(22,20,18);
	elseif :objType = 22 then
		arrInt = array(20,18);
	elseif :objType = 20 then
		arrInt = array(21,234000032,18,20);
	elseif :objType = 16 then
		arrInt = array(15,14,16);
	elseif :objType = 234000031 then
		arrInt = array(16,14);
	elseif :objType = 234000032 then
		arrInt = array(19,21);
	end if;
	
	arrTarget = :arrInt;
end