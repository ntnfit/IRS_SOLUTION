
CREATE PROCEDURE APZ_Picking_LoadSalesOrder (
IN docEntry integer, 
IN fromDate timestamp, 
IN toDate timestamp, 
IN docStatus nvarchar(100), 
IN bpCode nvarchar(100), 
IN userName nvarchar(100), 
IN branch integer, 
IN warehouse nvarchar(30))
AS
    BEGIN
declare mindate timestamp;
declare maxdate timestamp;
		IF toDate IS NULL
		then
			 toDate = CURRENT_DATE;
			end if;
			/*temp_var_0 integer;
SELECT :toDate INTO temp_var_0 FROM DUMMY;
IF :temp_var_0 IS NULL THEN toDate := CURRENT_DATE;
END IF;*/
		 -- Khai báo bảng dữ liệu combobox (tùy chọn)
		CREATE  LOCAL TEMPORARY TABLE #tblValidValue (
		"ColumnName" nvarchar(50) NOT NULL, 
		"Description" nvarchar(254) NOT NULL, 
		"Value" nvarchar(254) NOT NULL, 
		"BackColor" nvarchar(20), 
		"ForeColor" nvarchar(20));

		
		-- Khai bao bang Structure
        CREATE LOCAL TEMPORARY TABLE #tblStructure (
        ID integer , 
        "ColumnName" nvarchar(50) NOT NULL, 
        "Caption" nvarchar(100), 
        "DataType" varchar(50) DEFAULT 'nvarchar', 
        "SubType" varchar(100), 
        "Width" integer DEFAULT 75, 
        "GroupIndex" integer DEFAULT -1, 
        "Visible" tinyint DEFAULT 1, 
        "FormatType" nvarchar(20), 
        "DisplayFormat" nvarchar(20) NULL, 
        "ReadOnly" tinyint DEFAULT 0, 
        "AllowEdit" tinyint DEFAULT 1, 
        "AllowMove" tinyint DEFAULT 1, 
        "AllowSize" tinyint DEFAULT 1, 
        "AllowSort" tinyint DEFAULT 1, 
        "Fixed" varchar(1), 
        "BackColor" nvarchar(20), 
        "ForeColor" nvarchar(20), 
        "SummaryType" varchar(10), 
        "GroupSummaryType" varchar(10), 
        "GroupSummaryShowInFooter" tinyint, 
        "SummaryFormat" varchar(20), 
        "Expression" nvarchar(5000), 
        "Parent" tinyint);
		-- insert bảng Structure
        INSERT INTO  #tblStructure ("ColumnName", "Caption", "DataType", "SubType", "Width", "GroupIndex", "Visible", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "FormatType", "DisplayFormat", "Fixed")
         SELECT N'OrderEntry',		N'Doc No',			'linkedobject',	'17',	75,		-1, 1, 1, 1, 1, 1, 0, 'NUMERIC',	'',			NULL FROM DUMMY
				   UNION SELECT N'OrderLine',		N'Doc Row',			'int',			'',		50,		-1, 1, 1, 1, 1, 1, 1, 'NUMERIC',	'',			NULL FROM DUMMY
                   UNION SELECT N'CardCode',		N'BP Code',			'linkedobject',	'2',	100,	-1, 1, 1, 1, 1, 1, 1, '',			'',			NULL FROM DUMMY
                   UNION SELECT N'CardName',		N'BP Name',			'nvarchar',		'',		200,	-1, 1, 1, 1, 1, 1, 1, '',			'',			NULL FROM DUMMY
				   UNION SELECT N'TranType',		N'Trans Type',		'nvarchar',		'',		100,	-1, 1, 1, 1, 1, 1, 1, '',			'',			NULL FROM DUMMY
                   UNION SELECT N'ItemCode',		N'Item Code',		'nvarchar',		'',		100,	-1, 1, 1, 1, 1, 1, 1, '',			'',			NULL FROM DUMMY
				   UNION SELECT N'ItemName',		N'Item Name',		'nvarchar',		'',		70,		-1, 1, 1, 1, 1, 1, 1, '',			'',			NULL FROM DUMMY
				   UNION SELECT N'WhsCode',		N'Whse',			'nvarchar',		'',		50,		-1, 1, 1, 1, 1, 1, 1, '',			'',			NULL FROM DUMMY
                   UNION SELECT N'OpenQty',		N'Open',			'nvarchar',		'',		75,		-1, 1, 1, 1, 1, 1, 1, '',			'',			NULL FROM DUMMY
                   UNION SELECT N'PickQtty',		N'To Picking',		'NUMERIC',		'',		50,		-1, 1, 0, 1, 1, 1, 1, 'NUMERIC',	'n3',		NULL FROM DUMMY
                   UNION SELECT N'Available',		N'Available',		'NUMERIC',		'',		50,		-1, 1, 1, 1, 1, 1, 1, 'NUMERIC',	'n3',		NULL FROM DUMMY
				   UNION SELECT N'Fullfill',		N'Fullfill',		'NUMERIC',		'',		50,		-1, 1, 1, 1, 1, 1, 1, 'NUMERIC',	'n3',		NULL FROM DUMMY
				   UNION SELECT N'DocDate',		N'DocDate',			'DATETIME',		'',		50,		-1, 1, 1, 1, 1, 1, 1, 'datetime',	'dd/MM/yyyy',NULL FROM DUMMY
				   UNION SELECT N'DelDate',		N'DelDate',			'DATETIME',		'',		50,		-1, 1, 1, 1, 1, 1, 1, 'datetime',	'dd/MM/yyyy',NULL FROM DUMMY
				   UNION SELECT N'RemainDays',		N'RemainDays',		'NUMERIC',		'',		50,		-1, 1, 1, 1, 1, 1, 1, 'NUMERIC',	'',			NULL FROM DUMMY
				   UNION SELECT N'LineStatus',		N'LineStatus',		'nvarchar',		'',		100,	-1, 0, 1, 1, 1, 1, 1, '',			'',			NULL FROM DUMMY
				   UNION SELECT N'Checked',		N'Checked',			'nvarchar',			'',		40,		-1, 1, 0, 1, 1, 1, 1, '',			'',			'R' FROM DUMMY;

        SELECT *
            FROM #tblStructure ORDER BY ID;

		-- Khai báo bảng dữ liệu
       CREATE LOCAL TEMPORARY TABLE #tblData (
       "OrderEntry" integer, 
       "OrderLine" integer, 
       "CardCode" nvarchar(15), 
       "CardName" nvarchar(100), 
       "TranType" nvarchar(1), 
       "ItemCode" nvarchar(50), 
       "ItemName" nvarchar(254), 
       "WhsCode" nvarchar(8), 
       "OpenQty" decimal(19, 6), 
       "PickQtty" decimal(19, 6), 
       "Available" decimal(19, 6), 
       "Fullfill" decimal(19, 6), 
       "DocDate" timestamp, 
       "DelDate" timestamp, 
       "RemainDays" integer, 
       "LineStatus" char(1), 
       "ManBatchSerial" char(1), 
       "AbsEntry" integer DEFAULT 0, 
       "PickEntry" integer DEFAULT 0, 
       "Checked" char(1) DEFAULT 'N');
	
	
	SELECT IFNULL((SELECT MIN("DocDate") FROM ordr), CURRENT_DATE) into mindate FROM DUMMY;

	SELECT IFNULL((SELECT MAX("DocDate") FROM ordr), CURRENT_DATE) into maxdate FROM DUMMY;
	
	INSERT INTO #tblData ("OrderEntry", "OrderLine", "CardCode", "CardName", "TranType", "ItemCode", "ItemName", "WhsCode", "OpenQty", "PickQtty", "Available", "Fullfill", "DocDate", "DelDate", "RemainDays", "LineStatus", "ManBatchSerial")
	 (SELECT b."DocNum", a."LineNum", b."CardCode", b."CardName", a."TranType", a."ItemCode", a."Dscription", a."WhsCode", a."OpenQty" - IFNULL(d."PickQtty", 0), 0, c."OnHand", 0, b."DocDate", a."ShipDate", DAYS_BETWEEN(CURRENT_DATE, a."ShipDate"), a."LineStatus", CASE WHEN c."ManBtchNum" = 'Y' THEN 'B' WHEN c."ManSerNum" = 'Y' THEN 'S' ELSE 'N' END AS "ManBatchSerial" 
	 FROM RDR1 a INNER JOIN ORDR b ON b."DocEntry" = a."DocEntry" INNER JOIN OITM c ON c."ItemCode" = a."ItemCode" LEFT OUTER JOIN APZ_PKL1 d ON b."DocNum" = d."OrderEntry" AND a."LineNum" = d."OrderLine" 
	 WHERE (ifnull(:docStatus, '') = '' or :docStatus = 'O') AND a."OpenQty" > 0 AND (:bpCode = '' OR (:bpCode <> '' AND b."CardCode" = :bpCode)) AND (:warehouse = '' OR (:warehouse <> '' AND a."WhsCode" = :warehouse)) AND (:branch = -1 OR (:branch <> -1 AND b."BPLId" = :branch)) AND b."DocDate" BETWEEN IFNULL(NULLIF(:fromDate, ''), :mindate) AND IFNULL(NULLIF(:toDate, ''), :maxdate) AND a."OpenQty" - IFNULL(d."PickQtty", 0) > 0);
	
	SELECT * FROM #tblData ORDER BY "DelDate", "OrderEntry", "OrderLine";
	SELECT * FROM #tblValidValue;
	DROP TABLE #tblData;
END;



