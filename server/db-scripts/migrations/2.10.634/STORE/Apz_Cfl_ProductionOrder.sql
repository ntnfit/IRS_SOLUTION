
CALL APZ_EXISTSPROC('Apz_Cfl_ProductionOrder');

GO


CREATE PROCEDURE Apz_Cfl_ProductionOrder (
	IN 	Type VARCHAR(50) DEFAULT ''
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER

AS
BEGIN
	-- Khai báo bảng dữ liệu combobox (tùy chọn)
	DECLARE tblValidValue TABLE (
		"ColumnName" NVARCHAR(50) NOT NULL
		,"Value" NVARCHAR(254) NOT NULL
		,"Description" NVARCHAR(254) NOT NULL
		,"BackColor" NVARCHAR(20)
		,"ForeColor" NVARCHAR(20)
	);
	INSERT INTO :tblValidValue ("ColumnName", "Value", "Description")
		select N'Type',N'S' ,		N'Standard' from DUMMY
			UNION ALL
		select N'Type',N'D' ,		N'Disassembly' from DUMMY
			UNION ALL
		select N'Type',N'P' ,		N'Special' from DUMMY;

	CREATE LOCAL TEMPORARY COLUMN TABLE #tblStructure (
		 "ID" INT
		,"ColumnName" NVARCHAR(50) NOT NULL
		,"Caption" NVARCHAR(100)
		,"DataType" NVARCHAR(50) DEFAULT 'nvarchar'	--int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox) LinkedTable LinkedUDO* LinkedObject cfl(choosefromlist) LinkedCflObject
	                                        		-- color, validcolor (combobox+color),	linkedStore, LinkedCrystal
		,"SubType" NVARCHAR(500)					-- None Phone Percentage Address Image Measurement Price Quantity Rate Sum Time, UDT dùng cho LinkedTable, (udo code : dùng cho LinkedUDO)*	 , ObjectType(số): dùng cho LinkedObject
	                            					-- type of cfl - số của type hoặc tên store(storename @value, @userName, @branch) lấy dữ liệu cfl-store phải có structure, sub - sub form
		,"Width" INT DEFAULT 75
		,"GroupIndex" INT DEFAULT -1				-- dùng cho grid  default -1
		,"Visible" TINYINT DEFAULT 1
		,"FormatType" NVARCHAR(20)					-- numeric, datetime/ 
		,"DisplayFormat" NVARCHAR(20) NULL			-- '{0:n2}'
		,"FilterString" NVARCHAR(5000)
		,"ReadOnly" TINYINT DEFAULT 0
		,"AllowEdit" TINYINT DEFAULT 1
		,"AllowMove" TINYINT DEFAULT 1
		,"AllowSize" TINYINT DEFAULT 1
		,"AllowSort" TINYINT DEFAULT 1
		,"Fixed" VARCHAR(1)							-- L:Left / R:Right
		,"BackColor" NVARCHAR(20)					-- color column //list color https://msdn.microsoft.com/en-us/library/aa358802(v=vs.85).aspx
		,"ForeColor" NVARCHAR(20)
		,"SummaryType" NVARCHAR(10)					-- sum,average...
		,"GroupSummaryType" NVARCHAR(10)
		,"GroupSummaryShowInFooter" TINYINT
		,"SummaryFormat" NVARCHAR(20)				-- (AVR={0:#.##})/ Count ({0})/ Custom format/ Max (MAX={0})/ Min (MIN={0})/ Sum (SUM={0:#.##})
		,"Expression" NVARCHAR(5000)
		,"Parent" TINYINT							-- dùng cho treelist
		,"Key" TINYINT
		,"PivotArea" NVARCHAR(20)
		);	
	 
	 
	 
	INSERT INTO #tblStructure ("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
  		SELECT   		N'DocEntry',		N'DocEntry',			'int',			60,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'N',	NULL,		 NULL FROM DUMMY
		UNION SELECT	N'DocNum',			N'Doc. No.',			'int',			60,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'N',	NULL,		 NULL FROM DUMMY		
		UNION SELECT	N'LineNum',			N'Line Num',			'int',			60,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'N',	NULL,		 NULL FROM DUMMY
		UNION SELECT	N'SeriesName',			N'Series',				'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'N',	NULL,		 NULL FROM DUMMY
		UNION SELECT	N'DueDate',			N'Doc Due Date',		'date',			75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'N',	NULL,		 NULL FROM DUMMY
		UNION SELECT	N'ItemCode',		N'Product',				'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'N',	NULL,		 NULL FROM DUMMY
		UNION SELECT	N'ItemName',		N'Production Description','nvarchar',	75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'N',	NULL,		 NULL FROM DUMMY
		UNION SELECT	N'Comments',		N'Remark',				'nvarchar',		160,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'N',	NULL,		 NULL FROM DUMMY
		UNION SELECT	N'Type',			N'Type',				'validvalue',		160,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'N',	NULL,		 NULL FROM DUMMY;		
		
	SELECT * FROM #tblStructure;
	
	create local temporary table #tbl as (select top 0 *, -1 "LineNum", cast(null as nvarchar(50)) "SeriesName", -1 "UgpEntry", cast(null as nvarchar(100)) "InvntryUom", cast(null as nvarchar(100)) "ItemName", cast(null as numeric(19,6)) "Qty" from OWOR);
	insert into #tbl
	SELECT T0.*, t3."LineNum", t2."SeriesName", t1."UgpEntry", t1."InvntryUom", t3."ItemName", Abs((IFNULL(t3."PlannedQty", 0) - IFNULL(t3."IssuedQty", 0)))
	FROM OWOR T0 
	INNER JOIN WOR1 t3 ON T0."DocEntry" = t3."DocEntry" 
	INNER JOIN OITM t1 ON t3."ItemCode" = t1."ItemCode" 
	INNER JOIN NNM1 t2 ON T0."Series" = t2."Series" AND t2."ObjectCode" = '202'
	WHERE (:Type in ('I', 'R', 'D') AND T0."Status" = 'R' OR :Type not in ('I', 'R', 'D') AND T0."Status" in ('R', 'P')) AND t3."IssueType" = 'M'
		AND ((:Type = 'I' AND "Type" IN ('S','P') AND t3."PlannedQty" > 0)
		OR (:Type = 'R' AND ("Type" = 'D'  OR t3."PlannedQty" < 0)))
		AND IFNULL(t3."PlannedQty", 0) - IFNULL(t3."IssuedQty", 0) <> 0;
	
	update a set "ItemCode" = b."ItemCode", "Warehouse" = b."wareHouse" from #tbl a, WOR1 b where a."DocEntry"=b."DocEntry" and a."LineNum"=b."LineNum";
	
	insert into #tbl
	SELECT T0.*, -1, t2."SeriesName", t1."UgpEntry", t1."InvntryUom", t1."ItemName", ifnull(T0."PlannedQty",0) - ifnull(T0."CmpltQty",0)
	FROM OWOR T0 
	INNER JOIN OITM t1 ON T0."ItemCode" = t1."ItemCode" 
	INNER JOIN NNM1 t2 ON T0."Series" = t2."Series" AND t2."ObjectCode" = '202'
	WHERE (:Type in ('I', 'R', 'D') AND T0."Status" = 'R' OR :Type not in ('I', 'R', 'D') AND T0."Status" in ('R', 'P'))
		AND ((:Type='I' AND T0."Type"='D') 
		 OR (:Type='R' AND T0."Type" IN ('S','P')))
		AND ifnull(T0."PlannedQty",0) - ifnull(T0."CmpltQty",0) > 0;
	
	select * from #tbl order by "DocEntry", "LineNum";
			 
	select * from :tblValidValue;
	
	DROP TABLE #tblStructure;

END;