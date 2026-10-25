CREATE PROCEDURE Apz_SalesOrder_GetOrderByBarcode
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
	  , "ToWhs" NVARCHAR(50)
	  , "ToBinId" NVARCHAR(50)
	  , "ObjType" NVARCHAR(50) DEFAULT ''	-- batch: 10000044 / serial: 10000045 / line: 9999
	  , "SysNumber" INT DEFAULT 0			-- sysNumber cua serial
	 );
	 
	-- lấy docentry của phiếu
	SELECT IFNULL((SELECT "DocEntry" FROM ORDR WHERE "DocNum" = TO_NUMBER(:barcode)),0)  INTO docEntry FROM DUMMY;
	
   	INSERT INTO #tbl ("DocEntry", "LineNum", "CardCode", "CardName", "ItemCode", "ItemDescription", "DistNumber", "LineQty", "Quantity", "Whs", "UomName", "ObjType")
	SELECT a."DocEntry", a."LineNum", b."CardCode", b."CardName", a."ItemCode", a."Dscription", '', a."OpenQty", CASE WHEN c."ManBtchNum"='N' AND c."ManSerNum"='N' THEN a."Quantity" ELSE 0 END
		 , a."WhsCode", a."unitMsr", '9999'
	 	FROM RDR1 a
	 	INNER JOIN ORDR b ON b."DocEntry" = a."DocEntry"
	 	INNER JOIN OITM c ON c."ItemCode"= a."ItemCode"
	 	WHERE a."DocEntry"= :docEntry
	 	AND b."DocStatus" = 'O';
	 	
	SELECT IFNULL((SELECT COUNT(*) FROM #tbl),0) INTO rowCount FROM DUMMY;
	IF (:rowCount > 0) THEN
		BEGIN
			-- lấy các batch/serial đã gán vào phiếu        	  	
			INSERT INTO #tbl ("DocEntry", "LineNum", "ItemCode", "ItemDescription", "DistNumber", "Detail", "Quantity", "Whs", "ObjType", "SysNumber")
			SELECT a."SnBAllocateViewDocEntry", a."SnBAllocateViewDocLine", a."SnBAllocateViewItemCode", b."ItemName"
				, CASE
		            WHEN a."SnBAllocateViewMngBy"='10000044' THEN c."DistNumber"
		            WHEN a."SnBAllocateViewMngBy"='10000045' THEN d."DistNumber"
		            ELSE b."ItemCode" END 
		      	, CASE
		            WHEN a."SnBAllocateViewMngBy"='10000044' THEN c."Notes"
		            WHEN a."SnBAllocateViewMngBy"='10000045' THEN d."Notes"
		            ELSE b."ItemName" END, a."SnBAllocateViewAllocQty", a."SnBAllocateViewLocCode", a."SnBAllocateViewMngBy", IFNULL (d."SysNumber", 0)
				FROM "B1_SnBAllocateDocView" a
			 	INNER JOIN OITM          b ON b."ItemCode"= a."SnBAllocateViewItemCode"
			 	LEFT OUTER JOIN OBTN     c ON c."AbsEntry"= a."SnBAllocateViewSnbMdAbs"
			                               AND a."SnBAllocateViewMngBy"='10000044'
			 	LEFT OUTER JOIN OSRN     d ON d."AbsEntry"= a."SnBAllocateViewSnbMdAbs"
			                               AND A."SnBAllocateViewMngBy"='10000045'
			 	WHERE a."SnBAllocateViewDocType"='17'
			       AND a."SnBAllocateViewDocEntry"= :docEntry;			
			
			-- update số lượng đã allocate vào line
			UPDATE #tbl SET "Quantity" = IFNULL((SELECT SUM(a."Quantity") FROM #tbl a WHERE a."ObjType" <> '9999' AND a."LineNum" = #tbl."LineNum" AND a."ItemCode" = #tbl."ItemCode" GROUP BY a."LineNum"), 0) 
				WHERE "ObjType" = '9999'
				AND "Quantity" = 0;
		END;		
	END IF;
	
	SELECT * FROM #tbl ORDER BY "DocEntry", "LineNum", "DistNumber";
	
	-- xoá các bảng tạm
	DROP TABLE #tbl;
	DROP TABLE #tblStructure; 
END

