-- B1 DEPENDS: AFTER:SP:_TmSp_BootCreateGlobalTempTables2 AFTER:PT:PROCESS_END AFTER:SP:_TmSp_ValidateSpParam

Create Procedure CRSP_INVENTORY_TURNOVER_RATE(
	In FromDate NVarChar(20), 
	In ToDate nVarChar(20), 
	In Items NCLOB
	)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER 
As 
	IsExists INTEGER := 0;
BEGIN
	call _TmSp_ValidateSpParam(FromDate);
	call _TmSp_ValidateSpParam(ToDate);
	call _TmSp_ValidateSpParam(Items);
	
	-- Delete existing temp table if it exists
	SELECT COUNT(*) INTO IsExists 
	FROM M_TEMPORARY_TABLES
	WHERE TABLE_NAME = '#CRSP_INVENTORY_ITEMS_LOCAL' AND SCHEMA_NAME = CURRENT_SCHEMA and connection_id = current_connection; 
	
	IF :IsExists > 0 THEN
		DROP TABLE #CRSP_INVENTORY_ITEMS_LOCAL;
	END IF;
	
	CREATE LOCAL TEMPORARY COLUMN TABLE #CRSP_INVENTORY_ITEMS_LOCAL ("ItemCode" NVARCHAR(50));
	
	-- Insert selected result to temp table created above
	Exec ('INSERT INTO #CRSP_INVENTORY_ITEMS_LOCAL SELECT T0."ItemCode" FROM OITM T0 Where ' || :items);
	
	select 
			T1."LocType" as LocType,
			T1."LocCode" as LocCode,
			T1."ItemCode" as ItemCode,
			T1."CloseStock" as CloseStock,
			T4."OpenStock"as OpenStock,
			T5."IssueQty" as IssueQty,
			T6."LastDate" as LastDate,
			T2."OnHand" as OnHandTotal,
			T6."OnHand" as OnHand,
			T6."MinStock" as MinStock,
			T6."WhsCode" as WhsCode,
			T3."WhsName" as WhsName,
			T2."ItmsGrpCod" as ItmsGrpCod,
			T2."ItemName" as ItemName,
			T2."MinLevel" as MinLevel,
			T2."ByWh" as ByWh,
			T2."InvntryUom" as InvntryUom,
			T2."LeadTime" as LeadTime,
			T2."ItmsGrpNam" as ItmsGrpNam
	from
	(
		Select	"LocType", "LocCode", "ItemCode", Sum("InQty" - "OutQty") as "CloseStock" 
		From "OIVL"
		Where TO_DATE("DocDate") <= TO_DATE(:ToDate)  
		Group by "LocType", "LocCode", "ItemCode"
	) T1 /*CloseStock*/
	Join
	(
		SELECT	T0."ItemCode",
				T0."ItmsGrpCod", 
				T0."ItemName", 
				T0."OnHand",
				T0."MinLevel", 
				T0."ByWh", 
				T0."InvntryUom", 
				T0."LeadTime",
				T1."ItmsGrpNam"
		From "OITM"	T0
		Join "OITB"	T1 On	T0."ItmsGrpCod" = T1."ItmsGrpCod"
		Join #CRSP_INVENTORY_ITEMS_LOCAL T2 On T0."ItemCode" = T2."ItemCode"		
	) T2 /*OITM & OITB*/
		on	T1."ItemCode" = T2."ItemCode"
	Join
	(
		SELECT T0."WhsCode", T0."WhsName" 
		From "OWHS" T0
	) T3 /*OWHS*/ on	T1."LocCode" = T3."WhsCode"
	left join
	(
		Select	"LocType", "LocCode", "ItemCode", Sum("InQty" - "OutQty") as "OpenStock" 
		From "OIVL"
		Where TO_DATE("DocDate") < TO_DATE(:FromDate) 
		Group by "LocType", "LocCode", "ItemCode"
	) T4 /*OpenStock*/
	on	T1."LocType" = T4."LocType"
	and	T1."LocCode" = T4."LocCode"
	and	T1."ItemCode" = T4."ItemCode"
	left join
	(
		Select	"LocType", "LocCode", "ItemCode", Sum("OutQty") as "IssueQty" 
		From "OIVL"
		Where TO_DATE("DocDate") Between TO_DATE(:FromDate) And TO_DATE(:ToDate)
		Group by "LocType", "LocCode", "ItemCode"
	) T5 /*IssueQty*/
	on	T1."LocType" = T5."LocType"
	and	T1."LocCode" = T5."LocCode"
	and	T1."ItemCode" = T5."ItemCode"
	left join
	(
		select T1."LocType", T1."LocCode", T1."ItemCode", T1."LastDate", 
			   T2."WhsCode", T2."OnHand", T2."MinStock" from
		(
			Select	"LocType", "LocCode", "ItemCode", max(TO_DATE("DocDate")) as "LastDate"
			From "OIVL"
			Where "InQty" > 0 
			Group By "LocType", "LocCode", "ItemCode"
		) T1 /*LastDate*/
		join
		(
			SELECT "ItemCode", "WhsCode", "OnHand", "MinStock" From "OITW"
		) T2 /*OITW*/
		on	T1."ItemCode" = T2."ItemCode" and T1."LocCode" = T2."WhsCode"
	) T6
	on	T1."LocType" = T6."LocType"
	and	T1."LocCode" = T6."LocCode"
	and	T1."ItemCode" = T6."ItemCode";
	
	--DROP TABLE #CRSP_INVENTORY_ITEMS_LOCAL;	/* cause client of rev48 disconnected */
	
 END


