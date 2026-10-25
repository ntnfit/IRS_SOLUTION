-- B1 DEPENDS: BEFORE:PT:PROCESS_START

create procedure _TmSp_UpdateSubLineNumOILMAndOIVL
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

as
--	Version int;
--	strSql varchar(4096);
--	cond int;
begin
--  This upgrade is not covered by B1H version
--  B1H covers upgrade from version 882065
--
	exec('select "Version" from CINF');
/*
-- 1) prepare the related records  
--   select all the records to the temp talbe which is related to assembly bom
	select "Version" into Version  from cinf;
	
	select count(*) into cond from "PUBLIC"."M_TEMPORARY_TABLES" where  table_name = '#tempAsmOINM';
	if(:cond > 0) then 
	   drop table "#tempAsmOINM";
	end if;
	
	create local TEMPORARY TABLE "#tempAsmOINM"(
		"ItemCode" NVARCHAR (50), 
		"Warehouse" NVARCHAR (8), 
		"TransType" INTEGER, 
		"CreatedBy" INTEGER, 
		"DocLineNum" SMALLINT, 
		"CostMethod" NVARCHAR (1), 
		"SubLineNum" INTEGER
	);
	DELETE FROM "#tempAsmOINM" WHERE 1=1;
	
	select count(*) into cond from "PUBLIC"."M_TEMPORARY_TABLES" where  table_name = '#tmp_OILMAllParents';
	if(:cond > 0) then 
	   drop table "#tmp_OILMAllParents";
	end if;
	
	create local TEMPORARY TABLE "#tmp_OILMAllParents"(
		"MessageID" INTEGER not null,
		"ItemCode" NVARCHAR (50),
		"LocCode" NVARCHAR (8),
		"TransType" INTEGER,
		"DocEntry" INTEGER,
		"DocLineNum" SMALLINT,
		"NewSubLineNum" INTEGER
	);
	DELETE FROM "#tmp_OILMAllParents" WHERE 1=1;
	
	if :Version < 880000  then-- upgrade before 8.8 
--		strSQL := 'select "ItemCode", "Warehouse", "TransType", "CreatedBy", "DocLineNum", "CostMethod", "SubLineNum" into "#tempAsmOINM" from OINM where "SubLineNum" >= 0 and "TransType" <> 162 AND ("TransType" not in (165,166) or "InQty" <> "OutQty")'
		strSQL := 'insert into "#tempAsmOINM" select "ItemCode", "Warehouse", "TransType", "CreatedBy", "DocLineNum", "CostMethod", "SubLineNum" from OINM where "SubLineNum" >= 0 and "TransType" <> 162 AND ("TransType" not in (165,166) or "InQty" <> "OutQty")';
	else
--	select * from OILM;
		strSQL := 'insert into "#tempAsmOINM" select T0."ItemCode", T0."LocCode" as "Warehouse", T0."TransType", T0."DocEntry" as "CreatedBy", T0."DocLineNum", T1."CostMethod", T0."SubLineNum" from OILM T0 inner join OIVL T1 on T0."MessageID" = T1."MessageID" where T0."SubLineNum" >= 0 and T0."TransType" <> 162 union all select T0."ItemCode", T0."LocCode" as "Warehouse", T0."TransType", T0."DocEntry" as "CreatedBy", T0."DocLineNum", T2."CostMethod", T0."SubLineNum" from OILM T0 inner join OILM T1 on T0."BaseType" = T1."TransType" and T0."BaseAbsEnt" = T1."DocEntry" and T0."BaseLine" = T1."DocLineNum" and T0."ItemCode" = T1."ItemCode" and T0."LocCode" = T1."LocCode" and T0."TransType" = 13 and T0."BaseType" = 15 inner join OIVL T2 on T1."MessageID" = T2."MessageID" where T0."SubLineNum" >= 0 ';
	end if;
	
	select count(*) into cond from "PUBLIC"."M_TEMPORARY_TABLES" where  table_name = '#tmp_OILMAllParents_grouped';
	if(:cond > 0) then 
	   drop table "#tmp_OILMAllParents_grouped";
	end if;
	
	create local TEMPORARY TABLE "#tmp_OILMAllParents_grouped"(
		"ItemCode" NVARCHAR (50),
		"TransType" INTEGER,
		"DocEntry" INTEGER,
		"DocLineNum" SMALLINT, 
		"MinSubLineNum" INTEGER
	);
	DELETE FROM "#tmp_OILMAllParents_grouped" WHERE 1=1;

	select count(*) into cond from "PUBLIC"."M_TEMPORARY_TABLES" where  table_name = '#tmp_OILMSonsWHSNotChanged';
	if(:cond > 0) then 
	   drop table "#tmp_OILMSonsWHSNotChanged";
	end if;
	
	create local TEMPORARY COLUMN TABLE "#tmp_OILMSonsWHSNotChanged"(
		"ItemCode" NVARCHAR (50),
		"TransType" INTEGER,
		"DocEntry" INTEGER,
		"DocLineNum" SMALLINT,
		"BaseType" INTEGER,
		"BaseAbsEnt" INTEGER,
		"BaseLine" SMALLINT, 
		"MessageID" INTEGER not null, 
		"BSubLineNo" INTEGER
	);
	TRUNCATE TABLE "#tmp_OILMSonsWHSNotChanged";
	
	select count(*) into cond from "PUBLIC"."M_TEMPORARY_TABLES" where  table_name = '#tmp_OILMSonsWHSChanged';
	if(:cond > 0) then 
	   drop table "#tmp_OILMSonsWHSChanged";
	end if;
	
	create local TEMPORARY TABLE "#tmp_OILMSonsWHSChanged"(
		"ItemCode" NVARCHAR (50),
		"TransType" INTEGER,
		"DocEntry" INTEGER,
		"DocLineNum" SMALLINT, 
		"BaseType" INTEGER, 
		"BaseAbsEnt" INTEGER, 
		"BaseLine" SMALLINT, 
		"MessageID" INTEGER not null
	);
	DELETE FROM "#tmp_OILMSonsWHSChanged" WHERE 1=1;

-- [NOT FIFO]
-- HANDLE PARENTS
-- 2) Get all parents with rank
-- Create temp tables of all parents 
	exec(:strSQL);
	strSQL := ' Insert into "#tmp_OILMAllParents" select P1."MessageID",P1."ItemCode",P1."LocCode",P1."TransType",P1."DocEntry",P1."DocLineNum",P1."SubLineNum" As "NewSubLineNum" from OILM P1 INNER JOIN "#tempAsmOINM" P2 On P1."ItemCode" = P2."ItemCode" AND P1."LocCode" = P2."Warehouse" AND P1."TransType" = P2."TransType" AND P1."DocEntry" = P2."CreatedBy" AND P1."DocLineNum" = P2."DocLineNum" AND P2."CostMethod" <> N''F''';
	exec(:strSQL);				
	strSQL :='insert into "#tmp_OILMAllParents_grouped" select P1."ItemCode",P1."TransType",P1."DocEntry",P1."DocLineNum", MIN(P1."NewSubLineNum") As '||'"MinSubLineNum"'||' from "#tmp_OILMAllParents" P1 group by P1."TransType", P1."DocEntry", P1."DocLineNum", P1."ItemCode"';
	exec(:strSQL);
	strSQL := 'insert into "#tmp_OILMSonsWHSNotChanged" select S1."ItemCode",S1."TransType",S1."DocEntry",S1."DocLineNum",S1."BaseType",S1."BaseAbsEnt", S1."BaseLine", S1."MessageID", O2."SubLineNum" As '||'"BSubLineNo"'||' from OILM S1 INNER JOIN "#tempAsmOINM" O1 On	S1."ItemCode" = O1."ItemCode" AND S1."LocCode" = O1."Warehouse" AND S1."TransType" = O1."TransType" AND S1."DocEntry" = O1."CreatedBy" AND S1."DocLineNum" = O1."DocLineNum" AND O1."CostMethod" <> N''F'' INNER JOIN "#tempAsmOINM" O2 ON	S1."BaseType" = O2."TransType" AND S1."BaseLine" = O2."DocLineNum" AND  S1."ItemCode" = O2."ItemCode" AND	S1."LocCode" = O2."Warehouse" AND S1."BaseAbsEnt" = O2."CreatedBy" where S1."BaseType"<>-1 AND S1."BaseType" IS NOT NULL AND (S1."TransType" <> 15 OR S1."BaseType" <> 165) AND S1."BSubLineNo" = -1';	
	exec(:strSQL);

	-- strSQL :='update OILM set "BSubLineNo" = S2."BSubLineNo" from OILM S1 inner join "#tmp_OILMSonsWHSNotChanged" S2 On S1."MessageID" = S2."MessageID"';
	strSQL := '
		update OILM S1
		set "BSubLineNo" = (select "BSubLineNo" from "#tmp_OILMSonsWHSNotChanged" S2 where S1."MessageID" = S2."MessageID")
		where exists (select 1 from "#tmp_OILMSonsWHSNotChanged" S2 where S1."MessageID" = S2."MessageID")
	';
	exec(:strSQL);

	strSQL :='insert into "#tmp_OILMSonsWHSChanged" select S1."ItemCode",S1."TransType",S1."DocEntry",S1."DocLineNum", S1."BaseType", S1."BaseAbsEnt", S1."BaseLine", S1."MessageID" from OILM S1 INNER JOIN "#tempAsmOINM" O1 On S1."ItemCode" = O1."ItemCode" AND  S1."TransType" = O1."TransType" AND  S1."DocEntry" = O1."CreatedBy" AND  S1."DocLineNum" = O1."DocLineNum" AND  O1."CostMethod" <> N''F'' AND  S1."BaseType"<>-1 where (S1."TransType" <> 15 OR S1."BaseType" <> 165) AND S1."BSubLineNo" = -1';
	exec(:strSQL);

	-- strSQL :='UPDATE OILM set "BSubLineNo" = P1."MinSubLineNum" from OILM S2 inner join "#tmp_OILMSonsWHSChanged" S1 ON S2."MessageID" = S1."MessageID" inner join "#tmp_OILMAllParents_grouped" P1 ON S1."BaseType" = P1."TransType" AND  S1."BaseAbsEnt" = P1."DocEntry" AND S1."BaseLine" = P1."DocLineNum" AND S1."ItemCode"= P1."ItemCode"';
	strSQL := '
		UPDATE OILM S2 
		set "BSubLineNo" = (
			select P1."MinSubLineNum" 
			from "#tmp_OILMSonsWHSChanged" S1 
			inner join "#tmp_OILMAllParents_grouped" P1 ON S1."BaseType" = P1."TransType" AND S1."BaseAbsEnt" = P1."DocEntry" 
				AND S1."BaseLine" = P1."DocLineNum" AND S1."ItemCode"= P1."ItemCode" 
			where S2."MessageID" = S1."MessageID")
		where exists (
			select 1 
			from "#tmp_OILMSonsWHSChanged" S1 
			inner join "#tmp_OILMAllParents_grouped" P1 ON S1."BaseType" = P1."TransType" AND S1."BaseAbsEnt" = P1."DocEntry" 
				AND S1."BaseLine" = P1."DocLineNum" AND S1."ItemCode"= P1."ItemCode" 
			where S2."MessageID" = S1."MessageID") 
	';
	exec(:strSQL);
	
	-- strSQL :='UPDATE OILM Set "BSubLineNo" = S1."SubLineNum" from OILM S1 INNER JOIN "#tempAsmOINM" O1 On S1."ItemCode" = O1."ItemCode" AND  S1."TransType" = O1."TransType" AND S1."DocEntry" = O1."CreatedBy" AND  S1."DocLineNum" = O1."DocLineNum" AND  O1."CostMethod" = N''F'' AND ((S1."BaseType"<>-1 AND S1."BaseType" IS NOT NULL) AND (S1."TransType" <> 15 OR S1."BaseType" <> 165) AND ( S1."BaseAbsEnt" IN ( select "CreatedBy" FROM "#tempAsmOINM" P3 where S1."BaseType" = P3."TransType" AND S1."BaseAbsEnt" = P3."CreatedBy" AND S1."BaseLine" = P3."DocLineNum" AND S1."ItemCode" = P3."ItemCode")))';
	strSQL := '
		UPDATE OILM S1 
		Set "BSubLineNo" = "SubLineNum" 
		where exists (
			select 1 
			from "#tempAsmOINM" O1 
			where S1."ItemCode" = O1."ItemCode" AND S1."TransType" = O1."TransType" 
				AND S1."DocEntry" = O1."CreatedBy" AND S1."DocLineNum" = O1."DocLineNum" AND O1."CostMethod" = N''F'' 
				AND ((S1."BaseType"<>-1 AND S1."BaseType" IS NOT NULL) AND (S1."TransType" <> 15 OR S1."BaseType" <> 165)
					AND ( S1."BaseAbsEnt" IN (
						select "CreatedBy" 
						FROM "#tempAsmOINM" P3 
						where S1."BaseType" = P3."TransType" AND S1."BaseAbsEnt" = P3."CreatedBy" 
							AND S1."BaseLine" = P3."DocLineNum" AND S1."ItemCode" = P3."ItemCode"))) )
	';
	exec(:strSQL);

	-- strSQL :='UPDATE OIVL SET "BSubLineNo" = O2."BSubLineNo" FROM OIVL O1, OILM O2 WHERE O2."BSubLineNo" >= 0 AND O1."MessageID" = O2."MessageID"';
	strSQL :='
		UPDATE OIVL O1 
		SET "BSubLineNo" = (select O2."BSubLineNo" from OILM O2 where O2."BSubLineNo" >= 0 AND O1."MessageID" = O2."MessageID") 
		WHERE exists (select 1 from OILM O2 where O2."BSubLineNo" >= 0 AND O1."MessageID" = O2."MessageID")
	';
	exec(:strSQL);

	strSQL :='drop table "#tempAsmOINM"';
	exec(:strSQL);
	strSQL :='drop table "#tmp_OILMAllParents"';	
	exec(:strSQL);
	strSQL :='drop table "#tmp_OILMAllParents_grouped"';	
	exec(:strSQL);
	strSQL :='drop table "#tmp_OILMSonsWHSNotChanged"';
	exec(:strSQL);
	strSQL :=' drop table "#tmp_OILMSonsWHSChanged"';
	exec(:strSQL);
*/
end;


