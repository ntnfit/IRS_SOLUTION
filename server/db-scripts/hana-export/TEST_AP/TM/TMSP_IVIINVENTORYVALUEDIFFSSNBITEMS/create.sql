-- B1 DEPENDS: AFTER:SP:_TmSp_BootCreateGlobalTempTables AFTER:PT:PROCESS_END

/*HANA Inventory Value Differences - SnB items_1.0 */  

-- B1 DEPENDS: AFTER:SP:_TmSp_BootCreateGlobalTempTables AFTER:PT:PROCESS_END
-- DROP PROCEDURE TmSp_IVIInventoryValueDiffsSNBItems;

CREATE PROCEDURE TmSp_IVIInventoryValueDiffsSNBItems(out retCode INT)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
as  
	Perpetual_Inventory nvarchar(1);  
	isPricePerWH nvarchar(1);
	IsExistBvlBaseTMP integer;
	IsExistBvlTMP integer;
	IsExistIvlTMP integer;
	IsExistItwTMP integer;
	temp_var integer;

BEGIN  
	retCode :=  0;

	select "ContInvnt" into Perpetual_Inventory from "OADM";
	select "PriceSys" into isPricePerWH from "OADM";
	
	select count(*) into IsExistBvlBaseTMP from "PUBLIC"."M_TEMPORARY_TABLES" where schema_name = CURRENT_SCHEMA and table_name='#UBVL_Aggregation_Base';
	IF (:IsExistBvlBaseTMP > 0 ) then
		DROP TABLE "#UBVL_Aggregation_Base";
	end if; 
  
	select count(*) into IsExistBvlTMP from "PUBLIC"."M_TEMPORARY_TABLES" where schema_name = CURRENT_SCHEMA and table_name='#UBVL_Aggregation';
	IF (:IsExistBvlTMP > 0 ) then
		DROP TABLE "#UBVL_Aggregation";
	end if; 
    
	select count(*) into IsExistItwTMP from "PUBLIC"."M_TEMPORARY_TABLES" where schema_name = CURRENT_SCHEMA and table_name='#UITW_Aggregation';
	IF (:IsExistItwTMP  > 0 ) then
		DROP TABLE "#UITW_Aggregation";
	end if; 
    
	if :Perpetual_Inventory = 'Y' then
		-- TEMP TABLES
		CREATE LOCAL TEMPORARY COLUMN TABLE "#UBVL_Aggregation_Base" AS 
		(SELECT T0."ItemCode", T0."MdAbsEntry", T0."ManagedBy", SUM(T0."InvValue") AS "InvValue", MAX(T0."AbsEntry") AS "maxEntry" 
		 FROM UBVL T0 
		 WHERE "MdAbsEntry" IS NOT NULL 
		 GROUP BY T0."ItemCode", T0."MdAbsEntry", T0."ManagedBy");

		CREATE LOCAL TEMPORARY COLUMN TABLE "#UBVL_Aggregation" AS 
 		(SELECT T0.*, T1."AccQty" - t1."AccNegQ" AS "BVLQty" FROM "#UBVL_Aggregation_Base" T0, UBVL T1 WHERE T1."AbsEntry" = "maxEntry");
 		
-- UITM.StockValue  <=>  UBVL.InvValue
		if :isPricePerWH = 'Y' then
			-- TEMP TABLE BASED ON UITW
			CREATE LOCAL TEMPORARY COLUMN TABLE "#UITW_Aggregation" AS 
			(SELECT T0."ItemCode", SUM(T0."StockValue") AS "ITW_StockValue" FROM UITW T0 GROUP BY T0."ItemCode");


			select count(*) INTO temp_var from (
			SELECT "#UITW_Aggregation"."ItemCode", SUM("InvValue")
			FROM "#UITW_Aggregation", "#UBVL_Aggregation"
			WHERE "#UITW_Aggregation"."ItemCode" = "#UBVL_Aggregation"."ItemCode" 
			GROUP BY "#UITW_Aggregation"."ItemCode", "ITW_StockValue"
			HAVING "ITW_StockValue" <> SUM("InvValue"));
			
			DROP TABLE "#UITW_Aggregation";

		else 
			select count(*) INTO temp_var from (SELECT UITM."ItemCode", SUM("InvValue") FROM UITM, "#UBVL_Aggregation" 
			        WHERE UITM."ItemCode" = "#UBVL_Aggregation"."ItemCode" 
			        GROUP BY UITM."ItemCode", "StockValue" 
			        HAVING "StockValue" <> SUM("InvValue"));  
		end if;
		if :temp_var > 0 THEN 
			retCode := 1; 
		end if;
		

--UITM.OnHand  <=>  UBVL.calculatedOnHand
		if :retCode = 0 then
			select count(*) INTO temp_var from (
				SELECT UITM."ItemCode", SUM("BVLQty")
				FROM UITM, "#UBVL_Aggregation"
				WHERE UITM."ItemCode"="#UBVL_Aggregation"."ItemCode"
				GROUP BY UITM."ItemCode", "OnHand"
				HAVING "OnHand"<>SUM("BVLQty"));	
		end if;
	
		if :temp_var > 0 THEN 
			retCode := 2; 
		end if;
		
-- UBTN quantity pre batch   <=>  UBVL quantity per batch
		if :retCode = 0 then
				select count(*) INTO temp_var from ( 
				SELECT UBTN."ItemCode" , "BVLQty", (UBTN."Quantity" - UBTN."QuantOut") as "BTNQty"
				FROM "#UBVL_Aggregation", UBTN 
				WHERE UBTN."ItemCode"="#UBVL_Aggregation"."ItemCode" AND UBTN."AbsEntry"="#UBVL_Aggregation"."MdAbsEntry" and "#UBVL_Aggregation"."ManagedBy" = UBTN."ObjType"
				and "BVLQty"<>(UBTN."Quantity" - UBTN."QuantOut"));
		end if;
		
		if :temp_var > 0 THEN 
			retCode := 3; 
		end if;
	
		-- USRN quantity pre batch   <=>  UBVL quantity per batch	
		if :retCode = 0 then
				select count(*) INTO temp_var from ( 
					 SELECT USRN."ItemCode", "BVLQty", (USRN."Quantity" - USRN."QuantOut") AS "SRNQty" 
					 FROM "#UBVL_Aggregation", USRN 
					 WHERE USRN."ItemCode" = "#UBVL_Aggregation"."ItemCode" AND USRN."AbsEntry" = "#UBVL_Aggregation"."MdAbsEntry" AND "#UBVL_Aggregation"."ManagedBy" = USRN."ObjType" AND "BVLQty" <> (USRN."Quantity" - USRN."QuantOut"));
		end if;
		
		if :temp_var > 0 THEN 
			retCode := 4; 
		end if;
		
		-- UIVL Qties and TransValues <=> UBVL Qties and TransValues 		
		if :retCode = 0 then
			-- TEMP table for UIVL
			select count(*) into IsExistIvlTMP from "PUBLIC"."M_TEMPORARY_TABLES" where schema_name = CURRENT_SCHEMA and table_name='#UIVL_Aggregation_Item';
			IF (:IsExistIvlTMP > 0 ) then
				DROP TABLE "#UIVL_Aggregation_Item";
    		end if; 
    		CREATE LOCAL TEMPORARY COLUMN TABLE "#UIVL_Aggregation_Item" AS (
    			SELECT T0."ItemCode", SUM(T1."TransValue") AS "TransValue", SUM(IFNULL(T0."InQty", 0) - IFNULL(T0."OutQty", 0)) AS "QtyInUIVL" 
    			FROM UIVL T0 INNER JOIN UIVL1 T1 ON T0."TransSeq" = T1."TransSeq" WHERE T0."CostMethod"='B' GROUP BY T0."ItemCode"
    		);
			-- Stock Values
			if (:retCode = 0) then
					select count(*) INTO temp_var from (
	 					SELECT T0."ItemCode", SUM(T1."InvValue") 
	 					FROM "#UIVL_Aggregation_Item" T0, "#UBVL_Aggregation" T1 
	 					WHERE T0."ItemCode" = T1."ItemCode" 
	 					GROUP BY T0."ItemCode", T0."TransValue" 
	 					HAVING T0."TransValue" <> SUM(T1."InvValue"));
			end if;
		
			if :temp_var > 0 THEN 
				retCode := 5; 
			end if;
			-- Stock Qties
			if (:retCode = 0) then
						select count(*) INTO temp_var from ( 
							SELECT T0."ItemCode",(T0."QtyInUIVL") ,SUM(T1."BVLQty") FROM "#UIVL_Aggregation_Item" T0, "#UBVL_Aggregation" T1
							WHERE T0."ItemCode"=T1."ItemCode"
							GROUP BY T0."ItemCode",T0."QtyInUIVL"
							HAVING (T0."QtyInUIVL")<>SUM(T1."BVLQty"));
			end if;
			
			DROP TABLE "#UIVL_Aggregation_Item";
			
			if :temp_var > 0 THEN 
				retCode := 6; 
			end if;
		end if; -- UIVL <+> UBVL check

	end if;
	DROP TABLE "#UBVL_Aggregation_Base";
    DROP TABLE "#UBVL_Aggregation";

	if :retCode > 0 then
		retCode := -1;
	else
		retCode := 0;
	end if;
END;


