-- B1 DEPENDS: AFTER:SP:_TmSp_BootCreateGlobalTempTables2 AFTER:PT:PROCESS_END

CREATE PROCEDURE TmSp_IVSR_Check_DB_Candidate_For_IVI
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER 
AS
	StockIssue varchar(1) := 'N';
	tmpEntry int;
	
BEGIN
	-----------------------Start of special handle process for task 13458  CSN Note 1436758-------------------------
/* Detect if the database is affected by bug of task 13458 */
/* also detect if the database is affected by the 88 missing stock message */
/* this Part is similiar with Status sql, if change, please check Status sql also.*/
	-------------------------------------------------------------------------------- 
/* 1 odln*/ 
	SELECT count(t0."DocEntry") into tmpEntry
	    FROM   dln1 t0 
	           INNER JOIN odln t1 
	             ON t0."Quantity" <> 0 
	                AND t0."UpdInvntry" = 'Y'
	                AND t1."UpdInvnt" <> 'N'
	                AND t0."DocEntry" = t1."DocEntry"
	                AND IFNULL(t1."VersionNum", '') >= '08.80.000.00' 
	                AND t1."DocType" = 'I' 
	           LEFT JOIN dln14 t2
	             ON t0."TreeType" = 'A' 
	                AND t0."DocEntry" = t2."DocEntry" 
	                AND t0."LineNum" = t2."LineNum" 
	           INNER JOIN owhs t3 
	             ON t3."WhsCode" = CASE IFNULL(t2."DocEntry", -1) 
	                               WHEN -1 THEN t0."WhsCode" 
	                               ELSE t2."WhsCode" 
	                             END 
	                AND t3."DropShip" <> 'Y' 
	           INNER JOIN oitm t4 
	             ON t4."ItemCode" = CASE IFNULL(t2."DocEntry", -1) 
	                                WHEN -1 THEN t0."ItemCode" 
	                                ELSE t2."ItemCode" 
	                              END 
	                AND t4."InvntItem" = 'Y' 
	    WHERE  t0."LineNum" NOT IN (SELECT u0."DocLineNum" 
	                              FROM   oilm u0 
	                              WHERE  u0."TransType" = 15 
	                                     AND u0."DocEntry" = t0."DocEntry" 
	                                     AND u0."DocLineNum" = t0."LineNum" 
	                                     AND u0."SubLineNum" = IFNULL(t2."ChildNum", -1) 
	                                     AND u0."AccumType" = 1);
										 
	IF :tmpEntry > 0 
	  then
	      StockIssue := 'Y'; 
	  END if;
	
	--------------------------------------------------------------------------------  
/* 2 ordn*/ 
	SELECT count(t0."DocEntry") into tmpEntry
	    FROM   rdn1 t0 
	           INNER JOIN ordn t1 
	             ON t0."Quantity" <> 0 
	                AND t0."UpdInvntry" = 'Y' 
	                AND t1."UpdInvnt" <> 'N' 
	                AND t0."DocEntry" = t1."DocEntry" 
	                AND IFNULL(t1."VersionNum", '') >= '08.80.000.00' 
	                AND t1."DocType" = 'I' 
	           LEFT JOIN rdn14 t2 
	             ON t0."TreeType" = 'A' 
	                AND t0."DocEntry" = t2."DocEntry" 
	                AND t0."LineNum" = t2."LineNum" 
	           INNER JOIN owhs t3 
	             ON t3."WhsCode" = CASE IFNULL(t2."DocEntry", -1) 
	                               WHEN -1 THEN t0."WhsCode" 
	                               ELSE t2."WhsCode" 
	                             END 
	                AND t3."DropShip" <> 'Y' 
	           INNER JOIN oitm t4 
	             ON t4."ItemCode" = CASE IFNULL(t2."DocEntry", -1) 
	                                WHEN -1 THEN t0."ItemCode" 
	                                ELSE t2."ItemCode" 
	                              END 
	                AND t4."InvntItem" = 'Y' 
	    WHERE  t0."LineNum" NOT IN (SELECT u0."DocLineNum" 
	                              FROM   oilm u0 
	                              WHERE  u0."TransType" = 16 
	                                     AND u0."DocEntry" = t0."DocEntry" 
	                                     AND u0."DocLineNum" = t0."LineNum" 
	                                     AND u0."SubLineNum" = IFNULL(t2."ChildNum", -1) 
	                                     AND u0."AccumType" = 1);
										 
	IF :StockIssue <> 'Y' and :tmpEntry > 0 
	then
	    StockIssue := 'Y'; 
	END if;
	
	--------------------------------------------------------------------------------  
/* 3 oinv*/ 
	SELECT count(t0."DocEntry") into tmpEntry 
	    FROM   inv1 t0 
	           INNER JOIN oinv t1 
	             ON t0."Quantity" <> 0 
	                AND t0."UpdInvntry" = 'Y' 
	                AND t1."UpdInvnt" <> 'N' 
	                AND t0."DocEntry" = t1."DocEntry" 
	                AND IFNULL(t1."VersionNum", '') >= '08.80.000.00' 
	                AND t1."DocType" = 'I' 
	                AND t1."isIns" = 'N' 
	           LEFT JOIN inv14 t2 
	             ON t0."TreeType" = 'A' 
	                AND t0."DocEntry" = t2."DocEntry" 
	                AND t0."LineNum" = t2."LineNum" 
	           INNER JOIN owhs t3 
	             ON t3."WhsCode" = CASE IFNULL(t2."DocEntry", -1) 
	                               WHEN -1 THEN t0."WhsCode" 
	                               ELSE t2."WhsCode" 
	                             END 
	                AND t3."DropShip" <> 'Y' 
	           INNER JOIN oitm t4 
	             ON t4."ItemCode" = CASE IFNULL(t2."DocEntry", -1) 
	                                WHEN -1 THEN t0."ItemCode" 
	                                ELSE t2."ItemCode" 
	                              END 
	                AND t4."InvntItem" = 'Y' 
	    WHERE  t0."LineNum" NOT IN (SELECT u0."DocLineNum" 
	                              FROM   oilm u0 
	                              WHERE  u0."TransType" = 13 
	                                     AND u0."DocEntry" = t0."DocEntry" 
	                                     AND u0."DocLineNum" = t0."LineNum" 
	                                     AND u0."SubLineNum" = IFNULL(t2."ChildNum", -1) 
	                                     AND u0."AccumType" IN ( 1, 4 ));
										 
	IF :StockIssue <> 'Y' and :tmpEntry > 0 
	  then 
	      StockIssue := 'Y'; 
	  END if;
	
	--------------------------------------------------------------------------------  
/* 4 orin*/ 
 SELECT count(t0."DocEntry") into tmpEntry
	    FROM   rin1 t0 
	           INNER JOIN orin t1 
	             ON t0."Quantity" <> 0 
	                AND t0."UpdInvntry" = 'Y' 
	                AND t1."UpdInvnt" <> 'N' 
	                AND t0."BaseType" not in (16 ,203)
	                AND t0."DocEntry" = t1."DocEntry" 
	                AND IFNULL(t1."VersionNum", '') >= '08.80.000.00' 
	                AND t1."DocType" = 'I' 
	           INNER JOIN oinv t5 
	             ON t0."BaseEntry" = t5."DocEntry"
	                AND t0."BaseType" = t5."ObjType" 
	                AND t5."isIns" = 'N' 
	                AND t5."DocType" = 'I' 
	           LEFT JOIN rin14 t2 
	             ON t0."TreeType" = 'A' 
	                AND t0."DocEntry" = t2."DocEntry" 
	                AND t0."LineNum" = t2."LineNum" 
	           INNER JOIN owhs t3 
	             ON t3."WhsCode" = CASE IFNULL(t2."DocEntry", -1) 
	                               WHEN -1 THEN t0."WhsCode" 
	                               ELSE t2."WhsCode" 
	                             END 
	                AND t3."DropShip" <> 'Y' 
	           INNER JOIN oitm t4 
	             ON t4."ItemCode" = CASE IFNULL(t2."DocEntry", -1) 
	                                WHEN -1 THEN t0."ItemCode" 
	                                ELSE t2."ItemCode" 
	                              END 
	                AND t4."InvntItem" = 'Y' 
	    WHERE  t0."LineNum" NOT IN (SELECT u0."DocLineNum" 
	                              FROM   oilm u0 
	                              WHERE  u0."TransType" = 14 
	                                     AND u0."DocEntry" = t0."DocEntry" 
	                                     AND u0."DocLineNum" = t0."LineNum" 
	                                     AND u0."SubLineNum" = IFNULL(t2."ChildNum", -1) 
	                                     AND u0."AccumType" IN (0, 1) );
	IF :StockIssue <> 'Y' and :tmpEntry > 0 
	  then 
	      StockIssue := 'Y'; 
	  END if;
	
	--------------------------------------------------------------------------------  
/* 5 opdn*/ 
	SELECT count(t0."DocEntry") into tmpEntry
	    FROM   pdn1 t0 
	           INNER JOIN opdn t1 
	             ON t0."Quantity" <> 0 
	                AND t0."UpdInvntry" = 'Y' 
	                AND t1."UpdInvnt" <> 'N' 
	                AND t0."DocEntry" = t1."DocEntry" 
	                AND IFNULL(t1."VersionNum", '') >= '08.80.000.00' 
	                AND t1."DocType" = 'I' 
	           INNER JOIN owhs t2 
	             ON t2."WhsCode" = t0."WhsCode" 
	                AND t2."DropShip" <> 'Y' 
	           INNER JOIN oitm t3 
	             ON t3."ItemCode" = t0."ItemCode" 
	                AND t3."InvntItem" = 'Y' 
	    WHERE  t0."LineNum" NOT IN (SELECT u0."DocLineNum" 
	                              FROM   oilm u0 
	                              WHERE  u0."TransType" = 20 
	                                     AND u0."DocEntry" = t0."DocEntry" 
	                                     AND u0."DocLineNum" = t0."LineNum" 
	                                     AND u0."AccumType" = 1);
										 
	IF :StockIssue <> 'Y' and :tmpEntry > 0 
	  then
	      StockIssue := 'Y'; 
	  end if; 
	-------------------------------------------------------------------------------- 
/* 6 orpd*/ 
/*AP Return*/ 
	SELECT count(t0."DocEntry") into tmpEntry
	    FROM   rpd1 t0 
	           INNER JOIN orpd t1 
	             ON t0."Quantity" <> 0 
	                AND t0."UpdInvntry" = 'Y' 
	                AND t1."UpdInvnt" <> 'N' 
	                AND t0."DocEntry" = t1."DocEntry" 
	                AND IFNULL(t1."VersionNum", '') >= '08.80.000.00' 
	                AND t1."DocType" = 'I' 
	           INNER JOIN owhs t2 
	             ON t2."WhsCode" = t0."WhsCode" 
	                AND t2."DropShip" <> 'Y' 
	           INNER JOIN oitm t3 
	             ON t3."ItemCode" = t0."ItemCode" 
	                AND t3."InvntItem" = 'Y' 
	    WHERE  t0."LineNum" NOT IN (SELECT u0."DocLineNum" 
	                              FROM   oilm u0 
	                              WHERE  u0."TransType" = 21 
	                                     AND u0."DocEntry" = t0."DocEntry" 
	                                     AND u0."DocLineNum" = t0."LineNum" 
	                                     AND u0."AccumType" = 1);
										 
	IF :StockIssue <> 'Y' and :tmpEntry > 0 
	  then
	      StockIssue := 'Y'; 
	  END if;
	
	--------------------------------------------------------------------------------  
/* 8 opch*/
	SELECT count(t0."DocEntry") into tmpEntry
	    FROM   pch1 t0 
	           INNER JOIN opch t1 
	             ON t0."Quantity" <> 0 
	                AND t0."UpdInvntry" = 'Y' 
	                AND t1."UpdInvnt" <> 'N' 
	                AND t0."DocEntry" = t1."DocEntry" 
	                AND IFNULL(t1."VersionNum", '') >= '08.80.000.00' 
	                AND t1."DocType" = 'I' 
	                AND t1."isIns" = 'N' 
	           INNER JOIN owhs t2 
	             ON t2."WhsCode" = t0."WhsCode" 
	                AND t2."DropShip" <> 'Y' 
	           INNER JOIN oitm t3 
	             ON t3."ItemCode" = t0."ItemCode" 
	                AND t3."InvntItem" = 'Y' 
	    WHERE  t0."LineNum" NOT IN (SELECT u0."DocLineNum" 
	                              FROM   oilm u0 
	                              WHERE  u0."TransType" = 18 
	                                     AND u0."DocEntry" = t0."DocEntry" 
	                                     AND u0."DocLineNum" = t0."LineNum" 
	                                     AND u0."AccumType" = 1);
	
	IF :StockIssue <> 'Y' and :tmpEntry > 0 
	  then
	      StockIssue := 'Y'; 
	  END if;
	
	--------------------------------------------------------------------------------  
/* 9 orpc*/ 
/*AP Credit Memo*/ 
	SELECT count(t0."DocEntry") into tmpEntry
	    FROM   rpc1 t0 
	           INNER JOIN orpc t1 
	             ON t0."Quantity" <> 0 
	                AND t0."UpdInvntry" = 'Y' 
	                AND t1."UpdInvnt" <> 'N' 
	                AND t0."DocEntry" = t1."DocEntry" 
	                AND t0."BaseType" not in (21 ,204)
	                AND IFNULL(t1."VersionNum", '') >= '08.80.000.00' 
	                AND t1."DocType" = 'I' 
	           INNER JOIN opch t4 
	             ON t0."BaseEntry" = t4."DocEntry"
	                AND t0."BaseType" = t4."ObjType" 
	                AND t4."isIns" = 'N' 
	                AND t4."DocType" = 'I' 
	           INNER JOIN owhs t2 
	             ON t2."WhsCode" = t0."WhsCode" 
	                AND t2."DropShip" <> 'Y' 
	           INNER JOIN oitm t3 
	             ON t3."ItemCode" = t0."ItemCode" 
	                AND t3."InvntItem" = 'Y' 
	    WHERE  t0."LineNum" NOT IN (SELECT u0."DocLineNum" 
	                              FROM   oilm u0 
	                              WHERE  u0."TransType" = 19 
	                                     AND u0."DocEntry" = t0."DocEntry" 
	                                     AND u0."DocLineNum" = t0."LineNum" 
	                                     AND u0."AccumType" = 1);
	IF :StockIssue <> 'Y' and :tmpEntry > 0 
	  then
	      StockIssue := 'Y'; 
	  END if;
	
/* 10 oipf*/ 
/*Landed costs*/ 
	SELECT count(t0."DocEntry") into tmpEntry
	    FROM   ipf1 t0 
	           INNER JOIN oipf t1 
	             ON t0."DocEntry" = t1."DocEntry" 
	                AND IFNULL(t1."VersionNum", '') >= ( CASE (SELECT "ContInvnt" FROM oadm) 
	                                                     WHEN 'Y' THEN '08.80.000.00' 
	                                                     ELSE '08.80.225.06' 
	                                                   END ) 
	           INNER JOIN owhs t2 
	             ON t2."WhsCode" = t0."WhsCode" 
	                AND t2."DropShip" <> 'Y' 
	           INNER JOIN oitm t3 
	             ON t3."ItemCode" = t0."ItemCode" 
	                AND t3."InvntItem" = 'Y' 
	    WHERE  t0."LineNum" NOT IN (SELECT u0."DocLineNum" 
	                              FROM   oilm u0 
	                              WHERE  u0."TransType" = 69 
	                                     AND u0."DocEntry" = t0."DocEntry" 
	                                     AND u0."DocLineNum" = t0."LineNum" 
	                                     AND u0."AccumType" = 1);
										 
	IF :StockIssue <> 'Y' and :tmpEntry > 0 
	  then
	      StockIssue := 'Y'; 
	  END if;
	
/* 11 oign*/ 
/*Goods Receipt*/ 
	SELECT count(t0."DocEntry") into tmpEntry
	    FROM   ign1 t0 
	           INNER JOIN oign t1 
	             ON t0."DocEntry" = t1."DocEntry" 
	                AND IFNULL(t1."VersionNum", '') >= '08.80.000.00'
					INNER JOIN OITM t2 on t0."ItemCode" = t2."ItemCode" and t2."InvntItem" = 'Y' 
	    WHERE  t0."LineNum" NOT IN (SELECT u0."DocLineNum" 
	                              FROM   oilm u0 
	                              WHERE  u0."TransType" = 59 
	                                     AND u0."DocEntry" = t0."DocEntry" 
	                                     AND u0."DocLineNum" = t0."LineNum" 
	                                     AND u0."AccumType" = 1);
	IF :StockIssue <> 'Y' and :tmpEntry > 0 
	  then
	      StockIssue := 'Y'; 
	  END if;
	
/* 12 Goods Receipt for backflush components*/ 
/*Goods Receipt for backflush components    
e.g. when receipt from production order and bom with backflush components(N.B.by product is permitted),    
besides register message of bom itself, we will register message for component, this query     
is to detect the existance of message of component    
-- This is relevant for versions below 9.1, from 9.1 all components are issued via issue for production only 
*/ 

	SELECT count(t0."DocEntry") into tmpEntry
	    FROM   ign1 t0 
	           INNER JOIN oign t1 
	             ON t0."DocEntry" = t1."DocEntry" 
	                AND t0."BaseType" = 202 
	                AND IFNULL(t0."BaseLine", -1) = -1 
	                AND IFNULL(t1."VersionNum", '') >= '08.80.000.00'
	                AND IFNULL(t1."VersionNum", '') < '09.10.000.00' 
	           INNER JOIN owor t2 
	             ON t0."BaseEntry" = t2."DocEntry" 
	           INNER JOIN wor1 t3 
	             ON t0."BaseEntry" = t3."DocEntry" 
	                AND t3."IssueType" = 'B' 
	           INNER JOIN owhs t7 
	             ON t7."WhsCode" = t3."wareHouse" 
	                AND t7."DropShip" <> 'Y' 
	           INNER JOIN oitm t8 
	             ON t8."ItemCode" = t3."ItemCode" 
	                AND t8."InvntItem" = 'Y' 
	    WHERE  t3."LineNum" NOT IN (SELECT "AppObjLine" 
	                              FROM   oilm u0 
	                              WHERE  u0."TransType" = 59 
	                                     AND u0."BaseType" = 202 
	                                     AND u0."AppObjAbs" = t3."DocEntry" 
	                                     AND u0."AppObjLine" = t3."LineNum" 
	                                     AND u0."AccumType" = 1 
	                                     AND (u0."AppObjType" in ('C', 'B')));
												
	IF :StockIssue <> 'Y' and :tmpEntry > 0 
	  then
	      StockIssue := 'Y'; 
	  END if;
	
/* 13 oige*/ 
/*Goods Issue*/ 
	SELECT count(t0."DocEntry") into tmpEntry
	    FROM   ige1 t0 
	           INNER JOIN oige t1 
	             ON t0."DocEntry" = t1."DocEntry" 
	                AND IFNULL(t1."VersionNum", '') >= '08.80.000.00' 
					INNER JOIN OITM t2 on t0."ItemCode" = t2."ItemCode" and t2."InvntItem" = 'Y'
	    WHERE  t0."LineNum" NOT IN (SELECT u0."DocLineNum" 
	                              FROM   oilm u0 
	                              WHERE  u0."TransType" = 60 
	                                     AND u0."DocEntry" = t0."DocEntry" 
	                                     AND u0."DocLineNum" = t0."LineNum" 
	                                     AND u0."AccumType" = 1);
	IF :StockIssue <> 'Y' and :tmpEntry > 0 
	  then
	      StockIssue := 'Y'; 
	  end if;
	
/* 14 Goods Issue for backflush components*/ 
/*Goods Issue for backflush components    
e.g. when receipt from production order and bom with backflush components(N.B.by product is not permitted),    
besides register message of bom itself, we will register message for component, this query     
is to detect the existance of message of component 
-- This is relevant for versions below 9.1, from 9.1 all components are issued via issue for production only   
*/ 
	SELECT count(t0."DocEntry") into tmpEntry
	    FROM   ige1 t0 
	           INNER JOIN oige t1 
	             ON t0."DocEntry" = t1."DocEntry" 
	                AND t0."BaseType" = 202 
	                AND IFNULL(t0."BaseLine", -1) = -1 
	                AND IFNULL(t1."VersionNum", '') >= '08.80.000.00' 
	                AND IFNULL(t1."VersionNum", '') < '09.10.000.00' 
	           INNER JOIN owor t2 
	             ON t0."BaseEntry" = t2."DocEntry" 
	           INNER JOIN wor1 t3 
	             ON t0."BaseEntry" = t3."DocEntry" 
	                AND t3."IssueType" = 'B' 
	           INNER JOIN owhs t7 
	             ON t7."WhsCode" = t3."wareHouse" 
	                AND t7."DropShip" <> 'Y' 
	           INNER JOIN oitm t8 
	             ON t8."ItemCode" = t3."ItemCode" 
	                AND t8."InvntItem" = 'Y' 
	    WHERE  t3."LineNum" NOT IN (SELECT "AppObjLine" 
	                              FROM   oilm u0 
	                              WHERE  u0."TransType" = 60 
	                                     AND u0."BaseType" = 202 
	                                     AND u0."AppObjAbs" = t3."DocEntry" 
	                                     AND u0."AppObjLine" = t3."LineNum" 
	                                     AND u0."AccumType" = 1 
	                                     AND u0."AppObjType" = 'C');
										 
	IF :StockIssue <> 'Y' and :tmpEntry > 0 
	  then
	      StockIssue := 'Y'; 
	  END if;
/* 15 owtr*/ 
	SELECT count(t0."DocEntry") into tmpEntry
	    FROM   wtr1 t0 
	           INNER JOIN owtr t1 
	             ON t0."DocEntry" = t1."DocEntry" and T0."Quantity" <> 0
	                AND IFNULL(t1."VersionNum", '') >= '08.80.000.00' 
	    WHERE  t0."LineNum" NOT IN (SELECT u0."DocLineNum" 
	                              FROM   oilm u0 
	                              WHERE  u0."TransType" = 67 
	                                     AND u0."DocEntry" = t0."DocEntry" 
	                                     AND u0."DocLineNum" = t0."LineNum" 
	                                     AND u0."AccumType" = 1) 
	            OR t0."WhsCode" NOT IN (SELECT u1."LocCode" 
	                                  FROM   oilm u1 
	                                  WHERE  u1."TransType" = 67 
	                                         AND u1."DocEntry" = t0."DocEntry" 
	                                         AND u1."DocLineNum" = t0."LineNum" 
	                                         AND u1."AccumType" = 1) 
	            OR t0."FromWhsCod" NOT IN (SELECT u2."LocCode" 
	                                 FROM   oilm u2 
	                                 WHERE  u2."TransType" = 67 
	                                        AND u2."DocEntry" = t0."DocEntry" 
	                                        AND u2."DocLineNum" = t0."LineNum" 
	                                        AND u2."AccumType" = 1);
	IF :StockIssue <> 'Y' and :tmpEntry > 0 
	  then
	      StockIssue := 'Y'; 
	  END if;
	
/* 16 oiqi*/ 
/*Initial Quantity*/ 
/*VersionNum < 9.00*/
	SELECT count(t0."DocEntry") into tmpEntry
	    FROM   IQI1 t0 
				   INNER JOIN OIQI t1 
					 ON t0."DocEntry" = t1."DocEntry" 
						AND IFNULL(t1."VersionNum",'') >= '08.80.000.00' 
				   INNER JOIN OITM t2 on t0."ItemCode" = t2."ItemCode" and t2."InvntItem" = 'Y'
                    WHERE NOT EXISTS (SELECT 1 FROM OILM u0 
                                                      WHERE u0."TransType" = 310000001 
                                                              AND u0."AccumType" = 1
                                                              AND u0."ItemCode" = t0."ItemCode"
                                                              AND u0."LocCode" = t0."WhsCode"
                                                              AND u0."DocEntry" = t0."DocEntry");
											
	IF :StockIssue <> 'Y' and :tmpEntry > 0 
	  then
	      StockIssue := 'Y'; 
	  END if;
	
/* 17 oiqr*/ 
	SELECT count(t0."DocEntry") into tmpEntry
	    FROM   IQR1 t0 
				   INNER JOIN OIQR t1 
					 ON t0."DocEntry" = t1."DocEntry" 
						AND IFNULL(t1."VersionNum",'') >= '08.80.000.00' 
						AND t0."Quantity" <> 0 
				   INNER JOIN OITM t2 on t0."ItemCode" = t2."ItemCode" and t2."InvntItem" = 'Y'
                    WHERE NOT EXISTS (SELECT 1 FROM OILM u0 
                                                      WHERE u0."TransType" = 10000071 
                                                              AND u0."AccumType" = 1
                                                              AND u0."ItemCode" = t0."ItemCode"
                                                              AND u0."LocCode" = t0."WhsCode"
                                                              AND u0."DocEntry" = t0."DocEntry");
	IF :StockIssue <> 'Y' and :tmpEntry > 0 
	  then
	      StockIssue := 'Y'; 
	  END if;

/* 18 omrv*/ 
/*Material Revaluation for moving average, standard item*/ 
	SELECT count(t0."DocEntry") into tmpEntry
	    FROM   mrv1 t0 
	           INNER JOIN omrv t1 
	             ON t0."DocEntry" = t1."DocEntry" 
	                AND IFNULL(t1."VersionNum", '') >= '08.80.000.00' 
	                AND t0."EvalSystem" IN ( 'A', 'S' )
				INNER JOIN OITM t2 on t0."ItemCode" = t2."ItemCode" and t2."InvntItem" = 'Y'
	    WHERE  t0."LineNum" NOT IN (SELECT u0."DocLineNum" 
	                              FROM   oilm u0 
	                              WHERE  u0."TransType" = 162 
	                                     AND u0."DocEntry" = t0."DocEntry" 
	                                     AND u0."DocLineNum" = t0."LineNum" 
	                                     AND u0."AccumType" = 1);
	
	IF :StockIssue <> 'Y' and :tmpEntry > 0 
	  then
	      StockIssue := 'Y'; 
	  END if;
/* 19 OMRV fifo*/ 
/*Material Revaluation for FIFO item*/
	SELECT count(t0."DocEntry") into tmpEntry
	    FROM   mrv2 t0 
	           INNER JOIN omrv t1 
	             ON t0."DocEntry" = t1."DocEntry" 
	                AND IFNULL(t1."VersionNum",'') >= '08.80.000.00'
					INNER JOIN MRV1 t2 on t0."DocEntry" = t2."DocEntry"
					INNER JOIN OITM t3 on t3."ItemCode" = t2."ItemCode" and t3."InvntItem" = 'Y'
	    WHERE  t0."LineNum" NOT IN (SELECT u0."SubLineNum" 
	                              FROM   oilm u0 
	                              WHERE  u0."TransType" = 162 
	                                     AND u0."DocEntry" = t0."DocEntry" 
	                                     AND u0."DocLineNum" = t0."BaseLine" 
	                                     AND u0."SubLineNum" = t0."LineNum" 
	                                     AND u0."AccumType" = 1);
										 
	IF :StockIssue <> 'Y' and :tmpEntry > 0 
	  then
	    StockIssue := 'Y'; 
	  END if;
	  
/* 20 AR Correction Invoice*/  
	SELECT count(t0."DocEntry") into tmpEntry
	    FROM   (SELECT shb."DocEntry",
				shb."LineNum",
				shb."ItemCode",
				(shb."Quantity" + was."Quantity") AS "Quantity",
				shb."UpdInvntry",
				shb."TreeType",
				shb."WhsCode"
			FROM csi1 shb
			INNER JOIN csi1 was ON
				shb."CEECFlag" = 'S' and
				was."CEECFlag" = 'W' and
				was."DocEntry" = shb."DocEntry" and
				was."LineNum" = shb."CiOppLineN"
			) t0 
	           INNER JOIN ocsi t1 
	             ON t0."Quantity" <> 0 
	                AND t0."UpdInvntry" = 'Y' 
	                AND t1."UpdInvnt" <> 'N' 
	                AND t0."DocEntry" = t1."DocEntry" 
	                AND IFNULL(t1."VersionNum",'') >= '08.80.000.00' 
	                AND t1."DocType" = 'I' 
	                AND T1."isIns" = 'N'
	           LEFT JOIN csi14 t2 
	             ON t0."TreeType" = 'A' 
	                AND t0."DocEntry" = t2."DocEntry" 
	                AND t0."LineNum" = t2."LineNum" 
	           INNER JOIN owhs t3 
	             ON t3."WhsCode" = CASE IFNULL(t2."DocEntry", -1) 
	                               WHEN -1 
	                               THEN t0."WhsCode" 
	                               ELSE t2."WhsCode" 
	                             END 
	                AND t3."DropShip" <> 'Y' 
	           INNER JOIN oitm t4 
	             ON t4."ItemCode" = CASE IFNULL(t2."DocEntry",-1) 
	                                WHEN -1 
	                                THEN t0."ItemCode" 
	                                ELSE t2."ItemCode" 
	                              END 
	                AND t4."InvntItem" = 'Y' 
	    WHERE  t0."LineNum" NOT IN (SELECT u0."DocLineNum" 
	                              FROM   oilm u0 
	                              WHERE  u0."TransType" = 165 
	                                     AND u0."DocEntry" = t0."DocEntry" 
	                                     AND u0."DocLineNum" = t0."LineNum" 
	                                     AND u0."SubLineNum" = IFNULL(t2."ChildNum", -1) 
	                                     AND u0."AccumType" = 1);
										 
	IF :StockIssue <> 'Y' and :tmpEntry > 0 
	  then
	    	StockIssue := 'Y'; 
	  END if;
	  
/* 21 AR Correction Invoice Reversal*/ 
	SELECT count(t0."DocEntry") into tmpEntry
	    FROM   (SELECT shb."DocEntry",
				shb."LineNum",
				shb."ItemCode",
				(shb."Quantity" + was."Quantity") AS "Quantity",
				shb."UpdInvntry",
				shb."TreeType",
				shb."WhsCode"
			FROM csv1 shb
			INNER JOIN csv1 was ON
				shb."CEECFlag" = 'S' and
				was."CEECFlag" = 'W' and
				was."DocEntry" = shb."DocEntry" and
				was."LineNum" = shb."CiOppLineN"
			) t0 
	           INNER JOIN ocsv t1 
	             ON t0."Quantity" <> 0 
	                AND t0."UpdInvntry" = 'Y' 
	                AND t1."UpdInvnt" <> 'N' 
	                AND t0."DocEntry" = t1."DocEntry" 
	                AND IFNULL(t1."VersionNum",'') >= '08.80.000.00' 
	                AND t1."DocType" = 'I' 
	                AND t1."isIns" = 'N'
	           LEFT JOIN csv14 t2 
	             ON t0."TreeType" = 'A' 
	                AND t0."DocEntry" = t2."DocEntry" 
	                AND t0."LineNum" = t2."LineNum" 
	           INNER JOIN owhs t3 
	             ON t3."WhsCode" = CASE IFNULL(t2."DocEntry",-1) 
	                               WHEN -1 
	                               THEN t0."WhsCode" 
	                               ELSE t2."WhsCode" 
	                             END 
	                AND t3."DropShip" <> 'Y' 
	           INNER JOIN oitm t4 
	             ON t4."ItemCode" = CASE IFNULL(t2."DocEntry",-1) 
	                                WHEN -1 
	                                THEN t0."ItemCode" 
	                                ELSE t2."ItemCode" 
	                              END 
	                AND t4."InvntItem" = 'Y' 
	    WHERE  t0."LineNum" NOT IN (SELECT u0."DocLineNum" 
	                              FROM   oilm u0 
	                              WHERE  u0."TransType" = 166 
	                                     AND u0."DocEntry" = t0."DocEntry" 
	                                     AND u0."DocLineNum" = t0."LineNum" 
	                                     AND u0."SubLineNum" = IFNULL(t2."ChildNum",-1) 
	                                     AND u0."AccumType" = 1);
	IF :StockIssue <> 'Y' and :tmpEntry > 0 
	  then
	    	StockIssue := 'Y'; 
	  END if;
	  
/* 22 AP Correction Invoice*/ 
	SELECT count(t0."DocEntry") into tmpEntry
	    FROM   (SELECT shb."DocEntry",
				shb."LineNum",
				shb."ItemCode",
				(shb."Quantity" + was."Quantity") AS "Quantity",
				(shb."StockSum" + was."StockSum") as "StockSum",
				shb."UpdInvntry",
				shb."TreeType",
				shb."WhsCode"
			FROM cpi1 shb
			INNER JOIN cpi1 was ON
				shb."CEECFlag" = 'S' and
				was."CEECFlag" = 'W' and
				was."DocEntry" = shb."DocEntry" and
				was."LineNum" = shb."CiOppLineN"
			) t0 
	           INNER JOIN ocpi t1 
	             ON (t0."Quantity" <> 0 or t0."StockSum" <> 0)
	                AND t0."UpdInvntry" = 'Y' 
	                AND t1."UpdInvnt" <> 'N' 
	                AND t0."DocEntry" = t1."DocEntry" 
	                AND IFNULL(t1."VersionNum",'') >= '08.80.000.00' 
	                AND t1."DocType" = 'I' 
	                AND t1."isIns" = 'N'
	           INNER JOIN owhs t2 
	             ON t2."WhsCode" = t0."WhsCode" 
	                AND t2."DropShip" <> 'Y' 
	           INNER JOIN oitm t3 
	             ON t3."ItemCode" = t0."ItemCode" 
	                AND t3."InvntItem" = 'Y' 
	    WHERE  t0."LineNum" NOT IN (SELECT u0."DocLineNum" 
	                              FROM   oilm u0 
	                              WHERE  u0."TransType" = 163 
	                                     AND u0."DocEntry" = t0."DocEntry" 
	                                     AND u0."DocLineNum" = t0."LineNum" 
	                                     AND u0."AccumType" = 1);
										 
	IF :StockIssue <> 'Y' and :tmpEntry > 0 
	  then
	  		StockIssue := 'Y'; 
	  END if;
	  
/* 23  AP Correction Invoice Reversal*/ 
/*AP Correction Invoice Reversal*/
	SELECT count(t0."DocEntry") into tmpEntry
	    FROM   (SELECT shb."DocEntry",
				shb."LineNum",
				shb."ItemCode",
				(shb."Quantity" + was."Quantity") AS "Quantity",
				(shb."StockSum" + was."StockSum") as "StockSum",
				shb."UpdInvntry",
				shb."TreeType",
				shb."WhsCode"
			FROM cpv1 shb
			INNER JOIN cpv1 was ON
				shb."CEECFlag" = 'S' and
				was."CEECFlag" = 'W' and
				was."DocEntry" = shb."DocEntry" and
				was."LineNum" = shb."CiOppLineN"
			) t0 
	           INNER JOIN ocpv t1 
	             ON (t0."Quantity" <> 0 or t0."StockSum" <> 0)
	                AND t0."UpdInvntry" = 'Y' 
	                AND t1."UpdInvnt" <> 'N' 
	                AND t0."DocEntry" = t1."DocEntry" 
	                AND IFNULL(t1."VersionNum", '') >= '08.80.000.00' 
	                AND t1."DocType" = 'I' 
	                AND T1."isIns" = 'N'
	           INNER JOIN owhs t2 
	             ON t2."WhsCode" = t0."WhsCode" 
	                AND t2."DropShip" <> 'Y' 
	           INNER JOIN oitm t3 
	             ON t3."ItemCode" = t0."ItemCode" 
	                AND t3."InvntItem" = 'Y' 
	    WHERE  t0."LineNum" NOT IN (SELECT u0."DocLineNum" 
	                              FROM   oilm u0 
	                              WHERE  u0."TransType" = 164 
	                                     AND u0."DocEntry" = t0."DocEntry" 
	                                     AND u0."DocLineNum" = t0."LineNum" 
	                                     AND u0."AccumType" = 1);
										 
	IF :StockIssue <> 'Y' and :tmpEntry > 0 
	  then
	  		StockIssue := 'Y'; 
	  END if;
	-----------------------End of special handle process for task 13458-------------------------
	IF :StockIssue = 'Y' then
		SELECT 'Stock issues exists in the DB.  IVI need to be used.' As "Errors" FROM DUMMY;
	ELSE
		SELECT ' ' As "Errors" FROM DUMMY Where 1 <> 1;
	END IF;
end;


