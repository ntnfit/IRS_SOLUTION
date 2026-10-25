-- B1 DEPENDS: AFTER:PT:PROCESS_END AFTER:SP:_TmSp_BootCreateGlobalTempTables2
--drop procedure TmSp_InsertIntoTargetITMandTargetITW;

CREATE procedure TmSp_InsertIntoTargetITMandTargetITW(
	IN TargetITM nvarchar(4),
	IN BackUpSimulatedParameters nvarchar(1),
	IN IsInitMAP nvarchar(1),
	IN IsInitSTD nvarchar(1)
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
strMAP_STD_PricesInit nvarchar(100);
TargetITW nvarchar(4);
str1 NCLOB;
PriceDec smallint;
cnum int :=0;
cond int;
debugOuptputOnly nvarchar(1);
NeedInsert int :=1;
InsertCount int :=100000;
BEGIN

	debugOuptputOnly := 'N';
	
	
	if (:debugOuptputOnly = 'Y') then	
		create column table TmSp_InsertIntoTargetITMandTargetITW_debug ("STR" NCLOB null);
		delete from TmSp_InsertIntoTargetITMandTargetITW_debug;
		insert into TmSp_InsertIntoTargetITMandTargetITW_debug values ('----------------------');
		insert into TmSp_InsertIntoTargetITMandTargetITW_debug values ('-- Input parameters --');
		insert into TmSp_InsertIntoTargetITMandTargetITW_debug values (:TargetITM);
		insert into TmSp_InsertIntoTargetITMandTargetITW_debug values (:BackUpSimulatedParameters);
		insert into TmSp_InsertIntoTargetITMandTargetITW_debug values (:IsInitMAP);
		insert into TmSp_InsertIntoTargetITMandTargetITW_debug values (:IsInitSTD);
		insert into TmSp_InsertIntoTargetITMandTargetITW_debug values ('----------------------');
	end if;
	
	strMAP_STD_PricesInit := '';
	if (:TargetITM = 'UITM')then
	  TargetITW := 'UITW';
	end if;
	
	if (:TargetITM = 'SITM') then
	  TargetITW := 'SITW';
	end if;
	
	-- Restore simulated parameters in item definition
	if :BackUpSimulatedParameters = 'Y' then
		str1 := 'UPDATE ' || :TargetITM || N'  tbl1 SET 
					"EvalSystem" = (select "EvalSystem" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"), 
					"AvgPrice" =   (select "AvgPrice" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"), 
					"CardCode" =  (select "CardCode" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"ItmsGrpCod" = (select "ItmsGrpCod" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup1" = (select "QryGroup1" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup2" = (select "QryGroup2" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup3" = (select "QryGroup3" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup4" = (select "QryGroup4" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup5" = (select "QryGroup5" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup6" = (select "QryGroup6" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup7" = (select "QryGroup7" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"), 
					"QryGroup8" = (select "QryGroup8" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup9" = (select "QryGroup9" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup10" = (select "QryGroup10" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup11" = (select "QryGroup11" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup12" = (select "QryGroup12" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup13" = (select "QryGroup13" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup14" = (select "QryGroup14" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup15" = (select "QryGroup15" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup16" = (select "QryGroup16" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup17" = (select "QryGroup17" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup18" = (select "QryGroup18" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup19" = (select "QryGroup19" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup20" = (select "QryGroup20" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup21" = (select "QryGroup21" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup22" = (select "QryGroup22" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup23" = (select "QryGroup23" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup24" = (select "QryGroup24" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup25" = (select "QryGroup25" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup26" = (select "QryGroup26" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup27" = (select "QryGroup27" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup28" = (select "QryGroup28" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup29" = (select "QryGroup29" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup30" = (select "QryGroup30" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup31" = (select "QryGroup31" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup32" = (select "QryGroup32" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup33" = (select "QryGroup33" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup34" = (select "QryGroup34" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup35" = (select "QryGroup35" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup36" = (select "QryGroup36" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup37" = (select "QryGroup37" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup38" = (select "QryGroup38" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup39" = (select "QryGroup39" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup40" = (select "QryGroup40" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup41" = (select "QryGroup41" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup42" = (select "QryGroup42" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup43" = (select "QryGroup43" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup44" = (select "QryGroup44" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup45" = (select "QryGroup45" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup46" = (select "QryGroup46" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup47" = (select "QryGroup47" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup48" = (select "QryGroup48" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup49" = (select "QryGroup49" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup50" = (select "QryGroup50" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup51" = (select "QryGroup51" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup52" = (select "QryGroup52" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup53" = (select "QryGroup53" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup54" = (select "QryGroup54" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup55" = (select "QryGroup55" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup56" = (select "QryGroup56" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup57" = (select "QryGroup57" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup58" = (select "QryGroup58" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup59" = (select "QryGroup59" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup60" = (select "QryGroup60" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup61" = (select "QryGroup61" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup62" = (select "QryGroup62" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup63" = (select "QryGroup63" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode"),
					"QryGroup64" = (select "QryGroup64" from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode")
				WHERE EXISTS (select 1 from oitm tbl2 where tbl1."ItemCode" = tbl2."ItemCode")';
		
		if (:debugOuptputOnly = 'Y') then	
			insert into TmSp_InsertIntoTargetITMandTargetITW_debug values (:str1);
		else
			exec(:str1);
		end if;
		
	end if;

	delete from "TMP_InsertInto_NewItemsInTargetITM";

	--select all the items to copy to Target ITM
	exec ('insert into "TMP_InsertInto_NewItemsInTargetITM" select "ItemCode" from OITM where "ItemCode" not in (select "ItemCode" from ' ||  :TargetITM || ')');
	
   --copy the items to Target ITM
	exec ('insert into ' || :TargetITM || N' SELECT * FROM OITM where "ItemCode" in (select * from "TMP_InsertInto_NewItemsInTargetITM")');
    
   -- update TargetITM.EvalSystem for items with batch valuation method
	exec ('UPDATE T0 SET T0."EvalSystem" = T1."CostMethod" 
       FROM ' || :TargetITM || ' T0, OIVL T1 
       WHERE T1."MessageID" =  
       		(SELECT MAX("MessageID") 
       		FROM OIVL WHERE "CostMethod" <> ''B'' AND T0."ItemCode"="ItemCode") 
       	AND T0."EvalSystem"=''B'' AND T0."ItemCode" in (SELECT * FROM "TMP_InsertInto_NewItemsInTargetITM")');

    --activate the item and nullify accumulators
	exec('update  ' ||:TargetITM || N' set "PrchseItem" = ''Y'',"SellItem" = ''Y'',"OnHand" = 0,"validFor" = ''Y'',"validFrom" = NULL,"validTo" = NULL,"frozenFor" = ''N'',"frozenFrom" = NULL,"frozenTo" = NULL, "StockValue" = 0 where "ItemCode" in (select * from "TMP_InsertInto_NewItemsInTargetITM")');


	----------------- Sets starting prices for Target ITM ---------- BEGIN
    --update AvgPrice in invnt items to zero
	exec(' update ' ||  :TargetITM || N' set "AvgPrice" = 0 where "ItemCode" in (select * from "TMP_InsertInto_NewItemsInTargetITM") and "InvntItem" = ''Y''');

	strMAP_STD_PricesInit := ' AND (';
	IF (:IsInitMAP = 'Y') then
		strMAP_STD_PricesInit := strMAP_STD_PricesInit || ' T_TargetITM."EvalSystem" = ''A'' ';
	end if;
	IF (:IsInitSTD = 'Y') then
		IF (:IsInitMAP = 'Y') then
		   strMAP_STD_PricesInit := strMAP_STD_PricesInit || ' OR ';
		end if;
		strMAP_STD_PricesInit := strMAP_STD_PricesInit || ' T_TargetITM."EvalSystem" = ''S'' ';
	end if;
	strMAP_STD_PricesInit := strMAP_STD_PricesInit || ')';
	
	if (:IsInitSTD = 'Y' OR :IsInitMAP = 'Y') then
		--Copy prices of items not being used (without transaction); 
		--This applies for  MAP/STD items 
		str1 := 'UPDATE	' ||  :TargetITM || N' T_TargetITM SET "AvgPrice" = (SELECT "AvgPrice" FROM OITM T1 WHERE	T_TargetITM."ItemCode" NOT IN(SELECT U0."ItemCode" FROM OIVL U0) AND T_TargetITM."ItemCode" IN (select * from "TMP_InsertInto_NewItemsInTargetITM") AND T_TargetITM."ItemCode" = T1."ItemCode" ' || :strMAP_STD_PricesInit || ')
		WHERE EXISTS (SELECT 1 FROM OITM T1 WHERE	T_TargetITM."ItemCode" NOT IN(SELECT U0."ItemCode" FROM OIVL U0) AND T_TargetITM."ItemCode" IN (select * from "TMP_InsertInto_NewItemsInTargetITM") AND T_TargetITM."ItemCode" = T1."ItemCode" ' || :strMAP_STD_PricesInit || ')';
		
		if (debugOuptputOnly = 'Y') then	
			insert into TmSp_InsertIntoTargetITMandTargetITW_debug values (:str1);
		else
			exec (:str1);
		end if;
		
		--update AvgPrice items
		--This applies for  non-fifo nad non-snb items 
		select "PriceDec" into pricedec from OADM;
		
		str1 := '	UPDATE	' ||  :TargetITM || ' T_TargetITM 
		SET	"AvgPrice" = (SELECT ROUND ( abs(T1."SumStock") / nullif((T1."InQty" + T1."OutQty"),0), ' || :pricedec || ') FROM OIVL T1 
		WHERE	T_TargetITM."ItemCode" = T1."ItemCode" ' || :strMAP_STD_PricesInit || ' AND 
		(T1."TransSeq" IN (SELECT	MIN("TransSeq") FROM OIVL WHERE not ("TransType" = 18 and "InQty" = 0 and "OutQty" = 0 and "SumStock" = 0) GROUP BY	"ItemCode")) 
							AND (T1."OutQty" <> 0 OR T1."TransType" in (13,14,15,16,19,21,60,67,165,166) 
							OR (T1."InQty" <> 0 AND T_TargetITM."EvalSystem" = ''S'')) AND T_TargetITM."InvntItem" = ''Y'' 
		AND T_TargetITM."ItemCode" IN (SELECT * FROM "TMP_InsertInto_NewItemsInTargetITM"))
		WHERE EXISTS (SELECT 1 FROM OIVL T1 
		WHERE	T_TargetITM."ItemCode" = T1."ItemCode" ' || :strMAP_STD_PricesInit || ' AND 
		(T1."TransSeq" IN (SELECT	MIN("TransSeq") FROM OIVL WHERE not ("TransType" = 18 and "InQty" = 0 and "OutQty" = 0 and "SumStock" = 0) GROUP BY	"ItemCode")) 
							AND (T1."OutQty" <> 0 OR T1."TransType" in (13,14,15,16,19,21,60,67,165,166) 
							OR (T1."InQty" <> 0 AND T_TargetITM."EvalSystem" = ''S'')) AND T_TargetITM."InvntItem" = ''Y'' 
		AND T_TargetITM."ItemCode" IN (SELECT * FROM "TMP_InsertInto_NewItemsInTargetITM"))';
	
		if (debugOuptputOnly = 'Y') then	
			insert into TmSp_InsertIntoTargetITMandTargetITW_debug values (:str1);
		else
			exec (:str1);
		end if;
	
		--update AvgPrice by IVL1.CalcPrice in case abs(T1.SumStock) / nullif((T1.InQty + T1.OutQty),0) is zero
		str1 := ('UPDATE ' ||  :TargetITM || ' T_TargetITM SET "AvgPrice" = (SELECT ROUND ( T2."CalcPrice", ' || :pricedec || ') FROM OIVL T1, IVL1 T2 WHERE	T_TargetITM."ItemCode" = T1."ItemCode" ' || :strMAP_STD_PricesInit || ' AND T1."TransSeq" = T2."TransSeq" AND (T1."TransSeq" IN (SELECT	MIN("TransSeq") FROM		OIVL GROUP BY	"ItemCode")) AND ( T1."OutQty" <> 0 OR T1."TransType" in (13,14,15,16,19,21,60,67,165,166) OR (T1."InQty" <> 0 AND T_TargetITM."EvalSystem" = ''S''))AND T_TargetITM."InvntItem" = ''Y'' AND T_TargetITM."ItemCode" IN (SELECT * FROM "TMP_InsertInto_NewItemsInTargetITM") AND T_TargetITM."AvgPrice" = 0)
		WHERE EXISTS (SELECT 1 FROM OIVL T1, IVL1 T2 WHERE	T_TargetITM."ItemCode" = T1."ItemCode" ' || :strMAP_STD_PricesInit || ' AND T1."TransSeq" = T2."TransSeq" AND (T1."TransSeq" IN (SELECT	MIN("TransSeq") FROM		OIVL GROUP BY	"ItemCode")) AND ( T1."OutQty" <> 0 OR T1."TransType" in (13,14,15,16,19,21,60,67,165,166) OR (T1."InQty" <> 0 AND T_TargetITM."EvalSystem" = ''S''))AND T_TargetITM."InvntItem" = ''Y'' AND T_TargetITM."ItemCode" IN (SELECT * FROM "TMP_InsertInto_NewItemsInTargetITM") AND T_TargetITM."AvgPrice" = 0)');
		
		if (debugOuptputOnly = 'Y') then	
			insert into TmSp_InsertIntoTargetITMandTargetITW_debug values (:str1);
		else
			exec (:str1);
		end if;
	end if;
	----------------- Sets starting prices for Target ITM ---------- END

	-- create temp table here so later Execute can be used and after that the tamp table can be used
	delete from "TMP_InsertInto_NewItemsInTargetITW";
	
	--select all the items and whs to copy to TargetITW
   exec('insert into "TMP_InsertInto_NewItemsInTargetITW" ("ItemCode", "WhsCode") '
  		 ||' select top ' || :InsertCount ||' A."ItemCode", A."WhsCode" from OITW A '
   		 ||' where A."WhsCode" not in (select "WhsCode" from ' ||  :TargetITW || ' where "ItemCode" = A."ItemCode")');
	SELECT COUNT(*) INTO NeedInsert FROM "TMP_InsertInto_NewItemsInTargetITW";
	while :NeedInsert > 0
	do
		--copy the items and whs to TargetITW
		exec('insert into ' ||  :TargetITW || ' SELECT * FROM OITW A where exists (select 1 from "TMP_InsertInto_NewItemsInTargetITW" where "ItemCode" = A."ItemCode" and "WhsCode" = A."WhsCode")');

		--activate the item and nullify accumulators
		exec('update ' ||  :TargetITW || ' set "OnHand" = 0,"Locked" = ''N'',"Freezed" = ''N'',"StockValue" = 0 where exists (select 1 from "TMP_InsertInto_NewItemsInTargetITW" where "ItemCode" = ' ||  :TargetITW || N'."ItemCode" and "WhsCode" = ' ||  :TargetITW || N'."WhsCode")');

		
		
		----------------- Sets starting prices for Target ITW ---------- BEGIN
		--update AvgPrice in invnt items to zero
		exec ('update ' ||  :TargetITW || ' set "AvgPrice" = 0 where exists (select 1 from "TMP_InsertInto_NewItemsInTargetITW" where "ItemCode" = ' ||  :TargetITW || N'."ItemCode" and "WhsCode" = ' || :TargetITW || N'."WhsCode") and "ItemCode" in (select "ItemCode" from ' ||  :TargetITM || N' where "InvntItem" = ''Y'')');

		if (:IsInitSTD = 'Y' OR :IsInitMAP = 'Y') then
			--Copy prices of items not being used (without transaction)
			str1 := 'UPDATE	' ||  :TargetITW || ' T0 	SET	"AvgPrice" = (SELECT T1."AvgPrice" FROM OITW T1, ' ||  :TargetITM || ' T_TargetITM WHERE T0."ItemCode" NOT IN(SELECT U0."ItemCode" FROM OIVL U0) 
			AND EXISTS 
			(SELECT 1 FROM "TMP_InsertInto_NewItemsInTargetITW" where "ItemCode" = T0."ItemCode" and "WhsCode" = T0."WhsCode") 
			AND T0."ItemCode" = T1."ItemCode" AND T0."ItemCode" = T_TargetITM."ItemCode" 
			AND T0."WhsCode" = T1."WhsCode"  ' || :strMAP_STD_PricesInit || ' )
			WHERE EXISTS (SELECT 1 FROM OITW T1, ' ||  :TargetITM || ' T_TargetITM WHERE T0."ItemCode" NOT IN (SELECT U0."ItemCode" FROM OIVL U0) AND EXISTS (SELECT 1 FROM "TMP_InsertInto_NewItemsInTargetITW" where "ItemCode" = T0."ItemCode" and "WhsCode" = T0."WhsCode") 
			AND T0."ItemCode" = T1."ItemCode" AND T0."ItemCode" = T_TargetITM."ItemCode" 
			AND T0."WhsCode" = T1."WhsCode"  ' || :strMAP_STD_PricesInit || ' )';
		
			if (debugOuptputOnly = 'Y') then	
				insert into TmSp_InsertIntoTargetITMandTargetITW_debug values (:str1);
			else
				exec (:str1);
			end if;
		
			--update AvgPrice in non FIFO items
			str1 :=  ('UPDATE ' || :TargetITW || ' T0 SET "AvgPrice" = ' 
			|| '(SELECT ROUND (  abs(T1."SumStock") / nullif((T1."InQty" + T1."OutQty"),0), ' || :PriceDec || ' ) FROM	OIVL T1, ' 	|| :TargetITM || ' T_TargetITM, '
			|| ' (SELECT MIN("TransSeq") AS "TransSeq", "ItemCode", "LocCode" FROM OIVL WHERE not ("TransType" = 18 and "InQty" = 0 and "OutQty" = 0 and "SumStock" = 0) GROUP BY	"ItemCode", "LocCode" ) AS T3 '
			||	' WHERE	T0."ItemCode" = T1."ItemCode" AND T0."WhsCode" = T1."LocCode" AND T0."ItemCode" = T_TargetITM."ItemCode" ' 
			||	'  ' || :strMAP_STD_PricesInit || '  AND T1."TransSeq"  = T3."TransSeq" AND T1."ItemCode" = T3."ItemCode" AND T1."LocCode" = T3."LocCode" '
			||	' AND( T1."OutQty" <> 0 OR T1."TransType" in (13,14,15,16,19,21,60,67,165,166) ' 
			||	' OR (T1."InQty" <> 0 AND T_TargetITM."EvalSystem" = ''S'')) AND T_TargetITM."InvntItem" = ''Y'' ' 
			||	' AND EXISTS (SELECT 1 FROM "TMP_InsertInto_NewItemsInTargetITW" where "ItemCode" = T0."ItemCode" and "WhsCode" = T0."WhsCode"))
			WHERE EXISTS (SELECT 1 FROM	OIVL T1, ' 	|| :TargetITM || ' T_TargetITM , (SELECT MIN("TransSeq") AS "TransSeq", "ItemCode", "LocCode" FROM OIVL WHERE not ("TransType" = 18 and "InQty" = 0 and "OutQty" = 0 and "SumStock" = 0) GROUP BY	"ItemCode", "LocCode" ) AS T3 '
			||	' WHERE	T0."ItemCode" = T1."ItemCode" AND T0."WhsCode" = T1."LocCode" AND T0."ItemCode" = T_TargetITM."ItemCode" ' 
			||	'  ' || :strMAP_STD_PricesInit || '  AND T1."TransSeq" = T3."TransSeq" AND T1."ItemCode" = T3."ItemCode" AND T1."LocCode" = T3."LocCode" '
			||	' AND( T1."OutQty" <> 0 OR T1."TransType" in (13,14,15,16,19,21,60,67,165,166) ' 
			||	' OR (T1."InQty" <> 0 AND T_TargetITM."EvalSystem" = ''S'')) AND T_TargetITM."InvntItem" = ''Y'' ' 
			||	' AND EXISTS (SELECT 1 FROM "TMP_InsertInto_NewItemsInTargetITW" where "ItemCode" = T0."ItemCode" and "WhsCode" = T0."WhsCode"))');
			
			if (debugOuptputOnly = 'Y') then	
				insert into TmSp_InsertIntoTargetITMandTargetITW_debug values (:str1);
			else
				exec (:str1);
			end if;
		
			--update AvgPrice by IVL1.CalcPrice in case abs(T1.SumStock) / nullif((T1.InQty + T1.OutQty),0) is zero
			str1 := 'UPDATE	' || :TargetITW || ' T0 SET "AvgPrice" = '
			|| '( SELECT ROUND ( T3."CalcPrice", ' || :PriceDec || ') FROM OIVL T1, ' || :TargetITM || N' T_TargetITM, IVL1 T3, '
			|| ' (SELECT MIN("TransSeq") AS "TransSeq", "ItemCode", "LocCode" FROM OIVL GROUP BY "ItemCode", "LocCode") AS T4 '
			|| 'WHERE	T0."ItemCode" = T1."ItemCode" AND T0."WhsCode" = T1."LocCode" AND T0."ItemCode" = T_TargetITM."ItemCode"  '
			|| ' '||:strMAP_STD_PricesInit || '  AND T1."TransSeq" = T3."TransSeq" AND T1."TransSeq" = T4."TransSeq" AND T1."ItemCode" = T4."ItemCode" AND T1."LocCode" = T4."LocCode" '
			|| ' AND (T1."OutQty" <> 0 OR T1."TransType" in (13,14,15,16,19,21,60,67,165,166) '
			|| ' OR (T1."InQty" <> 0 AND T_TargetITM."EvalSystem" = ''S'')) AND T_TargetITM."InvntItem" = ''Y'' '
			|| ' AND EXISTS (SELECT 1 FROM "TMP_InsertInto_NewItemsInTargetITW" where "ItemCode" = T0."ItemCode" and "WhsCode" = T0."WhsCode") AND T0."AvgPrice" = 0)
			WHERE EXISTS (SELECT 1 FROM OIVL T1, ' || :TargetITM || N' T_TargetITM, IVL1 T3, (SELECT MIN("TransSeq") AS "TransSeq", "ItemCode", "LocCode" FROM OIVL GROUP BY "ItemCode", "LocCode") AS T4 '
			|| ' WHERE	T0."ItemCode" = T1."ItemCode" AND T0."WhsCode" = T1."LocCode" AND T0."ItemCode" = T_TargetITM."ItemCode"  ' 
			|| ' ' || :strMAP_STD_PricesInit || '  AND T1."TransSeq" = T3."TransSeq" AND T1."TransSeq" = T4."TransSeq" AND T1."ItemCode" = T4."ItemCode" AND T1."LocCode" = T4."LocCode" '
			|| ' AND (T1."OutQty" <> 0 OR T1."TransType" in (13,14,15,16,19,21,60,67,165,166) '
			|| ' OR (T1."InQty" <> 0 AND T_TargetITM."EvalSystem" = ''S''))AND T_TargetITM."InvntItem" = ''Y'' '
			|| ' AND EXISTS (SELECT 1 FROM "TMP_InsertInto_NewItemsInTargetITW" where "ItemCode" = T0."ItemCode" and "WhsCode" = T0."WhsCode") AND T0."AvgPrice" = 0)'; 
		
			if (debugOuptputOnly = 'Y') then	
				insert into TmSp_InsertIntoTargetITMandTargetITW_debug values (:str1);
			else
				exec (:str1);
			end if;
		end if;
		----------------- Sets starting prices for Target ITW ---------- END

		delete from "TMP_InsertInto_NewItemsInTargetITW";

		exec('insert into "TMP_InsertInto_NewItemsInTargetITW" ("ItemCode", "WhsCode") '
	  		 ||' select top ' || :InsertCount || ' A."ItemCode", A."WhsCode" from OITW A '
	   		 ||' where A."WhsCode" not in (select "WhsCode" from ' ||  :TargetITW || ' where "ItemCode" = A."ItemCode")');
		SELECT COUNT(*) INTO NeedInsert FROM "TMP_InsertInto_NewItemsInTargetITW";
	end while;

	if (:debugOuptputOnly = 'Y') then	
		select * from TmSp_InsertIntoTargetITMandTargetITW_debug;
		drop table TmSp_InsertIntoTargetITMandTargetITW_debug;
	end if;

	delete from "TMP_InsertInto_NewItemsInTargetITM";
	delete from "TMP_InsertInto_NewItemsInTargetITW";
end;


