CREATE PROCEDURE TmpSp_ReplaceAndLog
	(in AcctCodeSrc VARCHAR(20),
	in AcctCodeTrg VARCHAR(20),
	in type int)
AS
	tab VARCHAR(255);		--variable for table name
	col VARCHAR(255);		--variable for column name	
	strSql VARCHAR(300);	--variable for sql statement
	ups VARCHAR(300);		--variable for update statement
	level VARCHAR(2);		--level
	is_exist int;			--greater than 0, exist
BEGIN
	DECLARE CURSOR replaceCursor FOR 
		SELECT "TabName" as TAB, "ColName" as COL, "IntLevel" as LEVEL from CFTC;

	--drop and create table COA_TOOL_REC_EXIST
	is_exist := 0;
	SELECT COUNT(TABLE_NAME) into is_exist from SYS.TABLES WHERE "SCHEMA_NAME" = CURRENT_SCHEMA AND "TABLE_NAME" = 'COA_TOOL_REC_EXIST';
	IF :is_exist > 0 THEN
	     strSql := 'DROP TABLE COA_TOOL_REC_EXIST';
         EXECUTE IMMEDIATE (:strSql);
	END IF;		
	CREATE TABLE "COA_TOOL_REC_EXIST" ( "EXIST" VARCHAR(1));
		
	FOR cur_row AS replaceCursor DO
		tab := cur_row.TAB;
		col := cur_row.COL;	
		level := cur_row.LEVEL;
		is_exist := 0;
		
		IF :type = 1 AND :tab = 'OACT' AND (:col ='AcctCode' or :col = 'ActId' or :col= 'FormatCode') THEN
			strSql := 'skip ' ||:tab||' '||:col;
			--SELECT :strSql FROM DUMMY;
			CONTINUE;
		END IF;
		
		strSql := 'INSERT INTO COA_TOOL_REC_EXIST (SELECT TOP 1 ''A''  FROM "'||:tab|| '" Where "'||:col||'"='''|| :AcctCodeSrc||''')';
		--SELECT :strSql FROM DUMMY;
		EXECUTE IMMEDIATE (:strSql);
		SELECT COUNT("EXIST") INTO is_exist FROM  COA_TOOL_REC_EXIST;
		IF :is_exist = 1 THEN
			ups := 'UPDATE "' ||:tab||'" SET "'||:col||'" = '''||:AcctCodeTrg||''' WHERE "'|| :col|| '" = ''' ||:AcctCodeSrc ||'''';
			--SELECT :ups FROM DUMMY;
			strSql := 'INSERT INTO COA_TOOL_ACCT_REPLACE_LOG ("TAB","COL","LEVEL","ACCT_CODE_FROM", "ACCT_CODE_TO", "SQL_STATEMENT","EXEC_TIME") VALUES ('''||:tab||''','''||:col||''','''||:level||''','''||:AcctCodeSrc||''','''||:AcctCodeTrg||''','''||REPLACE(:ups,'''', '''''')||''','''||TO_VARCHAR(CURRENT_TIMESTAMP,'YYYY-MM-DD_HH24:MI:SS.FF3')||''');';
			--SELECT :strSql FROM DUMMY;
			EXECUTE IMMEDIATE (:strSql); --execute log
			IF :level = '1' THEN	
				EXECUTE IMMEDIATE (:ups); --execute prepared statement
				--SELECT :ups FROM DUMMY;
			END IF;
		END IF;
		DELETE FROM COA_TOOL_REC_EXIST WHERE 1=1; 
	END FOR;
END;


