

CREATE PROCEDURE Apz_UDT_GetListUDT(in UserName NVARCHAR(20) default '' ,in Branch NVARCHAR(20) default '')
AS
BEGIN
	if :UserName = ''
	then
		SELECT ifnull(a."TableName", '''') AS "TableName", IFNULL(a."Descr", '''') AS "Descr"
	    FROM OUTB a
	    WHERE "ObjectType" = 0 or "ObjectType" = 5
		ORDER BY "TableName";
	else
		SELECT ifnull(a."TableName", '') AS "TableName", IFNULL(a."Descr", '') AS "Descr"
	    FROM OUTB a inner join OPRM b on 'UDT_'|| a."TableName" = b."menucode" 
		where ("ObjectType" = 0 or "ObjectType" = 5) and USER_CODE = :UserName and  "authoriza" IN ('F', 'R') ORDER BY "TableName";
	end if;
end;




