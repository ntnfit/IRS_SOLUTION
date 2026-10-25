-- B1 DEPENDS: AFTER:PT:PROCESS_END AFTER:SP:_TmSp_BootCreateGlobalTempTables2

CREATE PROCEDURE TmSp_DAB002_Top5SLPFutureRemitByAgeDate(in agingDate1 nvarchar(8))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
agingDate nvarchar(20);
begin
	agingDate := CURRENT_DATE;

	--DELETE FROM "TMP_Top5SLP_Results";
	
	--INSERT INTO "TMP_Top5SLP_Results"

	Select TOP 5 T1.SLP, SUM(T1.balDue), T2."SlpName"
	FROM (	Select case when slp is null then -1 else slp end As SLP, balDue
		FROM
		(
			SELECT 	MAX(T5."SlpCode") as SLP, -MAX(T0."BalDueCred")-SUM(T1."ReconSum") as balDue
			FROM  
			JDT1 T0  
			INNER  JOIN ITR1 T1  ON  T1."TransId" = T0."TransId"  AND  T1."TransRowId" = T0."Line_ID"   
			INNER  JOIN OITR T2  ON  T2."ReconNum" = T1."ReconNum"   
			INNER  JOIN OCRD T4  ON  T4."CardCode" = T0."ShortName" 
		 	LEFT OUTER  JOIN "B1_JournalTransSourceView" T5  ON  T5."ObjType" = T0."TransType"  AND  
			T5."DocEntry" = T0."CreatedBy"  AND  (T5."TransType" <> N'I'  OR  (T5."TransType" = N'I'  
			AND  T5."InstlmntID" = T0."SourceLine" )) 
			WHERE T0."RefDate" <= :agingDate  AND  T4."CardType" = 'C'  
			AND  T2."ReconDate" > (:agingDate)  AND  T1."IsCredit" = 'C'
			AND T0."DueDate" >:agingDate 
			GROUP BY T0."TransId", T0."Line_ID" 
			HAVING MAX(T0."BalFcCred") <>- SUM(T1."ReconSumFC")  OR  MAX(T0."BalDueCred") <>- SUM(T1."ReconSum")   
			
			UNION ALL 
			--part 2
			SELECT 
			MAX(T5."SlpCode") as SLP, MAX(T0."BalDueDeb")+SUM(T1."ReconSum") as balDue 
			FROM  "JDT1" T0  
			INNER  JOIN "ITR1" T1  ON  T1."TransId" = T0."TransId"  AND  T1."TransRowId" = T0."Line_ID"   
			INNER  JOIN "OITR" T2  ON  T2."ReconNum" = T1."ReconNum"   
			INNER  JOIN "OCRD" T4  ON  T4."CardCode" = T0."ShortName"    
			LEFT OUTER  JOIN "B1_JournalTransSourceView" T5  
			ON  T5."ObjType" = T0."TransType"  AND  T5."DocEntry" = T0."CreatedBy"  
			AND  (T5."TransType" <> N'I'  OR  (T5."TransType" = N'I' 
			AND  T5."InstlmntID" = T0."SourceLine" ))  
			WHERE 
			T0."RefDate" <= :agingDate  AND  T4."CardType" = 'C'  
			AND  T2."ReconDate" > :agingDate  AND  T1."IsCredit" = 'D' 
			AND T0."DueDate">:agingDate 
			GROUP BY T0."TransId", T0."Line_ID" 
			HAVING MAX(T0."BalFcDeb") <>- SUM(T1."ReconSumFC")  OR  MAX(T0."BalDueDeb") <>- SUM(T1."ReconSum") 
			UNION ALL 
			--part3
			SELECT MAX(T3."SlpCode")  as SLP, 
			MAX(T0."BalDueDeb")-MAX(T0."BalDueCred") as balDue
			FROM  "JDT1" T0   
			INNER  JOIN "OCRD" T2  ON  T2."CardCode" = T0."ShortName" 
			AND T0."DueDate" >:agingDate  
			LEFT OUTER  JOIN "B1_JournalTransSourceView" T3  ON  T3."ObjType" = T0."TransType"  AND  
			T3."DocEntry" = T0."CreatedBy"  AND  (T3."TransType" <> N'I'  OR  (T3."TransType" = N'I'  
			AND  T3."InstlmntID" = T0."SourceLine" ))
			WHERE 
			T0."RefDate" <= :agingDate  AND  T2."CardType" = 'C'  
			AND  (T0."BalDueCred" <> T0."BalDueDeb"  OR  T0."BalFcCred" <> T0."BalFcDeb" ) 
			AND   NOT EXISTS 
			(SELECT 
				U0."TransId", U0."TransRowId" 
			FROM  "ITR1" U0  
				INNER  JOIN "OITR" U1  ON  U1."ReconNum" = U0."ReconNum"   
			WHERE T0."TransId" = U0."TransId"  AND  T0."Line_ID" = U0."TransRowId"  
				AND  U1."ReconDate" > :agingDate   
			GROUP BY U0."TransId", U0."TransRowId")   
			GROUP BY T0."TransId", T0."Line_ID"
			) as T0 ) T1
	inner join "OSLP" T2 on T1.SLP = T2."SlpCode"
	GROUP BY T1.SLP,T2."SlpName"
	ORDER BY SUM(T1.balDue) DESC;
	
	--DELETE FROM "TMP_Top5SLP_Results";
end;


