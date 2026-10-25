
CALL APZ_EXISTSPROC('APZ_InventoryTranfer_GetDataCopyFrom');

GO

CREATE PROCEDURE APZ_InventoryTranfer_GetDataCopyFrom(
	in docEntry nvarchar(1000),
	in fromObj int default -1)
AS
BEGIN
	
	declare fromDate timestamp;
	declare toDate timestamp;
	
	select ADD_DAYS(CURRENT_DATE,-1) into fromDate from DUMMY;
	select ADD_DAYS(CURRENT_DATE,1) into toDate FROM DUMMY;
	
	----------------------------------------------------- LẤY DỮ LIỆU CARCODE ----------------------------------------------------
	select distinct "CardCode" from OWTQ where :fromObj = 1250000001 and "DocEntry" in (select cast(Data as int) from Split(:docEntry,',')) and ifnull("CardCode",'') <> ''
	UNION ALL
	select distinct "CardCode" from ODRF where :fromObj = 112 and "ObjType" = '67' and "DocEntry" in (select cast(Data as int) from Split(:docEntry,',')) and ifnull("CardCode",'') <> '';
	----------------------------------------------------- END LẤY DỮ LIỆU CARDCODE ----------------------------------------------

	----------------------------------------------------- LẤY DỮ LIỆU HEADER ----------------------------------------------------
	create local temporary table #tblDoc as (select top 0 a.*, cast(null as nvarchar(254)) "CntctName", a."DocCur" "Currency",cast(0 as tinyint) "IsDraft" from ODRF a);
	insert into #tblDoc
		select a.*, b."Name" as "CntctName", a."DocCur" "Currency", 0 from ODRF a left join OCPR b on a."CntctCode" = b."CntctCode" where :fromObj = 112 and a."DocEntry" in (select cast(Data as int) from Split(:docEntry,','))
	UNION ALL 
		select  a.*, b."Name" as "CntctName", a."DocCur" "Currency", 0 from OWTQ a left join OCPR b on a."CntctCode" = b."CntctCode" where :fromObj <> 112 and a."DocEntry" in (select cast(Data as int) from Split(:docEntry,','));
	
	if :fromObj <> 112 then
		UPDATE #tblDoc SET "DocDate" = CURRENT_DATE, "DocDueDate" = CURRENT_DATE, "TaxDate" = CURRENT_DATE;
	end if;
	
	UPDATE #tblDoc SET "JrnlMemo" = n'Inventory Transfers -', "Comments" = IFNULL("Comments", '') || ' Based On Inventory Transfer Request ' || :docEntry || '.';

	if :fromObj = 112
	THEN
		update #tblDoc set "draftKey" = "DocEntry", "IsDraft"= 1;
	END IF;

	update #tblDoc set "DocEntry" = 0;
	select * from #tblDoc;
	drop table #tblDoc;
	--------------------------------------------------- END LẤY DỮ LIÊU HEADER ---------------------------------------------------

	--------------------------------------------------- LẤY DỮ LIỆU LINES -------------------------------------------------------
	create local temporary table #tempLineCopyto as (select top 0 a.*, "LineNum" as "LineNumBer" from WTQ1 a);
	
	insert into #tempLineCopyto
		select a.*, ROW_NUMBER() OVER(ORDER BY "LineNum" ASC) - 1 as "LineNumBer" from WTQ1 a where :fromObj = 1250000001 and a."ShipDate" <> '' and ifnull (a."OpenQty", 0) > 0 and a."LineStatus" = 'O' and 
						 a."DocEntry" in (select cast(Data as int) from Split(:docEntry,','))
		UNION ALL
	select a.*, ROW_NUMBER() OVER(ORDER BY "LineNum" ASC) - 1 as "LineNumBer" from DRF1 a where :fromObj = 112 and a."ObjType" = '67' and 
							a."DocEntry" in (select cast(Data as int) from Split(:docEntry,','));

	IF :fromObj = 1250000001
	THEN
		UPDATE a SET a."DocEntry" = 0, a."ObjType" = '67', a."BaseType" = '1250000001', a."BaseEntry" = a."DocEntry", a."BaseRef" = b."DocNum", a."BaseLine" = a."LineNum"
					, a."Quantity" = a."OpenQty", a."InvQty" = a."OpenInvQty", a."LineNum" = "LineNumBer", a."ShipDate" = CURRENT_DATE, a."SlpCode" = b."SlpCode" 
				FROM #tempLineCopyto a INNER JOIN OWTQ b ON a."DocEntry" = b."DocEntry";
	END IF;

	SELECT a.*, CASE WHEN ifnull(b."ManBtchNum", '') = 'Y' THEN 'B' WHEN ifnull(b."ManSerNum", '') = 'Y' THEN 'S' ELSE 'N' END AS "ManBtchSerial", ifnull(b."OnHand", 0) AS "OnHandItem"
		  , ifnull(b."IsCommited", 0) AS "IsCommittedItem", ifnull(b."OnOrder", 0) AS "OnOrderedItem", ifnull(c."OnHand", 0) AS "OnHandWhs", ifnull(c."IsCommited", 0) AS "IsCommittedWhs"
		  , ifnull(c."OnOrder", 0) AS "OnOrderedWhs", b."SalUnitMsr" AS "SalUnitMsr", ifnull(b."NumInSale", 0) AS "NumInSale", b."BuyUnitMsr", ifnull(b."NumInBuy", 0) AS "NumInBuy"
		  , ifnull(b."UgpEntry", -1) AS "UgpCode", b."InvntryUom" AS "InvUnitMsr", ifnull(d."AltQty", 1) / ifnull(d."BaseQty", 1) AS "NumInInven"
		  , CASE WHEN ifnull(a."OcrCode", '') <> '' AND ifnull(a."OcrCode2", '') <> '' AND ifnull(a."OcrCode3", '') <> '' AND ifnull(a."OcrCode4", '') <> '' AND ifnull(a."OcrCode5", '') <> '' THEN NULL 
		  		ELSE ifnull(a."OcrCode", '') || ';' || ifnull(a."OcrCode2", '') || ';' || ifnull(a."OcrCode3", '') || ';' || ifnull(a."OcrCode4", '') || ';' || ifnull(a."OcrCode5", '') END AS "Rules", c."AvgPrice" 
		 FROM #tempLineCopyto a INNER JOIN OITM b ON a."ItemCode" = b."ItemCode" LEFT OUTER JOIN OITW c ON a."ItemCode" = c."ItemCode" AND a."FromWhsCod" = c."WhsCode" 
		 						LEFT OUTER JOIN UGP1 d ON b."UgpEntry" = d."UgpEntry" AND d."UomEntry"=b."IUoMEntry" order by "VisOrder";
	--------------------------------------------------- END GET DATA LINES ----------------------------------------------------------

	-------------------------------------------------- GET DATA BATCH AND SERIAL ----------------------------------------------------------
	create local temporary table #TempBatchSerialBase as (SELECT top 0 T0.*,cast(null as int) "BinAbs", cast(null as nvarchar(228)) "BinCode", cast(null as NUMERIC(19,6)) "SelectedQty", cast(null as nvarchar(8)) "WhsCode", 
				cast(null as nvarchar(228)) "BinDescription", cast(null as int) "BaseLineNumber", cast(null as int) "BaseDocEntry",cast(null as char(1)) "ManBatchSerial",CAST(NULL AS NUMERIC(19,6)) "LogQty"
				, CAST(NULL AS CHAR(1)) "IsExits" , cast(null as smallint) "TranType", CAST(NULL AS CHAR(1)) "IsBase" FROM OBTN T0);

	-----gọi function lấy batch của phiếu  base
	CALL apz_fun_getbatchserial_byDocEntry(:docEntry, :fromObj, '#TempBatchSerialBase');

	---  xóa dữ liệu batch không có line tồn tại
	IF :fromObj <> '112'
	THEN
		DELETE FROM #TempBatchSerialBase a where not exists(select 1 from #tempLineCopyto b where a."BaseDocEntry" = b."BaseEntry" and a."BaseLineNumber" = b."BaseLine");
		Update a set "BaseLineNumber" = b."LineNum" from #TempBatchSerialBase a INNER JOIN #tempLineCopyto b ON a."BaseDocEntry" = b."BaseEntry" and a."BaseLineNumber" = b."BaseLine";
	END IF;

	delete from #TempBatchSerialBase a where not exists(select 1 from #tempLineCopyto where "LineNum" = a."BaseLineNumber");
	
	---- tạo temp table final để lưu kết quả sau khi tính toán deefault bin
	create local temporary table #TempBatchSerialBaseFinal as (select a.*, cast(null as int) ID, cast(null as int) "TempBinEntry", cast(null as nvarchar(50)) "TempBinCode", cast(null as nvarchar(254)) "TempBinDes"
																		, cast(null as numeric(19,6)) "TempQty" from #TempBatchSerialBase a inner join OWHS c on a."WhsCode" = c."WhsCode"
																where c."BinActivat" = 'N');
	
	if :fromObj = '1250000001'
	THEN
		--------------------------------update lại default bin cho các batch từ kho đi
		declare indexLoop int = 1;
		declare maxIndex int = 0;
		declare itemCode nvarchar(50);
		declare whsCode nvarchar(50);
		declare qty numeric(19,6);
		declare batchEntry int;
		declare isBatch nvarchar(1);
		declare lineNum int;
		
		------- lấy dữ liệu các dòng sẽ lấy default bin cho kho đi
		tempLineBinBatch =
			select a.*, row_number() over(order by a."BaseLineNumber") ID from #TempBatchSerialBase a inner join OWHS c on a."WhsCode" = c."WhsCode"
			where c."BinActivat" = 'Y';
			
		select ifnull((select max(id) from :tempLineBinBatch), 0) into maxIndex from dummy;
		
		while :indexLoop <= :maxIndex
		DO
		
			select "ItemCode", "LogQty", "WhsCode", "AbsEntry" into itemcode, qty, whsCode, batchEntry from :tempLineBinBatch where ID = :indexLoop;
			binData = select * from APZ_Function_WhsGetDefBinItem('O', :whsCode, :itemCode, :batchEntry, :qty);
			if exists(select 1 from :binData)
			THEN
			
				--delete from #TempBatchSerialBaseFinal where "BaseLineNumber" = :lineNum and "AbsEntry" = :batchEntry;
			
				insert into #TempBatchSerialBaseFinal
					select x.*, a."AbsEntry", a."BinCode", b."Descr", a."Qty" from (select * from :tempLineBinBatch where ID = :indexLoop) x, :binData a INNER JOIN OBIN b ON a."AbsEntry" = b."AbsEntry";
			ELSE
			
				insert into #TempBatchSerialBaseFinal
					select *, null, null, null, null from :tempLineBinBatch where ID = :indexLoop;
			
			END IF;
			indexLoop = :indexLoop + 1;
		
		END WHILE;
		
		update #TempBatchSerialBaseFinal set "BinAbs" = "TempBinEntry", "BinCode" = "TempBinCode", "BinDescription" = "TempBinDes", "LogQty" = "TempQty" where ifnull("TempBinEntry", 0) <> 0;

		-------------------------------- THÊM DỮ LIỆU DEFAULT TO BIN ALLOWCATION ------------------------------
		tempAllowToWhs = 
			SELECT a."AbsEntry", a."BaseLineNumber", a."ManBatchSerial", b."ItemCode", b."WhsCode", SUM(a."LogQty") AS "Quantity", row_number() over(order by a."AbsEntry") ID
												FROM #TempBatchSerialBaseFinal a 
													INNER JOIN #tempLineCopyto b ON a."BaseLineNumber" = b."LineNum" 
													inner join OWHS c on b."WhsCode" = c."WhsCode" where c."BinActivat" = 'Y'
									GROUP BY a."AbsEntry", a."BaseLineNumber", a."ManBatchSerial", b."WhsCode", b."ItemCode";
									
		select ifnull((select max(id) from :tempAllowToWhs), 0) into maxIndex from dummy;
		indexLoop = 1;
		
		while :indexLoop <= :maxIndex
		DO
		
			select "ItemCode", "Quantity", "WhsCode", "AbsEntry", "ManBatchSerial", "BaseLineNumber" into itemcode, qty, whsCode, batchEntry, isBatch, lineNum from :tempAllowToWhs where ID = :indexLoop;
			binData = select * from APZ_Function_WhsGetDefBinItem('I', :whsCode, :itemCode, :batchEntry, :qty);
			if exists(select 1 from :binData)
			THEN
			
				if :isBatch = 'B'
				THEN
				
					insert into #TempBatchSerialBaseFinal
						SELECT a.*, b."AbsEntry", b."BinCode", NULL, :whsCode, c."Descr", :lineNum, NULL, 'B', b."Qty", 'Y', 2, 'N', null, null, null, null, null
								FROM OBTN a, :binData b INNER JOIN OBIN c ON b."AbsEntry" = c."AbsEntry" where a."AbsEntry" = :batchEntry;
				
				ELSE
				
					insert into #TempBatchSerialBaseFinal
						SELECT a.*, b."AbsEntry", b."BinCode", NULL, :whsCode, c."Descr", :lineNum, NULL, 'B', b."Qty", 'Y', 2, 'N', null, null, null, null, null
								FROM OSRN a, :binData b INNER JOIN OBIN c ON b."AbsEntry" = c."AbsEntry" where a."AbsEntry" = :batchEntry;
				
				END IF;
			
			END IF;
			indexLoop = :indexLoop + 1;
		
		END WHILE;

	END IF;
	
	-------------------------------- END THÊM BIN ALLOWCATION --------------------------------------------

	select * from #TempBatchSerialBaseFinal;
	
	------------------------------------------------ END GET DATA BATCH AND SERIAL --------------------------------------------------------

	----------------------------------------------- GET DATA BIN ALLOWCATION --------------------------------------------------------------
	create local temporary table #tempBinAllowcation as (SELECT TOP 0 T0.*, CAST(NULL AS integer) AS "BinAbs", CAST(NULL AS nvarchar(50)) AS "BinCode", CAST(NULL AS decimal(19, 6)) AS "SelectedQty"
						, CAST(NULL AS nvarchar(50)) AS "WhsCode", CAST(NULL AS nvarchar(254)) AS "BinDescription", CAST(NULL AS integer) AS "BaseLineNumber", CAST(NULL AS integer) AS "BaseDocEntry"
						, CAST(NULL AS char(1)) AS "ManBatchSerial", CAST(NULL AS decimal(19, 6)) AS "LogQty", CAST(NULL AS char(1)) AS "IsExits", CAST(NULL AS integer) AS "TranType"
						, CAST(NULL AS char(1)) AS "IsBase" FROM OBTN T0);

	call apz_fun_document_getBinAllowcationLine_byDocEntry(:docentry, :fromObj, '#tempBinAllowcation');
	if :fromObj = '1250000001'
	THEN
		
		declare indexLoop int = 1;
		declare maxIndex int = 0;
		declare itemCode nvarchar(50);
		declare whsCode nvarchar(50);
		declare qty numeric(19,6);
		declare lineNum int;
		declare tempLineBin table ("ItemCode" nvarchar(50), "InvQty" numeric(19,6), "LineNum" int, "WhsCode" nvarchar(50), ID int);
		
		------- lấy dữ liệu các dòng sẽ lấy default bin cho kho đi
		insert into :tempLineBin 
			select a."ItemCode", a."InvQty", a."LineNum", a."FromWhsCod" "WhsCode", row_number() over(order by a."LineNum") ID from #tempLineCopyto a INNER JOIN OITM b ON a."ItemCode" = b."ItemCode" inner join OWHS c on a."FromWhsCod" = c."WhsCode"
			where b."ManBtchNum" = 'N' and b."ManSerNum" = 'N' and c."BinActivat" = 'Y';
			
		select ifnull((select max(id) from :tempLineBin), 0) into maxIndex from dummy;
		
		while :indexLoop <= :maxIndex
		DO
		
			select "ItemCode", "InvQty", "LineNum", "WhsCode" into itemcode, qty, lineNum, whsCode from :tempLineBin where ID = :indexLoop;
			insert into #tempBinAllowcation("ItemCode", "AbsEntry", "SysNumber", "BinAbs", "BinCode", "BinDescription", "CreateDate", "SelectedQty", "BaseLineNumber", "TranType", "LogQty", "WhsCode")
				select :itemCode, 0, 0, a."AbsEntry", a."BinCode", b."Descr", b."CreateDate", a."Qty", :lineNum, 1, a."Qty", :whsCode from APZ_Function_WhsGetDefBinItem('O', :whsCode, :itemCode, 0, :qty) a INNER JOIN OBIN b ON a."AbsEntry" = b."AbsEntry";
			indexLoop = :indexLoop + 1;
		
		END WHILE;
		
		---- lấy dữ liệu default bin cho kho tới
		delete from :tempLineBin;
		insert into :tempLineBin 
			select a."ItemCode", a."InvQty", a."LineNum", a."WhsCode", row_number() over(order by a."LineNum") ID from #tempLineCopyto a INNER JOIN OITM b ON a."ItemCode" = b."ItemCode" inner join OWHS c on a."WhsCode" = c."WhsCode"
			where b."ManBtchNum" = 'N' and b."ManSerNum" = 'N' and c."BinActivat" = 'Y';
			
		select ifnull((select max(id) from :tempLineBin), 0) into maxIndex from dummy;
		indexLoop = 1;
		while :indexLoop <= :maxIndex
		DO
		
			select "ItemCode", "InvQty", "LineNum", "WhsCode" into itemcode, qty, lineNum, whsCode from :tempLineBin where ID = :indexLoop;
			insert into #tempBinAllowcation("ItemCode", "AbsEntry", "SysNumber", "BinAbs", "BinCode", "BinDescription", "CreateDate", "SelectedQty", "BaseLineNumber", "TranType", "LogQty", "WhsCode")
				select :itemCode, 0, 0, a."AbsEntry", a."BinCode", b."Descr", b."CreateDate", a."Qty", :lineNum, 2, a."Qty", :whsCode from APZ_Function_WhsGetDefBinItem('I', :whsCode, :itemCode, 0, :qty) a INNER JOIN OBIN b ON a."AbsEntry" = b."AbsEntry";
			indexLoop = :indexLoop + 1;
		
		END WHILE;
	
	END IF;
	
	select * from #tempBinAllowcation;
	drop table #tempBinAllowcation;
	----------------------------------------------- END GET DATA BIN ALLOWCATION ---------------------------------------------------------

	Drop table #TempBatchSerialBase;
	drop table #TempBatchSerialBaseFinal;
	Drop table #tempLineCopyto;
END