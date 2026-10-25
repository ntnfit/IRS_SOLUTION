
CALL APZ_EXISTSPROC('APZ_InventoryTranfer_GetDataCopyFrom');

GO

CREATE PROCEDURE APZ_InventoryTranfer_GetDataCopyFrom(
	in docEntry nvarchar(1000),
	in fromObj int default -1)
AS
BEGIN
	
	declare fromDate timestamp;
	declare toDate timestamp;
	declare i int;
	
	declare batchSerial table
	(
		"AbsEntry" int,
		"BinAbs" int,
		"BinCode" nvarchar(228),
		"SelectedQty" decimal,
		"WhsCode" nvarchar(50),		
		"BinDescription" nvarchar(254),
		"BaseLineNumber" int,
		"BaseDocEntry" int,
		"ManBatchSerial" char(1),
		"LogQty"	decimal,		
		"IsExits" char(1),
		"TranType" int,
		"IsBase" char(1)
	);
	
	select ADD_DAYS(CURRENT_DATE,-1) into fromDate from DUMMY;
	select ADD_DAYS(CURRENT_DATE,1) into toDate FROM DUMMY;
	
	----------------------------------------------------- LẤY DỮ LIỆU CARCODE ----------------------------------------------------
	select distinct "CardCode" from OWTQ where :fromObj = 1250000001 and "DocEntry" in (select cast(Data as int) from Split(:docEntry,',')) and ifnull("CardCode",'') <> ''
	UNION ALL
	select distinct "CardCode" from ODRF where :fromObj = 112 and "ObjType" = '67' and "DocEntry" in (select cast(Data as int) from Split(:docEntry,',')) and ifnull("CardCode",'') <> '';
	----------------------------------------------------- END LẤY DỮ LIỆU CARDCODE ----------------------------------------------

	----------------------------------------------------- LẤY DỮ LIỆU HEADER ----------------------------------------------------
	--create local temporary table #tblDoc as (select top 0 a.*, cast(null as nvarchar(254)) "CntctName", a."DocCur" "Currency",cast(0 as tinyint) "IsDraft" from ODRF a);
	--https://help.sap.com/docs/SAP_HANA_PLATFORM/de2486ee947e43e684d39702027f8a94/36f718be73e745ceae0530aa80850ce3.html
	tblDoc =
		select a.*, b."Name" as "CntctName", a."DocCur" "Currency", cast(0 as tinyint) "IsDraft" from ODRF a left join OCPR b on a."CntctCode" = b."CntctCode" where :fromObj = 112 and a."DocEntry" in (select cast(Data as int) from Split(:docEntry,','))
	UNION ALL 
		select  a.*, b."Name" as "CntctName", a."DocCur" "Currency", cast(0 as tinyint) from OWTQ a left join OCPR b on a."CntctCode" = b."CntctCode" where :fromObj <> 112 and a."DocEntry" in (select cast(Data as int) from Split(:docEntry,','));
	
	if :fromObj <> 112 then
		for i in 1..record_count(:tblDoc) do
			tblDoc.("DocDate","DocDueDate","TaxDate")[:i]=(current_date,current_date,current_date);
		end for;
		--UPDATE #tblDoc SET "DocDate" = CURRENT_DATE, "DocDueDate" = CURRENT_DATE, "TaxDate" = CURRENT_DATE;
	end if;
	
	if :fromObj = 112
	THEN
		for i in 1..record_count(:tblDoc) do
			tblDoc.("draftKey","IsDraft")[:i]=(:tblDoc."DocEntry"[:i],1);
		end for;
		--update #tblDoc set "draftKey" = "DocEntry", "IsDraft"= 1;
	END IF;
	
	for i in 1..record_count(:tblDoc) do
		tblDoc.("DocEntry","JrnlMemo","Comments")[:i]=(0,'Inventory Transfers -',IFNULL(:tblDoc."Comments"[:i], '') || ' Based On Inventory Transfer Request ' || :docEntry || '.');
	end for;
	--UPDATE #tblDoc SET "JrnlMemo" = n'Inventory Transfers -', "Comments" = IFNULL("Comments", '') || ' Based On Inventory Transfer Request ' || :docEntry || '.';

	--update #tblDoc set "DocEntry" = 0;
	select * from :tblDoc;
	--drop table #tblDoc;
	--------------------------------------------------- END LẤY DỮ LIÊU HEADER ---------------------------------------------------

	--------------------------------------------------- LẤY DỮ LIỆU LINES -------------------------------------------------------
	--create local temporary table #tempLineCopyto as (select top 0 a.*, "LineNum" as "LineNumBer" from WTQ1 a);
	
	tempLineCopyto =
		select a.*, ROW_NUMBER() OVER(ORDER BY "LineNum" ASC) - 1 as "LineNumBer" from WTQ1 a where :fromObj = 1250000001 and a."ShipDate" <> '' and ifnull (a."OpenQty", 0) > 0 and a."LineStatus" = 'O' and 
						 a."DocEntry" in (select cast(Data as int) from Split(:docEntry,','))
		UNION ALL
		select a.*, ROW_NUMBER() OVER(ORDER BY "LineNum" ASC) - 1 as "LineNumBer" from DRF1 a where :fromObj = 112 and a."ObjType" = '67' and 
							a."DocEntry" in (select cast(Data as int) from Split(:docEntry,','));

	IF :fromObj = 1250000001
	THEN
		declare docNum, docEntry, slpCode int;
		
		for i in 1..record_count(:tempLineCopyto) do
			docEntry = :tempLineCopyto."DocEntry"[:i];
			select "DocNum", "SlpCode" into docNum, slpCode from OWTQ where "DocEntry"=:docEntry;
			
			tempLineCopyto.("DocEntry","ObjType","BaseType","BaseEntry","BaseRef","BaseLine","Quantity","InvQty","LineNum","ShipDate","SlpCode")[:i] = 
			(0,'67','1250000001',:tempLineCopyto."DocEntry"[:i],:docNum,:tempLineCopyto."LineNum"[:i],:tempLineCopyto."OpenQty"[:i],:tempLineCopyto."OpenInvQty"[:i],:tempLineCopyto."LineNumBer"[:i],current_date,:slpCode);
		end for;
		/*
		UPDATE a SET a."DocEntry" = 0, a."ObjType" = '67', a."BaseType" = '1250000001', a."BaseEntry" = a."DocEntry", a."BaseRef" = b."DocNum", a."BaseLine" = a."LineNum"
					, a."Quantity" = a."OpenQty", a."InvQty" = a."OpenInvQty", a."LineNum" = "LineNumBer", a."ShipDate" = CURRENT_DATE, a."SlpCode" = b."SlpCode" 
				FROM #tempLineCopyto a INNER JOIN OWTQ b ON a."DocEntry" = b."DocEntry";
		*/
	END IF;

	SELECT a.*, CASE WHEN ifnull(b."ManBtchNum", '') = 'Y' THEN 'B' WHEN ifnull(b."ManSerNum", '') = 'Y' THEN 'S' ELSE 'N' END AS "ManBtchSerial", ifnull(b."OnHand", 0) AS "OnHandItem"
		  , ifnull(b."IsCommited", 0) AS "IsCommittedItem", ifnull(b."OnOrder", 0) AS "OnOrderedItem", ifnull(c."OnHand", 0) AS "OnHandWhs", ifnull(c."IsCommited", 0) AS "IsCommittedWhs"
		  , ifnull(c."OnOrder", 0) AS "OnOrderedWhs", b."SalUnitMsr" AS "SalUnitMsr", ifnull(b."NumInSale", 0) AS "NumInSale", b."BuyUnitMsr", ifnull(b."NumInBuy", 0) AS "NumInBuy"
		  , ifnull(b."UgpEntry", -1) AS "UgpCode", b."InvntryUom" AS "InvUnitMsr", ifnull(d."AltQty", 1) / ifnull(d."BaseQty", 1) AS "NumInInven"
		  , CASE WHEN ifnull(a."OcrCode", '') <> '' AND ifnull(a."OcrCode2", '') <> '' AND ifnull(a."OcrCode3", '') <> '' AND ifnull(a."OcrCode4", '') <> '' AND ifnull(a."OcrCode5", '') <> '' THEN NULL 
		  		ELSE ifnull(a."OcrCode", '') || ';' || ifnull(a."OcrCode2", '') || ';' || ifnull(a."OcrCode3", '') || ';' || ifnull(a."OcrCode4", '') || ';' || ifnull(a."OcrCode5", '') END AS "Rules", c."AvgPrice" 
		 FROM :tempLineCopyto a INNER JOIN OITM b ON a."ItemCode" = b."ItemCode" LEFT OUTER JOIN OITW c ON a."ItemCode" = c."ItemCode" AND a."FromWhsCod" = c."WhsCode" 
		 						LEFT OUTER JOIN UGP1 d ON b."UgpEntry" = d."UgpEntry" AND d."UomEntry"=b."IUoMEntry" order by "VisOrder";
	--------------------------------------------------- END GET DATA LINES ----------------------------------------------------------

	call apz_fun_getbatchserial_byDocEntry(:docEntry, :fromObj, temp);
	insert into :batchSerial select * from :temp;

	---  xóa dữ liệu batch không có line tồn tại
	IF :fromObj <> '112'
	THEN
		DELETE FROM :batchSerial where ("BaseDocEntry","BaseLineNumber") not in (select "BaseEntry","BaseLine" from :tempLineCopyto);
		Update a set "BaseLineNumber" = b."LineNum" from :batchSerial a INNER JOIN :tempLineCopyto b ON a."BaseDocEntry" = b."BaseEntry" and a."BaseLineNumber" = b."BaseLine";
	END IF;
	
	--drop table #tempLineCopyto;
	
	select b.*,a."BinAbs",a."BinCode",a."SelectedQty",a."WhsCode",a."BinDescription",a."BaseLineNumber",a."BaseDocEntry",a."ManBatchSerial",a."LogQty",a."IsExits",a."TranType",a."IsBase"
	from :batchSerial a join OBTN b on a."ManBatchSerial"='B' and a."AbsEntry"=b."AbsEntry"
	union all
	select b.*,a."BinAbs",a."BinCode",a."SelectedQty",a."WhsCode",a."BinDescription",a."BaseLineNumber",a."BaseDocEntry",a."ManBatchSerial",a."LogQty",a."IsExits",a."TranType",a."IsBase"
	from :batchSerial a join OSRN b on a."ManBatchSerial"='S' and a."AbsEntry"=b."AbsEntry";
	
	-------------------------------- END THÊM BIN ALLOWCATION --------------------------------------------

	------------------------------------------------ END GET DATA BATCH AND SERIAL --------------------------------------------------------

	call apz_fun_document_getBinAllowcationLine_byDocEntry(:docentry, :fromObj, bin);
	select * from :bin a left join OBTN b on 1 = 0;
	
	----------------------------------------------- END GET DATA BIN ALLOWCATION ---------------------------------------------------------

END