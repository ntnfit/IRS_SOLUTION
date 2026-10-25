
CREATE PROCEDURE Apz_ExistsProc ( 
	IN procname VARCHAR(250)
) 
LANGUAGE SQLSCRIPT 
AS 
	myrowid integer;
BEGIN 
	myrowid := 0;
	SELECT COUNT(*) INTO myrowid FROM "PUBLIC"."PROCEDURES" WHERE schema_name =CURRENT_SCHEMA AND procedure_name=UPPER(:procname);
	IF (:myrowid > 0 ) THEN
	EXEC 'DROP PROCEDURE '||CURRENT_SCHEMA||'.'||UPPER(:procname);	
	END IF;
	
	SELECT COUNT(*) INTO myrowid FROM "PUBLIC"."PROCEDURES" WHERE schema_name =CURRENT_SCHEMA AND procedure_name=:procname;	
	IF (:myrowid > 0 ) THEN
	EXEC 'DROP PROCEDURE "'||CURRENT_SCHEMA||'"."'||:procname||'"';
	END IF;
END;



