CREATE PROCEDURE Apz_TransferRequest_GetDataByBarcode
(
	IN barcode 	NVARCHAR(254)
	,  type 	NVARCHAR(50) DEFAULT 'request'	-- request / period	/	listPeriod	/	listSalesOrder /	listTransferRequest | listDeliveryDraft
)
AS
BEGIN
	--Khai báo biến
	DECLARE rowCount INT;
	DECLARE docEntry INT;
		 
	 -- khai báo biên exception handle
	DECLARE existDataBarcode CONDITION FOR SQL_ERROR_CODE 10002;
    DECLARE EXIT HANDLER FOR existDataBarcode RESIGNAL;
    
	IF :type = '' OR :type = 'request' THEN
		-- khai báo bảng dữ liệu đầu ra
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
		  , "BinId" INT DEFAULT 0
		  , "BinCode" NVARCHAR(150) DEFAULT ''
		  , "ToWhs" NVARCHAR(50)
		  , "ToBinId" INT DEFAULT 0
		  , "ObjType" NVARCHAR(50) DEFAULT '9999'
		  , "SysNumber" INT DEFAULT 0
		 );
		 
		-- lấy docentry của phiếu
		SELECT IFNULL((SELECT "DocEntry" FROM OWTQ WHERE "DocNum" = TO_NUMBER(:barcode)),0) INTO docEntry FROM DUMMY;
		
	   	INSERT INTO #tbl ("DocEntry", "LineNum", "ItemCode", "ItemDescription", "DistNumber", "LineQty", "Quantity", "Whs", "BinId", "ToWhs", "ToBinId", "UomName")
		SELECT a."DocEntry", a."LineNum", a."ItemCode", a."Dscription", '', a."OpenQty", CASE WHEN :type = 'IN' AND c."ManBtchNum"='N' AND c."ManSerNum"='N' THEN a."OpenQty" ELSE 0 END
			 , a."FromWhsCod", 0, a."WhsCode", 0, a."unitMsr"
			 FROM WTQ1 a
			 INNER JOIN OWTQ b ON b."DocEntry" = a."DocEntry"
			 INNER JOIN OITM c ON c."ItemCode"= a."ItemCode"
			 WHERE b."DocEntry"= :docEntry
			 AND b."DocStatus" = 'O';
			 		
		SELECT IFNULL((SELECT COUNT(*) FROM #tbl),0) INTO rowCount FROM DUMMY;
		IF (:rowCount > 0) THEN
			BEGIN
				IF NOT EXISTS (SELECT 1 FROM APZ_LOG_ALLOCATE_BIN WHERE "DocEntry" = :docEntry) THEN
					BEGIN
						-- lấy các batch/serial đã gán vào phiếu        	  	
						INSERT INTO #tbl ("DocEntry", "LineNum", "ItemCode", "ItemDescription", "DistNumber", "Detail", "Quantity", "Whs", "ObjType", "SysNumber", "BinId", "BinCode")
						SELECT a."SnBAllocateViewDocEntry", a."SnBAllocateViewDocLine", a."SnBAllocateViewItemCode", b."ItemName"
							, CASE
					            WHEN a."SnBAllocateViewMngBy"='10000044' THEN c."DistNumber"
					            WHEN a."SnBAllocateViewMngBy"='10000045' THEN d."DistNumber"
					            ELSE b."ItemCode" END 
					      	, CASE
					            WHEN a."SnBAllocateViewMngBy"='10000044' THEN c."Notes"
					            WHEN a."SnBAllocateViewMngBy"='10000045' THEN d."Notes"
					            ELSE b."ItemName" END, a."SnBAllocateViewAllocQty", a."SnBAllocateViewLocCode", a."SnBAllocateViewMngBy", IFNULL (d."SysNumber", 0)
					        , e."BinAbs", e."BinCode"
						FROM "B1_SnBAllocateDocView" a
						 INNER JOIN OITM          b ON b."ItemCode"= a."SnBAllocateViewItemCode"
						 LEFT OUTER JOIN OBTN     c ON c."AbsEntry"= a."SnBAllocateViewSnbMdAbs"
						                               AND a."SnBAllocateViewMngBy"='10000044'
						 LEFT OUTER JOIN OSRN     d ON d."AbsEntry"= a."SnBAllocateViewSnbMdAbs"
						                               AND A."SnBAllocateViewMngBy"='10000045'	
						LEFT JOIN APZ_OITW_LOCATION e ON e."WhsCode" = a."SnBAllocateViewLocCode" 
						 							   AND (a."SnBAllocateViewMngBy"='10000044' AND e."AbsEntryBatch" = a."SnBAllocateViewSnbMdAbs")
						 							   OR (a."SnBAllocateViewMngBy"='10000045' AND e."AbsEntryBatch" = a."SnBAllocateViewSnbMdAbs")			 
						 WHERE a."SnBAllocateViewDocType"='1250000001'
						       AND a."SnBAllocateViewDocEntry"= :docEntry;	
					END;
				ELSE
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
						 WHERE a."SnBAllocateViewDocType"='1250000001'
						       AND a."SnBAllocateViewDocEntry"= :docEntry;	
						
						UPDATE #tbl SET "BinId" = IFNULL((SELECT TOP 1 a."BinId" FROM #tbl b, APZ_LOG_ALLOCATE_BIN a
															 	WHERE a."DocEntry" = b."DocEntry" 
																 	AND a."LineNum" = b."LineNum"
																 	AND a."ItemCode" = b."ItemCode"
																 	AND a."DistNumber" = b."DistNumber"), 0)		 			   
					 	WHERE "ObjType" != '9999'
					 		AND "BinId" = 0;
						
						UPDATE #tbl SET "BinCode" = IFNULL((SELECT TOP 1 a."BinCode" FROM #tbl b, APZ_LOG_ALLOCATE_BIN a
															 	WHERE a."DocEntry" = b."DocEntry" 
																 	AND a."LineNum" = b."LineNum"
																 	AND a."ItemCode" = b."ItemCode"
																 	AND a."DistNumber" = b."DistNumber"), '')		 			   
					 	WHERE "ObjType" != '9999'
					 		AND "BinCode" = '';
					 		 						
					END;
				END IF;
				-- update số lượng đã allocate vào line
				UPDATE #tbl SET "Quantity" = IFNULL((SELECT SUM(a."Quantity") FROM #tbl a WHERE a."ObjType" <> '9999' AND a."LineNum" = #tbl."LineNum" AND a."ItemCode" = #tbl."ItemCode" GROUP BY a."LineNum"), 0) 
				WHERE "ObjType" = '9999'
					AND "Quantity" = 0;
			END;		
		END IF;
		
		SELECT * FROM #tbl ORDER BY "DocEntry", "LineNum", "DistNumber";
		-- xoá các bảng tạm
		DROP TABLE #tbl;
	ELSEIF :type = 'period' THEN
		BEGIN			
			DECLARE soLan INT = 0;
			
			SELECT COUNT(1) + 1 INTO soLan DEFAULT 0 FROM "@INVENTORYCOUNTING_H" WHERE "U_PeriodCode" = :barcode;
			/*
			-- 29_12_2023: bỏ giới hạn số lần kiểm kê
			IF :soLan > 3 THEN
				SIGNAL existDataBarcode SET MESSAGE_TEXT = 'Kỳ kiểm kê này đã kiểm kê đủ 3 lần!';
				RETURN;
			END IF;
			*/
			SELECT "Code", "Name", "Code" AS "U_PeriodCode", :soLan "Slan" FROM OFPR WHERE "Code" = :barcode;
		END;
	ELSEIF :type = 'listTransferRequest' THEN
		SELECT '' AS "Code", 'Chọn số phiếu yêu cầu' "Name", NULL AS "DocEntry" FROM DUMMY
		UNION ALL
		SELECT TO_CHAR("DocNum") AS "Code", TO_CHAR("DocNum") AS "Name", "DocEntry" FROM OWTQ
		WHERE "DocStatus" = 'O';
	ELSEIF :type = 'listPeriod' THEN
		SELECT '' "Code", 'Chọn kỳ kiểm kê' "Name", '' "U_PeriodCode" FROM DUMMY
		UNION ALL
		SELECT "Code", "Name", "Code" AS "U_PeriodCode" FROM OFPR
		WHERE "PeriodStat" <> 'C';
	ELSEIF :type = 'listSalesOrder' THEN
		SELECT '' AS "Code", 'Chọn số đơn hàng' "Name", NULL AS "DocEntry" FROM DUMMY
		UNION ALL
		SELECT TO_CHAR("DocNum") AS "Code", TO_CHAR("DocNum") AS "Name", "DocEntry" FROM ORDR
		WHERE "DocStatus" = 'O';
	ELSEIF :type = 'listDeliveryDraft' THEN
		SELECT '' AS "Code", 'Chọn số phiếu draft' "Name", NULL AS "DocEntry", NOW() AS "CreateDate" FROM DUMMY
		UNION ALL
		SELECT TO_CHAR("DocNum") AS "Code", TO_CHAR("DocNum") AS "Name", "DocEntry", "CreateDate" FROM ODRF
		WHERE "ObjType" = '15' 
			AND "DocStatus" = 'O'
		ORDER BY "CreateDate" DESC, "DocEntry" DESC
		;
	END IF;
END

