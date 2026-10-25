-- B1 DEPENDS: BEFORE:PT:PROCESS_START

create procedure _TmSp_UpgradePCH1RPC1StockSumforND(
					IN objID int)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS
	SumDec int;
BEGIN

--	SET ANSI_WARNINGS OFF;--not supported current
	select "SumDec" into SumDec from OADM;
	IF :objID = 18 then
			Update PCH1 T0 
			set ("StockSum", "StockSumFc", "StockSumSc") = (
				select T0."StockSum" +	ROUND(T2."NDTaxSum",:SumDec), 
					T0."StockSumFc" + ROUND(T2."NDTaxSumFrgn",:SumDec),
					T0."StockSumSc" + ROUND(T2."NDTaxSumSys",:SumDec)
				from (
					select SUM("TaxSum"-"DeductTax") as "NDTaxSum", SUM("TaxSumFrgn"-"DdctTaxFrg") as "NDTaxSumFrgn", SUM("TaxSumSys"-"DdctTaxSys") as "NDTaxSumSys"
					from PCH4
					where "DocEntry" = T0."DocEntry" and "LineNum" = T0."LineNum" 
						and ("NonDdctAct" is NULL or "NonDdctAct" = N'') and "NonDdctPrc" >0  
				) T2
			)
			where exists (
				select 1 from OPCH T1
				where		T0."DocEntry" = T1."DocEntry"  
							and T1."DocType" = N'I' 
							and (T1."VersionNum" >'7.40.000.00' and T1."VersionNum" <= '7.40.999.00')
			);
    end if;
	
	IF( :objID = 19) then
			Update RPC1 T0 
			set ("StockSum", "StockSumFc", "StockSumSc") = (
				select T0."StockSum" +	ROUND(T2."NDTaxSum",:SumDec), 
					T0."StockSumFc" + ROUND(T2."NDTaxSumFrgn",:SumDec),
					T0."StockSumSc" + ROUND(T2."NDTaxSumSys",:SumDec)
				from (
					select SUM("TaxSum"-"DeductTax") as "NDTaxSum", SUM("TaxSumFrgn"-"DdctTaxFrg") as "NDTaxSumFrgn", SUM("TaxSumSys"-"DdctTaxSys") as "NDTaxSumSys"
					from RPC4
					where "DocEntry" = T0."DocEntry" and "LineNum" = T0."LineNum" 
						and ("NonDdctAct" is NULL or "NonDdctAct" = N'') and "NonDdctPrc" >0  
				) T2
			)
			where exists (
				select 1 from ORPC T1
				where		T0."DocEntry" = T1."DocEntry"  
							and T1."DocType" = N'I' 
							and (T1."VersionNum" >'7.40.000.00' and T1."VersionNum" <= '7.40.999.00')
			);
	end if;
END;


