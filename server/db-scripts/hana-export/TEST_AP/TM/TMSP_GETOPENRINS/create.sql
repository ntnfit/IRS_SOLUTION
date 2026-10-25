-- B1 DEPENDS: AFTER:PT:PROCESS_END

CREATE PROCEDURE  TmSp_GetOpenRINs(
in object  int,
in card nvarchar(100),
in frgnCurr nchar)

LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
begin

if (:object = 14)
then
	if (:frgnCurr = N'N')
	then
		SELECT  JDT1.* 
		FROM JDT1 INNER JOIN ORIN ON JDT1."CreatedBy"= ORIN."DocEntry" AND JDT1."TransType" = 14
		WHERE NOT EXISTS
			(select distinct("DocEntry") from RIN1 T0
			where T0."DocEntry"=ORIN."DocEntry" AND T0."BaseType" = 13)
		AND JDT1."ShortName" = :card 	AND JDT1."IntrnMatch"=0;
	
	else
	
		SELECT  JDT1.* 
		FROM JDT1 INNER JOIN ORIN ON JDT1."CreatedBy"= ORIN."DocEntry" AND JDT1."TransType" = 14
		WHERE NOT EXISTS
			(select distinct("DocEntry") from RIN1 T0
			where T0."DocEntry"=ORIN."DocEntry" AND T0."BaseType" = 13)
		AND JDT1."ShortName" = :card 	AND JDT1."IntrnMatch"=0
		AND (	(JDT1."FCCredit"<>0 AND JDT1."FCCredit" IS NOT NULL)  OR 
			(JDT1."FCDebit"<>0 AND JDT1."FCDebit" IS NOT NULL));
	end if;


else


	if (:frgnCurr = N'N')
	then
		SELECT  JDT1.* 
		FROM JDT1 INNER JOIN ORPC ON JDT1."CreatedBy"= ORPC."DocEntry" AND JDT1."TransType" = 19
		WHERE NOT EXISTS
			(select distinct("DocEntry") from RPC1 T0
			where T0."DocEntry"=ORPC."DocEntry" AND T0."BaseType" = 18)
		AND JDT1."ShortName" = :card 	AND JDT1."IntrnMatch"=0;
	
	else
	
		SELECT  JDT1.* 
		FROM JDT1 INNER JOIN ORPC ON JDT1."CreatedBy"= ORPC."DocEntry" AND JDT1."TransType" = 19
		WHERE NOT EXISTS
			(select distinct("DocEntry") from RPC1 T0
			where T0."DocEntry"=ORPC."DocEntry" AND T0."BaseType" = 18)
		AND JDT1."ShortName" = :card 	AND JDT1."IntrnMatch"=0
		AND (	(JDT1."FCCredit"<>0 AND JDT1."FCCredit" IS NOT NULL)  OR 
			(JDT1."FCDebit"<>0 AND JDT1."FCDebit" IS NOT NULL));

	end if;

end if;
end;


