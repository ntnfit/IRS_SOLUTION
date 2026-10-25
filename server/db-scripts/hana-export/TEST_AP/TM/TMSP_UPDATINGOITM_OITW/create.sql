-- B1 DEPENDS: AFTER:PT:PROCESS_END

--alter session set current_schema=SBODEMOUS;
--drop procedure TmSp_UpdatingOITM_OITW;
create procedure TmSp_UpdatingOITM_OITW(
	in SourceDB nvarchar(128),
	in UtilityDB nvarchar(128), 
	in isCompanyPerWh nvarchar (1)
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
as
begin
	if (:isCompanyPerWh = 'Y') then 
		exec('UPDATE  '|| :SourceDB  ||'."OITW" SET 	"AvgPrice" = t2."AvgPrice", "StockValue"  = t2."StockValue" FROM 	'|| :SourceDB  ||'."OITW" t1, '|| :UtilityDB ||'."OITW" t2 where 	t1."ItemCode" = t2."ItemCode" and  t1."WhsCode" = t2."WhsCode" ');
	else
		exec('UPDATE  '|| :SourceDB  ||'."OITM" SET 	"AvgPrice" = t2."AvgPrice", "StockValue"  = t2."StockValue" FROM 	'|| :SourceDB  ||'."OITM" t1, '|| :UtilityDB ||'."OITM" t2 where 	t1."ItemCode" = t2."ItemCode" ');
	end if;
	
	exec('UPDATE '|| :SourceDB  ||'."OITW" SET "OnHand"  = t2."OnHand" FROM '|| :SourceDB  ||'."OITW" t1, '|| :UtilityDB ||'."OITW" t2 where t1."ItemCode" = t2."ItemCode" and t1."WhsCode" = t2."WhsCode"');  
	exec('UPDATE  '|| :SourceDB  ||'."OITM" SET "OnHand"  = t2."OnHand" FROM '|| :SourceDB  ||'."OITM" t1, '|| :UtilityDB ||'."OITM" t2 where t1."ItemCode" = t2."ItemCode" ');

end;

--call TmSp_UpdatingOITM_OITW('SBODEMOUS','SBODEMOUS','c');


