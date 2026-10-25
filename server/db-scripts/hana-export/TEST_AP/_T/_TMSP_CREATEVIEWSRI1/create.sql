-- B1 DEPENDS: BEFORE:PT:CREATE_TABLES_TO_VIEWS

CREATE PROCEDURE _TmSp_CreateViewSRI1
language sqlscript 
SQL SECURITY INVOKER
as
sql1 varchar(4096);
ctmp int;
DBName nvarchar(255);
begin
	-- create view SRI1_LINK
	SELECT count(*) into ctmp FROM "PUBLIC".views WHERE view_name = 'SRI1_LINK' AND "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	IF  ctmp > 0 then
		exec('DROP VIEW ' || '"'|| CURRENT_SCHEMA || '"' || '.SRI1_LINK');
	end if;
	
	sql1 := 'CREATE view ' || '"'|| CURRENT_SCHEMA || '"' || '.SRI1_LINK as  SELECT b."LogEntry" as "LogEntry", b."ItemCode" as "ItemCode", c."SysNumber"		as "SysSerial", b."ApplyType"		as "BaseType", b."ApplyEntry"	as "BaseEntry", b."AppDocNum"		as "BaseNum", b."ApplyLine"		as "BaseLinNum", b."DocDate"		as "DocDate", b."LocCode"		as "WhsCode", b."CardCode"		as "CardCode", b."CardName"		as "CardName", ( CASE WHEN a."Quantity" > 0 THEN 0  ELSE 1 END ) as "Direction", b."CreateDate"	as "CreateDate", b."ItemName"		as "ItemName", ''N''				as "DataSource", NULL			as "UserSign", b."BaseType"		as "BsDocType", b."BaseEntry"		as "BsDocEntry", b."BaseLine"		as "BsDocLine" FROM		' || '"'|| CURRENT_SCHEMA || '"' || '.ITL1 a INNER JOIN	' || '"'|| CURRENT_SCHEMA || '"' || '.OITL b ON a."LogEntry" = b."LogEntry" INNER JOIN	OSRN c ON a."ItemCode" = c."ItemCode" and a."SysNumber" = c."SysNumber" ';
	exec(:sql1);
	
	-- create view SRI1
	SELECT count(*) into ctmp FROM "PUBLIC".views WHERE view_name = 'SRI1' AND "SCHEMA_NAME" = CURRENT_SCHEMA;
	IF  :ctmp > 0 then
		exec('DROP VIEW ' || '"'|| CURRENT_SCHEMA || '"' || '.SRI1');
	end if;
	
	sql1 := 'create view ' || '"'|| CURRENT_SCHEMA || '"' || '.SRI1 as SELECT "ItemCode","SysSerial",(select count(1) from ' || '"'|| CURRENT_SCHEMA || '"' || '.SRI1_LINK b where b."ItemCode" = a."ItemCode" and b."SysSerial" = a."SysSerial" and b."LogEntry" < a."LogEntry" ) as "LineNum","BaseType","BaseEntry","BaseNum","BaseLinNum","DocDate","WhsCode","CardCode","CardName","Direction","CreateDate","ItemName","DataSource","UserSign","BsDocType","BsDocEntry", "BsDocLine" from ' || '"'|| CURRENT_SCHEMA || '"' || '.SRI1_LINK a ';
	exec (:sql1);
end;


