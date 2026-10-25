
CREATE PROCEDURE APZ_CFL_GetDataListUoMForUoMPrices(
	in ItemCode nvarchar(50) default '',
	in PriceList int default 0,
	in ugpEntry int)
AS
BEGIN /* Grid Structure table */

	declare BaseUomEntry int;
	declare BasePrice numeric(19,6);
	
	-- Khai báo bảng dữ liệu combobox (tùy chọn)
	CREATE LOCAL TEMPORARY TABLE #tblValidValue
	(
		"ColumnName" NVARCHAR(50) NOT NULL
		,"Value" NVARCHAR(254) NOT NULL
		,"Display" NVARCHAR(254) NOT NULL
	);
	
	-- Khai báo bảng Structure
	Create local temporary table #tblStructure
	    (
	        "ColumnName" NVARCHAR(50) NOT NULL
		    ,"Caption" NVARCHAR(100) NULL
		    ,"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar' --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
			,"SubType" VARCHAR(20) NULL			--none phone percentage address image measurement price quantity rate sum time
		    ,"Width" INT NULL DEFAULT 75
		    ,"GroupIndex" INT NULL DEFAULT -1	-- dùng cho grid  default -1
		    ,"Visible" smallint NULL DEFAULT 1
		    ,"FormatType" NVARCHAR(20) NULL   -- numeric, datetime/ 
		    ,"DisplayFormat" NVARCHAR(20) NULL  -- '{0:n2}'
		    ,"ReadOnly" smallint NULL DEFAULT 0
		    ,"AllowEdit" smallint NULL DEFAULT 1
		    ,"AllowMove" smallint NULL DEFAULT 1
		    ,"AllowSize" smallint NULL DEFAULT 1
		    ,"AllowSort" smallint NULL DEFAULT 1
		    ,"Fixed" VARCHAR(1) NULL	-- L:Left / R:Right
			-- Summary Item
		    ,"SummaryType" VARCHAR(10) NULL	-- sum,average...
			-- Group Summary
		    ,"GroupSummaryType" VARCHAR(10) NULL	-- sum,average...
			--,Parent BIT NULL	-- dùng cho treelist
			--,[Key] BIT NULL		-- dùng cho treelist (identity)
	    );
	
	INSERT INTO #tblStructure("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType") 
			select n'UomEntry', n'UomEntry', 'int', 150, -1, 0, NULL, NULL, 1, 0, 1, 1, 1, '', NULL, NULL from dummy
	UNION ALL
			select N'UomCode',	N'UomCode', 'nvarchar', 150, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, '', NULL, NULL from DUMMY
	UNION ALL
			SELECT N'UomName', N'UomName',	'nvarchar',	150, -1, 1,	NULL, NULL,	1, 0, 1, 1,	1, '', NULL, NULL FROM DUMMY;
			
	select * from #tblStructure;
	
	select ifnull((Select "BaseUom" From OUGP Where "UgpEntry"=(Select "UgpEntry" from OITM where "ItemCode"=:ItemCode)),0) into BaseUomEntry from dummy;
	select ifnull((Select "Price" From ITM1 Where "PriceList"= :PriceList AND "ItemCode"= :ItemCode),0) into BasePrice from DUMMY;
	
	SELECT a."UomEntry", a."UomCode", a."UomName", (SELECT "BaseQty" / "AltQty" FROM UGP1 WHERE "UgpEntry" = :UgpEntry AND "UomEntry" = a."UomEntry") * :BasePrice AS "BasePrice", b."BaseQty" / b."AltQty" AS "ItemPerUnit" FROM OUOM a INNER JOIN UGP1 b ON a."UomEntry" = b."UomEntry" WHERE b."UgpEntry" = :ugpEntry;
END



