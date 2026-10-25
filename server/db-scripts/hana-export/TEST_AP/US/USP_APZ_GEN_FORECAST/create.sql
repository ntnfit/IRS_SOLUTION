
CREATE PROCEDURE USP_APZ_GEN_FORECAST
(
	fdate date, tdate date, 
	fitem nvarchar(50) default '', titem nvarchar(50) default '', itemGroup nvarchar(200)  default '',
	whscode nvarchar(10) default '', viewby char(1) default 'D', bwlength int default 0, fwlength int default 0, 
	bwbase nvarchar(100) default '', fwbase nvarchar(100) default '',
	ignore char(1) default 'Y', compareType nvarchar(3) default 'or', propeties nvarchar(254) default ''
)
AS
BEGIN

	declare bwfdate date;
	declare bwtdate date;
	declare fwfdate date;
	declare fwtdate date;
	declare pointdate date;
	
	declare tmpdate date;
	
	declare query ntext;
	DECLARE condi nvarchar(4000); 
	
	DECLARE daterage NVARCHAR(4000);
	
	declare qtyCols nvarchar(4000);
	declare priceCols nvarchar(4000);
	
	CREATE LOCAL TEMPORARY TABLE #tbl ("ItmCode" nvarchar(50) , "ItmName" nvarchar(100), "DftWH" nvarchar(20), "BackwardQty" decimal, "ForwardQty" decimal, "DocDate" timestamp, "Qty" decimal, "Note" nvarchar(100));
	-- Tính toán các mốc thời gian
	
	pointdate = cast(current_date as date);
	
	if :viewby='D' 		then 
		bwfdate = add_days(:pointdate, -:bwlength);
		fwtdate = add_days(:pointdate, :fwlength);
	else if :viewby='W' then 
		bwfdate = add_days(:pointdate,-:bwlength*7);
		fwtdate = add_days(:pointdate, :fwlength*7);
	else					 
		bwfdate = add_months(:pointdate, -:bwlength);
		fwtdate = add_months(:pointdate, :fwlength);
	end if;
	end if;
	
	bwtdate=:pointdate;
	fwfdate=add_days(:bwtdate,1);

--	select @bwfdate,@bwtdate,@fwfdate,@fwtdate
	-- Kỳ tính forecast
	create local temporary table #tblDate ("DDate" date);
	
	tmpdate = :fdate;
	while tmpdate<=:tdate do
	
		insert into #tblDate("DDate") values(:tmpdate);
		if :viewby='D' then
			tmpdate = add_days(:tmpdate, 1);
		else if :viewby='W'	then
			tmpdate = add_days(:tmpdate, 7);
		else	
			tmpdate = add_months(:tmpdate, 1);
		end if;
		end if;
		
	end while;
	--select * from #tblDate;
	--- DANH MỤC CHÍNH
	query = 'insert into #tbl("ItmCode","ItmName","DftWH","DocDate")
	select a."ItemCode",a."ItemName",''' || :whscode|| ''',b."DDate"
	from OITM a, #tblDate b
	where (ifnull('''||:fitem || ''','''') = '''' or a."ItemCode" >= ''' || :fitem|| ''') and 
	(ifnull(''' || :titem || ''','''') = '''' or a."ItemCode" <= ''' || :titem || ''') and 
		(ifnull(''' || :itemGroup || ''','''') = '''' or a."ItmsGrpCod" in (select cast(Data as int) from Split(''' || :itemGroup || ''','','')))
		and (ifnull(''' || :ignore || ''',''Y'') = ''Y'' or ''' || :ignore || '''= '''' or (';
	
	if :propeties = '' then
	
		select (select string_agg('A."' || COLUMN_NAME || '" = ''N''',' and ') FROM TABLE_COLUMNS WHERE SCHEMA_NAME = current_schema AND TABLE_NAME = 'OITM' and COLUMN_NAME LIKE 'QryGroup%') into condi from dummy;
		query = :query || :condi || '))';

	ELSE

		select (select string_agg( ' A."QryGroup'|| cast(Data as nvarchar) || '" = ''Y'' ' || :compareType) from Split(:propeties, ',')) into condi from dummy;
		if :compareType = 'and' then
			query = :query || :condi || ' 1=1))';
		else 
			query = :query || :condi || ' 2=1))';
		end if;

	END IF;

	ignore = IfNULL(:ignore, 'Y');
	execute immediate :query;
	--=========================================================================================================================================================================
	-- Tính toán số liệu dựa trên lịch sử và plan
	-- Backward
	create local temporary table #BW ("ObjType" nvarchar(20), "ItemCode" nvarchar(20), "Qty" decimal);
	insert into #BW
	SELECT a."ObjType", b."ItemCode", SUM(b."InvQty") FROM ORDR a, RDR1 b WHERE a."DocEntry" = b."DocEntry" AND a.CANCELED = 'N' AND b."ShipDate" BETWEEN :bwfdate AND :bwtdate GROUP BY a."ObjType", b."ItemCode" UNION ALL
	SELECT a."ObjType", b."ItemCode", SUM(b."InvQty") FROM ODLN a, DLN1 b WHERE a."DocEntry" = b."DocEntry" AND a.CANCELED = 'N' AND b."ShipDate" BETWEEN :bwfdate AND :bwtdate GROUP BY a."ObjType", b."ItemCode" UNION ALL
	SELECT a."ObjType", b."ItemCode", SUM(b."InvQty") FROM OINV a, INV1 b WHERE a."DocEntry" = b."DocEntry" AND a.CANCELED = 'N' AND b."ShipDate" BETWEEN :bwfdate AND :bwtdate GROUP BY a."ObjType", b."ItemCode" UNION ALL
	SELECT a."ObjType", b."ItemCode", SUM(b."InvQty") FROM OPOR a, POR1 b WHERE a."DocEntry" = b."DocEntry" AND a.CANCELED = 'N' AND b."ShipDate" BETWEEN :bwfdate AND :bwtdate GROUP BY a."ObjType", b."ItemCode" UNION ALL
	SELECT a."ObjType", b."ItemCode", SUM(b."InvQty") FROM OPDN a, PDN1 b WHERE a."DocEntry" = b."DocEntry" AND a.CANCELED = 'N' AND b."ShipDate" BETWEEN :bwfdate AND :bwtdate GROUP BY a."ObjType", b."ItemCode" UNION ALL
	SELECT a."ObjType", b."ItemCode", SUM(b."InvQty") FROM OPCH a, PCH1 b WHERE a."DocEntry" = b."DocEntry" AND a.CANCELED = 'N' AND b."ShipDate" BETWEEN :bwfdate AND :bwtdate GROUP BY a."ObjType", b."ItemCode" UNION ALL
	SELECT 2021 AS "ObjType", "ItemCode", SUM(a."PlannedQty") FROM OWOR a WHERE "DueDate" BETWEEN :bwfdate AND :bwtdate AND a."Type" = 'S' AND a."Status" IN ('P','R','L') GROUP BY "ItemCode" UNION ALL
	SELECT 2022 AS "ObjType", b."ItemCode", SUM(b."PlannedQty") FROM OWOR a, WOR1 b WHERE a."DueDate" BETWEEN :bwfdate AND :bwtdate AND a."Type" = 'S' AND a."Status" IN ('P','R','L') GROUP BY b."ItemCode" UNION ALL
	SELECT a."ObjType", b."ItemCode", SUM(b."InvQty") FROM OIGN a, IGN1 b WHERE a."DocEntry" = b."DocEntry" AND a.CANCELED = 'N' AND a."DocDate" BETWEEN :bwfdate AND :bwtdate GROUP BY a."ObjType", b."ItemCode" UNION ALL
	SELECT a."ObjType", b."ItemCode", SUM(b."InvQty") FROM OIGE a, IGE1 b WHERE a."DocEntry" = b."DocEntry" AND a.CANCELED = 'N' AND a."DocDate" BETWEEN :bwfdate AND :bwtdate GROUP BY a."ObjType", b."ItemCode";
	-- Forward
	create local temporary table #FW ("ObjType" nvarchar(20), "ItemCode" nvarchar(20), "Qty" decimal);
	insert into #FW
	SELECT a."ObjType", b."ItemCode", SUM(b."InvQty") FROM ORDR a, RDR1 b WHERE a."DocEntry" = b."DocEntry" AND a.CANCELED = 'N' AND b."ShipDate" BETWEEN :fwfdate AND :fwtdate GROUP BY a."ObjType", b."ItemCode" UNION ALL
	SELECT a."ObjType", b."ItemCode", SUM(b."InvQty") FROM ODLN a, DLN1 b WHERE a."DocEntry" = b."DocEntry" AND a.CANCELED = 'N' AND b."ShipDate" BETWEEN :fwfdate AND :fwtdate GROUP BY a."ObjType", b."ItemCode" UNION ALL
	SELECT a."ObjType", b."ItemCode", SUM(b."InvQty") FROM OINV a, INV1 b WHERE a."DocEntry" = b."DocEntry" AND a.CANCELED = 'N' AND b."ShipDate" BETWEEN :fwfdate AND :fwtdate GROUP BY a."ObjType", b."ItemCode" UNION ALL
	SELECT a."ObjType", b."ItemCode", SUM(b."InvQty") FROM OPOR a, POR1 b WHERE a."DocEntry" = b."DocEntry" AND a.CANCELED = 'N' AND b."ShipDate" BETWEEN :fwfdate AND :fwtdate GROUP BY a."ObjType", b."ItemCode" UNION ALL
	SELECT a."ObjType", b."ItemCode", SUM(b."InvQty") FROM OPDN a, PDN1 b WHERE a."DocEntry" = b."DocEntry" AND a.CANCELED = 'N' AND b."ShipDate" BETWEEN :fwfdate AND :fwtdate GROUP BY a."ObjType", b."ItemCode" UNION ALL
	SELECT a."ObjType", b."ItemCode", SUM(b."InvQty") FROM OPCH a, PCH1 b WHERE a."DocEntry" = b."DocEntry" AND a.CANCELED = 'N' AND b."ShipDate" BETWEEN :fwfdate AND :fwtdate GROUP BY a."ObjType", b."ItemCode" UNION ALL
	SELECT 2021 AS "ObjType", "ItemCode", SUM(a."PlannedQty") FROM OWOR a WHERE "DueDate" BETWEEN :fwfdate AND :fwtdate AND a."Type" = 'S' AND a."Status" IN ('P','R','L') GROUP BY "ItemCode" UNION ALL
	SELECT 2022 AS "ObjType", b."ItemCode", SUM(b."PlannedQty") FROM OWOR a, WOR1 b WHERE a."DueDate" BETWEEN :fwfdate AND :fwtdate AND a."Type" = 'S' AND a."Status" IN ('P','R','L') GROUP BY b."ItemCode" UNION ALL
	SELECT a."ObjType", b."ItemCode", SUM(b."InvQty") FROM OIGN a, IGN1 b WHERE a."DocEntry" = b."DocEntry" AND a.CANCELED = 'N' AND a."DocDate" BETWEEN :fwfdate AND :fwtdate GROUP BY a."ObjType", b."ItemCode" UNION ALL
	SELECT a."ObjType", b."ItemCode", SUM(b."InvQty") FROM OIGE a, IGE1 b WHERE a."DocEntry" = b."DocEntry" AND a.CANCELED = 'N' AND a."DocDate" BETWEEN :fwfdate AND :fwtdate GROUP BY a."ObjType", b."ItemCode";

--	select * from #BW where ObjType in (select Data from dbo.Split(@bwbase,','))
--	select * from #FW where ObjType in (select Data from dbo.Split(@fwbase,','))
	-- Tạm


	update #tbl set 
		"BackwardQty" = (select sum(x."Qty") from #BW x where x."ItemCode"=#tbl."ItmCode" and x."ObjType" in (select Data from Split(:bwbase,','))), 
		"ForwardQty" = (select sum(x."Qty") from #FW x where x."ItemCode"=#tbl."ItmCode" and x."ObjType" in (select Data from Split(:fwbase,',')));
	-- loại bỏ các item ko có số lươợg
	create local temporary table #DEL_ITEM as (select "ItmCode" from #tbl group by "ItmCode" having sum("BackwardQty")=0 and sum("ForwardQty")=0);
	delete from #tbl where "ItmCode" in (select x."ItmCode" from #DEL_ITEM x);


	if :bwlength+:fwlength > 0 then
	update #tbl set "Qty" = ("BackwardQty"+"ForwardQty")/(:bwlength+:fwlength);
	end if;
	--=========================================================================================================================================================================
	select (select string_agg('SUM(CASE WHEN "DocDate" = ''' || to_varchar("DDate") || ''' THEN "Qty" END) AS ' || '"Q' || to_varchar("DDate", 'YYYY.MM.DD') || '"', ',') from #tblDate) into query from dummy;
	
	query = 'create local temporary table #tmpQty as (
			select 0 "Code", 0 "LineID", "ItmCode", "ItmName", "DftWH", ' ||
			:query ||
			' from #tbl group by "ItmCode", "ItmName", "DftWH")';
			
	--select :query from dummy;
	execute immediate :query;
	
	select (select string_agg('SUM(CASE WHEN "DocDate" = ''' || to_varchar("DDate") || ''' THEN "Price" END) AS ' || '"P' || to_varchar("DDate", 'YYYY.MM.DD') || '"', ',') from #tblDate) into query from dummy;
	
	query = 'create local temporary table #tmpRevenue as (
			select 0 "Code", 0 "LineID", "ItmCode", "ItmName", "DftWH", ' ||
			:query ||
			' from (SELECT 0 "Code",0 "LineID",a."ItmCode",A."DocDate",A."DftWH",a."ItmName", Ifnull(A."Qty", 0) * Ifnull(c."Price", 0) "Price" FROM #tbl a
			Left JOIN ITM1 c ON a."ItmCode" = c."ItemCode" and c."PriceList" = 0) group by "ItmCode", "ItmName", "DftWH")';
			
	execute immediate :query;
	
	select (select string_agg('"Q' || to_varchar("DDate", 'YYYY.MM.DD') || '"', ',') from #tblDate) into qtyCols from dummy;
	select (select string_agg('"P' || to_varchar("DDate", 'YYYY.MM.DD') || '"', ',') from #tblDate) into priceCols from dummy;  
	
	query = 'select a."Code", a."LineID", a."ItmCode" "ItemCode", a."ItmName" "ItemName", a."DftWH" "WhsCode", '''' "BPCode",'''' "SplCode"'
			|| :qtyCols || ',' || :priceCols ||
			' from #tmpQty a inner join #tmpRevenue b on a."ItmCode" = b."ItmCode" and a."DftWH" = b."DftWH"';
			
	execute immediate :query;
	
	execute immediate 'Select "ItemCode",Ifnull("Price",0) "Price" from ITM1 where "PriceList" = 0';
	execute immediate 'drop table #tmpQty';
	execute immediate 'drop table #tmpRevenue';
	
	drop table #tbl;
	drop table #tblDate;
	drop table #BW;
	drop table #FW;
	drop table #DEL_ITEM;
	
END;




