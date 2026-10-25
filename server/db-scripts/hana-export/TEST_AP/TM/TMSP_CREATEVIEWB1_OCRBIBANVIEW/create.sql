-- B1 DEPENDS: BEFORE:PT:PROCESS_START

CREATE PROCEDURE TmSp_CreateViewB1_OcrbIBANView
language sqlscript   
SQL SECURITY INVOKER
AS
sql1 nvarchar(4999);
rownum int;
BEGIN

	SELECT COUNT(*) into rownum  FROM "PUBLIC"."VIEWS" where "VIEW_NAME" = 'B1_OcrbIBANView' AND "SCHEMA_NAME" = current_schema;
	 IF :rownum > 0
	 then
	sql1 := 'DROP VIEW "'||current_schema||'"."B1_OcrbIBANView"';
		exec (:sql1);
     end if;

	sql1 := 'CREATE VIEW "'||current_schema||'"."B1_OcrbIBANView"  '||
	'AS  '||
	'SELECT T0."AbsEntry", T0."CardCode", REPLACE(T0."IBAN", '' '', '''') "IBAN", T0."ISRBillerI", T0."CustIdNum" FROM "'||current_schema||'"."OCRB" T0 '; 

	exec (:sql1);

END


