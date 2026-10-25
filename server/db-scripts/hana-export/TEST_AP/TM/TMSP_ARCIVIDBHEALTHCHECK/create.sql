-- B1 DEPENDS: AFTER:PT:PROCESS_END

Create PROCEDURE TmSp_ArcIVIDBHealthCheck
(
in USD_Rate DECIMAL(21,6),
out ret integer
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
	v_Difference DECIMAL(32,6);
	v_notIncludedDocs DECIMAL(21,6);
	v_CurrentConsistency nvarchar(1);
	v_PreviousFix nvarchar(1);
	v_isPricePerWH nvarchar(1);
	v_CandidateForService nvarchar(1);
	v_isPerpetual nvarchar(1);
	v_CountPeviousUpdateDate integer;
	v_sumdec integer;
	v_Pricedec integer;
	v_IncStckSettings DECIMAL(32,6);
	v_cnt DECIMAL(21,6);
	v_cnt1 DECIMAL(21,6);
	v_cnt2 DECIMAL(21,6);
	v_exists varchar(100);
	v_flag TINYINT;
	v_USD_Rate DECIMAL(21,6);
BEGIN
ret := -100;
select "PriceDec" INTO v_Pricedec from oadm;
select top 1 "PriceSys" into v_isPricePerWH from OADM;
select ifnull(avg("Rate"),1) into v_USD_Rate from ortt where "Currency" = 'USD';

select count(*) into v_cnt from OADM where "ContInvnt" = 'N';
if :v_cnt = 1 then
	ret := 1;
end if;
	
------------------------------End Perpetual Inventory verification--------------------------------------

--------------------------------Historical data verification------------------------------------------


select distinct "Version" into v_cnt  from cinf;
IF :v_cnt < 880220  and :ret = -100 then

	create table ToStock (act nvarchar (20), trans DECIMAL(21,6));
	insert into ToStock	
	select "PriceDifAc", sum (IFNULL("PriceDiff",0)) from OINM group by "PriceDifAc" having  sum (IFNULL("PriceDiff",0)) <> 0
	Union
	select "VarianceAc", sum (IFNULL("VarVal",0)) from OINM group by "VarianceAc" having sum (IFNULL("VarVal",0)) <> 0
	Union
	select "ExcRateAct", sum (IFNULL("ExchDiff",0)) from OINM group by "ExcRateAct" having sum (IFNULL("ExchDiff",0)) <> 0 
	Union
	select "TrnsfrAct", sum (IFNULL("Allocation",0)) from OINM group by "TrnsfrAct" having sum (IFNULL("Allocation",0)) <> 0 
	Union
	select "CostAct", sum (IFNULL("CogsVal",0)) from OINM group by "CostAct" having sum (IFNULL("CogsVal",0)) <> 0 
	Union
	select "WipAct", sum (IFNULL("WipVal",0)) from OINM group by "WipAct" having sum (IFNULL("WipVal",0)) <> 0 
	Union
	select "WipVarAcc", sum (IFNULL("WipVarVal",0)) from OINM group by "WipVarAcc" having sum (IFNULL("WipVarVal",0)) <> 0 
	Union
	select "NegStckAct", sum (IFNULL("NegInvAdjs",0)) from OINM group by "NegStckAct" having sum (IFNULL("NegInvAdjs",0)) <> 0 
	Union
	select "IOffIncAcc", sum (IFNULL("IOffIncVal",0)) from OINM group by "IOffIncAcc" having sum (IFNULL("IOffIncVal",0)) <> 0 
	Union
	select "DOffDecAcc", sum (IFNULL("DOffDecVal",0)) from OINM group by "DOffDecAcc" having sum (IFNULL("DOffDecVal",0)) <> 0 
	Union
	select "DecAcc", sum (IFNULL("DecVal",0)) from OINM group by "DecAcc" having sum (IFNULL("DecVal",0)) <> 0 
	Union
	select "IncAct", sum (IFNULL("IncVal",0)) from OINM group by "IncAct" having sum (IFNULL("IncVal",0)) <> 0 ;
	--select * from tostock

	create table stacts	(stact nvarchar (20));
	insert into stacts select distinct "InvntAct" from OINM where "InvntAct" is not NULL and "InvntAct" <> '';
	
	select IFNULL(abs(sum (t0.trans)) , 0) into v_IncStckSettings
	from ToStock T0 inner join stacts T1
	on t0.act = t1.stact;
	
	drop table ToStock;
	drop table stacts;
		
	select abs(SUM(IFNULL(t0.sumTransVal,0) - IFNULL(t1.sumPosting,0))) into v_Difference
	from
		(select 
			"InvntAct",
			"TransType",
			"CreatedBy",
			"BASE_REF",
			"DocDate",
			sum("TransValue") sumTransVal
		from
			oinm
		where
			"TransValue" <> 0 
		group by
			"InvntAct",
			"TransType",
			"CreatedBy",
			"BASE_REF",
			"DocDate") t0
		Full Outer Join
		(select
			"Account",
			"TransType",
			"CreatedBy",
			"RefDate",
			sum("Debit") - sum("Credit") sumPosting
		from
			jdt1
		where
			"Account" in (select distinct "InvntAct" from oinm)
			and "TransType" in (select distinct "TransType" from oinm union select 58 from dummy) and "TransType" <> -2
		group by
			"Account",
			"RefDate",
			"TransType", 
			"CreatedBy") t1
	on
		t0."CreatedBy" = t1."CreatedBy" and 
		t0."InvntAct" = t1."Account" and
		t0."TransType" = t1."TransType";


	--------------------------------END Historical data verification------------------------------------------

	--------------------------------Current Data consistency verification-------------------------------------
	
	IF :v_isPricePerWH = 'Y' then 
		v_flag := 0;

		select top 1 TransValue into v_cnt from (select abs(sum("TransValue")) as TransValue from oinm
		group by "ItemCode", "Warehouse"
		having 
		(
		(sum("InQty"-"OutQty") = 0 and sum("TransValue")<> 0) or 
		(sum("InQty"-"OutQty") > 0 and sum("TransValue") < 0) or 
		(sum("InQty"-"OutQty") < 0 and sum("TransValue") > 0)) union select 0 from dummy) a 
		order by TransValue desc;
		
			
		
		if :v_cnt > 1 then 
			v_flag := 1;
		end if;
		/* end TESTING 2007 per whs: zero QTY/nonzero stockvalue OR negative QTY/positive stockvalue OR positive QTY/negative stockvalue */ 

		SELECT
			IFNULL(sum(abs(t0.trans - t1."StockValue")),0) into v_cnt
		From
			(SELECT sum("TransValue") Trans, "ItemCode", "Warehouse"  FROM OINM GROUP BY "ItemCode", "Warehouse") t0,
			(SELECT "StockValue", "ItemCode", "WhsCode" FROM OITW) t1,
			OITM t2
		WHERE
			t0."ItemCode" = t1."ItemCode" 
			and t0."ItemCode" = t1."WhsCode"
			and t2."ItemCode" = t0."ItemCode"
			and t2."EvalSystem" in ('A')
			and abs(t0.Trans - t1."StockValue") > 0.1;
	
		if :v_cnt/:v_USD_Rate > 100 then 
			v_flag := 1;
		end if;
			
		SELECT 
			IFNULL(sum(abs(t2.Trans)),0) into v_cnt
		FROM
			(SELECT 
				sum(t0."TransValue") Trans,
				sum(t0."OpenValue") OpenVal, 
				t0."ItemCode", 
				t0."Warehouse" 
			FROM
				OINM t0,
				OITM t1
			WHERE
				t1."ItemCode" = t0."ItemCode" 
				and t1."EvalSystem" = 'F'
			GROUP BY 
				t0."ItemCode", 
				t0."Warehouse" 
			HAVING
				abs(sum(t0."TransValue") - sum(t0."OpenValue")) > 0.1 and sum(t0."InQty")-sum(t0."OutQty") >= 0) t2;
				
		if :v_cnt/:v_USD_Rate > 100 then 
			v_flag := 1;
		end if;
				
		IF :v_flag = 1 THEN 
			/* start TESTING 2007 per whs: zero QTY/nonzero stockvalue OR negative QTY/positive stockvalue OR positive QTY/negative stockvalue */ 
			v_CurrentConsistency := 'N';  --affected
		ELSE
			v_CurrentConsistency := 'Y';  --not affected
		END IF;	
	END IF; -- v_isPricePerWH = 'Y'

	IF :v_isPricePerWH = 'N' then
	
		select top 1 TransValue into v_cnt from (select abs(sum("TransValue")) as TransValue from oinm
		group by "ItemCode"
		having 
		(
		(sum("InQty"-"OutQty") = 0 and sum("TransValue")<> 0) or 
		(sum("InQty"-"OutQty") > 0 and sum("TransValue") < 0) or 
		(sum("InQty"-"OutQty") < 0 and sum("TransValue") > 0)) union select 0 from dummy) a 
		order by TransValue desc;			
			
		if :v_cnt > 1 then 
			v_flag := 1;
		end if;
							
		SELECT
			IFNULL(sum(abs(t0.trans - t1."StockValue")),0) INTO v_cnt
		FROM
			(SELECT sum("TransValue") Trans, "ItemCode"  FROM OINM GROUP BY "ItemCode") t0,
			(SELECT "StockValue", "ItemCode" FROM OITM) t1,
			OITM t2
		WHERE
			t0."ItemCode" = t1."ItemCode" 
			and t2."ItemCode" = t0."ItemCode"
			and t2."EvalSystem" in ('A')
			and abs(t0.Trans - t1."StockValue") > 0.1;
		
		if :v_cnt/:v_USD_Rate > 100 then 
			v_flag := 1;
		end if;

		SELECT 
			IFNULL(sum(abs(t2.Trans)),0) into v_cnt
		FROM
			(SELECT 
				sum(t0."TransValue") Trans,
				sum(t0."OpenValue") OpenVal, 
				t0."ItemCode"
			FROM
				OINM t0,
				OITM t1
			WHERE
				t1."ItemCode" = t0."ItemCode" 
				and t1."EvalSystem" = 'F'
			GROUP BY 
				t0."ItemCode"
			HAVING
				abs(sum(t0."TransValue") - sum(t0."OpenValue")) > 0.1 and sum(t0."InQty")-sum(t0."OutQty") >= 0) t2;
				
		if :v_cnt/:v_USD_Rate > 100 then 
			v_flag := 1;
		end if;	
	
		IF :v_flag = 1 then
			/* Zero QTY/nonzero stockvalue OR negative QTY/positive stockvalue OR positive QTY/negative stockvalue */ 
			v_CurrentConsistency := 'N';  --affected
		ELSE
			v_CurrentConsistency := 'Y';  --not affected
		END IF;		 
	END IF; --v_isPricePerWH 
	--------------------------------END Current Data consistency verification-------------------------------------

	--------------------------------Previous fix verification----------------------------------------------------
	
	
	SELECT count(*) into v_cnt FROM "PUBLIC".tables where table_name = 'IVLG' and schema_name = current_schema;
	IF :v_cnt = 0 then
		v_PreviousFix := 'N';
	ELSE
		create local temporary table #IVLG (cnt integer);
		exec ('INSERT INTO #IVLG SELECT count(*) FROM IVLG WHERE CustDBupd=''Y''');
		SELECT cnt into v_cnt FROM #IVLG;
		if :v_cnt > 0 then
			v_PreviousFix := 'Y';
		else 
			v_PreviousFix := 'N';	
		end if;
		drop table #IVLG;
	End IF; 

	--------------------------------------------------------------------------------------------------------------

	--output results

	if ((abs((:v_Difference - :v_IncStckSettings)/ :v_USD_Rate) > 1000
				   AND :v_previousfix = 'N')
				 or :v_CurrentConsistency = 'N')  then
	
	-- Must run IVI before DA
	-- Database is candidate for the Inventory valuation Check-up service (Need to run Stock Tool)
		ret := -1;
	Else
		-- DA can be proceed
		-- Database is healthy
		ret := 0;
	end if;


--------------------------------------------------------------------------------------------------------------

END IF; -- <880220

--------------------------------------------------------------------------------------------------------------

---------------88 SCRIPT-------------------------------
select distinct "Version" into v_cnt from cinf;
IF :v_cnt >= 880220 and :ret = -100 then 
	--------------------------------Historical data verification------------------------------------------
	create table ToStock (act nvarchar (20), trans DECIMAL(21,6));
	insert into ToStock
	select "PriceDifAc", sum (IFNULL("PriceDiff",0)) from oivl group by "PriceDifAc" having  sum (IFNULL("PriceDiff",0)) <> 0
	Union
	select "VarianceAc", sum (IFNULL("VarVal",0)) from oivl group by "VarianceAc" having sum (IFNULL("VarVal",0)) <> 0
	Union
	select "ExcRateAct", sum (IFNULL("ExchDiff",0)) from oivl group by "ExcRateAct" having sum (IFNULL("ExchDiff",0)) <> 0 
	Union
	select "TrnsfrAct", sum (IFNULL("Allocation",0)) from oivl group by "TrnsfrAct" having sum (IFNULL("Allocation",0)) <> 0 
	Union
	select "CostAct", sum (IFNULL("CogsVal",0)) from oivl group by "CostAct" having sum (IFNULL("CogsVal",0)) <> 0 
	Union
	select "WipAct", sum (IFNULL("WipVal",0)) from oivl group by "WipAct" having sum (IFNULL("WipVal",0)) <> 0 
	Union
	select "WipVarAcc", sum (IFNULL("WipVarVal",0)) from oivl group by "WipVarAcc" having sum (IFNULL("WipVarVal",0)) <> 0 
	Union
	select "NegStckAct", sum (IFNULL("NegInvAdjs",0)) from oivl group by "NegStckAct" having sum (IFNULL("NegInvAdjs",0)) <> 0 
	Union
	select "IOffIncAcc", sum (IFNULL("IOffIncVal",0)) from oivl group by "IOffIncAcc" having sum (IFNULL("IOffIncVal",0)) <> 0 
	Union
	select "DOffDecAcc", sum (IFNULL("DOffDecVal",0)) from oivl group by "DOffDecAcc" having sum (IFNULL("DOffDecVal",0)) <> 0 
	Union
	select "DecAcc", sum (IFNULL("DecVal",0)) from oivl group by "DecAcc" having sum (IFNULL("DecVal",0)) <> 0 
	Union
	select "IncAct", sum (IFNULL("IncVal",0)) from oivl group by "IncAct" having sum (IFNULL("IncVal",0)) <> 0
	Union
	select "PAOffAcc", sum (IFNULL("PAOffVal",0)) from oivl group by "PAOffAcc" having sum (IFNULL("PAOffVal",0)) <> 0 
	Union
	select "PAAcc", sum (IFNULL("PAVal",0)) from oivl group by "PAAcc" having sum (IFNULL("PAVal",0)) <> 0;
	--select * from tostock

	create table stacts	(stact nvarchar (20));
	insert into stacts select distinct "InvntAct" from oivl where "InvntAct" is not NULL and "InvntAct" <> '';
	--select * from stacts

	select IFNULL(abs (sum (t0.trans)) , 0) INTO v_IncStckSettings
	from ToStock T0 inner join stacts T1
	on t0.act = t1.stact;
	drop table ToStock;
	drop table stacts;

	select  abs(SUM(IFNULL(t0.sumTransVal, 0) - IFNULL(t1.sumPosting, 0))) into v_Difference
	from    ( select	"InvntAct",
						"TransType",
						"CreatedBy",
						"BASE_REF",
						"DocDate",
						sum(ivl1."TransValue") sumTransVal
			  from      oivl
						inner join ivl1 on oivl."TransSeq" = ivl1."TransSeq"
			  where     "TransValue" <> 0
			  group by  "InvntAct",
						"TransType",
						"CreatedBy",
						"BASE_REF",
						"DocDate"
			) t0
			
			Full Outer Join ( select    "Account",
										"TransType", 		
										"CreatedBy",
										"RefDate",
										sum(IFNULL("Debit", 0)) - sum(IFNULL("Credit", 0)) sumPosting
							  from      jdt1
							  where     "Account" in ( select distinct
															 "InvntAct"
													 from   OIVL )
										and "TransType" in ( select distinct
																	"TransType"
															 from   OIVL union select 58 from dummy union select 10000046 from dummy)
															 
															 -- This JE is created by Data Archive, we also should consider it.
											
										and "TransType" <> -2
							  group by  "Account",
										"RefDate",
										"TransType", 		
										"CreatedBy"
							) t1 on t0."CreatedBy" = t1."CreatedBy"
									and t0."InvntAct" = t1."Account"
									and t0."TransType" = t1."TransType";

	--------------------------------END Historical data verification------------------------------------------



	--------------------------------Current Data consistency verification-------------------------------------

	/* Check inconsistencies in stock sub ledger - Differences between stockvalue,openvalue,transvalue */
	
	if :v_isPricePerWH = 'Y' then
		v_flag := 0;
		
	/* Zero QTY/nonzero stockvalue OR negative QTY/positive stockvalue OR positive QTY/negative stockvalue */ 


		select top 1 SumStock into v_cnt from (select abs(sum("SumStock")) as SumStock from oivl
		group by "ItemCode", "LocCode"
		having 
		(
		(sum("InQty"-"OutQty") = 0 and sum("SumStock")<> 0) or 
		(sum("InQty"-"OutQty") > 0 and sum("SumStock") < 0) or 
		(sum("InQty"-"OutQty") < 0 and sum("SumStock") > 0)) union select 0 from dummy) a 
		order by SumStock desc;

		if IFNULL(:v_cnt,0) > 1 then 
			v_flag := 1;
		end if;		
		

	/* Check consistency between sum of stock postings to stock value in items master data */ 
		select SUM(IFNULL("TransValue", 0)) into v_cnt from IVL1;
		select SUM(IFNULL(T0."StockValue", 0)) into v_cnt1 from OITW T0 
			inner join OITM T1 on T0."ItemCode" = T1."ItemCode" and T1."InvntItem" = 'Y';
			 
		if abs((:v_cnt - :v_cnt1)/:v_USD_Rate) > 100 then 
			v_flag := 1;
		end if;	

	/* Check comsistency between sum of stock postings to stock value in items master data */ 
		select  SUM(IFNULL("SumStock", 0)) into v_cnt from OIVL;
		select  SUM(IFNULL(T0."StockValue", 0)) into v_cnt1 from OITW T0 
		inner join OITM T1 on T0."ItemCode" = T1."ItemCode" and T1."InvntItem" = 'Y';
			
		if abs((:v_cnt - :v_cnt1)/:v_USD_Rate) > 100 then 
			v_flag := 1;
		end if;	
			
	/* Check consistency between sum open layers value + value of items in negative zone multiplied by statis price*/
	/* to stock postings value - fifo items */ 
		select SUM(IFNULL(Q."OpenValue", 0)) into v_cnt from OIVQ Q;
			  
		select SUM(IFNULL(W."AvgPrice", 0) * IFNULL(W."OnHand", 0)) into v_cnt1 from OITW W
			inner join OITM M on W."ItemCode" = M."ItemCode"
			and M."EvalSystem" = 'F' and  M."InvntItem" = 'Y'
			and W."OnHand" < 0;
				
		select  SUM(IFNULL("SumStock", 0)) into v_cnt2 from OIVL where "CostMethod" = 'F';
			  
		if ((:v_cnt + :v_cnt1)/:v_USD_Rate) - (:v_cnt2/:v_USD_Rate) > 100 then 
			v_flag := 1;
		end if;	

		IF	:v_flag = 1 then
			v_CurrentConsistency := 'N';  --affected
		else 
			v_CurrentConsistency := 'Y';  --not affected
		end if;
		
	elseif :v_isPricePerWH = 'N' then
		v_flag := 0;
	/*Zero QTY/nonzero stockvalue OR negative QTY/positive stockvalue OR positive QTY/negative stockvalue */ 
	
		select top 1 SumStock into v_cnt from (select abs(sum("SumStock")) as SumStock from oivl
		group by "ItemCode"
		having 
		(
		(sum("InQty"-"OutQty") = 0 and sum("SumStock")<> 0) or 
		(sum("InQty"-"OutQty") > 0 and sum("SumStock") < 0) or 
		(sum("InQty"-"OutQty") < 0 and sum("SumStock") > 0)) union select 0 from dummy) a 
		order by SumStock desc;

		if IFNULL(:v_cnt,0) > 1 then 
			v_flag := 1;
		end if;	
		
		/* Check consistency between sum of stock postings to stock value in items master data */ 

		select SUM(IFNULL("TransValue", 0)) INTO v_cnt from IVL1;
		select SUM(IFNULL("StockValue", 0)) INTO v_cnt1 from OITM where "InvntItem" = 'Y';
			
		if abs((:v_cnt - :v_cnt1)/:v_USD_Rate) > 100 then 
			v_flag := 1;
		end if;	
			
		/* Check consistency between sum of stock postings to stock value in items master data */ 
		select SUM(IFNULL("SumStock", 0)) into v_cnt from OIVL;
		select SUM(IFNULL("StockValue", 0)) into v_cnt1 from OITM where "InvntItem" = 'Y';

		if abs((:v_cnt - :v_cnt1)/:v_USD_Rate) > 100 then 
			v_flag := 1;
		end if;	

		/* Check consistency between sum open layers value + value of items in negative zone multiplied by statis price*/
		/* to stock postings value - fifo items */ 
		select SUM(IFNULL(Q."OpenValue", 0)) into v_cnt from OIVQ Q;
		
		select SUM(IFNULL(M."AvgPrice", 0) * IFNULL(M."OnHand", 0)) into v_cnt1
		  from   OITM M
		  where  M."EvalSystem" = 'F' and M."InvntItem" = 'Y'
				 and IFNULL(M."OnHand", 0) < 0;
		
		select  SUM("SumStock") into v_cnt2 from OIVL	where "CostMethod" = 'F';
		
		if ((:v_cnt + :v_cnt1)/:v_USD_Rate) - (:v_cnt2/:v_USD_Rate) > 100 then 
			v_flag := 1;
		end if;	

		IF	:v_flag = 1 then
			v_CurrentConsistency := 'N';  --affected
		else 
			v_CurrentConsistency := 'Y';  --not affected
		end if;
		
	end if; --:v_isPricePerWH = 'N' and 'Y'

			
	--------------------------------END Current Data consistency verification-------------------------------------


	--------------------------------Previous fix verification----------------------------------------------------

	select  count("UpdateDate") into v_CountPeviousUpdateDate from livi where   IFNULL("UpdateDate", '') <> '';
								  
	if :v_CountPeviousUpdateDate > 0 THEN
		v_PreviousFix := 'Y';
	else 
		v_PreviousFix := 'N';
	END IF;

	------------------------------------------------------------------------------------------------

	--output results
	if ((abs((:v_Difference - :v_IncStckSettings)/:v_USD_Rate) > 1000
				   AND :v_previousfix = 'N')
				 or :v_CurrentConsistency = 'N')  then
	
		-- Must run IVI before DA
		-- Database is candidate for the Inventory valuation Check-up service (Need to run Stock Tool)
		ret := -1;
	Else
		-- DA can be proceed
		-- Database is healthy
		ret := 0; 
	End if;
	------------------------------------------------------------------------------------------------
	
	END if; -->= 880220
	
END;


