CREATE PROCEDURE Apz_InventoryTransfer_GetTransferByBarcode
(
	IN barcode nvarchar(254)
)
AS
BEGIN
	CREATE LOCAL TEMPORARY TABLE #tbl
	(
	   "DocEntry" INT
	  , "LineNum" INT
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
	  , "ObjType" NVARCHAR(50) DEFAULT '9999'
	  , "SysNumber" INT DEFAULT 0
	 );
	 
   	INSERT INTO #tbl ("DocEntry", "LineNum", "ItemCode", "ItemDescription", "DistNumber", "LineQty", "Whs", "BinId", "ToWhs", "ToBinId", "UomName")
	SELECT a."DocEntry", a."LineNum", a."ItemCode", a."Dscription", '', a."Quantity", a."FromWhsCod", '', a."WhsCode", '', a."unitMsr"
	 FROM WTR1 a
	 WHERE a."DocEntry"= :barcode;

	-- lấy các batch/serial đã gán vào phiếu        	  	
	INSERT INTO #tbl ("DocEntry", "LineNum", "ItemCode", "ItemDescription", "DistNumber", "Detail", "Quantity", "Whs", "ObjType", "SysNumber")
	SELECT a."SnBAllocateViewDocEntry", a."SnBAllocateViewDocLine", a."SnBAllocateViewItemCode", b."ItemName", c."DistNumber"
	      , CASE
	            WHEN a."SnBAllocateViewMngBy"='10000044' THEN c."Notes"
	            WHEN a."SnBAllocateViewMngBy"='10000045' THEN d."Notes"
	            ELSE '' END, a."SnBAllocateViewAllocQty", a."SnBAllocateViewLocCode", a."SnBAllocateViewMngBy", IFNULL (d."SysNumber", 0)
	FROM "B1_SnBAllocateDocView" a
	 INNER JOIN OITM          b ON b."ItemCode"= a."SnBAllocateViewItemCode"
	 LEFT OUTER JOIN OBTN     c ON c."AbsEntry"= a."SnBAllocateViewSnbMdAbs"
	                               AND a."SnBAllocateViewMngBy"='10000044'
	 LEFT OUTER JOIN OSRN     d ON d."AbsEntry"= a."SnBAllocateViewSnbMdAbs"
	                               AND A."SnBAllocateViewMngBy"='10000045'
	 WHERE a."SnBAllocateViewDocType"='67'
	       AND a."SnBAllocateViewDocEntry"= :barcode;			
	
	-- update số lượng đã allocate vào line
	UPDATE #tbl SET "Quantity" = IFNULL((SELECT SUM(a."Quantity") FROM #tbl a WHERE a."ObjType" <> '9999' AND a."LineNum" = #tbl."LineNum" AND a."ItemCode" = #tbl."ItemCode" GROUP BY a."LineNum"), 0) 
	WHERE "ObjType" = '9999';
	
	SELECT * FROM #tbl ORDER BY "DocEntry", "LineNum", "DistNumber";

	DROP TABLE #tbl;
END

