
CREATE PROCEDURE APZ_BatchDetail_GetDataAutoSelect(
	in typeItem nvarchar(1) default '',
	in isAllocateBin nvarchar(1) default '',
	in itemCode nvarchar(50) default '',
	in whsCode nvarchar(8) default '',
	in qty numeric(19,6) default 0
)
AS
BEGIN

	Declare sqlQuery nvarchar(50000);
	Declare columsOrder nvarchar(50000);
	declare sumQty numeric(19,6);
	declare sumQtyBatch numeric(19,6);
	declare binAbs int;
	declare sumQtyBin numeric(19,6);
	
	SELECT IFnull((Select  string_agg('b."' || "ColumnId" || '" ' || case when "Type" = 'A' then 'ASC' else 'DESC' end  || ',')	FROM APZ_OABC),'') into columsOrder from DUMMY;
	
	if :columsOrder = ''
	then
		columsOrder = 'b."AbsEntry", b."SysNumber"';
	else
		columsOrder = LEFT(:columsOrder, LENGTH(:columsOrder) - 1);
	end if;
	
	create local temporary table #tempExistLocal ("LogQty" numeric(19,6) null, "BinAbs" int null, "AbsEntry" int null, "SysNumber" int null, "ItemCode" nvarchar(50) null);
	
	exec 'drop table #tempExistLocal;';
	exec 'create local temporary table #tempExistLocal as (select * from "#tempExist");';

	IF :isAllocateBin = 'Y'
	then 
		
		--auto select bin với các item không quản lí theo batch hoặc serial
		if ifnull(:typeItem, 'N') = 'N'
		then


			create local temporary table #tempBin ("InStock" numeric(19,6) null, "WhsCode" nvarchar(50) null, "LogQty" numeric(19,6) null, "CommitQty" numeric(19,6) null
					, "ManBatchSerial" char(1) null, "sumvalues" numeric(19,6) null, "BinAbs" int null);
					
			insert into #tempBin select * from (SELECT A."Quantity" - ifnull(A."CommitQty",0) - ifnull(d."LogQty", 0) "InStock", A."WhsCode", a."Quantity" "LogQty", a."CommitQty" "CommitQty", 'N' "ManBatchSerial",
																	SUM(A."Quantity" - ifnull(A."CommitQty",0) - ifnull(d."LogQty", 0)) OVER (ORDER BY a."BinAbs") "sumvalues", a."BinAbs"

			FROM    (SELECT "OnHandQty" AS "Quantity", "WhsCode", 0.0 AS "CommitQty", "BinAbs", "ItemCode" FROM OIBQ WHERE "ItemCode" = :ItemCode AND "WhsCode" = :whsCode) A 
						LEFT JOIN #tempExistLocal d ON a."BinAbs" = d."BinAbs" and a."ItemCode" = d."ItemCode"
			WHERE        A."Quantity" - A."CommitQty" - ifnull(d."LogQty", 0) > 0) a where "sumvalues" <= :qty;

			select ifnull((select MAX("BinAbs") from #tempBin),0) into binAbs from DUMMY;
			select ifnull((select "sumvalues" from #tempBin where "BinAbs" = :binAbs),0) into sumQtyBin from DUMMY;

			insert into #tempBin 
			SELECT TOP 1 :qty - :sumQtyBin AS "InStock", A."WhsCode", a."Quantity" AS "LogQty", a."CommitQty" AS "CommitQty", 'B' AS "ManBatchSerial", 0, a."BinAbs" 
				FROM (SELECT "OnHandQty" AS "Quantity", "WhsCode", 0.0 AS "CommitQty", "BinAbs", "ItemCode" FROM OIBQ WHERE "ItemCode" = :ItemCode AND "WhsCode" = :whsCode) AS A  
						LEFT JOIN #tempExistLocal d ON a."BinAbs" = d."BinAbs" AND a."ItemCode" = d."ItemCode" 
								WHERE a."BinAbs" > :binAbs AND A."Quantity" - A."CommitQty" - ifnull(d."LogQty", 0) > 0 ORDER BY a."BinAbs";


			SELECT c.*, a.*, b."BinCode" AS "BinCode", 0 AS "ToBinAbs", '' AS "ToBinCode", a."LogQty" - a."CommitQty" AS "SelectedQty" FROM #tempBin a 
									LEFT JOIN (SELECT TOP 0 * FROM OBTN) AS c ON 1 = 1
									LEFT JOIN OBIN b ON a."BinAbs" = b."AbsEntry";

			return;

		END if;
		
		create local temporary table #tempSum ("ItemCode" nvarchar(50) null
												, "BinAbs" int null
												, "BinCode" nvarchar(50) null
												, "Instock" numeric(19,6) null
												, "WhsCode" nvarchar(50) null
												, "DistNumber" nvarchar(50)
												, "AbsEntry" int null
												, "SysNumber" int null
												, "MnfSerial" nvarchar(50) null
												, "LotNumber" nvarchar(50) null
												, "WhsQty" numeric(19,6) null
												, "WhsCommit" numeric(19,6) null
												, "sumvalue" numeric(19,6) null);
	
		--lấy ra danh sách item - batch/serial - bin qty tương ứng
		insert into #tempSum select * from (SELECT a."ItemCode", a."BinAbs", a."BinCode", a."Instock" - IFNULL(c."LogQty", 0) "Instock", a."WhsCode", a."DistNumber", "AbsEntryBatch" "AbsEntry", a."SysNumber"
			, a."MnfSerial", a."LotNumber", b."Quantity" "WhsQty", IFNULL(b."CommitQty", 0) + IFNULL(d."LogQty", 0) "WhsCommit", SUM(a."Instock" - IFNULL(c."LogQty", 0)) OVER (PARTITION BY "AbsEntryBatch" order by a."BinAbs") 
			FROM APZ_OITW_LOCATION a
						INNER JOIN (SELECT "MdAbsEntry", "Quantity", "CommitQty", "SysNumber" FROM OBTQ WHERE "ItemCode" = :itemCode AND "WhsCode" = :whsCode AND :typeItem = 'B' 
										UNION ALL 
									SELECT "MdAbsEntry", "Quantity", "CommitQty", "SysNumber" FROM OSRQ WHERE "ItemCode" = :itemCode AND "WhsCode" = :whsCode AND :typeItem = 'S') b ON a."AbsEntryBatch" = b."MdAbsEntry" and a."SysNumber" = b."SysNumber"
						LEFT JOIN #tempExistLocal c ON ifnull(a."BinAbs",-1) = ifnull(c."BinAbs",0) and a."AbsEntryBatch" = c."AbsEntry" and a."SysNumber" = c."SysNumber"
						LEFT JOIN #tempExistLocal d ON a."AbsEntryBatch" = d."AbsEntry" and a."SysNumber" = d."SysNumber"

							where a."ItemCode" = :itemCode and "WhsCode" = :whsCode and ((:typeItem = 'B' and "ManBtchNum" = 'Y') or (:typeItem = 'S' and "ManSerNum" = 'Y'))) a where  "Instock" > 0 and "WhsQty" - "WhsCommit" > 0;--where sumvalue < WhsQty - WhsCommit
	
		--lấy danh sách item - bactch/serial- bin có sum lớn nhất nhỏ hơn số lượng variable của batch/serial trong kho
		create local temporary table #tempQty as (select * from #tempSum where "sumvalue" < "WhsQty" - "WhsCommit");

		--lấy danh sách các bin lớn nhất của bacth đã được lấy vào danh sách thỏa mãn
		create local temporary table #tempMissing as (SELECT b."AbsEntry", b."BinAbs", a."WhsQty" - a."WhsCommit" - a."sumvalue" AS "MissQty" FROM #tempQty a 
														INNER JOIN (SELECT MAX("BinAbs") AS "BinAbs", "AbsEntry" FROM #tempQty GROUP BY "AbsEntry") AS b ON a."AbsEntry" = b."AbsEntry" AND a."BinAbs" = b."BinAbs");

		--lấy ra danh sách các item - batch/serial - bin chưa được sum vào kết quả
		create local temporary table #tempGreatQty as (select * from #tempSum where "sumvalue" >= "WhsQty" - "WhsCommit");

		--insert thêm các item - batch/serial - bin nhỏ nhất vào kết quả với qty bằng số lượng còn thiếu
		INSERT INTO #tempQty 
			SELECT "ItemCode", a."BinAbs", "BinCode", CASE WHEN c."AbsEntry" IS NULL THEN a."WhsQty" - a."WhsCommit" ELSE c."MissQty" END, "WhsCode", "DistNumber", a."AbsEntry", a."SysNumber"
					, "MnfSerial", "LotNumber", a."WhsQty", a."WhsCommit", 0 FROM #tempGreatQty a 
								INNER JOIN (SELECT MIN("BinAbs") AS "BinAbs", "AbsEntry" FROM #tempGreatQty GROUP BY "AbsEntry") AS b ON a."AbsEntry" = b."AbsEntry" AND a."BinAbs" = b."BinAbs"
								LEFT JOIN #tempMissing c ON a."AbsEntry" = c."AbsEntry";

		--create table temp
		create local temporary table #temp as (SELECT TOP 0 *, cast(null as numeric(19,6)) AS "Instock", "AbsEntry" AS "BinAbs", cast(null as nvarchar(50)) AS "BinCode", cast(null as numeric(19,6)) AS "WhsQty"
					, cast(null as numeric(19,6)) AS "WhsCommit", cast(null as numeric(19,6)) AS "SelectedQty", cast(null as numeric(19,6)) AS "sumvalue" FROM OBTN);

		--get data sumvalue < qty
		sqlQuery = 'insert into #temp select * from (select b.*, "Instock", "BinAbs", "BinCode", "WhsQty", "WhsCommit", "WhsQty" - "WhsCommit",sum("Instock") over (order by ' || :columsOrder || ') "sumvalue" from #tempQty t1 
									INNER JOIN (SELECT * from OBTN where "ItemCode" = ''' || :itemCode || ''' and ''' || :typeItem || ''' = ''B''
									UNION ALL 
									SELECT * from OSRN where "ItemCode" = ''' || :itemCode || ''' and ''' || :typeItem || ''' = ''S'') b ON t1."AbsEntry" = b."AbsEntry") a where "sumvalue" < ' || cast(:qty as nvarchar) || ';';
		exec :sqlQuery;

		select ifnull((select MAX("sumvalue") from #temp),0) into sumQty from DUMMY;
		sumQty = :qty - :sumQty;

		sqlQuery = 'insert into #temp 
			select top 1 * from (select b.*, ' || cast(:sumQty as nvarchar) || ' "Instock", "BinAbs", "BinCode", "WhsQty", "WhsCommit", "WhsQty" - "WhsCommit",sum("Instock") over (order by ' || :columsOrder || ') "sumvalue" from #tempQty t1 INNER JOIN 
									(SELECT * from OBTN where "ItemCode" = ''' || :itemCode || ''' and ''' || :typeItem || ''' = ''B''
									UNION ALL 
									SELECT * from OSRN where "ItemCode" = ''' || :itemCode || ''' and ''' || :typeItem || ''' = ''S'') b ON t1."AbsEntry" = b."AbsEntry") a where "sumvalue" >= ' || cast(:qty as nvarchar) || ';';
		exec :sqlQuery;
		select * from #temp;
	ELSE

		IF :typeItem= 'B'
		then

			--get structure temp table
			create local temporary table #tempBatch as (SELECT TOP 0 *, cast(null as numeric(19,6)) AS "InStock", cast(null as nvarchar(50)) AS "WhsCode", cast(null as numeric(19,6)) AS "LogQty", cast(null as numeric(19,6)) AS "CommitQty"
					, cast(null as char(1)) AS "ManBatchSerial", cast(null as numeric(19,6)) AS "sumvalues" FROM OBTN);

			sqlQuery = 'INSERT INTO #tempBatch select * from (SELECT b.*, A."Quantity" - ifnull(A."CommitQty",0) - ifnull(d."LogQty", 0) "InStock", A."WhsCode", a."Quantity" "LogQty", a."CommitQty" "CommitQty", ''B'' "ManBatchSerial" , 
							SUM(A."Quantity" - ifnull(A."CommitQty",0) - ifnull(d."LogQty", 0)) OVER (ORDER BY ' || :columsOrder || ') "sumvalues"
			FROM            (Select "Quantity", "WhsCode", "CommitQty", "SysNumber", "MdAbsEntry" FROM OBTQ where "ItemCode"=''' || :ItemCode || ''' and "WhsCode"=''' || :whsCode || ''') A 
						INNER JOIN (Select * from OBTN where "ItemCode"=''' || :ItemCode || ''') b ON A."SysNumber" = b."SysNumber" and a."MdAbsEntry" = b."AbsEntry"  
						INNER JOIN OITM c ON b."ItemCode" = c."ItemCode" AND c."validFor" = ''Y''
						LEFT JOIN #tempExistLocal d ON b."DistNumber" = d."DistNumber" and b."ItemCode" = d."ItemCode"
			WHERE        ifnull(A."Quantity",0) - ifnull(A."CommitQty",0) - ifnull(d."LogQty", 0) > 0) a where "sumvalues" < ' || cast(:qty as nvarchar) || ';';

			exec :sqlQuery;
			--declare @absEntry int = ifnull((select MAX(AbsEntry) from #tempBatch),0)
			select ifnull((select max("sumvalues") from #tempBatch),0) into sumQtyBatch from dummy;

			sqlQuery = 'insert into #tempBatch 
			SELECT top 1 b.*,' || cast((:qty  - :sumQtyBatch) as nvarchar) || ' "InStock", A."WhsCode", a."Quantity", a."CommitQty", ''B'', 0
			FROM            (SELECT "Quantity", "WhsCode", "CommitQty", "SysNumber", "MdAbsEntry" FROM OBTQ where "ItemCode"=''' || :ItemCode || ''' and "WhsCode"='''|| :whsCode || ''') A 
						INNER JOIN (Select * from OBTN where "ItemCode"=''' || :ItemCode || ''') b ON A."SysNumber" = b."SysNumber" and a."MdAbsEntry" = b."AbsEntry"  
						INNER JOIN OITM c ON b."ItemCode" = c."ItemCode" AND c."validFor" = ''Y''
						LEFT JOIN #tempExistLocal d ON b."DistNumber" = d."DistNumber" and b."ItemCode" = d."ItemCode"
						LEFT JOIN #tempBatch e ON b."ItemCode" = e."ItemCode" and b."DistNumber" = e."DistNumber"
			WHERE e."ItemCode" is null and ifnull(A."Quantity",0) - ifnull(A."CommitQty",0) - ifnull(d."LogQty", 0) > 0 order by ' || :columsOrder || ';';
			
			exec :sqlQuery;

			select *, 0 "BinAbs", '' "BinCode", 0 "ToBinAbs", '' "ToBinCode", ifnull("LogQty",0) - ifnull("CommitQty",0) as "SelectedQty" from #tempBatch;


		ELSE 
			If :typeItem= 'S'
			then
				sqlQuery = 'select *, 0 "BinAbs", '''' "BinCode", 0 "ToBinAbs", '''' "ToBinCode", 1 "SelectedQty" from (SELECT  b.*, A."Quantity" "InStock", A."WhsCode", cast(0 as NUMERIC(19,6)) "LogQty"
						, ifnull(a."CommitQty",0) "CommitQty", SUM(A."Quantity" - ifnull(A."CommitQty",0)) OVER (ORDER BY ' || :columsOrder || ') "sumvalues"
					FROM            (SELECT "Quantity", "WhsCode", "CommitQty", "SysNumber", "MdAbsEntry" FROM OSRQ where "ItemCode"=''' || :ItemCode || ''' and "WhsCode"=''' || :whsCode || ''') A INNER JOIN
										 (Select * from OSRN where "ItemCode"=''' || :ItemCode || ''') b ON A."SysNumber" = b."SysNumber" and a."MdAbsEntry" = b."AbsEntry"  INNER JOIN
										 OITM c ON b."ItemCode" = c."ItemCode" AND c."validFor" = ''Y''
										 LEFT JOIN #tempExistLocal d ON  A."SysNumber" = d."SysNumber" and a."MdAbsEntry" = d."AbsEntry"
					WHERE        A."Quantity" - ifnull(A."CommitQty",0) > 0 and ifnull(d."ItemCode", '''') = '''') a where "sumvalues" <= ' || cast(:qty as nvarchar) || ';';
			end if;
			
			execute immediate :sqlQuery;
		END if;

	END if;

END




