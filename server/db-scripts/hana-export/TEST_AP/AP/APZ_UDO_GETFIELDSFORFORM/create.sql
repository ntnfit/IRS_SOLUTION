
CREATE PROCEDURE APZ_UDO_GetFieldsForForm(
	in tableName NVARCHAR(100)
,	in udoType NVARCHAR(1) default 'A'	--A/U : Add/Update
)
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
		select 1, n'IsVisible', n'Visible', 'bit', '', 60, -1, 1, NULL, NULL, 0, 1, 1, 1, 1, 'L', NULL, NULL from DUMMY
	UNION all
		select 2, n'IsEdit', n'Editable', 'bit', '', 60, -1, 1, NULL, NULL, 0, 1, 1, 1, 1, 'L', NULL, NULL from DUMMY
	Union ALL
		select 3, n'ColAlias', n'Column Alias', 'nvarchar', '', 75, -1, 1, NULL, NULL, 1, 1, 1, 1, 1, '', NULL, NULL from DUMMY
	UNION ALL
		select 4, n'ColDesc', n'Column Description', 'nvarchar', '', 100, -1, 1, NULL, NULL, 0, 1, 1, 1, 1, '', NULL, NULL from DUMMY;

	SELECT *  FROM #tblStructure ORDER BY ID;
		
	create local temporary table #tbl
        (
			"ColAlias" nvarchar(50) NOT NULL
	        ,"ColDesc" NVARCHAR(80)
	        ,"IsVisible" BOOLEAN	
			,"IsEdit" BOOLEAN	
        );
	IF :udoType = 'A'
	then
			INSERT INTO #tbl ("ColAlias", "ColDesc", "IsVisible", "IsEdit") 
				SELECT COLUMN_NAME, IFNULL(b."Descr", a.COLUMN_NAME), (CASE WHEN COLUMN_NAME IN ('Code','DocEntry') THEN true ELSE false END), false FROM TABLE_COLUMNS a 
													LEFT OUTER JOIN CUFD b ON b."TableID" = a.TABLE_NAME AND 'U_' || b."AliasID" = a.COLUMN_NAME WHERE SCHEMA_NAME = CURRENT_SCHEMA and TABLE_NAME = '@' || :tableName
													ORDER BY a.POSITION;
	ELSE
			INSERT INTO #tbl ("ColAlias", "ColDesc", "IsVisible", "IsEdit") 
				SELECT A.COLUMN_NAME, IFNULL(B."ColDesc", A.COLUMN_NAME), CASE WHEN B."Code" IS NULL THEN false ELSE true END, CASE WHEN IFNULL(B."ColEdit", 'N') = 'N' THEN false ELSE true END FROM TABLE_COLUMNS A 
										INNER JOIN OUDO C ON A.TABLE_NAME = '@' || C."TableName" 
										LEFT OUTER JOIN UDO3 B ON A.COLUMN_NAME = B."ColAlias" AND C."Code" = B."Code" AND B."SonNum" = 0 WHERE SCHEMA_NAME = CURRENT_SCHEMA and TABLE_NAME = '@' || :tableName
										ORDER BY A.POSITION;					
	END IF;

	SELECT * FROM #tbl;

END






