-- B1 DEPENDS: BEFORE:PT:PROCESS_START

CREATE PROCEDURE _TmSp_UpgradeImportExportProcess
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS
 v_sql varchar(100);
BEGIN

-- Process item A/R Invoices
exec ('INSERT INTO INV18 ("DocEntry", "LineNum", "LogInstanc") SELECT T1."DocEntry", T1."LineNum", T1."LogInstanc" FROM	INV1 T1	JOIN OINV T0 ON T1."DocEntry" = T0."DocEntry" WHERE	T0."DocType" = ''I''');
-- Process item A/R Credit Memos
exec ('INSERT INTO RIN18 ("DocEntry", "LineNum", "LogInstanc") SELECT T1."DocEntry", T1."LineNum", T1."LogInstanc" FROM	RIN1 T1	JOIN ORIN T0 ON T1."DocEntry" = T0."DocEntry" WHERE	T0."DocType" = ''I''');
-- Process item Deliveries
exec ('INSERT INTO DLN18 ("DocEntry", "LineNum", "LogInstanc") SELECT T1."DocEntry", T1."LineNum", T1."LogInstanc" FROM	DLN1 T1	JOIN ODLN T0 ON T1."DocEntry" = T0."DocEntry" WHERE	T0."DocType" = ''I''');
-- Process item Returns
exec ('INSERT INTO RDN18 ("DocEntry", "LineNum", "LogInstanc") SELECT T1."DocEntry", T1."LineNum", T1."LogInstanc" FROM	RDN1 T1 JOIN ORDN T0 ON T1."DocEntry" = T0."DocEntry" WHERE	T0."DocType" = ''I''');
-- Process item A/P Invoices
exec ('INSERT INTO PCH17 ("DocEntry", "LineNum", "LogInstanc") SELECT T1."DocEntry", T1."LineNum", T1."LogInstanc" FROM	PCH1 T1	JOIN OPCH T0 ON T1."DocEntry" = T0."DocEntry" WHERE	T0."DocType" = ''I''');
-- Process item A/P Credit Memos
exec ('INSERT INTO RPC17 ("DocEntry", "LineNum", "LogInstanc") SELECT T1."DocEntry", T1."LineNum", T1."LogInstanc" FROM RPC1 T1	JOIN ORPC T0 ON T1."DocEntry" = T0."DocEntry" WHERE	T0."DocType" = ''I''');
-- Process item Goods Receipt POs
exec ('INSERT INTO PDN17 ("DocEntry", "LineNum", "LogInstanc") SELECT T1."DocEntry", T1."LineNum", T1."LogInstanc" FROM	PDN1 T1	JOIN OPDN T0 ON T1."DocEntry" = T0."DocEntry" WHERE	T0."DocType" = ''I''');
-- Process item Goods Returns
exec ('INSERT INTO RPD17 ("DocEntry", "LineNum", "LogInstanc") SELECT T1."DocEntry", T1."LineNum", T1."LogInstanc" FROM RPD1 T1	JOIN ORPD T0 ON T1."DocEntry" = T0."DocEntry" WHERE	T0."DocType" = ''I''');
-- Process item A/R side line logs
exec ('INSERT INTO ADO18 ("DocEntry", "LineNum", "LogInstanc", "ObjectType") SELECT T1."DocEntry", T1."LineNum", T1."LogInstanc", T1."ObjType" FROM	ADO1 T1	JOIN ADOC T0 ON T1."DocEntry" = T0."DocEntry" AND T1."LogInstanc" = T0."LogInstanc"	AND T1."ObjType" = T0."ObjType"	WHERE T1."ObjType" IN (''13'', ''14'', ''15'', ''16'') AND T0."DocType" = ''I''');
-- Process item A/P side line logs
exec ('INSERT INTO ADO17 ("DocEntry", "LineNum", "LogInstanc", "ObjectType") SELECT	T1."DocEntry", T1."LineNum", T1."LogInstanc", T1."ObjType" FROM	ADO1 T1	JOIN ADOC T0 ON T1."DocEntry" = T0."DocEntry" AND T1."LogInstanc" = T0."LogInstanc" AND T1."ObjType" = T0."ObjType"	WHERE T1."ObjType" IN (''18'', ''19'', ''20'', ''21'') AND T0."DocType" = ''I''');

	
END;


