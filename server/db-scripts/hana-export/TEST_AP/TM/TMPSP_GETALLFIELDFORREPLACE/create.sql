CREATE PROCEDURE TmpSp_GetAllFieldForReplace
LANGUAGE SQLSCRIPT
AS
		--variable declaration
		tab VARCHAR(255);		--variable for table name
		col VARCHAR(255);		--variable for column name	
		--ins VARCHAR(300);		--variable for insert statement
		cnt int;
		fieldCount int;
		is_exist int;
		strSql VARCHAR(300);
		--strLOG VARCHAR(300);
BEGIN

	--compare with columns of specified types 
	DECLARE CURSOR findCursor FOR
		SELECT T0."TABLE_NAME" as TAB, T0."COLUMN_NAME" as COL FROM SYS.TABLE_COLUMNS T0
		INNER JOIN SYS.TABLES T1 ON T0."TABLE_NAME" = T1."TABLE_NAME"		
		WHERE T0."SCHEMA_NAME" = CURRENT_SCHEMA AND  T0. "DATA_TYPE_NAME" in ('NVARCHAR', 'VARCHAR' , 'NCHAR', 'CHAR') AND T0."LENGTH" >= 15 
		AND T1."IS_USER_DEFINED_TYPE" <> 'TRUE' AND T1."IS_TEMPORARY" <> 'TRUE';
	
	--clean old result
	DELETE FROM "CFTC" WHERE "IntLevel" = 2;
	
	--drop and create table COA_TOOL_FIND_RES
	is_exist := 0;
	SELECT COUNT(TABLE_NAME) into is_exist from SYS.TABLES WHERE "SCHEMA_NAME" = CURRENT_SCHEMA AND "TABLE_NAME" = 'COA_TOOL_FIND_RES';
	IF :is_exist > 0 THEN
	     strSql := 'DROP TABLE COA_TOOL_FIND_RES';
         EXECUTE IMMEDIATE (:strSql);
	END IF;
	CREATE TABLE "COA_TOOL_FIND_RES" ( "TAB_RES" VARCHAR(255), "COL_RES" VARCHAR(50));

	--drop and create table COA_TOOL_REC_EXIST
	is_exist := 0;
	SELECT COUNT(TABLE_NAME) into is_exist from SYS.TABLES WHERE "SCHEMA_NAME" = CURRENT_SCHEMA AND "TABLE_NAME" = 'COA_TOOL_REC_EXIST';
	IF :is_exist > 0 THEN
	     strSql := 'DROP TABLE COA_TOOL_REC_EXIST';
         EXECUTE IMMEDIATE (:strSql);
	END IF;		
	CREATE TABLE "COA_TOOL_REC_EXIST" ( "EXIST" VARCHAR(1));
	
	--drop and create table COA_TOOL_LOG
	is_exist := 0;
	SELECT COUNT(TABLE_NAME) into is_exist from SYS.TABLES WHERE "SCHEMA_NAME" = CURRENT_SCHEMA AND "TABLE_NAME" = 'COA_TOOL_LOG';
	IF :is_exist > 0 THEN
	     strSql := 'DROP TABLE COA_TOOL_LOG';
         EXECUTE IMMEDIATE (:strSql);
	END IF;
	CREATE TABLE "COA_TOOL_LOG" ("ID" VARCHAR(10) ,"VAL" VARCHAR(300));
	
	is_exist := 0;
	-- create table COA_TOOL_ACCT_REPLACE_LOG for later store procedure TmpSp_ReplaceAndLog use
	SELECT COUNT(TABLE_NAME) into is_exist from SYS.TABLES WHERE "SCHEMA_NAME" = CURRENT_SCHEMA AND "TABLE_NAME" = 'COA_TOOL_ACCT_REPLACE_LOG';
	IF :is_exist = 0 THEN
		CREATE TABLE "COA_TOOL_ACCT_REPLACE_LOG"  ("TAB" VARCHAR(255), "COL" VARCHAR(50),	"LEVEL" VARCHAR(2), "ACCT_CODE_FROM" VARCHAR(20), "ACCT_CODE_TO" VARCHAR(20), "SQL_STATEMENT" VARCHAR(500),"EXEC_TIME" VARCHAR(25));
	END IF;
	
	SELECT COUNT('A') INTO fieldCount FROM SYS.TABLE_COLUMNS 
		WHERE "SCHEMA_NAME" = CURRENT_SCHEMA AND  "DATA_TYPE_NAME" in ('NVARCHAR', 'VARCHAR' , 'NCHAR', 'CHAR') AND "LENGTH" >= 15;
	
	FOR cur_row AS findCursor DO
		tab := cur_row.TAB;
		col := cur_row.COL;	
		
		--strLog := tab || '+' || col;
		cnt := 5;
		SELECT COUNT('A') into cnt from CFTC WHERE "TabName"=:tab AND "ColName"=:col;
		--strLog := strLog || ' first_cnt=' || CAST(cnt as VARCHAR);
		
		IF :cnt = 0 THEN
			cnt := 5;
			SELECT COUNT('A') into cnt from COA_TOOL_FIND_RES WHERE "TAB_RES" = :tab AND COL_RES =:col;
			--strLog := strLog || ' second_cnt=' || CAST(cnt as VARCHAR);
		END IF;
		
		IF :cnt = 0 THEN
			strSql := 'INSERT INTO COA_TOOL_REC_EXIST (SELECT TOP 1 ''A''  FROM OACT Where "Postable" = ''Y'' AND "AcctCode" in (SELECT DISTINCT "'||:col||'" FROM "'||:tab||+'"))';
			EXECUTE IMMEDIATE (:strSql);				
			SELECT COUNT("EXIST") INTO cnt FROM  COA_TOOL_REC_EXIST;
			--strLog := strLog || ' third_cnt=' || CAST(cnt as VARCHAR);
			IF :cnt = 1 THEN
				strSql := 'INSERT INTO COA_TOOL_FIND_RES ("TAB_RES","COL_RES") VALUES ('''||:tab||''','''||:col||''');';
				EXECUTE IMMEDIATE (:strSql);	
			END IF; 		
		END IF;
		
		--write log to COA_TOOL_LOG 
		--ins := 'INSERT INTO COA_TOOL_LOG (ID, VAL) VALUES ('''||CAST(fieldCount AS VARCHAR)||''','''||:strLog||''');';
		--EXECUTE IMMEDIATE (:ins);
		DELETE FROM COA_TOOL_REC_EXIST WHERE 1=1;
		fieldCount := fieldCount-1;
  	 END FOR;
  	 INSERT INTO CFTC("TabName","ColName","IntLevel") SELECT TAB_RES,COL_RES,'2' FROM COA_TOOL_FIND_RES;
END;


