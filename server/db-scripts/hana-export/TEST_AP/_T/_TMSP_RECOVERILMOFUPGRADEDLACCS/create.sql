-- B1 DEPENDS: BEFORE:PT:PROCESS_START

create procedure _TmSp_RecoverILMofUpgradedLacCS
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

as
	mainCurrency nvarchar(3);
	PerpetualInventory nvarchar(1);
begin

select top 1 "MainCurncy" into mainCurrency from OADM;
select top 1 "ContInvnt" into PerpetualInventory from OADM;

if :PerpetualInventory = 'Y' then
	update	OILM as t0
	set	
		("Currency","DocRate","TotalLC","TotalFC","TotalSC","DocTotal") = 
			(select 
				t2."DocCur",
				t2."DocRate",
				(case 
						when t2."incCustom" = 'Y' then 
							case 
								when ifnull(t3."DocEntry", -1) = -1 then t1."TtlCostLC" 
								else (t1."TtlCostLC" - t3."TtlCostLC") 
							end
						else 
							case  
								when ifnull(t3."DocEntry", -1) = -1 then t1."TtlExpndLC" 
								else (t1."TtlExpndLC" - t3."TtlExpndLC") 
							end
				end ),	
				( case 
						when t2."incCustom" = 'Y' then 
							case 
								when ifnull(t3."DocEntry", -1) = -1 then t1."TtlCostFC" 
								else (t1."TtlCostFC" - t3."TtlCostFC") 
							end
						else 
							case 
								when ifnull(t3."DocEntry", -1) = -1 then t1."TtlExpndFC" 
								else (t1."TtlExpndFC" - t3."TtlExpndFC") 
							end
				end),
				( case  
						when t2."incCustom" = 'Y' then 
							case  
								when ifnull(t3."DocEntry", -1) = -1 then (t1."TtlCustSC" + t1."TtlExpndSC") 
								else ((t1."TtlCustSC" - t3."TtlCustSC") + (t1."TtlExpndSC" - t3."TtlExpndSC")) 
							end
						else 
							case  
								when ifnull(t3."DocEntry", -1) = -1 then t1."TtlExpndSC" 
								else (t1."TtlExpndSC" - t3."TtlExpndSC") 
							end
				end),
				( case 
						when t1."Currency" = :mainCurrency then t1."TtlCostLC" 
						else t1."TtlCostFC" 
				 end)
			from IPF1 t1
				inner join	OIPF t2 on	t1."DocEntry" = t2."DocEntry" 
				left join IPF1 t3 on 
				t1."BaseEntry" = t3."DocEntry" and t1."OrigLine" = t3."LineNum" 
			where 
				t0."TransType" = 69 and ifnull(t0."VersionNum", '') < '8.80.000.00' and t0."BaseType" = 69 and
				t0."DocEntry" = t1."DocEntry" 
						and t0."DocLineNum" = t1."LineNum" 
						and t0."ItemCode" = t1."ItemCode")
		where exists(select 1 from IPF1 t1
				inner join	OIPF t2 on	t1."DocEntry" = t2."DocEntry" 
				left join IPF1 t3 on 
				t1."BaseEntry" = t3."DocEntry" and t1."OrigLine" = t3."LineNum" 
			where 
				t0."TransType" = 69 and ifnull(t0."VersionNum", '') < '8.80.000.00' and t0."BaseType" = 69 and
				t0."DocEntry" = t1."DocEntry" 
						and t0."DocLineNum" = t1."LineNum" 
						and t0."ItemCode" = t1."ItemCode");	
end if;
end;


