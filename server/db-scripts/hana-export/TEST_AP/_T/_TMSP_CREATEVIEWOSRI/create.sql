-- B1 DEPENDS: BEFORE:PT:CREATE_TABLES_TO_VIEWS

CREATE PROCEDURE _TmSp_CreateViewOSRI
language sqlscript
SQL SECURITY INVOKER
as 
	sql1 nclob;
	extFields nclob;
	finalExtFields nclob;
	orgUDFField varchar(20);
	newUDFField varchar(20);
	countUDFField int;
	curFieldID int;
	dupFieldInIBT int;
	findInSRN int;
	sufixString varchar (20);
	sufixIndex int;	
	ctmp int;
	tableID varchar(10);
-----------------store local result of STR----------------------	
	chtmp1 varchar(1);
	chtmp2 varchar(1);
	chtmp3 varchar(1);
-----------------store local result of STR----------------------	
	DBName nvarchar(255);
	
begin
		select count(*) into ctmp 
		from  "PUBLIC".views 
		WHERE view_name = 'ixvITL_Min' AND "SCHEMA_NAME" = CURRENT_SCHEMA;

		
		IF  ctmp > 0 then	
			exec('drop view ' || '"' || CURRENT_SCHEMA || '"' || '."ixvITL_Min"');
		end if;
		sql1 := 'CREATE VIEW ' || '"' || CURRENT_SCHEMA || '"' || '."ixvITL_Min" AS SELECT MIN("LogEntry") AS "LogEntry", "ItemCode", "SysNumber" FROM ITL1 GROUP BY "ItemCode", "SysNumber" ';
		
		exec (:sql1);
		
		-- CREATE VIEW ixvITL_LastWhs
		select count(*) into ctmp from "PUBLIC".views where view_name = 'ixvITL_LastWhs' AND "SCHEMA_NAME" = CURRENT_SCHEMA;
	
	IF  ctmp > 0 then
		exec('DROP VIEW ' || '"' || CURRENT_SCHEMA || '"' || '."ixvITL_LastWhs"');
    end if;
	
	sql1 := 'CREATE VIEW ' || '"' || CURRENT_SCHEMA || '"' || '."ixvITL_LastWhs"	AS	SELECT		T0."ItemCode", lastLogEntry."SysNumber", T0."LocCode" AS "WhsCode" FROM		' || '"' || CURRENT_SCHEMA || '"' || '.OITL  T0 INNER JOIN	(SELECT	MAX("LogEntry") AS "LogEntry", "ItemCode", "SysNumber" FROM ' || '"' || CURRENT_SCHEMA || '"' || '.ITL1 GROUP BY "ItemCode", "SysNumber") AS lastLogEntry ON lastLogEntry."LogEntry" = T0."LogEntry" AND T0."ItemCode" = lastLogEntry."ItemCode" ';
	exec (:sql1);
	
	-- prepare UDF
	tableID := 'OSRN';
	select  Min("FieldID") into curFieldID FROM CUFD WHERE "TableID" = :tableID;
	select  count(1) into countUDFField FROM CUFD WHERE "TableID" = :tableID;
	
	if :countUDFField = 0 then
		tableID := 'OSRI';
		select Min("FieldID") into curFieldID FROM CUFD WHERE "TableID" = :tableID;
		select  count(1) into countUDFField  FROM CUFD WHERE "TableID" = :tableID;
	end if;
	
	extFields := '';
	finalExtFields := '';
	curFieldID := :curFieldID - 1;
	
		
	while :countUDFField > 0 do
		--get old column name
			select TOP 1 "AliasID" into orgUDFField FROM CUFD WHERE "TableID" = :tableID AND "FieldID" > :curFieldID ORDER BY "FieldID";
			select TOP 1 "FieldID" into curFieldID  FROM CUFD WHERE "TableID" = :tableID AND "FieldID" > :curFieldID ORDER BY "FieldID";
			newUDFField := :orgUDFField;
		
			if :tableID = 'OSRI' then
				SELECT count(1) into dupFieldInIBT FROM CUFD WHERE "TableID" = 'OIBT' and "AliasID" = :newUDFField;
				sufixIndex := 1;
				--get new column name
				while :dupFieldInIBT > 0 do
					sufixString := '_' || LTRIM(:sufixIndex);
					newUDFField := RTRIM(SUBSTRING(:orgUDFField, 1, (18 - LENGTH(:sufixString)))) || :sufixString;			
					SELECT count(1) into dupFieldInIBT FROM CUFD WHERE "TableID" = 'OIBT' and "AliasID" = :newUDFField;
					sufixIndex := :sufixIndex + 1;
				end while;
				--check new column name
				SELECT count(1) into findInSRN FROM CUFD WHERE "TableID" = 'OSRN' and "AliasID" = :newUDFField;				
			else 
				findInSRN := 1;
			end if;
		--add column into view
			if :findInSRN > 0
				then
					extFields := :extFields || ', T0."U_' || :newUDFField || '" AS "U_' || :orgUDFField || '" ';
					finalExtFields := :finalExtFields || ',"U_' || :orgUDFField || '" ';
			END if;
			countUDFField := :countUDFField - 1;					
	end while;
		-- create view OSRI
	select count(*) into ctmp FROM "PUBLIC".views WHERE view_name = 'OSRI' AND "SCHEMA_NAME" = CURRENT_SCHEMA;
	IF  ctmp > 0 then
		exec('DROP VIEW ' || '"' || CURRENT_SCHEMA || '"' || '. OSRI');
	end if;
	
	sql1 := 'CREATE VIEW ' || '"' || CURRENT_SCHEMA || '"' || '."OSRI" ( "ItemCode",'||
	 '"SysSerial","SuppSerial","IntrSerial","BatchId","ExpDate",'||
	 '"PrdDate","InDate","GrntStart","GrntExp","WhsCode","Located",'||
	 '"Notes","Quantity","BaseType","BaseEntry","BaseNum","BaseLinNum",'||
	 '"CreateDate","CardCode","CardName","ItemName","Status","Direction",'||
	 '"DataSource","UserSign","Transfered","Instance" ' || :finalExtFields ||') AS ('||
		 'select t11.* from ('||
			 'SELECT '||
			     'T0."ItemCode" AS "ItemCode",'||
			     'T0."SysNumber" AS "SysSerial",'||
			     'T0."MnfSerial" AS "SuppSerial",'||
			     'T0."DistNumber" AS "IntrSerial",'||
			     'T0."LotNumber" AS "BatchId",'||
			     'T0."ExpDate" AS "ExpDate",'||
			     'T0."MnfDate" AS "PrdDate",'||
			     'T0."InDate" AS "InDate",'||
			     'T0."GrntStart" AS "GrntStart",'||
			     'T0."GrntExp" AS "GrntExp",'||
			     'OSRQ."WhsCode" AS "WhsCode",'||
			     'T0."Location" AS "Located",'||
			     'T0."Notes" AS "Notes",'||
			     'T2."DocQty" AS "Quantity",'||
			     'T2."DocType" AS "BaseType",'||
			     'T2."DocEntry" AS "BaseEntry",'||
			     'T2."DocNum" AS "BaseNum",'||
			     'T2."DocLine" AS "BaseLinNum",'||
			     'T0."CreateDate" AS "CreateDate",'||
			     '( CASE when T2."DocType" = 59 or T2."DocType" = 60 or T2."DocType" = 10000071 or T2."DocType" = 310000001 or T2."DocType" = 58 then NULL ELSE T2."CardCode" END ) AS "CardCode",'||
			     '( CASE when T2."DocType" = 59 or T2."DocType" = 60 or T2."DocType" = 10000071 or T2."DocType" = 310000001 or T2."DocType" = 58 then NULL ELSE T2."CardName" END ) AS "CardName",'||
			     'T2."ItemName" AS "ItemName",'||
			    '(case when OSRQ."Quantity" is null then 1 when OSRQ."Quantity" = 0 then 1 when OSRQ."Quantity" = 1 then 0 else null end) AS "Status",'||
			     '0 AS "Direction",'||
			     'T0."DataSource" AS "DataSource",'||
			     'T0."UserSign" AS "UserSign",'||
			     'T0."Transfered" AS "Transfered",'||
			     'T0."Instance" AS "Instance" '||
			     :extFields||
			'FROM '|| '"' || CURRENT_SCHEMA || '"' || '.OSRN T0  '||
			    'LEFT JOIN ' || '"' || CURRENT_SCHEMA || '"' || '.OSRQ OSRQ  '||
			        'ON T0."ItemCode" = OSRQ."ItemCode" AND T0."SysNumber" = OSRQ."SysNumber"  '||
			    'INNER JOIN ' || '"' || CURRENT_SCHEMA || '"' || '."ixvITL_Min" T1  '||
			        'ON T0."ItemCode" = T1."ItemCode" AND T0."SysNumber" = T1."SysNumber"  '||
			    'INNER JOIN ' || '"' || CURRENT_SCHEMA || '"' || '.OITL T2  '||
			        'ON T2."LogEntry" = T1."LogEntry" AND T2."ItemCode" = T1."ItemCode"  '||
			'WHERE (OSRQ."Quantity" is null OR OSRQ."Quantity" = 0 or OSRQ."Quantity" = 1)  '||
			') t11  '||
			   'INNER JOIN ' || '"' || CURRENT_SCHEMA || '"' || '."ixvITL_LastWhs" T3  '||
			        'ON T11."ItemCode" = T3."ItemCode" AND T11."SysSerial" = T3."SysNumber"   AND t11."WhsCode" = T3."WhsCode"  '||
		') 	'||
	 'WITH READ ONLY';
	 exec(:sql1);
end;
