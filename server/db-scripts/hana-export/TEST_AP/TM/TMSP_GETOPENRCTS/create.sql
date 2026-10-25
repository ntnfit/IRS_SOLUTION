-- B1 DEPENDS: AFTER:PT:PROCESS_END

CREATE PROCEDURE  TmSp_GetOpenRCTs(
IN object  int,
IN card nvarchar(100),
IN frgnCurr nchar)

LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS
BEGIN
if (:object = 24)
THEN
	if (:frgnCurr = N'N')
	THEN
		SELECT JDT1.* FROM 
		JDT1 INNER JOIN ORCT ON JDT1."CreatedBy"= ORCT."DocEntry"
			
		WHERE
		JDT1."ShortName" = :card AND
		ORCT."PayNoDoc"=N'Y'  AND
		ORCT."NoDocSum" = (ORCT."DocTotal" -ORCT."VatSum" +ORCT."WtSum") AND
		JDT1."TransType" = 24  AND
		JDT1."IntrnMatch"=0 AND
		JDT1."Closed"=N'N' AND
		(JDT1."SourceLine" Is Null or  JDT1."SourceLine"<> -6);
	
	else

		SELECT JDT1.* FROM 
		JDT1 INNER JOIN ORCT ON JDT1."CreatedBy"= ORCT."DocEntry"
		
		WHERE
		JDT1."ShortName" =:card AND
		ORCT."PayNoDoc"=N'Y'  AND
		ORCT."NoDocSumFC" = (ORCT."DocTotalFC" - ORCT."VatSumFC" +ORCT."WtSumFrgn")  AND
		JDT1."TransType" =24  AND
		JDT1."IntrnMatch"=0 AND
		JDT1."Closed"=N'N' AND
		(JDT1."SourceLine" Is Null or  JDT1."SourceLine"<> -6);
	end IF;

else


	if (:frgnCurr = N'N')
	THEN
		SELECT JDT1.* FROM 
		JDT1 INNER JOIN OVPM ON JDT1."CreatedBy"= OVPM."DocEntry"
		
		WHERE
		JDT1."ShortName" = :card AND
		OVPM."PayNoDoc"=N'Y'  AND
		OVPM."NoDocSum" = (OVPM."DocTotal" - OVPM."VatSum" +OVPM."WtSum") AND
		JDT1."TransType" = 46  AND
		JDT1."IntrnMatch"=0 AND
		JDT1."Closed"=N'N' AND
		(JDT1."SourceLine" Is Null or  JDT1."SourceLine"<> -6);
	
	else
	
		SELECT JDT1.* FROM 
		JDT1 INNER JOIN OVPM ON JDT1."CreatedBy"= OVPM."DocEntry"
		
		WHERE
		JDT1."ShortName" = :card AND
		OVPM."PayNoDoc"=N'Y'  AND
		OVPM."NoDocSumFC" = (OVPM."DocTotalFC" - OVPM."VatSumFC" +OVPM."WtSumFrgn")  AND
		JDT1."TransType" = 46  AND
		JDT1."IntrnMatch"=0 AND
		JDT1."Closed"=N'N' AND
		(JDT1."SourceLine" Is Null or  JDT1."SourceLine"<> -6);
	end IF;
end IF;
END;


