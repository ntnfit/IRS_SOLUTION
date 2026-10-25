
CALL APZ_EXISTSPROC('APZ_Outsourcing_GetBinData');

GO

CREATE  PROCEDURE APZ_Outsourcing_GetBinData(
	in code VARCHAR(5000),
	in typeDoc Int,
	in itemCode VARCHAR(50)
	)
AS
BEGIN
	Declare ID Int;
	
	IF :typeDoc = 1
	THEN 
		Select "U_issueID" Into ID From "@TBL_OUTS_DOCNUM" Where "Code" = :code;
		
		create local temporary table #temp ("ItemCode" nvarchar(50) null, "DistNumber" nvarchar(50) null,"BinAbs" int null, "Quantity" numeric(19,6) null,"DocLine" int null, "DocEntry" int null,"LocCode" nvarchar(50) null
				,"ManBtchNum" char(1) null,"ManSerNum" char(1) null,"AbsEntry" int null,"TranType" int null);

		insert into #temp
		SELECT a."ItemCode", CASE WHEN oitm."ManBtchNum"='Y' THEN c."DistNumber" WHEN oitm."ManSerNum"='Y' THEN d."DistNumber" ELSE '' END "DistNumber",bb."BinAbs",
			CASE WHEN bb."BinAbs"<>0 THEN bb."Quantity" ELSE case when a."StockEff" = 1 then sum(b."Quantity")
																  when a."StockEff" = 3 then sum(b."OrderedQty")
															else sum(b."AllocQty") end END "Quantity",
			a."DocLine",a."DocEntry",a."LocCode",oitm."ManBtchNum",oitm."ManSerNum",b."MdAbsEntry" "AbsEntry"
			,case when sum(a."DocQty") < 0 then 1 else 2 end "TranType"		
		from oitl a
			INNER JOIN oitm ON a."ItemCode"=oitm."ItemCode"
			inner join ITL1 b on a."LogEntry" = b."LogEntry"
			left join OBTL bb on b."LogEntry" = bb."ITLEntry" and b."MdAbsEntry"=bb."SnBMDAbs"
			left join OBTN c on b."MdAbsEntry" = c."AbsEntry" AND c."ItemCode"=b."ItemCode"
			LEFT JOIN osrn d ON b."MdAbsEntry"=d."AbsEntry" AND d."ItemCode"=b."ItemCode"
		where a."DocType"='60' and (a."AllocateTp"='60' or a."AllocateTp"=-1) and a."DocEntry" = :ID
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
					INNER JOIN OBTQ x ON T1."AbsEntry" = x."MdAbsEntry" and T1."LocCode" = x."WhsCode"    
					LEFT JOIN OBIN T2 ON T1."BinAbs"=T2."AbsEntry" WHERE T1."ManBtchNum"='Y'
		UNION all
		SELECT T0.*,IFNULL(T2."AbsEntry",0) "BinAbs", T2."BinCode", ifnull(x."Quantity",0) - ifnull(x."CommitQty",0) "SelectedQty", T1."LocCode" "WhsCode", T2."BinCode" AS "BinDescription",T1."DocLine" "BaseLineNumber"
				,T1."DocEntry" "BaseDocEntry",'S' "ManBatchSerial",ABS(T1."Quantity") "LogQty",'Y' "IsExits", t1."TranType"
			FROM OSRN T0 
						INNER JOIN #temp T1 ON T0."AbsEntry"=T1."AbsEntry"
						INNER JOIN OSRQ x ON T1."AbsEntry" = x."MdAbsEntry" and T1."LocCode" = x."WhsCode"
						LEFT JOIN OBIN T2 ON T1."BinAbs"=T2."AbsEntry"
			WHERE T1."ManSerNum"='Y' AND ABS(t1."Quantity") <> 0;

		Select "DistNumber", "BinCode" AS "FromBinCode", "SelectedQty" AS "AvailQty", "LogQty" AS "Quantity", b."BaseLinNum" AS "LineNum","AbsEntry" AS "BtSAbsEntry",
			"BinAbs" AS "FromBinAbs","MnfSerial", "LotNumber", 'Y' AS "BinActivat" 
		From #tblQuantity a JOIN IBT1 b On a."ItemCode" = b."ItemCode" And a."DistNumber" = b."BatchNum" And b."BaseType" = '60' And b."BaseEntry" = :ID
		Order By "DistNumber";

		Drop Table #temp;
		Drop Table #tblQuantity;
		
	ELSE 
		Select "U_receiptID" Into ID From "@TBL_OUTS_DOCNUM" Where "Code" = :code;
		
		create local temporary table #temp ("ItemCode" nvarchar(50) null, "DistNumber" nvarchar(50) null,"BinAbs" int null, "Quantity" numeric(19,6) null,"DocLine" int null, "DocEntry" int null,"LocCode" nvarchar(50) null
				,"ManBtchNum" char(1) null,"ManSerNum" char(1) null,"AbsEntry" int null,"TranType" int null);

		insert into #temp
		SELECT a."ItemCode", CASE WHEN oitm."ManBtchNum"='Y' THEN c."DistNumber" WHEN oitm."ManSerNum"='Y' THEN d."DistNumber" ELSE '' END "DistNumber",bb."BinAbs",
			CASE WHEN bb."BinAbs"<>0 THEN bb."Quantity" ELSE case when a."StockEff" = 1 then sum(b."Quantity")
																  when a."StockEff" = 3 then sum(b."OrderedQty")
															else sum(b."AllocQty") end END "Quantity",
			a."DocLine",a."DocEntry",a."LocCode",oitm."ManBtchNum",oitm."ManSerNum",b."MdAbsEntry" "AbsEntry"
			,case when sum(a."DocQty") < 0 then 1 else 2 end "TranType"		
		from oitl a
			INNER JOIN oitm ON a."ItemCode"=oitm."ItemCode"
			inner join ITL1 b on a."LogEntry" = b."LogEntry"
			left join OBTL bb on b."LogEntry" = bb."ITLEntry" and b."MdAbsEntry"=bb."SnBMDAbs"
			left join OBTN c on b."MdAbsEntry" = c."AbsEntry" AND c."ItemCode"=b."ItemCode"
			LEFT JOIN osrn d ON b."MdAbsEntry"=d."AbsEntry" AND d."ItemCode"=b."ItemCode"
		where a."DocType"='59' and (a."AllocateTp"='59' or a."AllocateTp"=-1) and a."DocEntry" = :ID
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
					INNER JOIN OBTQ x ON T1."AbsEntry" = x."MdAbsEntry" and T1."LocCode" = x."WhsCode"    
					LEFT JOIN OBIN T2 ON T1."BinAbs"=T2."AbsEntry" WHERE T1."ManBtchNum"='Y'
		UNION all
		SELECT T0.*,IFNULL(T2."AbsEntry",0) "BinAbs", T2."BinCode", ifnull(x."Quantity",0) - ifnull(x."CommitQty",0) "SelectedQty", T1."LocCode" "WhsCode", T2."BinCode" AS "BinDescription",T1."DocLine" "BaseLineNumber"
				,T1."DocEntry" "BaseDocEntry",'S' "ManBatchSerial",ABS(T1."Quantity") "LogQty",'Y' "IsExits", t1."TranType"
			FROM OSRN T0 
						INNER JOIN #temp T1 ON T0."AbsEntry"=T1."AbsEntry"
						INNER JOIN OSRQ x ON T1."AbsEntry" = x."MdAbsEntry" and T1."LocCode" = x."WhsCode"
						LEFT JOIN OBIN T2 ON T1."BinAbs"=T2."AbsEntry"
			WHERE T1."ManSerNum"='Y' AND ABS(t1."Quantity") <> 0;
		
		Select "DistNumber", "BinCode" AS "ToBinCode", "SelectedQty" AS "AvailQty",  "LogQty" AS "Quantity", b."BaseLinNum" AS "LineNum", "AbsEntry" AS "BtSAbsEntry",
			"MnfSerial", "LotNumber", 'Y' AS "BinActivat" 
		From #tblQuantity a JOIN IBT1 b On a."ItemCode" = b."ItemCode" And a."DistNumber" = b."BatchNum" And b."BaseType" = '59' And b."BaseEntry" = :ID
		Where a."ItemCode" = :itemCode 
		Order By "DistNumber";
		
		Drop Table #temp;
		Drop Table #tblQuantity;
	End If;
END	