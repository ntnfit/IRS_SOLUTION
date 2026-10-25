
CREATE  PROCEDURE Apz_Udf_GetLinkedTable( 
	in LinkedTable NVARCHAR(50)
,	userName NVARCHAR(50) default ''
,	branch INT default -1)
AS
BEGIN
	DECLARE objType NVARCHAR(10);
	
	select "ObjectType" into objType from OUTB where "TableName"=:LinkedTable;
	
	if :objType in (0, 1, 5) then
		EXECUTE IMMEDIATE('SELECT "Code", "Name" FROM "@' || :LinkedTable || '"');
	else
		EXECUTE IMMEDIATE('SELECT "DocEntry" AS "Code", "DocNum" AS "Name" FROM "@' || :LinkedTable || '"');
	end if;
END;

