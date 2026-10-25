-- B1 DEPENDS: BEFORE:PT:PROCESS_START

CREATE PROCEDURE _TmSp_CreateViewB1_EDFLatestLogsView
language sqlscript
SQL SECURITY INVOKER
AS
  sql1    nvarchar(4999);
  rowNum  int;
BEGIN
  SELECT COUNT(*) INTO rowNum FROM "PUBLIC"."VIEWS" WHERE "VIEW_NAME" = 'B1_EDFLatestLogsView' AND "SCHEMA_NAME" = current_schema;
  IF :rowNum > 0
  THEN
    sql1 := 'DROP VIEW  "'||current_schema||'"."B1_EDFLatestLogsView"';
    EXEC(:sql1);
  END IF;

	sql1 := '
CREATE VIEW "B1_EDFLatestLogsView" ( "AbsEntry",
	 "LogNum",
	 "LogMessage",
	 "UserSign",
	 "CreateDate",
	 "UserSign2",
	 "UpdateDate",
	 "LogInstanc",
	 "CreateTS",
	 "UpdateTS",
	 "LogType",
	 "LogData",
	 "LogOpDate",
	 "LogOpTS",
	 "RankByDT" ) AS SELECT
	 "TBLRANK"."AbsEntry" ,
	 "TBLRANK"."LogNum" ,
	 "TBLRANK"."LogMessage" ,
	 "TBLRANK"."UserSign" ,
	 "TBLRANK"."CreateDate" ,
	 "TBLRANK"."UserSign2" ,
	 "TBLRANK"."UpdateDate" ,
	 "TBLRANK"."LogInstanc" ,
	 "TBLRANK"."CreateTS" ,
	 "TBLRANK"."UpdateTS" ,
	 "TBLRANK"."LogType" ,
	 "TBLRANK"."LogData" ,
	 "TBLRANK"."LogOpDate" ,
	 "TBLRANK"."LogOpTS" ,
	 "TBLRANK"."RankByDT" 
FROM (SELECT
	 *,
	 CAST(RANK() OVER (PARTITION BY "AbsEntry",
	 "LogType" 
			ORDER BY "LogOpDate" DESC,
	 "LogOpTS" DESC,
	 "LogNum" DESC) AS Integer) AS "RankByDT" 
	FROM "ECM3" ) tblRank 
WHERE tblRank."RankByDT" = 1 
ORDER BY "AbsEntry",
	 "LogType" WITH READ ONLY
';

	EXEC(:sql1);
END


