
CALL APZ_EXISTSPROC('APZ_TranferRequest_GetDataCopyFrom');

GO

CREATE PROCEDURE APZ_TranferRequest_GetDataCopyFrom(in docEntry nvarchar(1000), in fromObj int default -1)
AS
BEGIN
	
	declare fromDate timestamp;-- =  (select DATEADD(DAY,-1,GetDate()))
	declare toDate timestamp;-- =  (select DATEADD(DAY,1,GetDate()))
	declare i int;

	fromdate = ADD_DAYS(current_date, -1);
	toDate = ADD_DAYS(current_date, 1);
	
	select distinct "CardCode" from ODRF where :fromObj = 112 and "ObjType" = '1250000001' and "DocEntry" in (select cast(Data as int) from Split(:docEntry,',')) and ifnull("CardCode",'') <> '';
	
	--create local temporary table #tblDoc as (select top 0 *, cast(null as nvarchar(254)) "CntctName", cast(null as nvarchar(3)) "Currency", cast(1 as tinyint) "IsDraft" from ODRF);

	tblDoc =
		select a.*, b."Name" as "CntctName", a."DocCur" "Currency", cast(1 as tinyint) "IsDraft" from ODRF a left join OCPR b on a."CntctCode" = b."CntctCode" where :fromObj = 112 and a."DocEntry" in (select cast(Data as int) from Split(:docEntry,','));

	if :fromObj = 112
	THEN
		for i in 1..record_count(:tblDoc) do
			tblDoc.("draftKey","IsDraft")[:i]=(:tblDoc."DocEntry"[:i],1);
		end for;
		--update #tblDoc set "draftKey" = "DocEntry", "IsDraft" = 1;
	END IF;
	
	for i in 1..record_count(:tblDoc) do
		tblDoc.("DocEntry")[:i]=(0);
	end for;
	--update #tblDoc set "DocEntry" = 0;

	select * from :tblDoc;
	--drop table #tblDoc;
	
	--create local temporary table #tempLineCopyto as (select top 0 * from DRF1);

	tempLineCopyto =
		select a.* from DRF1 a inner join OITM b on a."ItemCode" = b."ItemCode" where :fromObj = 112 and a."ObjType" = '1250000001' 
					and ((b."validFor" = 'Y' and IFNULL(b."frozenFor",'N') = 'N' and (IFNULL(b."validFrom", :fromDate) <= current_date and IFNULL(b."validTo", :toDate) >= current_date)) 
					OR (IFNULL(b."validFor",'N') = 'N' and b."frozenFor" = 'Y' and (IFNULL(b."frozenFrom", :fromDate) > current_date or IFNULL(b."frozenTo", :toDate) < current_date))
					or (b."validFor" = 'Y' and b."frozenFor" = 'Y' and (IFNULL(b."validFrom", :fromDate) <= current_date and IFNULL(b."validTo", :toDate) >= current_date) and
					(IFNULL(b."frozenFrom", :fromDate) > current_date or IFNULL(b."frozenTo", :toDate) < current_date))
					OR (b."validFor" = 'N' AND b."frozenFor" = 'N')) and a."DocEntry" in (select cast(Data as int) from Split(:docEntry,','));

	select a.*,case when b."ManBtchNum" = 'Y' then 'B' when b."ManSerNum" = 'Y' then 'S' else 'N' End "ManBtchSerial"
			, IFNULL(b."OnHand",0) as "OnHandItem", IFNULL(b."IsCommited",0) as "IsCommittedItem", IFNULL(b."OnOrder",0) as "OnOrderedItem", IFNULL(c."OnHand",0) as "OnHandWhs", 
			IFNULL(c."IsCommited",0) as "IsCommittedWhs", IFNULL(c."OnOrder",0) as "OnOrderedWhs", b."SalUnitMsr" as "SalUnitMsr", IFNULL(b."NumInSale",0) as "NumInSale", b."BuyUnitMsr",IFNULL(b."NumInBuy",0) "NumInBuy",
			IFNULL(b."UgpEntry",-1) as "UgpCode", b."InvntryUom" as "InvUnitMsr", IFNULL(d."AltQty", 1) / IFNULL(d."BaseQty", 1) as "NumInInven", case when IFNULL(a."OcrCode", '') <> '' and IFNULL(a."OcrCode2", '') <> '' and IFNULL(a."OcrCode3", '') <> '' and IFNULL(a."OcrCode4", '') <> '' and IFNULL(a."OcrCode5", '') <> '' then null 
				else IFNULL(a."OcrCode", '') || ';' || IFNULL(a."OcrCode2", '') || ';' || IFNULL(a."OcrCode3", '') || ';' || IFNULL(a."OcrCode4", '') || ';' || IFNULL(a."OcrCode5", '') end "Rules", c."AvgPrice"
	 from :tempLineCopyto a inner join OITM b on a."ItemCode" = b."ItemCode"
						    left join OITW c on a."ItemCode" = c."ItemCode" and a."FromWhsCod" = c."WhsCode"
							left join UGP1 d on b."UgpEntry" = d."UgpEntry" and d."UomEntry"=b."IUoMEntry";
	
	--drop table #tempLineCopyto;
	
	call apz_fun_getbatchserial_byDocEntry(:docEntry, '112', batchSerial);
	select b.*,a."BinAbs",a."BinCode",a."SelectedQty",a."WhsCode",a."BinDescription",a."BaseLineNumber",a."BaseDocEntry",a."ManBatchSerial",a."LogQty",a."IsExits",a."TranType",a."IsBase"
	from :batchSerial a join OBTN b on a."ManBatchSerial"='B' and a."AbsEntry"=b."AbsEntry"
	union all
	select b.*,a."BinAbs",a."BinCode",a."SelectedQty",a."WhsCode",a."BinDescription",a."BaseLineNumber",a."BaseDocEntry",a."ManBatchSerial",a."LogQty",a."IsExits",a."TranType",a."IsBase"
	from :batchSerial a join OSRN b on a."ManBatchSerial"='S' and a."AbsEntry"=b."AbsEntry";
	
	call apz_fun_document_getBinAllowcationLine_byDocEntry(:docentry, '112', bin);
	select * from :bin a left join OBTN b on 1 = 0;

END