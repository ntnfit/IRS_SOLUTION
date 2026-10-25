-- B1 DEPENDS: BEFORE:PT:PROCESS_START

CREATE PROCEDURE _TmSp_AliasUpdateAfter
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS
	cnt int;
	sqlString NCLOB;
BEGIN
	sqlString := 'UPDATE OBTD SET "DateID"= (SELECT MAX(_Date) FROM OBTD_TEMP WHERE OBTD_TEMP.BatchNum=OBTD."BatchNum"),
		"MemoID"=(SELECT MAX(_Memo) FROM OBTD_TEMP WHERE OBTD_TEMP.BatchNum=OBTD."BatchNum")
		WHERE EXISTS(SELECT COUNT(*) FROM OBTD_TEMP WHERE OBTD_TEMP.BatchNum=OBTD."BatchNum")';
	exec (:sqlString);
	
	sqlString := 'TRUNCATE TABLE OBTD_TEMP';
	exec (:sqlString);
	
	sqlString := 'DROP TABLE OBTD_TEMP';
	exec (:sqlString);
	
	sqlString := 'UPDATE OCHH SET "UserID"=(SELECT MAX(_User_) FROM OCHH_TEMP WHERE OCHH_TEMP.CheckKey=OCHH."CheckKey")
		WHERE EXISTS(SELECT COUNT(*) FROM OCHH_TEMP WHERE OCHH_TEMP.CheckKey=OCHH."CheckKey")';
	exec (:sqlString);
	sqlString := 'TRUNCATE TABLE OCHH_TEMP';
	exec (:sqlString);
	sqlString := 'DROP TABLE OCHH_TEMP';
	exec (:sqlString);
	
	sqlString := 'UPDATE OIPF SET "Descr"=(SELECT MAX(_Comment) FROM OIPF_TEMP WHERE OIPF_TEMP.DocEntry=OIPF."DocEntry")
		WHERE EXISTS(SELECT COUNT(*) FROM OIPF_TEMP WHERE OIPF_TEMP.DocEntry=OIPF."DocEntry")';
	exec (:sqlString);
	sqlString := 'TRUNCATE TABLE OIPF_TEMP';
	exec (:sqlString);
	sqlString := 'DROP TABLE OIPF_TEMP';
	exec (:sqlString);
	
	sqlString := 'UPDATE OCFL SET "DateID"=(SELECT MAX(_Date) FROM OCFL_TEMP WHERE OCFL_TEMP.Code=OCFL."Code")
		WHERE EXISTS(SELECT COUNT(*) FROM OCFL_TEMP WHERE OCFL_TEMP.Code=OCFL."Code")';
	exec (:sqlString);
	sqlString := 'TRUNCATE TABLE OCFL_TEMP';
	exec (:sqlString);
	sqlString := 'DROP TABLE OCFL_TEMP';
	exec (:sqlString);
	
	SELECT COUNT(*) INTO cnt FROM "PUBLIC"."TABLES" WHERE "TABLE_NAME"='CDRU_TEMP';
	IF :cnt>0 THEN
		sqlString := 'UPDATE CDRU SET "UserID"=(SELECT MAX(_User) FROM CDRU_TEMP WHERE CDRU_TEMP.objectID=CDRU."ObjectId")
			WHERE EXISTS(SELECT COUNT(*) FROM CDRU_TEMP WHERE CDRU_TEMP.objectID=CDRU."ObjectId")';
		exec (:sqlString);
		sqlString := 'TRUNCATE TABLE CDRU_TEMP';
		exec (:sqlString);
		sqlString := 'DROP TABLE CDRU_TEMP';
		exec (:sqlString);
	END IF;
	
	SELECT COUNT(*) INTO cnt FROM "PUBLIC"."TABLES" WHERE "TABLE_NAME"='OOPR_TEMP';
	IF :cnt>0 THEN		
		sqlString := 'UPDATE OOPR  SET "CloPrcnt"=(SELECT MAX(_Percent) FROM OOPR_TEMP WHERE OOPR_TEMP.OpprID=OOPR."OpprId")
			WHERE EXISTS(SELECT COUNT(*) FROM OOPR_TEMP WHERE OOPR_TEMP.OpprID=OOPR."OpprId")';
		exec (:sqlString);
  		sqlString := 'TRUNCATE TABLE OOPR_TEMP';
		exec (:sqlString);
  		sqlString := 'DROP TABLE OOPR_TEMP';
		exec (:sqlString);
 	END IF;
 	
 	SELECT COUNT(*) INTO cnt FROM "PUBLIC"."TABLES" WHERE "TABLE_NAME"='OOST_TEMP';
	IF :cnt>0 THEN
		sqlString := 'UPDATE OOST SET "CloPrcnt" =(SELECT MAX(_Percent) FROM OOST_TEMP WHERE OOST_TEMP.Num=OOST."Num")
			WHERE EXISTS(SELECT COUNT(*) FROM OOST_TEMP WHERE OOST_TEMP.Num=OOST."Num")';
		exec (:sqlString);
		sqlString := 'TRUNCATE TABLE OOST_TEMP';
		exec (:sqlString);
		sqlString := 'DROP TABLE OOST_TEMP';
		exec (:sqlString);
	END IF;   
END;


