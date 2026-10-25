
CREATE PROCEDURE APZ_UDO_GetFieldsForFind(
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

        INSERT INTO #tblStructure ("ID", "ColumnName", "Caption", "DataType", "SubType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType") 
			select 1, n'IsCheck', n' ', 'bit', '', 50, -1, 1, NULL, NULL, 0, 1, 1, 1, 1, 'L', NULL, NULL from DUMMY
		Union ALL 
			select 2, n'ColAlias', n'Column Alias', 'nvarchar', '', 75, -1, 1, NULL, NULL, 1, 1, 1, 1, 1, '', NULL, NULL from DUMMY
		Union ALL
			SELECT 3, n'ColumnDesc', n'Column Description', 'nvarchar', '', 100, -1, 1, NULL, NULL, 0, 1, 1, 1, 1, '', NULL, NULL FROM DUMMY;

	SELECT *
        FROM #tblStructure ORDER BY ID;
		
	create local temporary table #tbl
        (
			"ColAlias" nvarchar(50) NOT NULL
	        ,"ColumnDesc" NVARCHAR(80)
	        ,"IsCheck" smallint	
        );
	IF :udoType = 'A'
		then
		
			INSERT INTO #tbl ("ColAlias", "ColumnDesc", "IsCheck") 
				SELECT COLUMN_NAME, IFNULL(b."Descr", a.COLUMN_NAME), 
							(CASE WHEN COLUMN_NAME IN ('Code','Name','DocNum','CreateDate','UpdateDate') THEN 1 ELSE 0 END) FROM TABLE_COLUMNS a 
												LEFT OUTER JOIN CUFD b ON b."TableID" = a.TABLE_NAME AND 'U_' || b."AliasID" = a.COLUMN_NAME WHERE SCHEMA_NAME = CURRENT_SCHEMA and TABLE_NAME = '@' || :tableName
												ORDER BY a.POSITION;

		else
			INSERT INTO #tbl ("ColAlias", "ColumnDesc", "IsCheck") 
				SELECT A.COLUMN_NAME, IFNULL(B."ColumnDesc", A.COLUMN_NAME), CASE WHEN B."Code" IS NULL THEN 0 ELSE 1 END FROM TABLE_COLUMNS A 
							INNER JOIN OUDO C ON A.TABLE_NAME = '@' || C."TableName" LEFT OUTER JOIN UDO2 B ON A.COLUMN_NAME = B."ColAlias" AND C."Code" = B."Code" WHERE SCHEMA_NAME = CURRENT_SCHEMA and C."TableName" = :tableName
							ORDER BY A.POSITION;
		END IF;

	SELECT * FROM #tbl ORDER BY "ColAlias";

END






