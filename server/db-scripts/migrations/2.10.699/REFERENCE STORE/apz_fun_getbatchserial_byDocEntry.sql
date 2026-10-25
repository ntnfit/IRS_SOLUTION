
CALL APZ_EXISTSPROC('apz_fun_getbatchserial_byDocEntry');

GO

-- apz_document_getbatchserial_byDocEntry '5115',112
CREATE  PROCEDURE apz_fun_getbatchserial_byDocEntry(
	in DocEntry VARCHAR(5000),
	in ObjType VARCHAR(20),
	in tempTableName nvarchar(50)
)
AS
BEGIN

IF :ObjType = '112'
then

	create local temporary table #tempDraft as (SELECT top 0 *, 0 "BinAbs", cast(null as nvarchar(50)) "BinCode", cast(null as numeric(19,6)) "SelectedQty", cast(null as nvarchar(8)) "WhsCode", cast(null as nvarchar(254)) "BinDescription"
			, cast(null as int) "BaseLineNumber", cast(0 as int) "BaseDocEntry", cast(null as char(1)) "ManBatchSerial", cast(null as numeric(19,6)) "LogQty", cast(null as char(1)) "IsExits", cast(null as int) "TranType", cast(null as char(1)) "IsBase" from OBTN);
	--get data bastch/serial line

	insert into #tempDraft
	SELECT d.*, IFNULL(c."BinAbs",0) "BinAbs", c."BinCode", case when IFNULL(c."BinAbs",0) = 0 then a."Quantity" else c."Quantity" end "SelectedQty", ifnull(c."WhsCode",a."WhsCode"), c."BinCode" "BinDescription"
			, a."LineNum" "BaseLineNumber", a."AbsEntry" "BaseDocEntry", 'B' "ManBatchSerial",
				case when IFNULL(c."BinAbs",0) = 0 then a."Quantity" else c."Quantity" end "LogQty", 'Y' "IsExits", ifnull(c."TranType",1), 'N' "IsBase"
		from
			(select * from DRF16 where "AbsEntry" = :DocEntry and "ObjId" = '10000044') a
			INNER JOIN oitm b ON a."ItemCode"=b."ItemCode"
			LEFT JOIN OBTN d ON a."ObjAbs" = d."AbsEntry"
			LEFT JOIN (select t1."LineNum", t1."SnBType", t1."SnBMDAbs", t2."AbsEntry" "BinAbs", t2."BinCode",t1."Quantity",
							case when t1."ObjType"=67 and t1."BinActTyp"=1 then t3."WhsCode" else t1."WhsCode" end "WhsCode",
							case when t1."ObjType"=67 and t1."BinActTyp"=1 then 2 else 1 end "TranType"
						from DRF19 t1 join DRF1 t3 on t1."DocEntry"=t3."DocEntry" and t1."LineNum"=t3."LineNum"
						JOIN OBIN t2 ON t1."BinAbs" = t2."AbsEntry" 
						where t1."DocEntry" = :DocEntry) c ON a."LineNum" = c."LineNum" and a."ObjId" = c."SnBType" and a."ObjAbs" = c."SnBMDAbs"
	UNION ALL 
	SELECT d.*, IFNULL(c."BinAbs",0) "BinAbs", c."BinCode", case when IfNULL(c."BinAbs",0) = 0 then a."Quantity" else c."Quantity" end "SelectedQty", ifnull(c."WhsCode",a."WhsCode"), c."BinCode" "BinDescription",
				a."LineNum" "BaseLineNumber", a."AbsEntry" "BaseDocEntry", 'S' "ManBatchSerial",
				case when IFNULL(c."BinAbs",0) = 0 then a."Quantity" else c."Quantity" end "LogQty", 'Y' "IsExits", ifnull(c."TranType",1) "TranType", 'N' "IsBase"
		from
			(select * from DRF16 where "AbsEntry" = :DocEntry and "ObjId" = '10000045') a
			INNER JOIN oitm b ON a."ItemCode"=b."ItemCode"
			LEFT JOIN OSRN d ON a."ObjAbs" = d."AbsEntry"
			LEFT JOIN (select t1."LineNum", t1."SnBType", t1."SnBMDAbs", t2."AbsEntry" "BinAbs", t2."BinCode",t1."Quantity",
							case when t1."ObjType"=67 and t1."BinActTyp"=1 then t3."WhsCode" else t1."WhsCode" end "WhsCode",
							case when t1."ObjType"=67 and t1."BinActTyp"=1 then 2 else 1 end "TranType"
						from DRF19 t1 join DRF1 t3 on t1."DocEntry"=t3."DocEntry" and t1."LineNum"=t3."LineNum"
						JOIN OBIN t2 ON t1."BinAbs" = t2."AbsEntry" 
						where t1."DocEntry" = :DocEntry) c ON a."LineNum" = c."LineNum" and a."ObjId" = c."SnBType" and a."ObjAbs" = c."SnBMDAbs"
	UNION ALL 
	SELECT d.*, IFNULL(c."BinAbs",0) "BinAbs", c."BinCode", case when IFNULL(c."BinAbs",0) = 0 then a."Quantity" else c."Quantity" end "SelectedQty", a."WhsCode", c."BinCode" "BinDescription", a."LineNum" "BaseLineNumber", a."AbsEntry" "BaseDocEntry", 'B' "ManBatchSerial",
				case when IFNULL(c."BinAbs",0) = 0 then a."Quantity" else c."Quantity" end "LogQty", 'N' "IsExits", 1 "TranType", 'N' "IsBase"
		from
			(select * from DRF16 where "AbsEntry" = :DocEntry and "ObjId" = '10000068') a
			INNER JOIN oitm b ON a."ItemCode"=b."ItemCode"
			LEFT JOIN ODBN d ON a."ObjAbs" = d."AbsEntry"
			LEFT JOIN (select t1."LineNum", t1."SnBType", t1."SnBMDAbs", t2."AbsEntry" "BinAbs", t2."BinCode", t1."Quantity" from DRF19 t1 INNER JOIN OBIN t2 ON t1."WhsCode" = t2."WhsCode" and t1."BinAbs" = t2."AbsEntry"
																							where "DocEntry" = :DocEntry) c ON a."LineNum" = c."LineNum" and a."ObjId" = c."SnBType" and a."ObjAbs" = c."SnBMDAbs"
	UNION ALL 
	SELECT d.*, IFNULL(c."BinAbs",0) "BinAbs", c."BinCode", case when IFNULL(c."BinAbs",0) = 0 then a."Quantity" else c."Quantity" end "SelectedQty", a."WhsCode", c."BinCode" "BinDescription", a."LineNum" "BaseLineNumber", a."AbsEntry" "BaseDocEntry", 'S' "ManBatchSerial",
				case when IFNULL(c."BinAbs",0) = 0 then a."Quantity" else c."Quantity" end "LogQty", 'N' "IsExits", 1 "TranType", 'N' "IsBase"
		from
			(select * from DRF16 where "AbsEntry" = :DocEntry and "ObjId" = '10000069') a
			INNER JOIN oitm b ON a."ItemCode"=b."ItemCode"
			LEFT JOIN ODSN d ON a."ObjAbs" = d."AbsEntry"
			LEFT JOIN (select t1."LineNum", t1."SnBType", t1."SnBMDAbs", t2."AbsEntry" "BinAbs", t2."BinCode", t1."Quantity" from DRF19 t1 INNER JOIN OBIN t2 ON t1."WhsCode" = t2."WhsCode" and t1."BinAbs" = t2."AbsEntry" 
																							where "DocEntry" = :DocEntry) c ON a."LineNum" = c."LineNum" and a."ObjId" = c."SnBType" and a."ObjAbs" = c."SnBMDAbs";

	UPDATE #tempDraft set "ObjType" = '10000044' where "ObjType" = '10000068';
	UPDATE #tempDraft set "ObjType" = '10000045' where "ObjType" = '10000069';

	UPDATE a set a."BinCode" = b."BinCode", a."BinAbs" = b."AbsEntry", a."BinDescription" = b."BinCode" from #tempDraft a INNER JOIN (select "WhsCode", "AbsEntry", "BinCode" from OBIN where "SysBin" = 'Y') b ON a."WhsCode" = b."WhsCode" where ifnull(a."BinAbs",0) = 0;
	
	exec 'drop table ' || :temptableName || ';';
	exec 'create local temporary table ' || :temptablename || ' as (select * from #tempDraft);';
	drop table #tempDraft;

ELSEIF :ObjType='700021'
then

--SELECT * FROM #temp_obtl
		create local temporary table #tbltemp("ItemCode" nvarchar(50) null, "DistNumber" nvarchar(50) null,"BinAbs" int null,"Quantity" numeric(19,6) null,"DocLine" int null,"DocEntry" int null,"LocCode" nvarchar(50) null
				,"ManBtchNum" char(1) null, "ManSerNum" char(1) null, "AbsEntry" int null);
						
		insert into #tbltemp
		SELECT a."ItemCode", a."DistNumber" "DistNumber",bb."BinAbs" "BinAbs",A."OpenQty" "Quantity",a."TransRow" "DocLine",a."TransNo" "DocEntry",a."WhsCode" "LocCode",oitm."ManBtchNum",oitm."ManSerNum"
				,a."BatchSerialEntry" "AbsEntry" 
		from
			APZ_SHIP1 a INNER JOIN oitm ON a."ItemCode"=oitm."ItemCode"
			LEFT JOIN OSBQ bb  
			 ON a."ItemCode"=bb."ItemCode" AND a."BatchSerialEntry"=bb."SnBMDAbs" AND bb."WhsCode"=a."WhsCode" AND bb."OnHandQty">0

		WHERE a."DocEntry" IN (SELECT DISTINCT data FROM Split(:DocEntry,','));
		--SELECT * FROM #tbltemp

		-- apz_document_getbatchserial_byDocEntry '1,2,3,4,5','700021'
	
	create local temporary table #tblshiping as (select top 0 *,cast(null as int) "BinAbs", cast(null as nvarchar(50)) "BinCode", cast(null as numeric(19,6)) "SelectedQty", cast(null as nvarchar(50)) "WhsCode"
			, cast(null as nvarchar(254)) "BinDescription", cast(null as int) "BaseLineNumber",cast(null as int) "BaseDocEntry", cast(null as char(1)) "ManBatchSerial",cast(null as NUMERIC(19,6)) "LogQty",
						cast(null as char(1)) "IsExits" FROM OBTN);
	
	insert into #tblshiping
		SELECT * FROM (
		SELECT T0.*,IFNULL(T2."AbsEntry",0) "BinAbs", T2."BinCode", T1."Quantity" "SelectedQty", T1."LocCode" "WhsCode", T2."BinCode" AS "BinDescription",T1."DocLine" "BaseLineNumber",T1."DocEntry" "BaseDocEntry"
			,'B' "ManBatchSerial",0 LogQty,'Y' "IsExits" 
		
		FROM OBTN T0 INNER JOIN #tbltemp T1 ON T0."AbsEntry"=T1."AbsEntry"      
				left JOIN OBIN T2 ON T1."BinAbs"=T2."AbsEntry"
	    WHERE T1."ManBtchNum"='Y' AND ABS(t1."Quantity")>0
		UNION all
		SELECT T0.*,IfNULL(T2."AbsEntry",0) "BinAbs", IFNULL(T2."BinCode",''), T1."Quantity" "SelectedQty", T1."LocCode" "WhsCode", Ifnull(T2."BinCode",'') AS "BinDescription",T1."DocLine" "BaseLineNumber"
				,T1."DocEntry" "BaseDocEntry",'S' "ManBatchSerial",0 "LogQty",'Y' "IsExits"
		
		FROM OSRN T0 INNER JOIN #tbltemp T1 ON T0."AbsEntry"=T1."AbsEntry"     
					left JOIN OBIN T2 ON T1."BinAbs"=T2."AbsEntry"
	    WHERE T1."ManSerNum"='Y' AND ABS(t1."Quantity")>0) TEMP;

	
	UPDATE #tblshiping SET "LogQty"=ABS("SelectedQty");
	
	exec 'drop table ' || :temptableName || ';';
	exec 'create local temporary table ' || :temptablename || ' as (SELECT *, 1 "TranType", ''N'' "IsBase" FROM #tblshiping);';
	drop table #tbltemp;
	drop table #tblshiping;

	--SELECT DISTINCT ItemCode,BaseLineNumber,BaseDocEntry FROM #tblshiping
	-- apz_document_getbatchserial_byDocEntry '1,2,3,4,5','700021'
ELSE
	create local temporary table #temp ("ItemCode" nvarchar(50) null, "DistNumber" nvarchar(50) null,"BinAbs" int null, "Quantity" numeric(19,6) null,"DocLine" int null, "DocEntry" int null,"LocCode" nvarchar(50) null
				,"ManBtchNum" char(1) null,"ManSerNum" char(1) null,"AbsEntry" int null,"TranType" int null);

	insert into #temp
	SELECT a."ItemCode", CASE WHEN oitm."ManBtchNum"='Y' THEN c."DistNumber" WHEN oitm."ManSerNum"='Y' THEN d."DistNumber" ELSE '' END "DistNumber",bb."BinAbs",
		CASE WHEN bb."BinAbs"<>0 THEN bb."Quantity" ELSE case when a."StockEff" = 1 then sum(b."Quantity")
															  when a."StockEff" = 3 then sum(b."OrderedQty")
														else sum(b."AllocQty") end END "Quantity",
		a."DocLine",a."DocEntry",a."LocCode",oitm."ManBtchNum",oitm."ManSerNum",b."MdAbsEntry" "AbsEntry"
		,case when :ObjType != '67' or sum(a."DocQty") < 0 then 1 else 2 end "TranType"		
		from oitl a
			INNER JOIN oitm ON a."ItemCode"=oitm."ItemCode"
			inner join ITL1 b on a."LogEntry" = b."LogEntry"
			left join OBTL bb on b."LogEntry" = bb."ITLEntry" and b."MdAbsEntry"=bb."SnBMDAbs"
			left join OBTN c on b."MdAbsEntry" = c."AbsEntry" AND c."ItemCode"=b."ItemCode"
			LEFT JOIN osrn d ON b."MdAbsEntry"=d."AbsEntry" AND d."ItemCode"=b."ItemCode"
		where a."DocType"=:ObjType and (a."AllocateTp"=:ObjType or a."AllocateTp"=-1) and a."DocEntry" IN (SELECT DATA FROM Split(:DocEntry,','))
		group by c."DistNumber" ,bb."BinAbs",bb."Quantity",a."DocLine",a."DocEntry",a."StockEff",c."ItemCode",a."LocCode",a."ItemCode",
		oitm."ManSerNum",oitm."ManBtchNum",c."DistNumber",d."DistNumber",b."MdAbsEntry";

	create local temporary table #tblQuantity as (SELECT top 0 *,cast(null as int) "BinAbs", cast(null as nvarchar(50)) "BinCode", cast(null as numeric(19,6)) "SelectedQty", cast(null as nvarchar(50)) "WhsCode"
			, cast(null as nvarchar(254)) "BinDescription", cast(null as int) "BaseLineNumber", cast(null as int) "BaseDocEntry",cast(null as char(1)) "ManBatchSerial",cast(null as numeric(19,6)) "LogQty"
						,cast(null as char(1)) "IsExits", cast(null as int) "TranType" FROM  OBTN);
			
	insert into #tblQuantity
	SELECT T0.*,IFNULL(T2."AbsEntry",0) "BinAbs", T2."BinCode", ifnull(x."Quantity",0) - ifnull(x."CommitQty",0) "SelectedQty", T1."LocCode" "WhsCode", T2."BinCode" AS "BinDescription",T1."DocLine" "BaseLineNumber"
			,T1."DocEntry" "BaseDocEntry",'B' "ManBatchSerial",ABS(T1."Quantity") "LogQty",'Y' "IsExits", t1."TranType" 
		FROM  OBTN T0 
				INNER JOIN #temp T1 ON T0."AbsEntry"=T1."AbsEntry"
				left JOIN OBTQ x ON T1."AbsEntry" = x."MdAbsEntry" and T1."LocCode" = x."WhsCode"    
				LEFT JOIN OBIN T2 ON T1."BinAbs"=T2."AbsEntry" WHERE T1."ManBtchNum"='Y'
	UNION all
	SELECT T0.*,IFNULL(T2."AbsEntry",0) "BinAbs", T2."BinCode", ifnull(x."Quantity",0) - ifnull(x."CommitQty",0) "SelectedQty", T1."LocCode" "WhsCode", T2."BinCode" AS "BinDescription",T1."DocLine" "BaseLineNumber"
			,T1."DocEntry" "BaseDocEntry",'S' "ManBatchSerial",ABS(T1."Quantity") "LogQty",'Y' "IsExits", t1."TranType"
		FROM OSRN T0 
					INNER JOIN #temp T1 ON T0."AbsEntry"=T1."AbsEntry"
					left JOIN OSRQ x ON T1."AbsEntry" = x."MdAbsEntry" and T1."LocCode" = x."WhsCode"
					LEFT JOIN OBIN T2 ON T1."BinAbs"=T2."AbsEntry"
		WHERE T1."ManSerNum"='Y' AND ABS(t1."Quantity") <> 0;
	--UPDATE #tblQuantity SET LogQty=ABS(SelectedQty)


	exec 'drop table ' || :temptableName || ';';
	exec 'create local temporary table ' || :temptablename || ' as (SELECT *, ''N'' "IsBase" FROM #tblQuantity);';
	drop table #temp;
	drop table #tblQuantity;

END if;	

END