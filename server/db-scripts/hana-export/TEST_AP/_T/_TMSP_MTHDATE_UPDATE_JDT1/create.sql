-- B1 DEPENDS: BEFORE:PT:PROCESS_START

CREATE PROCEDURE _TmSp_MthDate_Update_JDT1
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS

exists1 int;
begin
-----------------------------------------------------------
 select count(*) into exists1 from "PUBLIC".objects where object_name = 'JDT1_TEMP' and schema_name = current_schema;
 if :exists1 <> 0 then
	drop table JDT1_TEMP;
 end if;

EXEC('create  table JDT1_TEMP(DocEntry int,'
||									'InvType nvarchar(20),'
||									'CardCode nvarchar(15),'
||									'DocDate timestamp) ');
--------------------------Receipt-----------------------------
EXEC('INSERT INTO JDT1_TEMP '
|| 'SELECT RCT2."DocEntry",RCT2."InvType",ORCT."CardCode",ORCT."DocDate" '
|| 'FROM RCT2 INNER JOIN ORCT ON RCT2."DocNum" = ORCT."DocNum" '
||           'INNER JOIN JDT1 ON (ORCT."TransId" = JDT1."TransId" AND '
||           'JDT1."Account" <> JDT1."ShortName") '
|| 'WHERE JDT1."IntrnMatch" = -1 AND ORCT."TransId" = JDT1."TransId" AND '
||       'JDT1."Account" <> JDT1."ShortName" AND '
||      'JDT1."Line_ID" = (SELECT Top 1 "Line_ID" '
||						   'FROM JDT1 INNER JOIN ORCT ON ORCT."TransId" = JDT1."TransId" ' 
||						             'INNER JOIN RCT2 ON RCT2."DocNum" = ORCT."DocNum" '
||						   'WHERE  JDT1."IntrnMatch" = -1 AND ' 
||								  'JDT1."Account" <> JDT1."ShortName") ');
-------------------------Payments to sup.---------------------- 
EXEC('INSERT INTO JDT1_TEMP '
|| 'SELECT VPM2."DocEntry",VPM2."InvType",OVPM."CardCode",OVPM."DocDate" '
|| 'FROM VPM2 INNER JOIN OVPM ON VPM2."DocNum" = OVPM."DocNum" '
||           'INNER JOIN JDT1 ON (OVPM."TransId" = JDT1."TransId" AND '
||           'JDT1."Account" <> JDT1."ShortName") '
|| 'WHERE JDT1."IntrnMatch" = -1 AND OVPM."TransId" = JDT1."TransId" AND '
||       'JDT1."Account" <> JDT1."ShortName" AND '
||       'JDT1."Line_ID" = (SELECT Top 1 "Line_ID" '
||						   'FROM JDT1 INNER JOIN OVPM ON OVPM."TransId" = JDT1."TransId"'
||									 'INNER JOIN VPM2 ON VPM2."DocNum" = OVPM."DocNum" '
||						   'WHERE  JDT1."IntrnMatch" = -1 AND '
||								  'JDT1."Account" <> JDT1."ShortName") ');
-------------------------Credit note----------------------------- 
EXEC('INSERT INTO JDT1_TEMP '
|| 'SELECT RIN1."BaseEntry",RIN1."BaseType",ORIN."CardCode",ORIN."DocDate" '
|| 'FROM RIN1 INNER JOIN ORIN ON RIN1."DocEntry" = ORIN."DocEntry" '
||           'INNER JOIN JDT1 ON (ORIN."TransId" = JDT1."TransId" AND '
||           'JDT1."Account" <> JDT1."ShortName") '
|| 'WHERE JDT1."IntrnMatch" = -1 AND RIN1."BaseEntry" <> 0 AND '
||       'NOT RIN1."BaseEntry" is NULL ');
------------------------Purshase credit note--------------------- 
EXEC('INSERT INTO JDT1_TEMP '
|| 'SELECT RPC1."BaseEntry",RPC1."BaseType",ORPC."CardCode",ORPC."DocDate" '
|| 'FROM RPC1 INNER JOIN ORPC ON RPC1."DocEntry" = ORPC."DocEntry" '
||           'INNER JOIN JDT1 ON (ORPC."TransId" = JDT1."TransId" AND '
||           'JDT1."Account" <> JDT1."ShortName") '
|| 'WHERE JDT1."IntrnMatch" = -1 AND RPC1."BaseEntry" <> 0 AND '
||       'NOT RPC1."BaseEntry" is NULL ');
----------------------------------------------------------------
EXEC('UPDATE JDT1_TEMP '
|| 'SET DocEntry = OJDT."TransId" '
|| 'FROM JDT1_TEMP INNER JOIN OJDT ON '
||      'JDT1_TEMP.InvType = OJDT."TransType" AND '
||      'JDT1_TEMP.DocEntry = OJDT."CreatedBy" '
|| 'WHERE  JDT1_TEMP.InvType = 13 OR JDT1_TEMP.InvType = 18 ');
----------------------------------------------------------------
EXEC('UPDATE JDT1 '
|| 'SET "MthDate" = JDT1_TEMP.DocDate '
|| 'FROM JDT1 INNER JOIN JDT1_TEMP ON  '
||      'JDT1."TransId" = JDT1_TEMP.DocEntry '
|| 'WHERE JDT1."IntrnMatch" = -1 AND JDT1."Account" <> JDT1."ShortName" ');

EXEC('UPDATE JDT1 '
|| 'SET "MthDate" = "RefDate" '
|| 'WHERE "IntrnMatch" = -1 AND "MthDate" is NULL ');

EXEC('UPDATE JDT1 '
|| 'SET  "MthDate" = OMTH."MatchDate" '
|| 'FROM JDT1 INNER JOIN OMTH ON  '
|| 'JDT1."IntrnMatch" = OMTH."MatchNum" AND  '
|| 'JDT1."ShortName" = OMTH."MthAcctCod" '
|| 'WHERE JDT1."IntrnMatch" > 0 AND OMTH."IsInternal" = ''I'' AND '
||       'JDT1."MthDate" is NULL ');
----------------------------------------------------------------
DROP TABLE JDT1_TEMP;
end;


