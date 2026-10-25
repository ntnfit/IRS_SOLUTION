
CREATE PROCEDURE APZ_UDO_GetFieldsChild(in tableName NVARCHAR(100))
AS
BEGIN
	-- Khai báo bảng Structure
	create local temporary table #tblStructure
    (
		"ID" INT
		,"ColumnName" NVARCHAR(50)
		,"Caption" NVARCHAR(100)
		,"DataType" VARCHAR(50) DEFAULT 'nvarchar' --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox) LinkedTable LinkedUDO* LinkedObject cfl(choosefromlist) LinkedCflObject
												-- color, validcolor (combobox+color),	linkedStore, LinkedCrystal
		,"SubType" VARCHAR(100)	-- None Phone Percentage Address Image Measurement Price Quantity Rate Sum Time, UDT dùng cho LinkedTable, (udo code : dùng cho LinkedUDO)*	 , ObjectType(số): dùng cho LinkedObject
									-- type of cfl - số của type hoặc tên store(storename @value, @userName, @branch) lấy dữ liệu cfl-store phải có structure, sub - sub form
		,"Width" INT DEFAULT 75
		,"GroupIndex" INT DEFAULT -1	-- dùng cho grid  default -1
		,"Visible" smallint DEFAULT 1
		,"FormatType" NVARCHAR(20)   -- numeric, datetime/ 
		,"DisplayFormat" NVARCHAR(20) NULL  -- '{0:n2}'
		,"ReadOnly" smallint DEFAULT 0
		,"AllowEdit" smallint DEFAULT 1
		,"AllowMove" smallint DEFAULT 1
		,"AllowSize" smallint DEFAULT 1
		,"AllowSort" smallint DEFAULT 1
		,"Fixed" VARCHAR(1)	-- L:Left / R:Right
		,"BackColor" NVARCHAR(20) -- color column //list color https://msdn.microsoft.com/en-us/library/aa358802(v=vs.85).aspx
		,"ForeColor" NVARCHAR(20)
		-- Summary Item
		,"SummaryType" VARCHAR(10)	-- sum,average...
		,"GroupSummaryType" VARCHAR(10)	-- sum,average...
		,"GroupSummaryShowInFooter" smallint
		,"SummaryFormat" VARCHAR(20) -- (AVR={0:#.##})/ Count ({0})/ Custom format/ Max (MAX={0})/ Min (MIN={0})/ Sum (SUM={0:#.##})
		,"Expression" NVARCHAR(5000)
    );

    INSERT INTO #tblStructure (ID, "ColumnName", "Caption", "DataType", "SubType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType") 
		select 1, n'SonNum', n'SonNum', 'int', '', 60, -1, 0, NULL, NULL, 1, 1, 1, 1, 1, 'L', NULL, NULL from dummy
	UNION ALL
		select 2, n'ColIsUsed', n'Visible', 'bit', '', 60, -1, 1, NULL, NULL, 0, 1, 1, 1, 1, 'L', NULL, NULL from DUMMY
	UNION ALL
		select 3, n'ColEdit', n'Editable', 'bit', '', 60, -1, 1, NULL, NULL, 0, 1, 1, 1, 1, 'L', NULL, NULL from DUMMY
	UNION ALL 
		select 4, n'ColAlias', n'Column Alias', 'nvarchar', '', 75, -1, 1, NULL, NULL, 1, 1, 1, 1, 1, '', NULL, NULL from DUMMY
	UNION ALL
		select 5, n'ColDesc', n'Column Description', 'nvarchar', '', 100, -1, 1, NULL, NULL, 0, 1, 1, 1, 1, '', NULL, NULL from dummy;
	
	SELECT *  FROM #tblStructure ORDER BY ID;
		
	create local temporary table #tbl
        (
			"SonNum" INT
			,"ColAlias" nvarchar(50) NOT NULL
	        ,"ColDesc" NVARCHAR(80)
	        ,"ColIsUsed" smallint	
			,"ColEdit" smallint	
        );
		
		INSERT INTO #tbl ("SonNum", "ColAlias", "ColDesc", "ColIsUsed", "ColEdit") 
			SELECT DISTINCT -1, COLUMN_NAME, "ColDesc", "ColIsUsed", "ColEdit"
				FROM (select A.COLUMN_NAME, IFNULL(B."ColDesc", A.COLUMN_NAME) "ColDesc", CASE WHEN IFNULL(B."ColIsUsed", 'N') <> 'Y' THEN 0 ELSE 1 END AS "ColIsUsed"
								, CASE WHEN IFNULL(B."ColEdit", 'N') <> 'Y' THEN 0 ELSE 1 END  "ColEdit"
							from TABLE_COLUMNS A INNER JOIN OUTB D ON SUBSTRING(A.TABLE_NAME, 2, 19) = D."TableName" 
								LEFT OUTER JOIN UDO4 B ON A.COLUMN_NAME = B."ColAlias" AND B."Code" = D."UsedInObj" 
								LEFT OUTER JOIN CUFD c ON c."TableID" = A.TABLE_NAME AND 'U_' || c."AliasID" = A.COLUMN_NAME WHERE a.SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME = '@' || :tableName
								ORDER BY A.POSITION) x;

	SELECT * FROM #tbl;

END;




