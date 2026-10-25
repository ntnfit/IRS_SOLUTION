CREATE PROCEDURE Apz_Delivery_GetDataByDraftKey
(
	IN barcode nvarchar(254)
)
AS
BEGIN
	--Khai báo biến
	DECLARE rowCount INT;
	DECLARE docEntry INT;
	
	CREATE LOCAL TEMPORARY TABLE #tblStructure
    (
	     "ColumnName" NVARCHAR(50) NOT NULL
	    ,"Caption" NVARCHAR(100) NULL
	    ,"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar' --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
		,"SubType" VARCHAR(20) NULL			--none phone percentage address image measurement price quantity rate sum time
	    ,"Width" INT NULL DEFAULT 75
	    ,"GroupIndex" INT NULL DEFAULT -1	-- dng cho grid  default -1
	    ,"Visible" TINYINT NULL DEFAULT '1'
	    ,"FormatType" NVARCHAR(20) NULL   -- numeric, datetime/ 
	    ,"DisplayFormat" NVARCHAR(20) NULL  -- '{0:n2}'
	    ,"ReadOnly" TINYINT NULL DEFAULT '0'
	    ,"AllowEdit" TINYINT NULL DEFAULT '1'
	    ,"AllowMove" TINYINT NULL DEFAULT '1'
	    ,"AllowSize" TINYINT NULL DEFAULT '1'
	    ,"AllowSort" TINYINT NULL DEFAULT '1'
	    ,"Fixed" VARCHAR(1) NULL	-- L:Left / R:Right
		-- Summary Item
	    ,"SummaryType" VARCHAR(10) NULL	-- sum,average...
		-- Group Summary
	    ,"GroupSummaryType" VARCHAR(10) NULL	-- sum,average...
		--,Parent BIT NULL	-- dng cho treelist
		--,[Key] BIT NULL		-- dng cho treelist (identity)
    );
    SELECT * FROM #tblStructure;
    
    -- khai báo bảng dữ liệu đầu ra
	CREATE LOCAL TEMPORARY TABLE #tbl
	(
	   "DocEntry" INT
	  , "LineNum" INT
	  , "CardCode" NVARCHAR(50)
	  , "CardName" NVARCHAR(200)
	  , "ItemCode" NVARCHAR(50)
	  , "ItemDescription" NVARCHAR(200)
	  , "DistNumber" NVARCHAR(100)
	  , "Detail" NVARCHAR(5000)
	  , "LineQty" NUMERIC(19,6) DEFAULT 0
	  , "Quantity" NUMERIC(19,6) DEFAULT 0
	  , "UomName" NVARCHAR(50)
	  , "Whs" NVARCHAR(50)
	  , "BinId" NVARCHAR(50)
	  , "BinCode" NVARCHAR(150) DEFAULT ''
	  , "ToWhs" NVARCHAR(50)
	  , "BinActivat" NVARCHAR(50)
	  , "ToBinId" NVARCHAR(50)
	  , "ObjType" NVARCHAR(50) DEFAULT ''	-- batch: 10000044 / serial: 10000045 / line: 9999
	  , "SysNumber" INT DEFAULT 0			-- sysNumber cua serial
	  , "ManBtchNum" NVARCHAR(50)
	  , "ManSerNum" NVARCHAR(50)
	 );
	 
	-- lấy docentry của phiếu
	SELECT IFNULL((SELECT TOP 1 "DocEntry" FROM ODRF WHERE "ObjType" = '15' AND "DocNum" = :barcode ORDER BY "DocEntry" DESC),0) INTO docEntry FROM DUMMY;
	
   	INSERT INTO #tbl ("DocEntry", "LineNum", "CardCode", "CardName", "ItemCode", "ItemDescription", "DistNumber", "LineQty", "Quantity", "Whs", "UomName", "ObjType", "BinActivat", "ManBtchNum", "ManSerNum")
	SELECT a."DocEntry", a."LineNum", b."CardCode", b."CardName", a."ItemCode", a."Dscription", '', a."Quantity", CASE WHEN c."ManBtchNum"='N' AND c."ManSerNum"='N' THEN a."Quantity" ELSE 0 END
		 , a."WhsCode", a."unitMsr", '9999', d."BinActivat", c."ManBtchNum", c."ManSerNum"
	 	FROM DRF1 a
	 	INNER JOIN ODRF b ON b."DocEntry" = a."DocEntry"
	 	INNER JOIN OITM c ON c."ItemCode"= a."ItemCode"
	 	INNER JOIN OWHS d ON d."WhsCode"= a."WhsCode"
	 	WHERE a."DocEntry"= :docEntry
		 	AND b."ObjType" = '15'
		 	AND b."DocStatus" = 'O'
	 	ORDER BY a."LineNum";
	
	-- lấy thông tin batch đã allocate
	INSERT INTO #tbl ("DocEntry", "LineNum", "ItemCode", "ItemDescription", "DistNumber", "Detail", "LineQty", "Quantity", "UomName", "Whs", "ToWhs", "ToBinId", "ObjType", "BinId", "BinCode")
	SELECT b."AbsEntry", b."LineNum", a."ItemCode", c."itemName", c."DistNumber", c."MnfSerial", d."Quantity" , b."Quantity", '', a."WhsCode", '', 0, '10000044'
		 , a."BinAbs", a."BinCode"
		 FROM APZ_OITW_LOCATION a
		 INNER JOIN DRF16 b ON b."ItemCode"= a."ItemCode"
							   AND a."WhsCode"= b."WhsCode"
							   AND b."AbsEntry"= :docEntry
							   AND a."AbsEntryBatch" = b."ObjAbs"
		 INNER JOIN OBTN c ON c."AbsEntry"= b."ObjAbs"
		 INNER JOIN DRF1 d ON d."DocEntry" = b."AbsEntry" 
		 					AND	d."LineNum" = b."LineNum" 
		 WHERE b."AbsEntry"= :docEntry;
	
	-- tính tổng sl đã allocate
	UPDATE #tbl SET "Quantity" = IFNULL((SELECT SUM(a."Quantity") FROM #tbl a WHERE a."ObjType" <> '9999' AND a."LineNum" = #tbl."LineNum" AND a."ItemCode" = #tbl."ItemCode" GROUP BY a."LineNum"), 0) 
				WHERE "ObjType" = '9999'
				AND "Quantity" = 0;
				
	SELECT * FROM #tbl ORDER BY "DocEntry", "LineNum", "DistNumber";
	
	-- xoá các bảng tạm
	DROP TABLE #tbl;
	DROP TABLE #tblStructure; 
END

