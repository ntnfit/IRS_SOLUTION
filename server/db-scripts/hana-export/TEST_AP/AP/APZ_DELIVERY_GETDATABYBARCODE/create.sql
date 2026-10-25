CREATE PROCEDURE Apz_Delivery_GetDataByBarcode
(
	IN barcode NVARCHAR(254),	-- '00000000600004'
	draftKey NVARCHAR(50) DEFAULT ''	-- draft Num DO
)
LANGUAGE SQLSCRIPT AS
BEGIN
	--Khai báo biến
	DECLARE docEntry INT = 0;
	DECLARE toBinId INT= 0;
		
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
	  , "ObjType" NVARCHAR(50) 		--DEFAULT ''	-- batch: 10000044 / serial: 10000045 / line: 9999
	  , "SysNumber" INT 			--DEFAULT 0	-- sysNumber cua serial
	 );	
	 
	 -- khai báo biên exception handle
	DECLARE existDataBarcode CONDITION FOR SQL_ERROR_CODE 10002;
    DECLARE EXIT HANDLER FOR existDataBarcode RESIGNAL;
    
	IF (NOT EXISTS (SELECT 1 FROM OBTN WHERE "DistNumber" = :barcode)) THEN			
    	SIGNAL existDataBarcode SET MESSAGE_TEXT = 'Barcode không đúng hoặc sai định dạng!';
	END IF;	
	 
	-- lấy docentry của phiếu
	SELECT IFNULL((SELECT TOP 1 "DocEntry" FROM ODRF WHERE "ObjType" = '15' AND "DocNum" = :draftKey ORDER BY "DocEntry" DESC),0) INTO docEntry FROM DUMMY;
	
	IF :docEntry > 0 THEN		
		INSERT INTO :tbl ("DocEntry", "LineNum", "ItemCode", "ItemDescription", "DistNumber", "Detail", "LineQty", "Quantity", "Instock", "UomName", "Whs", "ToWhs", "ToBinId", "ObjType", "BinId", "BinCode")
		SELECT b."DocEntry", b."LineNum", a."ItemCode", b."Dscription", :barcode, c."MnfSerial", b."OpenQty", a."Instock", "Instock", b."unitMsr", a."WhsCode", '', 0, '10000044'
			 , a."BinAbs", a."BinCode"
		 FROM APZ_OITW_LOCATION a
		 INNER JOIN DRF1 b ON b."ItemCode"=a."ItemCode"
						   AND a."WhsCode"=b."WhsCode"
						   AND b."DocEntry"= :docEntry
		 INNER JOIN OBTN c ON c."AbsEntry"=a."AbsEntryBatch"
						   AND a."ManBtchNum"='Y'			 
		 WHERE a."DistNumber"= :barcode
			AND b."DocEntry"= :docEntry
			AND a."WhsCode" IN ('C','C-L','C-I');	-- 03/01/2024: Cần phải Scan Item đúng kho C, C-L, C-I (kho thành phẩm). Nếu không đúng kho C, C-L, C-I sẽ không cho lưu phiếu

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

