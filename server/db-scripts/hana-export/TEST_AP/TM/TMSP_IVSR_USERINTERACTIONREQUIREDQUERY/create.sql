-- B1 DEPENDS: AFTER:PT:PROCESS_END

CREATE PROCEDURE TmSp_IVSR_UserInteractionRequiredQuery

LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS


cntResults int;
CntScenarios int;
Sys_Currency nvarchar(3);
Main_Currency nvarchar(3);
rownum int;
rowcount int;

newAcctDe nvarchar(1);	
CurrPeriodCat nvarchar (10);
CmpyhasStockAct int;  
CmpyhasReturnAct int; 
oivlCnt			int;
oivlInvActOKCnt	int;
oivlRtnActOKCnt	int;

BEGIN

--IF object_id('[tempdb]..#TempCheckStockResults') IS NOT NULL DROP TABLE [#TempCheckStockResults]

delete from "IVSRUserInteractionRequiredQuery_TempCheckStockResults";
delete from "IVSRUserInteractionRequiredQuery_SBO_RSP_IVI_Affected_Item";
delete from "IVSRUserInteractionRequiredQuery_SBO_RSP_IVI_Affected_WareHouse";

select "MainCurncy"  INTO main_currency from "OADM";
select "SysCurrncy" INTO Sys_Currency from "OADM";
CntScenarios := 0;

select count(T0."ItemCode") into rowcount
            
     from   "WOR1" T0
            INNER JOIN "OITM" T1 ON T0."ItemCode" = T1."ItemCode"
     WHERE  T1."InvntItem" = 'N'
            AND T1."EvalSystem" <> 'S';
If :rowcount > 0 
    then
		INSERT INTO "IVSRUserInteractionRequiredQuery_TempCheckStockResults" VALUES (N'Non inventory & non Standard item in Production_Items_Lines');
        CntScenarios := :CntScenarios + 1;
    end if;



select COUNT(T0."ItemCode") INTO rowcount
     FROM   "OIVL" T0
            INNER JOIN "ITT1" t2 on T0."ItemCode" = t2."Father"
            inner join "OITM" T1 ON T0."ItemCode" = T1."ItemCode"
                                  or t2."Code" = t1."ItemCode"
     WHERE  T0."TransType" = 68
            AND T1."InvntItem" = 'N'
            AND T1."EvalSystem" <> 'S';

    If :rowcount > 0 
    then
		INSERT INTO "IVSRUserInteractionRequiredQuery_TempCheckStockResults" VALUES (N'Non inventory & Non Standard items in Old Work Order_ OIVL');
        CntScenarios := :CntScenarios + 1;
    end if;



select COUNT(T0."ItemCode") INTO rowcount
     FROM   "WKO1" T0
            inner join "OWKO" T1 ON T0."OrderNum" = T1."OrderNum"
     WHERE  T1."Status" = 'E'
            AND T1."OrderNum" not in ( select "CreatedBy"
                                     from   "OIVL"
                                     where  "TransType" = 68 )
            and exists ( select "Code"
                         from   "ITT1"
                         where  "Father" = t0."ItemCode"
                                and "Code" in ( select    "ItemCode"
                                              from      "OITM"
                                              where     "InvntItem" = 'N'
                                                        and "EvalSystem" <> 'S' ) );

    If :rowcount > 0 
    then
		INSERT INTO "IVSRUserInteractionRequiredQuery_TempCheckStockResults" VALUES (N'ITT1 with non Inventory & Non Standard Items with no entry in oivl');
        CntScenarios := :CntScenarios + 1;
    end if;
    
----------------Advanced G/L Account Determiniation ---begin-------------------------------------------------
/**
 * check U_Inventory account or sales return account were not defined, 
 * when 'Advance GL Account Determination' is actived.
 *
 * 
*/
SELECT "NewAcctDe" INTO newAcctDe FROM "OADM" ;
SELECT "Category" INTO CurrPeriodCat FROM "OFPR" WHERE "F_RefDate" <= CURRENT_DATE AND "T_RefDate" >= CURRENT_DATE ;

/*are the Accouts defined in company level? */
CmpyhasStockAct := 0;  
CmpyhasReturnAct := 0;
SELECT COUNT("AbsEntry")INTO CmpyhasStockAct FROM "OACP" 
		WHERE "StockAct" is not null AND "StockAct" <> '' AND "PeriodCat" = :CurrPeriodCat;
		
SELECT COUNT("AbsEntry") INTO CmpyhasReturnAct FROM "OACP" 
		WHERE "RturnngAct" is not null AND "RturnngAct" <> '' AND "PeriodCat" = :CurrPeriodCat;


SELECT COUNT("TransType") INTO oivlCnt FROM "OIVL" ;

/*check inventory account*/
oivlInvActOKCnt := :oivlCnt ;
IF ( :newAcctDe = 'Y' AND :CmpyhasStockAct = 0) THEN
	SELECT COUNT(*) INTO oivlInvActOKCnt FROM
	(
		SELECT DISTINCT ("OIVL"."TransSeq")
		FROM "OIVL" INNER JOIN "OITM" ON ("OITM"."ItemCode" = "OIVL"."ItemCode")
			, "OGAR"			
		WHERE (IFNULL("OGAR"."StockAct", '') <> '' )
			AND ("OGAR"."ItemCode" = "OIVL"."ItemCode" OR "OGAR"."ItemCode" = '!^|')
			AND ("OGAR"."WhsCode" = "OIVL"."LocCode" OR "OGAR"."WhsCode" = '!^|')
			AND ("OGAR"."ItmsGrpCod" = "OITM"."ItmsGrpCod" OR "OGAR"."ItmsGrpCod" = -1)
			AND ("OGAR"."GLMethod" = "OITM"."GLPickMeth" OR "OGAR"."GLMethod" = 'A' or "OITM"."GLPickMeth" = 'A')
			AND "OGAR"."PeriodCat" = :CurrPeriodCat
			AND "OGAR"."Active" = 'Y' 
			
	);
END IF;

/*check return account*/
oivlRtnActOKCnt := :oivlCnt ;
IF ( :newAcctDe = 'Y' AND :CmpyhasReturnAct = 0) THEN
	SELECT COUNT(*) INTO oivlRtnActOKCnt FROM
	(
		SELECT DISTINCT ("OIVL"."TransSeq")
		FROM "OIVL" INNER JOIN "OITM" ON ("OITM"."ItemCode" = "OIVL"."ItemCode")
			, "OGAR"			
		WHERE  "OIVL"."TransType" NOT IN ( 16, 14 )
			OR 
			IFNULL("OGAR"."RturnngAct", '') <> '' AND "OIVL"."TransType" IN ( 16, 14 )
			AND ("OGAR"."ItemCode" = "OIVL"."ItemCode" OR "OGAR"."ItemCode" = '!^|')
			AND ("OGAR"."WhsCode" = "OIVL"."LocCode" OR "OGAR"."WhsCode" = '!^|')
			AND ("OGAR"."ItmsGrpCod" = "OITM"."ItmsGrpCod" OR "OGAR"."ItmsGrpCod" = -1)
			AND ("OGAR"."GLMethod" = "OITM"."GLPickMeth" OR "OGAR"."GLMethod" = 'A' or "OITM"."GLPickMeth" = 'A')
			AND "OGAR"."PeriodCat" = :CurrPeriodCat
			AND "OGAR"."Active" = 'Y' 
			
	);
END IF;

IF (:newAcctDe='Y' AND (:oivlCnt > :oivlRtnActOKCnt OR :oivlCnt > :oivlInvActOKCnt) ) THEN
	INSERT INTO "IVSRUserInteractionRequiredQuery_TempCheckStockResults" VALUES (N'Inventory account or sales return account were not defined');
        CntScenarios := :CntScenarios + 1;
END IF;
------------------------------Advanced G/L Account Determiniation ---end---------------------------------------------

select COUNT(T0."ItemCode") INTO rowcount
     FROM   ( SELECT    T0."ItemCode",
                        T0."GLMethod",
                        T1."WhsCode",
                        CASE T0."GLMethod"
                          WHEN 'L' THEN T1."BalInvntAc"
                          WHEN 'W' THEN T2."BalInvntAc"
                          WHEN 'C' THEN T3."BalInvntAc"
                        END "BalInvntAc",
                        CASE T0."GLMethod"
                          WHEN 'L' THEN T1."ReturnAc"
                          WHEN 'W' THEN T2."ReturnAc"
                          WHEN 'C' THEN T3."ReturnAc"
                        END "ReturnAc"
              FROM      "OITM" T0
                        INNER JOIN "OITW" T1 ON T1."ItemCode" = T0."ItemCode"
                        INNER JOIN "OWHS" T2 ON T2."WhsCode" = T1."WhsCode"
                        INNER JOIN "OITB" T3 ON T3."ItmsGrpCod" = T0."ItmsGrpCod"
            ) T0
     WHERE  ( IFNULL(T0."BalInvntAc", '') = ''
              AND ( SELECT  COUNT(1)
                    FROM    "OIVL" T1
                    WHERE   T1."ItemCode" = T0."ItemCode"
                            AND T1."LocCode" = T0."WhsCode"
                  ) > 0
            )
            OR ( IFNULL(T0."ReturnAc", '') = ''
                 AND ( SELECT   COUNT(1)
                       FROM     "OIVL" T1
                       WHERE    T1."ItemCode" = T0."ItemCode"
                                AND T1."LocCode" = T0."WhsCode"
                                AND T1."TransType" IN ( 16, 14 )
                     ) > 0
               );

    If :rowcount > 0 and :newAcctDe <> 'Y'
    then
		INSERT INTO "IVSRUserInteractionRequiredQuery_TempCheckStockResults" VALUES (N'Inventory account or sales return account were not defined');
        CntScenarios := :CntScenarios + 1;
    end if;



--select count(*) INTO rownum from Tables where TABLE_NAME = 'SBO_RSP_IVI_Affected_Item';
--IF :rownum > 0
--      then               
--    drop table "SBO_RSP_IVI_Affected_Item";
--    end if;
--CREATE TABLE "SBO_RSP_IVI_Affected_Item"
--    (
--      "Description" nvarchar(100),
--      "ItemCode" nvarchar(50),
--      "DocNum" nvarchar(100),
--     "DocEntry" int,
--     "Document_Type" nvarchar(100)
--    );
	
	
insert  into "IVSRUserInteractionRequiredQuery_SBO_RSP_IVI_Affected_Item"
        SELECT  distinct
                'Item code in document doesnt exist in Items Master Data' as "Description",
                t1."ItemCode",
                t0."DocNum",              --CAST(t0."DocNum" as nvarchar(100))
                t0."DocEntry",
                'AR Delivery' as "Document_Type"
        from    "DLN1" t1
                inner join "ODLN" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t0."DocType" = 'I'                                      
                                      and t1."ItemType" = 4
                                      and t1."ItemCode" not in (
                                      select    "ItemCode"
                                      from      "OITM" )
        union
        SELECT  distinct
                'Item code in document doesnt exist in Items Master Data' as "Description",
                t1."ItemCode",
                t0."DocNum",              --CAST(t0."DocNum" as nvarchar(100))
                t0."DocEntry",
                'AR Return' as "Document_Type"
        from    "RDN1" t1
                inner join "ORDN" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t0."DocType" = 'I'
                                      and t1."ItemType" = 4
                                      and t1."ItemCode" not in (
                                      select    "ItemCode"
                                      from      "OITM" )
        union
        SELECT  distinct
                'Item code in document doesnt exist in Items Master Data' as "Description",
                t1."ItemCode",
                t0."DocNum",              --CAST(t0."DocNum" as nvarchar(100))
                t0."DocEntry",
                'AR Invoice' as "Document_Type"
        from    "INV1" t1
                inner join "OINV" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t0."DocType" = 'I'
                                      and t1."ItemType" = 4
                                      and t1."ItemCode" not in (
                                      select    "ItemCode"
                                      from      "OITM" )
        
        union
        SELECT  distinct
                'Item code in document doesnt exist in Items Master Data' as "Description",
                t1."ItemCode",
                t0."DocNum",              --CAST(t0."DocNum" as nvarchar(100))
                t0."DocEntry",
                'AR Credit Memo' as "Document_Type"
        from    "RIN1" t1
                inner join "ORIN" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t0."DocType" = 'I'
                                      and t1."ItemType" = 4
                                      and t1."ItemCode" not in (
                                      select    "ItemCode"
                                      from      "OITM" )
        
        union
        SELECT  distinct
                'Item code in document doesnt exist in Items Master Data' as "Description",
                t1."ItemCode",
                t0."DocNum",              --CAST(t0."DocNum" as nvarchar(100))
                t0."DocEntry",
                'AR Correction Invocie' as "Document_Type"
        from    "CSI1" t1
                inner join "OCSI" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t0."DocType" = 'I'
                                      and t1."ItemType" = 4
                                      and t1."ItemCode" not in (
                                      select    "ItemCode"
                                      from      "OITM" )
    
        union
        SELECT  distinct
                'Item code in document doesnt exist in Items Master Data' as "Description",
                t1."ItemCode",
                t0."DocNum",              --CAST(t0."DocNum" as nvarchar(100))
                t0."DocEntry",
                'AR Correction Reversal' as "Document_Type"
        from    "CSV1" t1
                inner join "OCSV" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t0."DocType" = 'I'
                                      and t1."ItemType" = 4
                                      and t1."ItemCode" not in (
                                      select    "ItemCode"
                                      from      "OITM" )
        
        union
        SELECT  distinct
                'Item code in document doesnt exist in Items Master Data' as "Description",
                t1."ItemCode",
                t0."DocNum",              --CAST(t0."DocNum" as nvarchar(100))
                t0."DocEntry",
                'GRPO' as "Document_Type"
        from    "PDN1" t1
                inner join "OPDN" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t0."DocType" = 'I'
                                      and t1."ItemType" = 4
                                      and t1."ItemCode" not in (
                                      select    "ItemCode"
                                      from      "OITM" )
        
        union
        SELECT  distinct
                'Item code in document doesnt exist in Items Master Data' as "Description",
                t1."ItemCode",
                t0."DocNum",              --CAST(t0."DocNum" as nvarchar(100))
                t0."DocEntry",
                'GR' as "Document_Type"
        from    "RPD1" t1
                inner join "ORPD" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t0."DocType" = 'I'
                                      and t1."ItemType" = 4
                                      and t1."ItemCode" not in (
                                      select    "ItemCode"
                                      from      "OITM" )
        
        union
        SELECT  distinct
                'Item code in document doesnt exist in Items Master Data' as "Description",
                t1."ItemCode",
                t0."DocNum",              --CAST(t0."DocNum" as nvarchar(100))
                t0."DocEntry",
                'AP Invoice' as "Document_Type"
        from    "PCH1" t1
                inner join "OPCH" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t0."DocType" = 'I'
                                      and t1."ItemType" = 4
                                      and t1."ItemCode" not in (
                                      select    "ItemCode"
                                      from      "OITM" )
        
        union
        SELECT  distinct
                'Item code in document doesnt exist in Items Master Data' as "Description",
                t1."ItemCode",
                t0."DocNum",              --CAST(t0."DocNum" as nvarchar(100))
                t0."DocEntry",
                'AP Credit Memo' as "Document_Type"
        from    "RPC1" t1
                inner join "ORPC" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t0."DocType" = 'I'
                                      and t1."ItemType" = 4
                                      and t1."ItemCode" not in (
                                      select    "ItemCode"
                                      from      "OITM" )
       
        union
        SELECT  distinct
                'Item code in document doesnt exist in Items Master Data' as "Description",
                t1."ItemCode",
                t0."DocNum",              --CAST(t0."DocNum" as nvarchar(100))
                t0."DocEntry",
                'AP Correction Invocie' as "Document_Type"
        from    "CPI1" t1
                inner join "OCPI" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t0."DocType" = 'I'
                                      and t1."ItemType" = 4
                                      and t1."ItemCode" not in (
                                      select    "ItemCode"
                                      from      "OITM" )
        
        union
        SELECT  distinct
                'Item code in document doesnt exist in Items Master Data' as "Description",
                t1."ItemCode",
                t0."DocNum",              --CAST(t0."DocNum" as nvarchar(100))
                t0."DocEntry",
                'AP Correction Reversal' as "Document_Type"
        from    "CPV1" t1
                inner join "OCPV" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t0."DocType" = 'I'
                                      and t1."ItemType" = 4
                                      and t1."ItemCode" not in (
                                      select    "ItemCode"
                                      from      "OITM" )
        
        union
        SELECT  distinct
                'Item code in document doesnt exist in Items Master Data' as "Description",
                t1."ItemCode",
                t0."DocNum",              --CAST(t0."DocNum" as nvarchar(100))
                t0."DocEntry",
                'Goods Receipt' as "Document_Type"
        from    "IGN1" t1
                inner join "OIGN" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t1."ItemType" = 4
                                      and t1."ItemCode" not in (
                                      select    "ItemCode"
                                      from      "OITM" )
       
        union
        SELECT  distinct
                'Item code in document doesnt exist in Items Master Data' as "Description",
                t1."ItemCode",
                t0."DocNum",              --CAST(t0."DocNum" as nvarchar(100))
                t0."DocEntry",
                'Goods Issue' as "Document_Type"
        from    "IGE1" t1
                inner join "OIGE" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t1."ItemType" = 4
                                      and t1."ItemCode" not in (
                                      select    "ItemCode"
                                      from      "OITM" )
       
        union
        SELECT  distinct
                'Item code in document doesnt exist in Items Master Data' as "Description",
                t1."ItemCode",
                t0."DocNum",              --CAST(t0."DocNum" as nvarchar(100))
                t0."DocEntry",
                'Inventory Transfer' as "Document_Type"
        from    "WTR1" t1
                inner join "OWTR" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t1."ItemType" = 4
                                      and t1."ItemCode" not in (
                                      select    "ItemCode"
                                      from      "OITM" )
        
        union
        SELECT  distinct
                'Item code in document doesnt exist in Items Master Data' as "Description",
                t1."ItemCode",
                t0."SerialNum",              --CAST(t0."DocNum" as nvarchar(100))
                t0."OrderNum",
                'Old Work Order' as "Document_Type"
        from    "WKO1" t1
                inner join "OWKO" t0 on t0."OrderNum" = t1."OrderNum"
                                      and t1."ItemCode" not in (
                                      select    "ItemCode"
                                      from      "OITM" )
        
        union
        SELECT  distinct
                'Item code in document doesnt exist in Items Master Data' as "Description",
                t1."ItemCode",
                t0."DocNum",              --CAST(t0."DocNum" as nvarchar(100))
                t0."DocEntry",
                'Production Order_Components' as "Document_Type"
        from    "WOR1" t1
                inner join "OWOR" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t1."ItemType" = 4
                                      and t1."ItemCode" not in (
                                      select    "ItemCode"
                                      from      "OITM" )
        
        union
        SELECT  distinct
                'Item code in document doesnt exist in Items Master Data' as "Description",
                t0."ItemCode",
                t0."DocNum",              --CAST(t0."DocNum" as nvarchar(100))
                t0."DocEntry",
                'Production Order_BOM' as "Document_Type"
        from    "OWOR" t0
        where   t0."ItemCode" not in ( select "ItemCode"
                                     from   "OITM" )
       
        union
        SELECT  distinct
                'Item code in document doesnt exist in Items Master Data' as "Description",
                t1."ItemCode",
                t1."Ref1",
                t1."MessageID",
                'Stock log' as "Document_Type"
        from    "OILM" t1
        where   t1."ItemCode" not in ( select "ItemCode"
                                     from   "OITM" );
 
select COUNT("ItemCode") INTO rowcount
     from   "IVSRUserInteractionRequiredQuery_SBO_RSP_IVI_Affected_Item";
If :rowcount > 0
    then
		INSERT INTO "IVSRUserInteractionRequiredQuery_TempCheckStockResults" VALUES (N'Item code in document doesnt exist in Items Master Data');
        CntScenarios := :CntScenarios + 1;
    end if;

       
--select count(*) into rownum from Tables where TABLE_NAME = 'SBO_RSP_IVI_Affected_WareHouse';
--IF :rownum > 0
--      then               
--    drop table "SBO_RSP_IVI_Affected_WareHouse";
--    end if;

--CREATE TABLE "SBO_RSP_IVI_Affected_WareHouse"
--    (
--      "Description" nvarchar(100),
--      "ItemCode" nvarchar(50),
--      "DocNum" nvarchar(100),
--      "DocEntry" int,
--      "Document_Type" nvarchar(100),
--      "WareHouse" nvarchar(100)
--    );

insert  into "IVSRUserInteractionRequiredQuery_SBO_RSP_IVI_Affected_WareHouse"
        SELECT  distinct
                'WareHouse in document doesnt exist Warehouse defintion' as "Description",
                t1."ItemCode",
                t0."DocNum",--cast (t0."DocNum" as nvarchar(100)),
                t0."DocEntry",
                'AR Delivery' as "Document_Type",
                t1."WhsCode"
        from    "DLN1" t1
                inner join "ODLN" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t0."DocType" = 'I'
                                      and t1."WhsCode" not in (
                                      select    "WhsCode"
                                      from      "OWHS" )
        union
        SELECT  distinct
                'WareHouse in document doesnt exist Warehouse defintion' as "Description",
                t1."ItemCode",
                t0."DocNum",--cast (t0."DocNum" as nvarchar(100)),
                t0."DocEntry",
                'AR Return' as "Document_Type",
                t1."WhsCode"
        from    "RDN1" t1
                inner join "ORDN" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t0."DocType" = 'I'
                                      and t1."WhsCode" not in (
                                      select    "WhsCode"
                                      from      "OWHS" )
        
        union
        SELECT  distinct
                'WareHouse in document doesnt exist Warehouse defintion' as "Description",
                t1."ItemCode",
                t0."DocNum",--cast (t0."DocNum" as nvarchar(100)),
                t0."DocEntry",
                'AR Invoice' as "Document_Type",
                t1."WhsCode"
        from    "INV1" t1
                inner join "OINV" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t0."DocType" = 'I'
                                      and t1."WhsCode" not in (
                                      select    "WhsCode"
                                      from      "OWHS" )
        
        union
        SELECT  distinct
                'WareHouse in document doesnt exist Warehouse defintion' as "Description",
                t1."ItemCode",
                t0."DocNum",--cast (t0."DocNum" as nvarchar(100)),
                t0."DocEntry",
                'AR Credit Memo' as "Document_Type",
                t1."WhsCode"
        from    "RIN1" t1
                inner join "ORIN" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t0."DocType" = 'I'
                                      and t1."WhsCode" not in (
                                      select    "WhsCode"
                                      from      "OWHS" )
       
        union
        SELECT  distinct
                'WareHouse in document doesnt exist Warehouse defintion' as "Description",
                t1."ItemCode",
                t0."DocNum",--cast (t0."DocNum" as nvarchar(100)),
                t0."DocEntry",
                'AR Correction Invocie' as "Document_Type",
                t1."WhsCode"
        from    "CSI1" t1
                inner join "OCSI" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t0."DocType" = 'I'
                                      and t1."WhsCode" not in (
                                      select    "WhsCode"
                                      from      "OWHS" )
        
        union
        SELECT  distinct
                'WareHouse in document doesnt exist Warehouse defintion' as "Description",
                t1."ItemCode",
                t0."DocNum",--cast (t0."DocNum" as nvarchar(100)),
                t0."DocEntry",
                'AR Correction Reversal' as "Document_Type",
                t1."WhsCode"
        from    "CSV1" t1
                inner join "OCSV" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t0."DocType" = 'I'
                                      and t1."WhsCode" not in (
                                      select    "WhsCode"
                                      from      "OWHS" )
      
        union
        SELECT  distinct
                'WareHouse in document doesnt exist Warehouse defintion' as "Description",
                t1."ItemCode",
                t0."DocNum",--cast (t0."DocNum" as nvarchar(100)),
                t0."DocEntry",
                'GRPO' as "Document_Type",
                t1."WhsCode"
        from    "PDN1" t1
                inner join "OPDN" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t0."DocType" = 'I'
                                      and t1."WhsCode" not in (
                                      select    "WhsCode"
                                      from      "OWHS" )
        
        union
        SELECT  distinct
                'WareHouse in document doesnt exist Warehouse defintion' as "Description",
                t1."ItemCode",
                t0."DocNum",--cast (t0."DocNum" as nvarchar(100)),
                t0."DocEntry",
                'GR' as "Document_Type",
                t1."WhsCode"
        from    "RPD1" t1
                inner join "ORPD" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t0."DocType" = 'I'
                                      and t1."WhsCode" not in (
                                      select    "WhsCode"
                                      from      "OWHS" )
     
        union
        SELECT  distinct
                'WareHouse in document doesnt exist Warehouse defintion' as "Description",
                t1."ItemCode",
                t0."DocNum",--cast (t0."DocNum" as nvarchar(100)),
                t0."DocEntry",
                'AP Invoice' as "Document_Type",
                t1."WhsCode"
        from    "PCH1" t1
                inner join "OPCH" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t0."DocType" = 'I'
                                      and t1."WhsCode" not in (
                                      select    "WhsCode"
                                      from      "OWHS" )
      
        union
        SELECT  distinct
                'WareHouse in document doesnt exist Warehouse defintion' as "Description",
                t1."ItemCode",
                t0."DocNum",--cast (t0."DocNum" as nvarchar(100)),
                t0."DocEntry",
                'AP Credit Memo' as "Document_Type",
                t1."WhsCode"
        from    "RPC1" t1
                inner join "ORPC" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t0."DocType" = 'I'
                                      and t1."WhsCode" not in (
                                      select    "WhsCode"
                                      from      "OWHS" )
        
        union
        SELECT  distinct
                'WareHouse in document doesnt exist Warehouse defintion' as "Description",
                t1."ItemCode",
                t0."DocNum",--cast (t0."DocNum" as nvarchar(100)),
                t0."DocEntry",
                'AP Correction Invocie' as "Document_Type",
                t1."WhsCode"
        from    "CPI1" t1
                inner join "OCPI" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t0."DocType" = 'I'
                                      and t1."WhsCode" not in (
                                      select    "WhsCode"
                                      from      "OWHS" )
        
        union
        SELECT  distinct
                'WareHouse in document doesnt exist Warehouse defintion' as "Description",
                t1."ItemCode",
                t0."DocNum",--cast (t0."DocNum" as nvarchar(100)),
                t0."DocEntry",
                'AP Correction Reversal' as "Document_Type",
                t1."WhsCode"
        from    "CPV1" t1
                inner join "OCPV" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t0."DocType" = 'I'
                                      and t1."WhsCode" not in (
                                      select    "WhsCode"
                                      from      "OWHS" )
        
        union
        SELECT  distinct
                'WareHouse in document doesnt exist Warehouse defintion' as "Description",
                t1."ItemCode",
                t0."DocNum",--cast (t0."DocNum" as nvarchar(100)),
                t0."DocEntry",
                'Goods Receipt' as "Document_Type",
                t1."WhsCode"
        from    "IGN1" t1
                inner join "OIGN" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t1."WhsCode" not in (
                                      select    "WhsCode"
                                      from      "OWHS" )
        
        union
        SELECT  distinct
                'WareHouse in document doesnt exist Warehouse defintion' as "Description",
                t1."ItemCode",
                t0."DocNum",--cast (t0."DocNum" as nvarchar(100)),
                t0."DocEntry",
                'Goods Issue' as "Document_Type",
                t1."WhsCode"
        from    "IGE1" t1
                inner join "OIGE" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t1."WhsCode" not in (
                                      select    "WhsCode"
                                      from      "OWHS" )
        
        union
        SELECT  distinct
                'WareHouse in document doesnt exist Warehouse defintion' as "Description",
                '' as "ItemCode",
                t0."DocNum",
                t0."DocEntry",
                'Inventory Transfer' as "Document_Type",
                t0."Filler"
        from    "OWTR" t0
        where   t0."Filler" not in ( select   "WhsCode"
                                   from     "OWHS" )
    
        union
        SELECT  distinct
                'WareHouse in document doesnt exist Warehouse defintion' as "Description",
                t1."ItemCode",
                t0."DocNum",--cast (t0."DocNum" as nvarchar(100)),
                t0."DocEntry",
                'Inventory Transfer' as "Document_Type",
                t1."WhsCode"
        from    "WTR1" t1
                inner join "OWTR" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t1."WhsCode" not in (
                                      select    "WhsCode"
                                      from      "OWHS" )
        
        union
        SELECT  distinct
                'WareHouse in document doesnt exist Warehouse defintion' as "Description",
                t1."ItemCode",
                t0."SerialNum",--cast (t0."DocNum" as nvarchar(100)),
                t0."OrderNum",
                'Old Work Order' as "Document_Type",
                t1."WhsCode"
        from    "WKO1" t1
                inner join "OWKO" t0 on t0."OrderNum" = t1."OrderNum"
                                      and t1."WhsCode" not in (
                                      select    "WhsCode"
                                      from      "OWHS" )
       
        union
        SELECT  distinct
                'WareHouse in document doesnt exist Warehouse defintion' as "Description",
                t1."ItemCode",
                t0."DocNum",--cast (t0."DocNum" as nvarchar(100)),
                t0."DocEntry",
                'Production Order_Components' as "Document_Type",
                t1."wareHouse"
        from    "WOR1" t1
                inner join "OWOR" t0 on t0."DocEntry" = t1."DocEntry"
                                      and t1."wareHouse" not in (
                                      select    "WhsCode"
                                      from      "OWHS" )
    
        union
        SELECT  distinct
                'WareHouse in document doesnt exist Warehouse defintion' as "Description",
                t0."ItemCode",
                t0."DocNum",--cast (t0."DocNum" as nvarchar(100)),
                t0."DocEntry",
                'Production Order_BOM' as "Document_Type",
                t0."Warehouse"
        from    "OWOR" t0
        where   t0."Warehouse" not in ( select    "Warehouse"
                                      from      "OWHS" )
        
        union
       
        SELECT distinct
                'WareHouse in document doesnt exist Warehouse defintion' as "Description",
                t1."ItemCode",
                t1."Ref1",
                t1."MessageID",
                'Stock log' as "Document_Type",
                t1."LocCode"
        from    "OILM" t1
        where   t1."LocCode" not in ( select  "WhsCode"
                                    from    "OWHS" );

select COUNT("ItemCode") INTO rowcount
     from   "IVSRUserInteractionRequiredQuery_SBO_RSP_IVI_Affected_WareHouse";
If :rowcount > 0 
    then
	INSERT INTO "IVSRUserInteractionRequiredQuery_TempCheckStockResults" VALUES (N'WareHouse in document doesnt exist Warehouse defintion');
        CntScenarios := :CntScenarios + 1;
    end if;



 select COUNT("DocDate") INTO rowcount
     from   "OILM" t0
            inner join "ORTT" t1 on t0."DocDate" = t1."RateDate"
                                  and t0."PriceCurr" = t1."Currency"
                                  and IFNULL("PriceCurr", '') <> ''
                                  and IFNULL("PriceCurr", '') <> :Main_Currency
                                  and IFNULL(t1."Rate", 0) = 0;
If :rowcount > 0  
    then
	INSERT INTO "IVSRUserInteractionRequiredQuery_TempCheckStockResults" VALUES (N'Price Rate is missing in exchange rate table');
       CntScenarios := :CntScenarios + 1;
    end if;


if :Main_Currency <> :Sys_Currency 
  then  
  select COUNT(t0."RefDate") INTO rowcount
       from   "OJDT" t0 
        where  t0."TransType" not in (-3,-5,10000046) 
               and not exists (select t1."RateDate" 
                               from   "ORTT" t1 
                               where  t0."RefDate" = t1."RateDate" 
                                      and :Sys_Currency = t1."Currency");   
    If :rowcount > 0  
    then
	INSERT INTO "IVSRUserInteractionRequiredQuery_TempCheckStockResults" VALUES (N'Price Rate is missing in exchange rate table');
       CntScenarios := :CntScenarios + 1;
    end if;
     
  end if;



Select COUNT(T0."ItemCode") INTO rowcount
     from   "OITM" T0
     where  t0."InvntItem" = 'Y'
            and t0."EvalSystem" is null;
if :rowcount > 0 
    then
		INSERT INTO "IVSRUserInteractionRequiredQuery_TempCheckStockResults" VALUES (N'No valuation method is defined for item');         
       CntScenarios := :CntScenarios + 1;
    end if;

select COUNT("ItemCode") INTO rowcount
     from   "OITM"
     where  "TreeType" = 'P'
            and "ItemCode" not in ( select    "Code"
                                  from      "OITT" )
            and ( "ItemCode" in ( select  "ItemCode"
                                from    "IGN1"
                                where   "BaseType" = 202 )
                  or "ItemCode" in ( select   "ItemCode"
                                   from     "IGE1"
                                   where    "BaseType" = 202 )
                );
if :rowcount > 0 
    then
		INSERT INTO "IVSRUserInteractionRequiredQuery_TempCheckStockResults" VALUES (N'BOM item has no BOM defintion');         
       CntScenarios := :CntScenarios + 1;
    end if;
	

Select COUNT(T1."ItemCode") into rowcount
	from
     (select o."ItemCode" from OITM o where o."InvntItem" = 'N' and (ifnull(o."OnOrder",0) <> 0 or
     ifnull(o."IsCommited",0) <> 0 or ifnull(o."OnHand",0) <> 0 or
     ifnull(o."Consig",0) <> 0 or IfNULL(o."StockValue",0) <> 0)
	 union all
     select i."ItemCode"
     from OITW i INNER JOIN OITM o on o."ItemCode"= i."ItemCode"
	 where o."InvntItem" = 'N' and (ifnull(i."OnOrder",0) <> 0 or ifnull(i."IsCommited",0) <> 0 or
	 ifnull(i."OnHand",0) <> 0 or ifnull(i."Consig",0) <> 0 or ifnull(i."StockValue",0) <> 0)) T1;
	 
if :rowcount > 0 
	then
		INSERT INTO "IVSRUserInteractionRequiredQuery_TempCheckStockResults" VALUES (N'Non Inventory Item with non zero quantities or stockvalue - RSP task 1177631');
	end if;
  
select * from "IVSRUserInteractionRequiredQuery_TempCheckStockResults";

delete from "IVSRUserInteractionRequiredQuery_TempCheckStockResults";
delete from "IVSRUserInteractionRequiredQuery_SBO_RSP_IVI_Affected_Item";
delete from "IVSRUserInteractionRequiredQuery_SBO_RSP_IVI_Affected_WareHouse";

END;


