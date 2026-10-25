-- B1 DEPENDS: BEFORE:PT:PROCESS_START

CREATE PROCEDURE _TmSp_FormattedSearchUpdate(
IN ui01 int,IN ui02 int,IN ui03 int,IN ui04 int,IN ui05 int,IN ui06 int,
IN ui07 int,IN ui08 int,IN ui09 int,IN ui10 int,IN ui11 int,IN ui12 int,
IN ui13 int,IN ui14 int,IN ui15 int,IN ui16 int,IN ui17 int,IN ui18 int,
IN ui19 int,IN ui20 int,IN ui21 int,IN ui22 int,IN ui23 int,IN ui24 int,
IN ui25 int,IN ui26 int,IN ui27 int,IN ui28 int,IN ui29 int)

LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS
------------------------------------------------------------------
db01 int;
db02 int;
db03 int;
db04 int;
db05 int;
db06 int;
db07 int;
db08 int;
db09 int;
db10 int;
db11 int;
db12 int;
db13 int;
db14 int;
db15 int;
db16 int;
db17 int;
db18 int;
db19 int;
db20 int;
db21 int;
db22 int;
db23 int;
db24 int;
db25 int;
db26 int;
db27 int;
db28 int;
db29 int;
FieldID varchar (20);
ColId varchar (20);
ByField nvarchar (1);
FormID varchar (20);
ItemId varchar (20);
hasNext int;
CURSOR Currs for 
	select  "FieldID","ColID","ByField","FormID","ItemID"
	From CSHS
	Where "FieldID" NOT LIKE 'U%' AND "FieldID" IS NOT NULL;
Begin
hasNext := 1;

	-------------------------------------------------------------------
	SELECT count (1) into db01 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'ODLN' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;				
	SELECT count (1) into db02 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'DLN1' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;
	SELECT count (1) into db03 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'DLN1' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;				
	SELECT count (1) into db04 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'WTR1' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;
	SELECT count (1) into db05 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'IGN1' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;				
	SELECT count (1) into db06 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'OCRD' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;				
	SELECT count (1) into db07 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'CRD1' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;								
	SELECT count (1) into db08 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'OCPR' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;
	SELECT count (1) into db09 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'OITM' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;
	SELECT count (1) into db10 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'OPLN' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;
	SELECT count (1) into db11 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'OCLG' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;
	SELECT count (1) into db12 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'OACT' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;
	SELECT count (1) into db13 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'ORCT' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;
	SELECT count (1) into db14 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'RCT2' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;
	SELECT count (1) into db15 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'RCT4' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;
	SELECT count (1) into db16 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'RCT1' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;
	SELECT count (1) into db17 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'RCT3' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;
	SELECT count (1)into db18 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'ODPS' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;
	SELECT count (1) into db19 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'OITT' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;
	SELECT count (1)  into db20 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'ITT1' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;
	SELECT count (1) into db21 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'OWKO' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;
	SELECT count (1) into db22 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'WKO1' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;
	SELECT count (1) into db23 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'OJDT' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;
	SELECT count (1) into db24 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'JDT1' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;
	SELECT count (1) into db25 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'JDT1' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;
	SELECT count (1) into db26 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'OOCR' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;
	SELECT count (1) into db27 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'OPRC' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;
	SELECT count (1) into db28 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'OBGS' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;
	SELECT count (1) into db29 from "PUBLIC"."TABLE_COLUMNS" c INNER JOIN
					"PUBLIC".objects o on o.object_oid = c.table_oid
					WHERE o.object_name = 'OPRJ' 
					AND c.column_name NOT LIKE 'U[_]%'
					AND o.SCHEMA_NAME = CURRENT_SCHEMA
					AND c.SCHEMA_NAME = CURRENT_SCHEMA;
	-------------------------------------------------------------------

	Open Currs;
	Fetch Currs Into FieldID,ColId,ByField,FormID,ItemId;
	if NOT Currs::NOTFOUND then
		hasNext := -1;
	end if;
	
	while :hasNext > 0 do 
		
		---------1-Documents----------------------------------------------
		IF TO_BIGINT(FieldID) >= :ui01 AND 
		:ByField = 'Y' AND
		(:FormID = '133' OR
		 :FormID = '137' OR
		 :FormID = '149' OR
		 :FormID = '139' OR
		 :FormID = '140' OR
		 :FormID = '180' OR
		 :FormID = '179' OR
		 :FormID = '142' OR
		 :FormID = '143' OR
		 :FormID = '182' OR
		 :FormID = '181' OR
		 :FormID = '940' OR
		 :FormID = '720' OR
		 :FormID = '721' OR
		 :FormID = '141') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'ODLN'		
				AND POSITION = TO_BIGINT(FieldID) - :ui01 + :db01 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;	   
		---------2-Documents-Lines-Items----------------------------------
		ELSEIF TO_BIGINT(FieldID) >= :ui02 AND
		:ByField = 'N' AND
		(:FormID = '133' OR
		 :FormID = '137' OR
		 :FormID = '149' OR
		 :FormID = '139' OR
		 :FormID = '140' OR
		 :FormID = '180' OR
		 :FormID = '179' OR
		 :FormID = '142' OR
		 :FormID = '143' OR
		 :FormID = '182' OR
		 :FormID = '181' OR
		 :FormID = '940' OR
		 :FormID = '720' OR
		 :FormID = '721' OR
		 :FormID = '141') AND
		 (:ItemId = '38') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'DLN1'		
				AND POSITION = TO_BIGINT(FieldID) - :ui02 + :db02 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;
		---------3-Documents-Lines-Service------------------------------
		ELSEIF TO_BIGINT(FieldID) >= :ui03 AND
		:ByField = 'N' AND
		(:FormID = '133' OR
		 :FormID = '137' OR
		 :FormID = '149' OR
		 :FormID = '139' OR
		 :FormID = '140' OR
		 :FormID = '180' OR
		 :FormID = '179' OR
		 :FormID = '142' OR
		 :FormID = '143' OR
		 :FormID = '182' OR
		 :FormID = '181' OR
		 :FormID = '141') AND
		 (:ItemId = '39') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'DLN1'		
				AND POSITION = TO_BIGINT(FieldID) - :ui03 + :db03 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;
		--------4-Stock Transfer-Lines-----------------------------------
		ELSEIF TO_BIGINT(FieldID) >= :ui04 AND
		:ByField = 'N' AND
		(:FormID = '940')  THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'WTR1'		
				AND POSITION = TO_BIGINT(FieldID) - :ui04 + :db04 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;
		--------5-Goods Receipt\Issue-------------------------------------
		ELSEIF TO_BIGINT(FieldID) >= :ui05 AND
		:ByField = 'N' AND
		(:FormID = '720' OR
		 :FormID = '721') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'IGN1'		
				AND POSITION = TO_BIGINT(FieldID) - :ui05 + :db05 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;
		--------6-BP------------------------------------------------------
		ELSEIF TO_BIGINT(FieldID) >= :ui06 AND 
		:ByField = 'Y' AND
		(:FormID = '134') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'OCRD'		
				AND POSITION = TO_BIGINT(FieldID) - :ui06 + :db06 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;		
		--------7-BP-Addresses--------------------------------------------
		ELSEIF TO_BIGINT(FieldID) >= :ui07 AND 
		:ByField = 'N' AND
		(:FormID = '134') AND 
		(:ItemId = '178') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'CRD1'		
				AND POSITION = TO_BIGINT(FieldID) - :ui07 + :db07 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;
		--------8-BP-Contact Persons--------------------------------------
		ELSEIF TO_BIGINT(FieldID) >= :ui08 AND 
		:ByField = 'N' AND
		(:FormID = '134') AND 
		(:ItemId = '107') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'OCPR'		
				AND POSITION = TO_BIGINT(FieldID) - :ui08 + :db08 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;
		--------9-Items--------------------------------------------------
		ELSEIF TO_BIGINT(FieldID) >= :ui09 AND 
		:ByField = 'Y' AND
		(:FormID = '150') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'OITM'		
				AND POSITION = TO_BIGINT(FieldID) - :ui09 + :db09 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;
		-------10-Price Lists---------------------------------------------
		ELSEIF TO_BIGINT(FieldID) >= :ui10 AND 
		:ByField = 'N' AND
		(:FormID = '155') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'OPLN'		
				AND POSITION = TO_BIGINT(FieldID) - :ui10 + :db10 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;
		--------11-Contacts----------------------------------------------
		ELSEIF TO_BIGINT(FieldID) >= :ui11 AND 
		:ByField = 'Y' AND
		(:FormID = '651') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'OCLG'		
				AND POSITION = TO_BIGINT(FieldID) - :ui11 + :db11 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;
		--------12-Accounts----------------------------------------------
		ELSEIF TO_BIGINT(FieldID) >= :ui12 AND 
		:ByField = 'Y' AND
		(:FormID = '804') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'OACT'		
				AND POSITION = TO_BIGINT(FieldID) - :ui12 + :db12 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;
		--------13-Payments----------------------------------------------
		ELSEIF TO_BIGINT(FieldID) >= :ui13 AND 
		:ByField = 'Y' AND
		(:FormID = '170' OR
		 :FormID = '426' OR
		 :FormID = '146' OR
		 :FormID = '196') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'ORCT'		
				AND POSITION = TO_BIGINT(FieldID) - :ui13 + :db13 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;
		--------14-Payments-Invoices------------------------------------
		ELSEIF TO_BIGINT(FieldID) >=:ui14  AND 
		:ByField = 'N' AND 
		(:FormID = '170' OR
		 :FormID = '426') AND 
		(:ItemId = '20') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'RCT2'		
				AND POSITION = TO_BIGINT(FieldID) - :ui14 + :db14 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;
		--------15-Payments-Accounts-------------------------------------
		ELSEIF TO_BIGINT(FieldID) >= :ui15 AND 
		:ByField = 'N' AND 
		(:FormID = '170' OR
		 :FormID = '426') AND 
		(:ItemId = '71') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'RCT4'		
				AND POSITION = TO_BIGINT(FieldID) - :ui15 + :db15 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;
		--------16-Payments-Checks---------------------------------------
		ELSEIF TO_BIGINT(FieldID) >= :ui16 AND 
		:ByField = 'N' AND 
		(:FormID = '146' OR
		 :FormID = '196')  AND 
		(:ItemId = '28') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'RCT1'		
				AND POSITION = TO_BIGINT(FieldID) - :ui16 + :db16 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;
		--------17-Payments-Credit--------------------------------------
		ELSEIF TO_BIGINT(FieldID) >= :ui17 AND
		:ByField = 'N' AND 
		(:FormID = '146' OR
		 :FormID = '196')  AND 
		(:ItemId = '112') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'RCT3'		
				AND POSITION = TO_BIGINT(FieldID) - :ui17 + :db17 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;
		--------18-Deposits---------------------------------------------
		ELSEIF TO_BIGINT(FieldID) >= :ui18 AND 
		:ByField = 'Y' AND
		(:FormID = '606') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'ODPS'		
				AND POSITION = TO_BIGINT(FieldID) - :ui18 + :db18 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;
		--------19-Product Tree-----------------------------------------
		ELSEIF TO_BIGINT(FieldID) >= :ui19 AND 
		:ByField = 'Y' AND
		(:FormID = '672') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'OITT'		
				AND POSITION = TO_BIGINT(FieldID) - :ui19 + :db19 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;
		--------20-Product Tree-Lines-----------------------------------
		ELSEIF TO_BIGINT(FieldID) >= :ui20 AND 
		:ByField = 'N' AND
		(:FormID = '672') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'ITT1'		
				AND POSITION = TO_BIGINT(FieldID) - :ui20 + :db20 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;
		--------21-Work Orders-------------------------------------------
		ELSEIF TO_BIGINT(FieldID) >= :ui21 AND 
		:ByField = 'Y' AND
		(:FormID = '677') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'OWOK'		
				AND POSITION = TO_BIGINT(FieldID) - :ui21 + :db21 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;
		-------22-Work Orders-Lines--------------------------------------
		ELSEIF TO_BIGINT(FieldID) >= :ui22 AND 
		:ByField = 'N' AND
		(:FormID = '677') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'WKO1'		
				AND POSITION = TO_BIGINT(FieldID) - :ui22 + :db22 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;
		--------23-Journal Entries---------------------------------------
		ELSEIF TO_BIGINT(FieldID) >= :ui23 AND 
		:ByField = 'Y' AND
		(:FormID = '392' OR 
		 :FormID = '390') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'OJDT'		
				AND POSITION = TO_BIGINT(FieldID) - :ui23 + :db23 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;
		--------24-Journal Entries-Lines-Tax-----------------------------
		ELSEIF TO_BIGINT(FieldID) >= :ui24 AND 
		:ByField = 'N' AND
		(:FormID = '392') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'JDT1'		
				AND POSITION = TO_BIGINT(FieldID) - :ui24 + :db24 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;
		--------25-Journal Entries-Lines-NoTax----------------------------
		ELSEIF TO_BIGINT(FieldID) >= :ui25 AND 
		:ByField = 'N' AND
		(:FormID = '390') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'JDT1'		
				AND POSITION = TO_BIGINT(FieldID) - :ui25 + :db25 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;
		--------26-Profit Centers------------------------------------------
		ELSEIF TO_BIGINT(FieldID) >= :ui26 AND 
		:ByField = 'Y' AND
		(:FormID = '811') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'OOCR'		
				AND POSITION = TO_BIGINT(FieldID) - :ui26 + :db26 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;
		--------27-Distribution Rules-------------------------------------
		ELSEIF TO_BIGINT(FieldID) >= :ui27 AND 
		:ByField = 'Y' AND
		(:FormID = '810') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'OPRC'		
				AND POSITION = TO_BIGINT(FieldID) - :ui27 + :db27 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;
		--------28-Budget--------------------------------------------------
		ELSEIF TO_BIGINT(FieldID) >= :ui28 AND 
		:ByField = 'N' AND
		(:FormID = '452') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'OBGS'		
				AND POSITION = TO_BIGINT(FieldID) - :ui28 + :db28 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;
		--------29-Projects-----------------------------------------------
		ELSEIF TO_BIGINT(FieldID) >= :ui29 AND 
		:ByField = 'N' AND
		(:FormID = '711') THEN
			SELECT COLUMN_NAME into FieldID FROM "PUBLIC"."TABLE_COLUMNS" 
			WHERE   TABLE_NAME = 'OPRJ'		
				AND POSITION = TO_BIGINT(FieldID) - :ui29 + :db29 + 1
				AND SCHEMA_NAME = CURRENT_SCHEMA;
		-----------------------------------------------------------------
		END IF;
		
		Fetch Currs Into FieldID,ColId,ByField,FormID,ItemId;
		
		if NOT Currs::NOTFOUND then
			hasNext := -1;
		end if;
		
		Update CSHS Set "FieldID" = :FieldID;
	end while;

	Close Currs;

end;


