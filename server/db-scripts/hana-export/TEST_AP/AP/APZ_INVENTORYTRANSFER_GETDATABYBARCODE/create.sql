CREATE PROCEDURE Apz_InventoryTransfer_GetDataByBarcode
(
	IN barcode NVARCHAR(254),	-- '00000000600004'
	whs NVARCHAR(50) DEFAULT '',
	fromWhs NVARCHAR(50) DEFAULT '',
	requestId INT
)
LANGUAGE SQLSCRIPT AS
BEGIN
	--Khai báo biến
	DECLARE docEntry INT = 0;
	DECLARE toBinId INT= 0;
	DECLARE toBinCode NVARCHAR(150)= :whs;
		
	DECLARE tblStructure TABLE
    (
	  	ID INT 
	    ,"ColumnName" NVARCHAR(50) PRIMARY KEY
	    ,"Caption" NVARCHAR(100)
	    ,"DataType" VARCHAR(20) --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox) LinkedUDO*
	    ,"SubType" VARCHAR(500)			--None Phone Percentage Address Image Measurement Price Quantity Rate Sum Time (udo code : dùng cho LinkedUDO)*
	    ,"Width" INT
	    ,"GroupIndex" INT 	-- dùng cho grid  default -1
	    ,"Visible" TINYINT
	    ,"FormatType" NVARCHAR(20)    -- numeric, datetime/ 
	    ,"DisplayFormat" NVARCHAR(20)   -- '{0:n2}'
	    ,"ReadOnly" TINYINT
	    ,"AllowEdit" TINYINT
	    ,"AllowMove" TINYINT
	    ,"AllowSize" TINYINT
	    ,"AllowSort" TINYINT
	    ,"Fixed" VARCHAR(1) 	-- L:Left / R:Right
		-- Summary Item
	    ,"SummaryType" VARCHAR(20) 	-- sum,average...
		-- Group Summary
	    ,"GroupSummaryType" VARCHAR(20) 	-- sum,average...
		,"SummaryFormat" VARCHAR(20) -- (AVR={0::.::})/ Count ({0})/ Custom format/ Max (MAX={0})/ Min (MIN={0})/ Sum (SUM={0::.::})
		,"Expression" NVARCHAR(5000)
	    ,"Parent" TINYINT NULL	-- dùng cho treelist
	    ,"Key" TINYINT NULL	-- dùng cho treelist (identity)
	);
   	
	-- khai báo bảng dữ liệu đầu ra
	DECLARE tbl TABLE
	(
	   "DocEntry" INT
	  , "LineNum" INT
	  , "ItemCode" NVARCHAR(50)
	  , "ItemDescription" NVARCHAR(200)
	  , "DistNumber" NVARCHAR(100)
	  , "Detail" NVARCHAR(5000)
	  , "LineQty" NUMERIC(19,6) 	--DEFAULT 0
	  , "Quantity" NUMERIC(19,6) 	--DEFAULT 0
	  , "Instock" NUMERIC(19,6) 	--DEFAULT 0
	  , "UomName" NVARCHAR(50)
	  , "Whs" NVARCHAR(50)
	  , "BinId" INT 				--DEFAULT 0
	  , "BinCode" NVARCHAR(150)
	  , "ToWhs" NVARCHAR(50)
	  , "ToBinId" INT 				--DEFAULT 0
	  , "ToBinCode" NVARCHAR(150)
	  , "ObjType" NVARCHAR(50) 		--DEFAULT ''	-- batch: 10000044 / serial: 10000045 / line: 9999
	  , "SysNumber" INT 			--DEFAULT 0	-- sysNumber cua serial
	 );	
	 
	 -- khai báo biên exception handle
	DECLARE existDataBarcode CONDITION FOR SQL_ERROR_CODE 10002;
    DECLARE EXIT HANDLER FOR existDataBarcode RESIGNAL;
    
	IF (NOT EXISTS (SELECT 1 FROM OBTN WHERE "DistNumber" = :barcode)) THEN			
    	SIGNAL existDataBarcode SET MESSAGE_TEXT = 'Barcode không đúng hoặc sai định dạng!';
	END IF;	
	
	-- lấy bin id theo bin code
	IF :whs <> '' AND (EXISTS (SELECT 1 FROM OBIN WHERE "BinCode" = :whs))
	THEN
		SELECT	"WhsCode", "AbsEntry", "BinCode"
			INTO whs, toBinId, toBinCode
			FROM OBIN 
			WHERE "BinCode" = :whs;
	END IF;
	
	IF (:whs <> '' AND NOT EXISTS (SELECT 1 FROM OWHS WHERE "WhsCode" = :whs)) THEN			
    	SIGNAL existDataBarcode SET MESSAGE_TEXT = 'Kho/Bin không đúng!';
	END IF;	
	
	IF :requestId > 0 THEN
		-- lấy docentry của phiếu
		SELECT IFNULL((SELECT "DocEntry" FROM OWTQ WHERE "DocNum" = :requestId),0)  INTO docEntry FROM DUMMY;
		
		INSERT INTO :tbl ("DocEntry", "LineNum", "ItemCode", "ItemDescription", "DistNumber", "Detail", "LineQty", "Quantity", "Instock", "UomName", "Whs", "ToWhs", "ToBinId", "ObjType", "BinId", "BinCode", "ToBinCode")
		SELECT b."DocEntry", b."LineNum", a."ItemCode", b."Dscription", :barcode, c."MnfSerial", b."Quantity", a."Instock", "Instock", b."unitMsr", a."WhsCode", :whs, :toBinId, '10000044'
			 , a."BinAbs", a."BinCode", :toBinCode
		 FROM APZ_OITW_LOCATION a
		 INNER JOIN WTQ1 b ON b."ItemCode"=a."ItemCode"
						   --AND a."WhsCode"=b."FromWhsCod"
						   AND b."DocEntry"= :docEntry
		 INNER JOIN OBTN c ON c."AbsEntry"=a."AbsEntryBatch"
						   AND a."ManBtchNum"='Y'			 
		 WHERE a."DistNumber"= :barcode
			AND b."DocEntry"= :docEntry;
	
		--IF EXISTS (SELECT 1 FROM :tbl) THEN
		--	UPDATE :tbl SET "Quantity" = 0,
		--					"LineQty" = 0;
		--END IF;
	ELSE
		INSERT INTO :tbl ("DocEntry", "LineNum", "ItemCode", "ItemDescription", "DistNumber", "Detail", "LineQty", "Quantity", "Instock", "UomName", "Whs", "ToWhs", "ToBinId", "ObjType", "BinId", "BinCode", "ToBinCode")
		SELECT 0, 0, a."ItemCode", b."ItemName", :barcode, c."MnfSerial", a."Instock", a."Instock", "Instock", b."BuyUnitMsr", a."WhsCode", :whs, :toBinId, '10000044' , a."BinAbs", a."BinCode", :toBinCode
		 FROM APZ_OITW_LOCATION a
		 INNER JOIN OITM b ON b."ItemCode" = a."ItemCode"
		 INNER JOIN OBTN c ON c."AbsEntry"=a."AbsEntryBatch"
						   AND a."ManBtchNum"='Y'			 
		 WHERE a."DistNumber"= :barcode;
		
	END IF;
	
	--mặc định giá trị
	UPDATE :tbl SET "LineQty" = 0 WHERE "LineQty" IS NULL;
	UPDATE :tbl SET "Quantity" = 0 WHERE "Quantity" IS NULL;
	UPDATE :tbl SET "Instock" = 0 WHERE "Instock" IS NULL;
	UPDATE :tbl SET "BinId" = 0 WHERE "BinId" IS NULL;
	UPDATE :tbl SET "ToBinId" = 0 WHERE "ToBinId" IS NULL;
	UPDATE :tbl SET "ObjType" = '' WHERE "ObjType" IS NULL;
	UPDATE :tbl SET "SysNumber" = 0 WHERE "SysNumber" IS NULL;
	
	
	SELECT * FROM :tblStructure;
	SELECT * FROM :tbl;
	
END

