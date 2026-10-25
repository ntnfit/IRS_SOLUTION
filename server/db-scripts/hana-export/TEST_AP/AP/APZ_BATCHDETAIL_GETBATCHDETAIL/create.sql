
CREATE PROCEDURE APZ_BatchDetail_GetBatchDetail (
	IN 	AbsEntry INT DEFAULT -1
,		ItemCode NVARCHAR(50) DEFAULT ''
,		Status NVARCHAR(1) DEFAULT ''
,		DistNumber NVARCHAR(50) DEFAULT ''
,		MnfSerial NVARCHAR(50) DEFAULT ''
,		LotNumber NVARCHAR(50) DEFAULT ''
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER

AS
BEGIN
	-- Khai báo bảng dữ liệu combobox (tùy chọn)
	CREATE LOCAL TEMPORARY COLUMN TABLE #tblValidValue (
		 "ColumnName" NVARCHAR(50) NOT NULL
		 ,"Value" NVARCHAR(254) NOT NULL
		 ,"Description" NVARCHAR(254) NOT NULL
		 ,"BackColor" NVARCHAR(20)
		 ,"ForeColor" NVARCHAR(20)
	 );
	 INSERT INTO #tblValidValue  ("ColumnName", "Value", "Description")
		SELECT 			N'Status',N'0' ,	N'Released' 		FROM DUMMY
		UNION SELECT 	N'Status',N'1' ,	N'Not Accessible' 	FROM DUMMY	
		UNION SELECT 	N'Status',N'2' ,	N'Locked' 			FROM DUMMY;

	CREATE LOCAL TEMPORARY TABLE #tblStructure (
		 "ID" INT
		,"VisIndex" INT
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
		,"Fixed" VARCHAR(1)							-- L:Left / R:Right  --vgrid: T:Top / B:Bottom
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
	 
	INSERT INTO #tblStructure ("ID", "ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "FormatType", "DisplayFormat", "SummaryType", "Fixed", "SubType")
		SELECT   		1,  N'ItemCode',		N'Item Number',			'linkedobject',	25,		-1,				1,			1,			1,			0,0,1,NULL,NULL,NULL,'T','4' FROM DUMMY
		UNION SELECT	2,  N'itemName',		N'Item Description',	'nvarchar',		25,		-1,				1,			1,			1,			0,0,1,NULL,NULL,NULL,'T',NULL FROM DUMMY
		UNION SELECT	3,  N'Status',			N'Status',				'validvalue',	25,		-1,				1,			0,			1,			0,0,1,NULL,NULL,NULL,'T',NULL FROM DUMMY
		UNION SELECT	4,  N'DistNumber',		N'Batch',				'nvarchar',		25,		-1,				1,			1,			1,			0,0,1,NULL,NULL,NULL,'',NULL FROM DUMMY
		UNION SELECT	5,  N'MnfSerial',		N'Batch Attribute 1',	'nvarchar',		25,		-1,				1,			0,			1,			0,0,1,NULL,NULL,NULL,'',NULL FROM DUMMY
		UNION SELECT	6,  N'LotNumber',		N'Batch Attribute 2',	'nvarchar',		25,		-1,				1,			0,			1,			0,0,1,NULL,NULL,NULL,'',NULL FROM DUMMY
		UNION SELECT	7,  N'InDate',			N'Admission Date',		'datetime',		25,		-1,				1,			0,			1,			0,0,1,NULL,NULL,NULL,'',NULL FROM DUMMY
		UNION SELECT	8,  N'MnfDate',			N'Manufacturing Date',	'datetime',		25,		-1,				1,			0,			1,			0,0,1,NULL,NULL,NULL,'',NULL FROM DUMMY
		UNION SELECT	9,  N'ExpDate',			N'Expiration Date',		'datetime',		25,		-1,				1,			0,			1,			0,0,1,NULL,NULL,NULL,'',NULL FROM DUMMY
		UNION SELECT	10, N'Notes',			N'Details',				'nvarchar',		25,		-1,				1,			0,			1,			0,0,1,NULL,NULL,NULL,'',NULL FROM DUMMY
		UNION SELECT	11, N'SysNumber',		N'System Number',		'int',			25,		-1,				1,			1,			1,			0,0,1,NULL,NULL,NULL,'',NULL FROM DUMMY
		UNION SELECT	12, N'CostTotal',		N'Cost',				'numeric',		25,		-1,				1,			1,			1,			0,0,1,'numeric','{0:n0}',NULL,'',NULL FROM DUMMY;
	INSERT INTO #tblStructure ("ID", "ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort")
		SELECT ROW_NUMBER() OVER ( ORDER BY b.POSITION) + 12, 'U_' || a."AliasID", a."Descr", b.DATA_TYPE_NAME, 25, -1, 1, 0, 1, 0, 0, 1 
			FROM CUFD a 
			INNER JOIN TABLE_COLUMNS b ON b.COLUMN_NAME = 'U_' || a."AliasID" 
			WHERE SCHEMA_NAME = CURRENT_SCHEMA 
				AND b.TABLE_NAME = 'OBTN'  AND a."TableID" = 'OBTN'
			ORDER BY POSITION;
	UPDATE #tblStructure SET "VisIndex" = ID - 1;
							
	IF :AbsEntry > 0 THEN 
		SELECT * FROM #tblStructure ORDER BY ID;
		SELECT * FROM OBTN WHERE "AbsEntry" = :AbsEntry;
	ELSEIF :AbsEntry = 0 THEN 
		SELECT * FROM #tblStructure ORDER BY ID;
		SELECT b.* FROM OADM a LEFT OUTER JOIN OBTN b ON 1 = 2;
	ELSEIF :AbsEntry < 0 THEN 
		-- điều chỉnh lại formsetting
		UPDATE #tblStructure SET "Width" = 75, "AllowSize" = 1;
		SELECT * FROM #tblStructure ORDER BY ID;
		SELECT * FROM OBTN a 
			WHERE (:ItemCode = '' OR (:ItemCode <> '' AND a."ItemCode" = :ItemCode)) 
				AND (:Status = '' OR (:Status <> '' AND a."Status" = :Status)) 
				AND (:DistNumber = '' OR (:DistNumber <> '' AND a."DistNumber" = :DistNumber)) 
				AND (:MnfSerial = '' OR (:MnfSerial <> '' AND a."MnfSerial" = :MnfSerial)) 
				AND (:LotNumber = '' OR (:LotNumber <> '' AND a."LotNumber" = :LotNumber));
	END IF;
	SELECT * FROM #tblValidValue;
	
	DROP TABLE #tblStructure;
	DROP TABLE #tblValidValue;
END;




