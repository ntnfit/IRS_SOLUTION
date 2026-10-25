-- B1 DEPENDS: BEFORE:PT:CREATE_TABLES_TO_VIEWS

CREATE PROCEDURE _TmSp_CreateViewOIBT
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS
sql1 nclob;
	extFields nclob;
	orgUDFField varchar(20);
	countUDFField int;
	curFieldID int;
	findInBTN int;
	tableID varchar(10);
	rownum int;
BEGIN

	SELECT COUNT(*) into rownum  FROM "PUBLIC"."VIEWS" where "VIEW_NAME" = 'ixvITL_Min' AND "SCHEMA_NAME" = CURRENT_SCHEMA;

	-- DROP UNUSED VIEW ixvITL_Min
	IF  rownum > 0 then
		exec('DROP VIEW ' || '"'|| CURRENT_SCHEMA || '"' || '."ixvITL_Min"');
	end if;

	tableID := 'OBTN';
	select Min("FieldID") , count(1) into curFieldID,countUDFField FROM CUFD WHERE "TableID" = :tableID;

	if :countUDFField = 0 
	then
		tableID := 'OIBT';
		select Min("FieldID"), count(1) into curFieldID,countUDFField FROM CUFD WHERE "TableID" = :tableID;
	end if;

	extFields := '';
	curFieldID := :curFieldID - 1;

	while :countUDFField > 0
		do
		select TOP 1 "AliasID" , "FieldID" into orgUDFField, curFieldID FROM CUFD WHERE "TableID" = :tableID AND "FieldID" > :curFieldID  ORDER BY "FieldID";

		SELECT count(1) into findInBTN FROM CUFD WHERE "TableID" = 'OBTN' and "AliasID" = :orgUDFField;
		
		if :findInBTN > 0
			then 
			extFields := :extFields || ', OBTN."U_' || :orgUDFField || '" AS "U_' || :orgUDFField || '" ';
			END if;
		countUDFField := :countUDFField - 1;
		END while;


	-- create view OIBT
	SELECT COUNT(*) into rownum  FROM "PUBLIC"."VIEWS" where "VIEW_NAME" = 'OIBT' AND "SCHEMA_NAME" = CURRENT_SCHEMA;

	IF  rownum > 0 then
		exec('DROP VIEW ' || '"'|| CURRENT_SCHEMA || '"' || '.OIBT');
	end if;


sql1 := 'CREATE VIEW  OIBT '||
		'AS '||

		'SELECT '||
					'OBTN."ItemCode"	AS "ItemCode", '||
					'OBTN."DistNumber"	AS "BatchNum", '||
					'OBTQ."WhsCode"		AS "WhsCode", '||
					'OITL."ItemName"	AS "ItemName", '||
					'OBTN."MnfSerial"	AS "SuppSerial", '||
					'OBTN."LotNumber"	AS "IntrSerial", '||
					'OBTN."ExpDate"		AS "ExpDate", '||
					'OBTN."MnfDate"		AS "PrdDate", '||
					'OBTN."InDate"		AS "InDate", '||
					'OBTW."Location"	AS "Located", '||
					'OBTN."Notes"		AS "Notes", '||
					'OBTQ."Quantity"	AS "Quantity", '||
					'OITL."DocType"		AS "BaseType", '||
					'OITL."DocEntry"	AS "BaseEntry", '||
					'OITL."DocNum"		AS "BaseNum", '||
					'OITL."DocLine"		AS "BaseLinNum", '||
					'( '||
						'CASE WHEN OITL."DocType" = 59 OR OITL."DocType" = 60 '||
						'		OR OITL."DocType" = 10000071 OR OITL."DocType" = 310000001 '||
						'		OR OITL."DocType" = 58 THEN NULL '||
						'ELSE OITL."CardCode" END '||
					')			AS "CardCode", '||
					'( '||
					'	CASE WHEN OITL."DocType" = 59 OR OITL."DocType" = 60 '||
					'			OR OITL."DocType" = 10000071 OR OITL."DocType" = 310000001 '||
					'			OR OITL."DocType" = 58 THEN NULL '||
					'	ELSE OITL."CardName" END '||
					')			AS "CardName", '||
					'OBTN."CreateDate"	AS "CreateDate", '||
					'OBTN."Status"		AS "Status", '||
					'0			AS "Direction", '||
					'OBTQ."CommitQty"	AS "IsCommited", '||
					'0			AS "OnOrder", '||
					'0			AS "Consig", '||
					'OBTN."DataSource"	AS "DataSource", '||
					'OBTN."UserSign"	AS "UserSign", '||
					'OBTN."Transfered"	AS "Transfered", '||
					'OBTN."Instance"	AS "Instance", '||
                    			'OBTN."SysNumber"	AS "SysNumber", '||
					'OBTN."LogInstanc"	AS "LogInstanc", '||
					'OBTN."UserSign2"	AS "UserSign2", '||
					'OBTN."UpdateDate"	AS "UpdateDate" '||
					:extFields ||	
		'FROM		OBTN '||
		'LEFT JOIN	OBTQ OBTQ ON OBTN."ItemCode" = OBTQ."ItemCode" AND OBTN."SysNumber" = OBTQ."SysNumber" ' ||
		'INNER JOIN	OBTW OBTW ON OBTN."ItemCode" = OBTW."ItemCode" AND OBTN."SysNumber" = OBTW."SysNumber" AND OBTQ."WhsCode" = OBTW."WhsCode" '||
		'INNER JOIN OITL ON OITL."LogEntry" IN ( '||
				'SELECT MIN(T1."LogEntry") AS "LogEntry" '||
				'FROM OITL T1 '||
				'INNER JOIN ITL1 T2 ON T1."LogEntry" = T2."LogEntry" AND T1."ItemCode" = T2."ItemCode" AND T1."ItemCode" = OITL."ItemCode" AND T2."SysNumber" = OBTN."SysNumber" '||
				'GROUP BY T1."ItemCode", T2."SysNumber" ) '||
			'AND OITL."ItemCode" = OBTN."ItemCode"';

		exec (:sql1);

END
