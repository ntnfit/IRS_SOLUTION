CREATE PROCEDURE _TmSp_UpgradeProductionSubLine
language sqlscript
SQL SECURITY INVOKER
as 
v_sql nvarchar(4000);	

begin
	v_sql:= 'update  "t"
	Set "t"."INMSubLine" = "Source"."Rank"
	from "WOR3" "t"
	inner join 
		(select "DocEntry", "LineNum", "LayerID",
		"Rank"() OVER (PARTITION BY "DocEntry" ORDER BY "DocEntry", "LineNum", "LayerID") - 1 AS "Rank"
		from "WOR3") as "Source"
	on "t"."DocEntry" = "Source"."DocEntry" and "t"."LineNum" = "Source"."LineNum" and "t"."LayerID" = "Source"."LayerID"';
	exec(:v_sql);
	
	update  "t"
	Set "t"."SubLineNum" = "Source"."Rank"
	from "OILM" "t"
	inner join 
		(select "MessageID",
		"Rank"() OVER (PARTITION BY "DocEntry" ORDER BY "MessageID") - 1 AS "Rank"
		from "OILM" where "TransType"=202 and "ActionType" in (7,8)) as "Source"
	on "t"."MessageID" = "Source"."MessageID" and "t"."TransType"=202 and "t"."ActionType" in (7,8);
	
	update  "t"
	Set "t"."SubLineNum" = "Source"."SubLineNum"
	from "OIVL" "t"
	inner join 
		(select "MessageID", "SubLineNum", "ActionType" from "OILM") as "Source"
	on "t"."MessageID" = "Source"."MessageID" and "t"."TransType"=202 and "Source"."ActionType" in (7,8); 
	
	v_sql:= 'update "MRV2" set "INMSubLine" = 0 where "INMTransTy" = 202';
	exec(:v_sql);
	
	v_sql:= 'update  "t"
	Set "t"."INMSubLine" = "Source"."SubLineNum"
	from "MRV2" "t"
	inner join "MRV1" on "t"."DocEntry" = "MRV1"."DocEntry" and "t"."BaseLine"="MRV1"."LineNum"
	inner join 
		(select "OILM"."MessageID", "OILM"."LocCode", "OILM"."ItemCode", "OILM"."Quantity", "OILM"."SubLineNum", "OILM"."DocEntry", "OIVL"."TransSeq" from "OIVL" 
		inner join "OILM" on "OILM"."MessageID" = "OIVL"."MessageID" 
		where "OILM"."TransType"=202 and "OILM"."ActionType" in (7,8)) as "Source"
	on "t"."IVLTransSe" = "Source"."TransSeq" and "MRV1"."ItemCode" = "Source"."ItemCode" and "MRV1"."WhsCode" = "Source"."LocCode" and "t"."INMTransTy"=202 and "t"."INMCreatBy" = "Source"."DocEntry" and "t"."INMOpenQty"="Source"."Quantity" and "t"."INMSubLine" = -1';
	exec(:v_sql);

	v_sql:= ' do begin
			declare i int;
			while 1 = 1 do
			update  "u"
			Set "u"."INMSubLine" = "Result"."SubLineNum"
			from "MRV2" "u"
			inner join 
			(select "t"."DocEntry", "t"."BaseLine", min("t"."LineNum") "LineNum", min("Source"."SubLineNum") "SubLineNum"
			from "MRV2" "t"
			inner join "MRV1" on "t"."DocEntry" = "MRV1"."DocEntry" and "t"."BaseLine"="MRV1"."LineNum"
			inner join 
				(select "OILM"."MessageID", "OILM"."LocCode", "OILM"."ItemCode", "OILM"."Quantity", "OILM"."SubLineNum", "OILM"."DocEntry", "OIVL"."TransSeq" from "OIVL" 
				inner join "OILM" on "OILM"."MessageID" = "OIVL"."MessageID" 
				LEFT JOIN "MRV2" "b" ON "OILM"."SubLineNum" = "b"."INMSubLine" 
				where "OILM"."TransType"=202 and "OILM"."ActionType" in (7,8) and "b"."INMSubLine" is NULL) as "Source"
			on "MRV1"."ItemCode" = "Source"."ItemCode" and "MRV1"."WhsCode" = "Source"."LocCode" and "t"."INMTransTy"=202 and "t"."INMCreatBy" = "Source"."DocEntry" and "t"."INMOpenQty"="Source"."Quantity" and "t"."INMSubLine" = -1
			group by "t"."DocEntry", "t"."BaseLine" ) as "Result"
			on "u"."DocEntry" = "Result"."DocEntry" and "u"."BaseLine" = "Result"."BaseLine" and "u"."LineNum" = "Result"."LineNum";
			
			SELECT ::ROWCOUNT INTO i FROM DUMMY;
			
			if :i = 0 then 
				break;
			end if;
	
		END while;
	end;';		
	exec(:v_sql);
end;


