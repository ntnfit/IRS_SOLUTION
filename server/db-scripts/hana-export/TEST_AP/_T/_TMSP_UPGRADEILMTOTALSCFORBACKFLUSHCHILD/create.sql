-- B1 DEPENDS: BEFORE:PT:PROCESS_START

create procedure _TmSp_UpgradeILMTotalSCforBackFlushChild
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS 
	dec TINYINT;
BEGIN 
      /*   
      *Task: 14305  
      *this query is trying to fix the TotalSC in OILM for production back flush child while we upgrade compamy DB to 8.8  
      */ 
	  select "SumDec" into dec from OADM; 
	  
	  UPDATE oilm t1
      SET    "TotalSC" = Round ("TotalLC" * t1."TotalLC" / nullif(t1."TotalLC",0), :dec) 
      WHERE  "ApplObj" = 202 
             AND t1."ApplObj" = 202 
             AND "AppObjAbs" = t1."AppObjAbs" 
             AND "AppObjType" = 'C' 
             AND t1."AppObjType" = 'P' 
             AND "DocEntry" = t1."DocEntry" 
             AND "TransType" = t1."TransType" 
             AND "DocLineNum" = t1."DocLineNum" 
			 -- AND Isnull(t1.totallc, 0) <> 0 ;
             AND t1."TotalLC" is not null; 
END;


