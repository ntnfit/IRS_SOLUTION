-- B1 DEPENDS: AFTER:PT:PROCESS_END AFTER:SP:_TmSp_BootCreateGlobalTempTables2

CREATE PROCEDURE TmSp_IVSR_SupportInteractionRequiredQuery
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
openQty	 DECIMAL(21,6);
qtySum DECIMAL(21,6);
transNum int;
instance int;
lastTransNum int;
lastInstance int;
qty_decimals int;
sum_decimals int;
Main_Currency nvarchar(3);
DB_Impacted nvarchar(1);
Recalc_By_OILM nvarchar(1);
roundmthd nvarchar(1);
OCRNdeciamls int;
directrate nvarchar(1); 
Update_before nvarchar(1);
DBImpacted nvarchar(1);
stockissue nvarchar(1);
firstTranstionDate datetime;
cha TIMESTAMP;
rowcount int;
rownum int;
rowcount1 int;
rownum1 int;
BEGIN

	DELETE from "TMP_IVSR_SupInterReq_Results";

	firstTranstionDate := '1950-01-01'; 
	select "QtyDec" into qty_decimals from "OADM";
	select "SumDec" into sum_decimals from "OADM";
	select "MainCurncy" into main_currency from "OADM";
	select "RoundMthd" into roundmthd from "OADM";
	select "Decimals" into OCRNdeciamls from ocrn where "CurrCode" = :main_currency;
	select "DirectRate" into directrate from "OADM";

	select COUNT(1) INTO rowcount from livi where "UpdateDate" is not null;
	if :rowcount = 1 then 
		Update_before := 'Y';
	else 
		Update_before := 'N';
	end if;
	if :roundmthd = 'Y' and :OCRNdeciamls <> -1 then
		sum_decimals := :OCRNdeciamls;
	end if;

	-- ==================================================================================================
	-- ==================================================================================================
	-- ==================================================================================================
	--							SOURCE: IVI_SupportInteractionRequiredQuery.sql
	-- ==================================================================================================

	DB_Impacted := 'N';
	Recalc_By_OILM := 'Y';
		
	select COUNT(1) INTO rowcount
	from  "OITM" t0
	where "TreeType" in ( 'A', 'S' ) and "InvntItem" = 'Y';
	if :rowcount > 0 then 
		DB_Impacted := 'Y';
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'DB is impacted, please contact SAP support. ErrorCode = 310');
	end if;


	select COUNT(1) INTO rowcount
	from      "MRV1"
	where     "EvalSystem" = 'N';
	if :rowcount > 0 then 
		DB_Impacted := 'Y';
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'DB is impacted, please contact SAP support. ErrorCode = 401.');
	end if;

	SELECT COUNT(t0."MessageID") into rowcount 
	FROM   "OILM" t0 
	INNER JOIN "OITM" t1 ON t0."AccumType" = 1 AND t0."ItemCode" = t1."ItemCode" AND t1."InvntItem" = 'Y' 
		AND "LocCode" NOT IN (SELECT "WhsCode" FROM "OWHS" WHERE  "DropShip" = 'Y') 
		AND t0."Price" <> 0 
		AND IFNULL(t0."PriceCurr",'') <> :Main_Currency 
		AND IFNULL(t0."PriceRate",0) = 0 
		AND (t0."TransType" IN (-2,58) 
			AND t0."ActionType" <> 2);

	if :rowcount > 0 then
		DB_Impacted := 'Y';
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'DB is impacted, please contact SAP support. ErrorCode = 501.');
	end if;


	SELECT COUNT(t0."MessageID") INTO rowcount
	FROM   "OILM" t0 
	INNER JOIN "OITM" t1 ON t0."AccumType" = 1 AND t0."ItemCode" = t1."ItemCode" AND t1."InvntItem" = 'Y' 
		AND "LocCode" NOT IN (SELECT "WhsCode" FROM "OWHS" WHERE  "DropShip" = 'Y') 
		AND IFNULL(t0."TotalLC",0) = 0 -- TotalLC is zero and TotalSC/TotalFC not zero 
		AND (IFNULL(t0."TotalSC",0) <> 0 
			OR IFNULL(t0."TotalFC",0) <> 0) 
		AND (t0."TransType" IN (-2,58) 
			AND t0."ActionType" <> 2);

	IF :rowcount > 0 then 
		DB_Impacted := 'Y'; 
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'DB is impacted, please contact SAP support. ErrorCode = 502.');
	end if; 

	SELECT COUNT(t0."MessageID") INTO rowcount 
	FROM   "OILM" t0 
	INNER JOIN "OITM" t1 ON t0."AccumType" = 1 AND t0."ItemCode" = t1."ItemCode" AND t1."InvntItem" = 'Y' 
		AND "LocCode" NOT IN (SELECT "WhsCode" FROM "OWHS" WHERE  "DropShip" = 'Y') 
		AND IFNULL(t0."Price",0) <> 0 --- TotalLC is zero and Price/EffectQty not zero in OILM 
		AND IFNULL(t0."TotalLC",0) = 0 
		AND (t0."TransType" IN (-2,58) 
			AND t0."ActionType" <> 2) 
		AND Round(t0."Price" * t0."Quantity",:sum_decimals) <> 0;

	IF  :rowcount > 0 then 
		DB_Impacted := 'Y';
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'DB is impacted, please contact SAP support. ErrorCode = 503.');		     
	END if;


	--WKO
	SELECT count(T0."OrderNum") into rowcount
	FROM   "WKO1" T0
	INNER JOIN "OITM" T1 ON T0."ItemCode" = T1."ItemCode"
		and T1."InvntItem" = 'Y'
	WHERE  T0."OrderNum" not in ( Select "OrderNum" from   "OWKO" );

	if :rowcount > 0 then
		DB_Impacted := 'Y';
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'DB is impacted, please contact SAP support. ErrorCode = 15.');		     
	end if;
	--WOR
	SELECT count(T0."DocEntry") into rowcount
	FROM   "WOR1" T0
	INNER JOIN "OITM" T1 ON T0."ItemCode" = T1."ItemCode" and T1."InvntItem" = 'Y'
	WHERE  T0."DocEntry" not in ( Select "DocEntry" from   "OWOR" );

	if :rowcount > 0 then
		DB_Impacted := 'Y';
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'DB is impacted, please contact SAP support. ErrorCode = 17.');
	end if;

	--MRV
	SELECT count(T0."DocEntry") into rowcount
	FROM   "MRV1" T0
	INNER JOIN "OITM" T1 ON T0."ItemCode" = T1."ItemCode"
		and T1."InvntItem" = 'Y'
	WHERE  T0."DocEntry" not in ( Select "DocEntry" from "OMRV" );

	if :rowcount > 0 then
		DB_Impacted := 'Y';
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'DB is impacted, please contact SAP support. ErrorCode = 18.');
	end if;


	--WKO
	SELECT count(T0."OrderNum") into rowcount
	FROM   OWKO T0
	WHERE  T0."OrderNum" not in ( Select "OrderNum" from WKO1 );

	if :rowcount > 0 then 
		DB_Impacted := 'Y';
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'DB is impacted, please contact SAP support. ErrorCode = 34.');
	end if;

	--WOR
	select COUNT(t0."DocEntry") INTO rowcount
	from   "OWOR" t0 
	where  t0."DocEntry" not in (select "DocEntry" from   "WOR1") 
		and (exists (select 1 from   "IGN1" t1 where  t1."BaseEntry" = t0."DocEntry") 
			or exists (select 1 from   "IGE1" t2 where  t2."BaseEntry" = t0."DocEntry"));
		
	if :rowcount > 0 then 
		DB_Impacted := 'Y';
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'DB is impacted, please contact SAP support. ErrorCode = 35.');
	end if; 
	--MRV

	SELECT count(T0."DocEntry") into rowcount
	FROM   "OMRV" T0
	WHERE  T0."DocEntry" not in ( Select "DocEntry" from   "MRV1" );

	if :rowcount > 0 then 
		DB_Impacted := 'Y';
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'DB is impacted, please contact SAP support. ErrorCode = 37.');
	end if;

	select COUNT(t0."DocEntry") INTO rowcount
	from   "IGN1" t0
	inner join "OWOR" t1 on t0."BaseType" = 202 and t0."BaseEntry" = t1."DocEntry" and t0."BaseLine" is null and t0."ItemCode" <> t1."ItemCode";

	if :rowcount > 0 then 
		DB_Impacted := 'Y';
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'DB is impacted, please contact SAP support. ErrorCode = 180.');
	end if;

	select COUNT(t0."DocEntry") INTO rowcount
	from "IGE1" t0
	inner join "OWOR" t1 on t0."BaseType" = 202 and t0."BaseEntry" = t1."DocEntry" and t0."BaseLine" is null and t0."ItemCode" <> t1."ItemCode";

	if :rowcount > 0 then 
		DB_Impacted := 'Y';
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'DB is impacted, please contact SAP support. ErrorCode = 181.');
	end if;

	select count(t0."DocEntry") into rowcount 
	from "IGN1" t0
	inner join "WOR1" t1 on t0."BaseType" = 202 and t0."BaseEntry" = t1."DocEntry" and t0."BaseLine" = t1."LineNum" and t0."ItemCode" <> t1."ItemCode" ;

	if :rowcount > 0 THEN 
		DB_Impacted := 'Y';
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'DB is impacted, please contact SAP support. ErrorCode = 182.');
	end IF;

	select COUNT(t0."DocEntry") INTO rowcount
	from "IGE1" t0
	inner join "WOR1" t1 on t0."BaseType" = 202 and t0."BaseEntry" = t1."DocEntry" and t0."BaseLine" = t1."LineNum" and t0."ItemCode" <> t1."ItemCode";
	if :rowcount > 0 then 
		DB_Impacted := 'Y';
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'DB is impacted, please contact SAP support. ErrorCode = 183.');
	end if;

	Select count(t0."CreateDate") INTO rowcount 
	from   "OMRV" t0
	where  t0."CreateDate" > CURRENT_TIMESTAMP;

	if :rowcount > 0 THEN
		DB_Impacted := 'Y';
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'DB is impacted, please contact SAP support. ErrorCode = 248.');
	end IF;

	Select count(t0."CreateDate") into rowcount 
	from   "OWOR" t0
	where  t0."CreateDate" > CURRENT_TIMESTAMP;

	if :rowcount > 0 THEN
		DB_Impacted := 'Y';
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'DB is impacted, please contact SAP support. ErrorCode = 249.');
	end IF;

	Select count(t0."CreateDate") into rowcount
	from "OWKO" t0
	where t0."CreateDate" > CURRENT_TIMESTAMP or t0."FinishDate" > CURRENT_TIMESTAMP;

	if :rowcount > 0 THEN
		DB_Impacted := 'Y';
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'DB is impacted, please contact SAP support. ErrorCode = 250.');
	end IF;

	SELECT COUNT(T0."OrderNum") INTO rowcount
	FROM  "OWKO" T0
	LEFT OUTER JOIN "OINM" T1 ON T0."OrderNum" = T1."CreatedBy"
		AND T1."TransType" = 68
	LEFT OUTER JOIN "OJDT" T2 ON T0."OrderNum" = T2."CreatedBy"
		AND T2."TransType" = N'68'
	WHERE  T1."TransSeq" IS NULL AND T2."TransId" IS NULL and T0."Status" = 'E' and t0."FinishDate" IS NULL;

	if :rowcount > 0 THEN
		DB_Impacted := 'Y';
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'DB is impacted, please contact SAP support. ErrorCode = 270.');
	end IF;

	SELECT COUNT(t1."ItemCode") INTO rowcount 
	FROM   "WOR1" t1 
	INNER JOIN "OITM" t2 ON t1."ItemCode" = t2."ItemCode" 
	INNER JOIN "OITW" t3 ON t1."ItemCode" = t3."ItemCode" AND t1."wareHouse" = t3."WhsCode" 
	WHERE t2."InvntItem" = 'N' AND IFNULL(t1."CompTotal",0) = 0 AND Round((CASE WHEN (SELECT "PriceSys" FROM "OADM") = 'N' 
																			THEN t2."AvgPrice" 
																			ELSE t3."AvgPrice" 
																			END ) * t1."IssuedQty", :sum_decimals) <> 0;
																			
	IF :rowcount > 0 then 
		DB_Impacted := 'Y'; 
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'DB is impacted, please contact SAP support. ErrorCode = 504.');
	END if; 

	SELECT COUNT(t1."ItemCode") INTO rowcount
	FROM   "UWOR1" t1 
	INNER JOIN "OITM" t2 ON t1."ItemCode" = t2."ItemCode" 
	INNER JOIN "OITW" t3 ON t1."ItemCode" = t3."ItemCode" AND t1."wareHouse" = t3."WhsCode" 
	WHERE t2."InvntItem" = 'N' AND IFNULL(t1."CompTotal",0) = 0 AND Round((CASE WHEN (SELECT "PriceSys" FROM "OADM") = 'N' 
																			THEN t2."AvgPrice" 
																			ELSE t3."AvgPrice" 
																			END ) * t1."IssuedQty", :sum_decimals) <> 0;

	IF :rowcount > 0 then 
		DB_Impacted := 'Y'; 
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'DB is impacted, please contact SAP support. ErrorCode = 505.');
	END if;

--1--

------------------------------------------------
	select count("MessageID") into rowcount FROM  oilm WHERE  oilm."BaseAbsEnt" <> oilm."AppObjAbs" AND oilm."BaseType" = 202;
	if :rowcount > 0 then	
		DB_Impacted := 'Y';
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'TODO[[[db is impacted, please contact SAP support. ErrorCode = 506.]]]');
	end if;
	
	select count(T0."DocEntry") into rowcount from WOR1 t0 
	join oitm t1 on t0."ItemCode" = t1."ItemCode"
	left join oilm t2 on t0."DocEntry" = t2."DocEntry" and t2."TransType" = 202 and t0."LineNum" = t2."DocLineNum"
	where t1."InvntItem" = 'Y' and t0."DocEntry" in (select "DocEntry" from OILM where "TransType"=202) 
		and t2."MessageID" is null;
		
	if :rowcount > 0 then
		DB_Impacted := 'Y';
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'TODO[[[db is impacted, please contact SAP support. ErrorCode = 507.]]]');
	END if;
	
	
	select count("MessageID") into rowcount
	FROM OILM t0 
	WHERE t0."TransType" = 202 and t0."Quantity" <> t0."EffectQty"
		and not exists (SELECT 1 FROM OILM t1 WHERE t1."MessageID" < t0."MessageID"
			and T1."TransType" in (59,60) and T1."AppObjAbs" = t0."DocEntry" and T1."AppObjLine" = t0."DocLineNum") 
		and not (T0."DocLineNum" = -1 and T0."EffectQty" = 0) 
		and t0."AccumType" in (2,3);

	if :rowcount > 0 then
		DB_Impacted := 'Y';
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'TODO[[[db is impacted, please contact SAP support. ErrorCode = 508.]]]');
	END if;


	select count(1) into rowcount from DLN1 T1
	Join OILM T0 on T0."TransType" = 15 and T1."DocEntry" = T0."DocEntry" and T1."LineNum" = T0."DocLineNum"
	JOIN DLN14 T3 on T3."DocEntry" = T1."DocEntry" and T3."LineNum" = T1."LineNum"
	where (T1."TreeType" <> 'A' or T0."TreeType"<> 'A') and (T0."VersionNum"<'08.81.313.05' or T0."VersionNum" IS NULL)
	UNION 
	select count(1) from INV1 T1
	Join OILM T0 on  T0."TransType" = 13 and T1."DocEntry" = T0."DocEntry" and T1."LineNum" = T0."DocLineNum"
	JOIN INV14 T3 on T3."DocEntry" = T1."DocEntry" and T3."LineNum" = T1."LineNum"
	where (T1."TreeType"<> 'A' or T0."TreeType"<> 'A') and (T0."VersionNum"='08.81.313.05' or T0."VersionNum"='08.80.239.21')
	UNION
	select count(1) from RDN1 T1
	Join OILM T0 on  T0."TransType" = 16 and T1."DocEntry" = T0."DocEntry" and T1."LineNum" = T0."DocLineNum"
	JOIN RDN14 T3 on T3."DocEntry" = T1."DocEntry" and T3."LineNum" = T1."LineNum"
	where (T1."TreeType"<> 'A' or T0."TreeType"<> 'A') and (T0."VersionNum"='08.81.313.05' or T0."VersionNum"='08.80.239.21')
	UNION
	select count(1) from RIN1 T1
	Join OILM T0 on  T0."TransType" = 14 and T1."DocEntry" = T0."DocEntry" and T1."LineNum" = T0."DocLineNum"
	JOIN RIN14 T3 on T3."DocEntry" = T1."DocEntry" and T3."LineNum" = T1."LineNum"
	where (T1."TreeType"<> 'A' or T0."TreeType"<> 'A') and (T0."VersionNum"='08.81.313.05' or T0."VersionNum"='08.80.239.21')
	UNION
	select count(1) from CSI1 T1
	Join OILM T0 on  T0."TransType" = 165 and T1."DocEntry" = T0."DocEntry" and T1."LineNum" = T0."DocLineNum"
	JOIN CSI14 T3 on T3."DocEntry" = T1."DocEntry" and T3."LineNum" = T1."LineNum"
	where (T1."TreeType"<> 'A' or T0."TreeType" <> 'A') and (T0."VersionNum"='08.81.313.05' or T0."VersionNum"='08.80.239.21')
	UNION
	select count(1) from CSV1 T1
	Join OILM T0 on T0."TransType" = 166 and T1."DocEntry" = T0."DocEntry" and T1."LineNum" = T0."DocLineNum"
	JOIN CSV14 T3 on T3."DocEntry" = T1."DocEntry" and T3."LineNum" = T1."LineNum"
	where (T1."TreeType"<> 'A' or T0."TreeType"<> 'A') and (T0."VersionNum"='08.81.313.05' or T0."VersionNum"='08.80.239.21');

	if :rowcount > 0 then
		DB_Impacted := 'Y';
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'TODO[[[db is impacted, please contact SAP support. ErrorCode = 509.]]]');
	END if;
	
	select count("MessageID") into rowcount from OILM where "Quantity" < 0 and "DocLineNum" <> -1 and 
		"TransType" = 202 and "AccumType" = 3;
		
	if :rowcount > 0 then
		DB_Impacted := 'Y';
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'TODO[[[db is impacted, please contact SAP support. ErrorCode = 510.]]]');
	END if;

	select count(1) into rowcount 
	from (WKO1 t0 LEFT OUTER JOIN OILM t1 
		on t0."OrderNum" = t1."DocEntry" and t0."LineID" = t1."DocLineNum" and t0."ItemCode" = t1."ItemCode" 
			and	((t0."Quantity" > 0 and t1."ActionType" = 1) or (t0."Quantity" < 0 and t1."ActionType" = 2)) 
			and	t1."TransType" = 68) 
		where t1."MessageID" is null
	and exists (select count(1) 
				from OILM t2 
				where t2."DocEntry" = t0."OrderNum" and t2."DocLineNum" = t0."LineID" and t2."TransType" = 68);

	if :rowcount > 0 then
		DB_Impacted := 'Y';
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'TODO[[[db is impacted, please contact SAP support. ErrorCode = 511.]]]');
	End if;


	-- check if any WKO1 order has quantity inconsistency with OILM transtype=68 and actiontype (2 for quantity>0 and 1 for quantity<0)
	select count(1) into rowcount 
	from WKO1 t0 inner join 
		(select top 1 * from OILM where "TransType" = 68 order by "MessageID", "DocEntry", "DocLineNum") t1
			on t0."OrderNum" = t1."DocEntry" and t0."LineID" = t1."DocLineNum" 
				and t0."ItemCode" = t1."ItemCode" 
				and ((t0."Quantity" > 0 and t1."ActionType" = 2) or (t0."Quantity" < 0 and t1."ActionType" = 1));
		
	if :rowcount > 0 then
		DB_Impacted := 'Y';
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'TODO[[[db is impacted, please contact SAP support. ErrorCode = 512.]]]');
	End if;

	-- check for some work orders in OILM that are having only one action type (in or out)
	select count(1) into rowcount from (select count(*) from OILM where "TransType" = 68 group by "DocEntry", "DocLineNum"
	having COUNT(distinct "ActionType") = 1);
	
	--order by DocEntry, DocLineNum
	if :rowcount > 0 then
		DB_Impacted := 'Y';
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'TODO[[[db is impacted, please contact SAP support. ErrorCode = 513.]]]');
	End if;

	-- Check Supp_518_Missing warehouse
	-- Solution: add missing warehouse to Item master data or OITW, see IM 100011 / 2014 / IVI Fails after OITW deletion
	SELECT count(1) INTO rowcount FROM OIVL l FULL OUTER JOIN OITW w ON l."ItemCode" = w."ItemCode" AND l."LocCode" = w."WhsCode" WHERE w."WhsCode" IS NULL;
	if :rowcount > 0 then
		DB_Impacted := 'Y';
		INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'TODO[[[db is impacted, please contact SAP support. ErrorCode = 518.]]]');
	End if;

------------------------------------------End of Recalculation by OILM & recalculation by UILM _ shared part

--1--
		  
		  
	IF :Recalc_By_OILM = 'Y' THEN 
		  
		SELECT COUNT(t0."MessageID") INTO rowcount 
		FROM   "OILM" t0 
		INNER JOIN "OILM" t1 
			ON t0."BaseType" = t1."TransType" 
				AND t0."BaseAbsEnt" = t1."DocEntry" 
				AND t0."BaseLine" = t1."DocLineNum" 
				AND t0."ItemCode" = t1."ItemCode" 
				AND t0."AccumType" = 1 
				AND t1."AccumType" = 1 
				AND t0."Quantity" > t1."Quantity" 
				AND NOT (t0."TransType" = 67 
					AND t0."BaseType" = 20) 
				AND NOT (t0."BaseType" in (163, 164,165,166))
				AND NOT (t0."TransType" in (163, 164,165,166));

		if :rowcount > 0 then 
			INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'Db is impacted, please contact SAP support. ErrorCode = 402.');
		END if;
				
		SELECT COUNT("DocEntry") INTO rowcount 
		FROM   "OILM" 
		WHERE  "TransType" IN (59,60) AND "BaseType" = 202 AND "ApplObj" IS NULL;
		
		IF :rowcount > 0 then 
			INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'Db is impacted, please contact SAP support. ErrorCode = 405.');
		END if; 
		
		select COUNT("VersionNum") INTO rowcount 
		from   "OILM" 
		where  '08.80.000.00' <= IFNULL("VersionNum",'') 
			and IFNULL("VersionNum",'') < '08.80.228.09';
			
		if :Update_before = 'Y' and :rowcount > 0 then 
			select count("TransSeq") into rownum
			from   "OIVL" 
			where  "TransType" = 68 and "InQty" <> 0 and "OutQty" <> 0;
			if :rownum > 0 then 
				INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'Db is impacted, please contact SAP support. ErrorCode = 408.');
			end if;	     
		end if;            
	
		--PASSED

		DB_Impacted := 'N';
		DELETE FROM "TMP_IVSR_SupInterReq_TotalLcZeroPQNotZero";
		
	    insert into "TMP_IVSR_SupInterReq_TotalLcZeroPQNotZero"
		SELECT t0.*
		FROM   "OILM" t0 
		INNER JOIN "OITM" t1 ON t0."AccumType" = 1 
			AND t0."ItemCode" = t1."ItemCode" 
			AND  t1."InvntItem" = 'Y' 
			AND "LocCode" NOT IN (SELECT "WhsCode" FROM "OWHS" WHERE  "DropShip" = 'Y') 
			AND (IFNULL(t0."Price",0) <> 0 
				AND IFNULL(t0."TotalLC",0) = 0 
				AND (t0."TransType" IN (18,19,20,163,164,69) 
					OR (t0."TransType" = 21 
						AND t0."BaseType" <> 20) 
					OR (t0."TransType" = 59 
						AND t0."BaseType" <> 202) 
					OR (t0."TransType" IN (/*-2,58,--in 503*/310000001,10000071) 
						AND t0."ActionType" <> 2)) 
				AND Round(t0."Price" * t0."Quantity", :sum_decimals) <> 0); 
				
		SELECT Count(1) INTO rowcount
		FROM   "TMP_IVSR_SupInterReq_TotalLcZeroPQNotZero";

	
		IF :rowcount <> 0 THEN 

			SELECT COUNT(t0."MessageID") INTO rownum 
			FROM   "TMP_IVSR_SupInterReq_TotalLcZeroPQNotZero" t0 
			INNER JOIN "OPCH" t1 ON t0."TransType" = 18 
				AND t0."DocEntry" = t1."DocEntry" 
				AND ((IFNULL(t1."VersionNum",'') >= '06.80.200.00' 
						AND IFNULL(t1."VersionNum",'') < '06.80.999.00') --05A SP1   
					OR (IFNULL(t1."VersionNum",'') >= '07.40.000.00' 
						AND IFNULL(t1."VersionNum",'') < '07.40.999.00')--05B    
					OR (IFNULL(t1."VersionNum",'') >= '08.00.000.00')) -->07version   
			inner join "PCH1" t2 on t0."DocEntry" = t2."DocEntry"
				and t0."DocLineNum" = t2."LineNum"
				and Round(t2."Price" * t2."Quantity",:sum_decimals) <> 0;

			IF :rownum > 0 then 
				DB_Impacted := 'Y'; 
			END if;
			
			SELECT COUNT(t0."MessageID") INTO rownum 
			FROM   "TMP_IVSR_SupInterReq_TotalLcZeroPQNotZero" t0 
			INNER JOIN "ORPC" t1 
				ON t0."TransType" = 19 
				AND t0."DocEntry" = t1."DocEntry" 
				AND ((IFNULL(t1."VersionNum",'') >= '06.80.200.00' 
						AND IFNULL(t1."VersionNum",'') < '06.80.999.00') --05A SP1   
					OR (IFNULL(t1."VersionNum",'') >= '07.40.000.00' 
						AND IFNULL(t1."VersionNum",'') < '07.40.999.00')--05B    
					OR (IFNULL(t1."VersionNum",'') >= '08.00.000.00')) -->07version   
			inner join "RPC1" t2 on t0."DocEntry" = t2."DocEntry"
				and t0."DocLineNum" = t2."LineNum"
				and Round(t2."Price" * t2."Quantity",:sum_decimals) <> 0;
				
			IF :rownum > 0 then 
				DB_Impacted := 'Y'; 
			END if;

			--OPDN 
			SELECT COUNT(t0."MessageID") INTO rownum 
			FROM   "TMP_IVSR_SupInterReq_TotalLcZeroPQNotZero" t0 
			INNER JOIN "OPDN" t1 ON t0."TransType" = 20 
				AND t0."DocEntry" = t1."DocEntry" 
				AND ((IFNULL(t1."VersionNum",'') >= '06.80.200.00' 
						AND IFNULL(t1."VersionNum",'') < '06.80.999.00') --05A SP1   
					OR (IFNULL(t1."VersionNum",'') >= '07.40.000.00' 
						AND IFNULL(t1."VersionNum",'') < '07.40.999.00')--05B    
					OR (IFNULL(t1."VersionNum",'') >= '08.00.000.00')) -->07version  
			inner join "PDN1" t2 on t0."DocEntry" = t2."DocEntry"
				and t0."DocLineNum" = t2."LineNum"
				and Round(t2."Price" * t2."Quantity",:sum_decimals) <> 0;
				
			IF :rownum > 0 then	 
				DB_Impacted := 'Y';
			END if;

			--OCPI 
			SELECT COUNT(t0."MessageID") INTO rownum 
			FROM   "TMP_IVSR_SupInterReq_TotalLcZeroPQNotZero" t0 
			INNER JOIN "OCPI" t1 ON t0."TransType" = 163 
				AND t0."DocEntry" = t1."DocEntry" 
				AND ((IFNULL(t1."VersionNum",'') >= '06.80.200.00' 
					AND IFNULL(t1."VersionNum",'') < '06.80.999.00') --05A SP1   
					OR (IFNULL(t1."VersionNum",'') >= '07.40.000.00' 
					AND IFNULL(t1."VersionNum",'') < '07.40.999.00')--05B    
					OR (IFNULL(t1."VersionNum",'') >= '08.00.000.00'));

			IF :rownum > 0 then	 
				DB_Impacted := 'Y';
			END if;

			--OCPV 
			SELECT COUNT(t0."MessageID")  INTO rownum 
			FROM   "TMP_IVSR_SupInterReq_TotalLcZeroPQNotZero" t0 
			INNER JOIN "OCPV" t1 ON t0."TransType" = 164 
				AND t0."DocEntry" = t1."DocEntry" 
				AND ((IFNULL(t1."VersionNum",'') >= '06.80.200.00' 
						AND IFNULL(t1."VersionNum",'') < '06.80.999.00') --05A SP1   
					OR (IFNULL(t1."VersionNum",'') >= '07.40.000.00' 
						AND IFNULL(t1."VersionNum",'') < '07.40.999.00')--05B    
					OR (IFNULL(t1."VersionNum",'') >= '08.00.000.00'));

			IF :rownum > 0 then	 
				DB_Impacted := 'Y';
			END if; 

			--OIPF 
			SELECT COUNT(t0."MessageID") INTO rownum 
			FROM   "TMP_IVSR_SupInterReq_TotalLcZeroPQNotZero" t0 
			INNER JOIN "OIPF" t1 ON t0."TransType" = 69 
				AND t0."DocEntry" = t1."DocEntry" 
				AND ((IFNULL(t1."VersionNum",'') >= '06.80.200.00' 
						AND IFNULL(t1."VersionNum",'') < '06.80.999.00') --05A SP1   
					OR (IFNULL(t1."VersionNum",'') >= '07.40.000.00' 
						AND IFNULL(t1."VersionNum",'') < '07.40.999.00')--05B    
					OR (IFNULL(t1."VersionNum",'') >= '08.00.000.00'));

			IF :rownum > 0 then	 
				DB_Impacted := 'Y';
			END if;

			--ORPD 
			SELECT COUNT(t0."MessageID") INTO rownum  
			FROM   "TMP_IVSR_SupInterReq_TotalLcZeroPQNotZero" t0 
			INNER JOIN "ORPD" t1 ON t0."TransType" = 21 
				AND t0."DocEntry" = t1."DocEntry" 
				AND ((IFNULL(t1."VersionNum",'') >= '06.80.200.00' 
						AND IFNULL(t1."VersionNum",'') < '06.80.999.00') --05A SP1   
					OR (IFNULL(t1."VersionNum",'') >= '07.40.000.00' 
						AND IFNULL(t1."VersionNum",'') < '07.40.999.00')--05B    
					OR (IFNULL(t1."VersionNum",'') >= '08.00.000.00'))
			inner join "RPD1" t2 on t0."DocEntry" = t2."DocEntry"
				and t0."DocLineNum" = t2."LineNum"
				and Round(t2."Price" * t2."Quantity") <> 0;

			IF :rownum > 0 then	 
				DB_Impacted := 'Y';
			END if;

			--oign 
			SELECT COUNT(t0."MessageID") INTO rownum  
			FROM   "TMP_IVSR_SupInterReq_TotalLcZeroPQNotZero" t0 
			INNER JOIN "OIGN" t1 ON t0."TransType" = 59 
				AND t0."DocEntry" = t1."DocEntry" 
				AND ((IFNULL(t1."VersionNum",'') >= '06.80.200.00' 
						AND IFNULL(t1."VersionNum",'') < '06.80.999.00') --05A SP1   
					OR (IFNULL(t1."VersionNum",'') >= '07.40.000.00' 
						AND IFNULL(t1."VersionNum",'') < '07.40.999.00')--05B    
					OR (IFNULL(t1."VersionNum",'') >= '08.00.000.00'))
			inner join "IGN1" t2 on t0."DocEntry" = t2."DocEntry"
				and t0."DocLineNum" = t2."LineNum"
				and Round(t2."Price" * t2."Quantity",:sum_decimals) <> 0;

			IF :rownum > 0 then	 
				DB_Impacted := 'Y';
			END if;

			--no version check for -2 ,58 
			SELECT COUNT(t0."MessageID") INTO rownum
			FROM   "TMP_IVSR_SupInterReq_TotalLcZeroPQNotZero" t0 
			WHERE  t0."TransType" IN (/*-2,58,--in 503*/310000001,10000071) 
				AND t0."ActionType" <> 2;
				
			IF :rownum > 0 then	 
				DB_Impacted := 'Y';
			END if; 
		END IF; --rownum <> 0

		DELETE FROM "TMP_IVSR_SupInterReq_TotalLcZeroPQNotZero"; 

		IF :DB_Impacted = 'Y' then 
			INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'Db is impacted, please contact SAP support. ErrorCode = 409.');
		END if;

        SELECT COUNT(t0."MessageID") INTO rowcount 
		FROM   "OILM" t0 
		INNER JOIN "OITM" t1 ON t0."AccumType" = 1 
			AND t0."ItemCode" = t1."ItemCode" 
			AND t1."InvntItem" = 'Y' 
			AND "LocCode" NOT IN (SELECT "WhsCode" FROM "OWHS" WHERE  "DropShip" = 'Y') 
			AND IFNULL(t0."Price",0) <> 0  
			AND IFNULL(t0."PriceCurr",'') <> :Main_Currency
			AND IFNULL(t0."PriceRate",0) = 0 
			AND (t0."TransType" IN (18,19,20,21,163,164,69) 
				OR (t0."TransType" = 59	AND t0."BaseType" <> 202) 
				OR (t0."TransType" IN (/*-2,58,--in 501*/310000001,10000071) 
					AND t0."ActionType" <> 2))
			AND Ifnull(T0."TotalLC", 0) <> Ifnull(T0."ExpensesLC", 0);

		IF :rowcount > 0 then 
			INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'Db is impacted, please contact SAP support. ErrorCode = 410.');
		END if;

		SELECT COUNT(t0."MessageID") INTO rowcount 
		FROM   "OILM" t0 
		INNER JOIN "OITM" t1 ON t0."AccumType" = 1 
			AND t0."ItemCode" = t1."ItemCode" 
			AND t1."InvntItem" = 'Y' 
			AND "LocCode" NOT IN (SELECT "WhsCode" FROM "OWHS" WHERE  "DropShip" = 'Y') 
			AND	(IFNULL(t0."TotalLC",0) = 0 AND (IFNULL(t0."TotalFC",0) <> 0) 
				AND (t0."TransType" IN (18,19,20,21,163,164,69) 
					OR (t0."TransType" = 59 
					AND t0."BaseType" <> 202) 
					OR (t0."TransType" IN (/*-2,58,--in 502*/310000001,10000071) 
						AND t0."ActionType" <> 2)));

		IF :rowcount > 0 then 
			INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'Db is impacted, please contact SAP support. ErrorCode = 411.');
		END if;

		SELECT COUNT(t0."MessageID") INTO rowcount
		FROM   "OILM" t0 
		INNER JOIN "OITM" t1 ON t0."AccumType" = 1 
			AND t0."ItemCode" = t1."ItemCode" 
			AND t1."InvntItem" = 'Y' 
			AND "LocCode" NOT IN (SELECT "WhsCode" FROM "OWHS" WHERE  "DropShip" = 'Y') 
			AND (IFNULL(t0."LocCode",'') = '' 
				OR (t0."CreateDate" > CURRENT_TIMESTAMP));
				
		IF :rowcount > 0 then 
			INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'Db is impacted, please contact SAP support. ErrorCode = 413.');
		END if; 

		SELECT COUNT(t0."MessageID") INTO rowcount
		FROM   "OILM" t0 
		INNER JOIN "CPV1" t1 ON t0."TransType" = 164 
			AND t0."DocEntry" = t1."DocEntry" 
			AND t0."DocLineNum" = t1."LineNum" 
			AND (IFNULL(t0."BaseAbsEnt", -1) <> IFNULL(t1."BaseEntry", -1) 
				OR IFNULL(t0."BaseType", -1) <> IFNULL(t1."BaseType", -1));
				
		IF :rowcount > 0 then 
			INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'Db is impacted, please contact SAP support. ErrorCode = 414.');
		END if;

		SELECT COUNT(t0."MessageID") INTO rowcount
		FROM   "OILM" t0 
		INNER JOIN "CSV1" t1 ON t0."TransType" = 166 
			AND t0."DocEntry" = t1."DocEntry" 
			AND t0."DocLineNum" = t1."LineNum" 
			AND (IFNULL(t0."BaseAbsEnt", -1) <> IFNULL(t1."BaseEntry", -1) 
				OR IFNULL(t0."BaseType", -1) <> IFNULL(t1."BaseType", -1));
			
		IF :rowcount > 0 then 
			INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'Db is impacted, please contact SAP support. ErrorCode = 415.');
		END if;

		SELECT COUNT("VersionNum") INTO rowcount 
		FROM   "OILM" 
		WHERE  "VersionNum" >= '08.80.000.00' 
			AND "VersionNum" <= '08.80.228.09';  
			
		IF :rowcount > 0 then 

			SELECT count(t0."DocEntry") INTO rownum
			FROM   "CPI1" t0, 
			"CPI1" t1 
			WHERE  t0."DocEntry" = t1."DocEntry" 
				AND t0."LineNum" = t1."CiOppLineN" 
				AND ( t0."BaseType" = ( -1 ) 
					OR t0."BaseType" IS NULL ) 
				AND (t1."BaseType" IS NOT NULL 
					AND t1."BaseType" > ( -1 ) ); 
					
			IF :rownum > 0 then 
				DB_Impacted := 'Y'; 
				INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'Db is impacted, please contact SAP support. ErrorCode = 420.');
			END if;

			SELECT count(t0."DocEntry") INTO rownum
			FROM   "CPV1" t0, 
			"CPV1" t1 
			WHERE  t0."DocEntry" = t1."DocEntry" 
				AND t0."LineNum" = t1."CiOppLineN" 
				AND ( t0."BaseType" = ( -1 ) 
					OR t0."BaseType" IS NULL ) 
				AND (t1."BaseType" IS NOT NULL 
					AND t1."BaseType" > ( -1 ) ); 
					
			IF :rownum > 0 then 
				DB_Impacted := 'Y'; 
				INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'Db is impacted, please contact SAP support. ErrorCode = 421.');
			END if;

			SELECT count(t0."DocEntry") INTO rownum
			FROM   "CSI1" t0, "CSI1" t1 
			WHERE  t0."DocEntry" = t1."DocEntry" 
				AND t0."LineNum" = t1."CiOppLineN" 
				AND (t0."BaseType" = ( -1 ) 
					OR t0."BaseType" IS NULL) 
				AND (t1."BaseType" IS NOT NULL 
				AND t1."BaseType" > ( -1 ));

			IF :rownum > 0 then 
				DB_Impacted := 'Y'; 
				INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'Db is impacted, please contact SAP support. ErrorCode = 422.');
			END if;

			SELECT count(t0."DocEntry") INTO rownum
			FROM   "CSV1" t0, "CSV1" t1 
			WHERE  t0."DocEntry" = t1."DocEntry" 
				AND t0."LineNum" = t1."CiOppLineN" 
				AND (t0."BaseType" = ( -1 ) 
					OR t0."BaseType" IS NULL ) 
				AND (t1."BaseType" IS NOT NULL 
					AND t1."BaseType" > ( -1 ) ); 

			IF :rownum > 0 then 
				DB_Impacted := 'Y'; 
				INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'Db is impacted, please contact SAP support. ErrorCode = 423.');
			END if; 
		END if; --rowcount > 0


		select COUNT(T0."MessageID") INTO rownum 
		from "OILM" T0
		left join "OILM" T1	on T0."BaseAbsEnt" = T1."DocEntry" and T0."BaseType" = T1."TransType"
		where (T0."BaseAbsEnt" > 0 or T0."BaseType" > 0) and T0."BaseType" not in (17,22,23,202,67,540000006,1250000001)
			AND T0."AccumType" = 1 AND T0."ActionType" not in (3,4)
			AND NOT (T0."BaseType"=18 AND T0."BaseAbsEnt" IN (select "DocEntry" from "OPCH" where "UpdInvnt" = 'O')) --AP Res. Inv -> XXX
			AND NOT (T0."BaseType"=13 AND T0."BaseAbsEnt" IN (select "DocEntry" from "OINV" where "UpdInvnt" = 'C')) --AR Res. Inv -> XXX
			AND NOT (T0."BaseType"=18 AND T0."TransType" = 20) --Res. Inv -> GRPO (but Res. Inv was deleted)
			AND NOT (T0."BaseType"=13 AND T0."TransType" = 15) --Res. Inv -> DLN (but Res. Inv was deleted) 
			AND NOT (T0."BaseType"=163 AND T0."TransType" = 20) --Res. Inv -> CI -> GRPO (GRPO is calculated as not based within IVI/IVSR)  
			AND NOT (T0."BaseType"=165 AND T0."TransType" = 15) --Res. Inv -> CI -> DLN (DLN is calculated as not based within IVI/IVSR)   
			AND NOT (T0."BaseType" in (67,20,59) AND T0."TransType" = 67) -- WTR is handled as non based doc.
			AND NOT (T0."BaseType"=234000031 AND T0."TransType" = 16) -- RRR->RDN
			AND NOT (T0."BaseType"=234000031 AND T0."TransType" = 14) -- RRR->RIN
			AND NOT (T0."BaseType"=234000032 AND T0."TransType" = 19) -- RRR->RPC
			AND NOT (T0."BaseType"=234000032 AND T0."TransType" = 21) -- RRR->RPD
			AND t0."TransType" <> 202 --Production order create/update - basetype=1 by known bug
			AND (IFNULL(T1."DocEntry",0) = 0 or IFNULL(T1."TransType",0) = 0);
			
		IF :rownum > 0 then 
			DB_Impacted := 'Y'; 
			INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'DB is impacted, please contact SAP support. ErrorCode = 425.');
		END if;

		--2--
		SELECT count("MessageID") into rownum from oilm	where "TransType" = 69 and ifnull("TotalLC",0) = 0 and ifnull("VersionNum", '') < '08.00.000.00';

		IF :rownum > 0 then
			DB_Impacted := 'Y'; 
			INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'TODO[[[db is impacted, please contact SAP support. ErrorCode = 426.]]]');
		end if;
		
		--Special Treatment for Supp_416_419 as IVI/IVSR has different condition

		--- Assembly BoM subline numbers were wrongly updated for several scenarios.  
		--- Assembly BoM qty could be wrongly calculated.  
		--- As a result of issue reported in Task 13010 
		SELECT count("VersionNum") into rowcount1 
		FROM   "OILM" 
		WHERE  "VersionNum" >= '08.80.000.00' 
			AND "VersionNum" <= '08.80.228.09';
		IF :rowcount1 > 0 then 
			SELECT COUNT(t0."DocEntry") INTO rownum1 
			FROM   "CSI1" t0 
			INNER JOIN "OCSI" t1 ON t0."DocEntry" = t1."DocEntry" 
			INNER JOIN "OITM" t2 ON t0."ItemCode" = t2."ItemCode"
			WHERE  IFNULL(t1."VersionNum", '') < '08.80.000.00' 
			AND t1."Instance" = 0     
			AND t2."TreeType" = 'A'; 

			IF :rownum1 > 0 then 
				DB_Impacted := 'Y'; 
				INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'TODO[[[db is impacted, please contact SAP support. ErrorCode = 416.]]]');
			END if;

			SELECT COUNT(t0."DocEntry") INTO rownum1  
			FROM   "CSV1" t0 
			INNER JOIN "OCSV" t1 ON t0."DocEntry" = t1."DocEntry" 
			INNER JOIN "OITM" t2 ON t0."ItemCode" = t2."ItemCode"
			WHERE IFNULL(t1."VersionNum", '') < '08.80.000.00' 
				AND t1."Instance" = 0     
				AND t2."TreeType" = 'A';  

			IF :rownum1 > 0 then 
				DB_Impacted := 'Y'; 
				INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'TODO[[[db is impacted, please contact SAP support. ErrorCode = 417.]]]');
			END if;

			SELECT COUNT(t0."DocEntry") into rownum1  
			FROM   "CSI1" t0 
			INNER JOIN "OCSI" t1 ON t0."DocEntry" = t1."DocEntry" 
			INNER JOIN "OITM" t2 ON t0."ItemCode" = t2."ItemCode"
			WHERE IFNULL(t1."VersionNum", '') < '08.80.000.00' 
				AND t1."isIns" = 'Y' 
				AND ( t2."InvntItem" = 'Y' 
					OR t2."TreeType" = 'A' ) 
				AND t1."Instance" = 0 ; 

			IF :rownum1 > 0 then 
				DB_Impacted := 'Y'; 
				INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'TODO[[[db is impacted, please contact SAP support. ErrorCode = 418.]]]');
			END if;

			SELECT COUNT(t0."DocEntry") into rownum1  
			FROM   "CPI1" t0 
			INNER JOIN "OCPI" t1 ON t0."DocEntry" = t1."DocEntry" 
			INNER JOIN "OITM" t2 ON t0."ItemCode" = t2."ItemCode"
			WHERE  IFNULL(t1."VersionNum", '') < '08.80.000.00' 
				AND t1."isIns" = 'Y' 
				AND ( t2."InvntItem" = 'Y' 
					OR t2."TreeType" = 'A' ) 
				AND t1."Instance" = 0 ; 

			IF :rownum1 > 0 then 
				DB_Impacted := 'Y'; 
				INSERT INTO "TMP_IVSR_SupInterReq_Results" VALUES (N'TODO[[[db is impacted, please contact SAP support. ErrorCode = 419.]]]');
			END if; 
		END if; --416_419
	
	--2--
	END IF; --Recalc_By_OILM

	DELETE FROM "TMP_IVSR_SupInterReq_TotalLcZeroPQNotZero";

	SELECT * FROM "TMP_IVSR_SupInterReq_Results";
	DELETE FROM "TMP_IVSR_SupInterReq_Results";


END;


