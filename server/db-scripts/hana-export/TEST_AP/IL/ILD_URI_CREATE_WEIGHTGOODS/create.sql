CREATE procedure ILD_URI_CREATE_WEIGHTGOODS(out t table(MsgCode int, Mess nvarchar(254)))
as
begin
	declare headerFields nvarchar(4000) = '"DocDate","DocNo","TruckPlateNo","ContainerNo","TrailerPlateNo","ContainerType","SealNo","SOPONo","CardCode","CardName","DriverName","InWeight","InTime","OutWeight","OutTime","TareWeight","GrossWeight","NetWeight","TruckRegisteredWeight","TrailerRegisteredWeight","ContainerRegisteredWeight","AlarmMessage","RegisterRemark","WeighingRemark","WeighingType","WeigherName"';
	declare detailFields nvarchar(4000) = 'ItemCode,ItemName,ItemType,DeliveryTruckWeight,DeliveryGoodsWeight,DeliveryEmptyTruckWeight,PackingWeight,BagQuantity,PalletWeight,PalletQuantity';
	declare detailQuote nvarchar(4000) = '"ItemCode","ItemName","ItemType","DeliveryTruckWeight","DeliveryGoodsWeight","DeliveryEmptyTruckWeight","PackingWeight","BagQuantity","PalletWeight","PalletQuantity"';
	DECLARE tblMandatory, tblDetailMandatory TABLE
	(
		"RowNum" int,
	    "FieldName" NVARCHAR(100),
	    "Description" NVARCHAR(500)
	);
	declare query nvarchar(4000);
	
	declare cnt int;
	declare i int = 0;
	declare fieldName nvarchar(50);
	declare description nvarchar(200);
	declare docNo, soPONo nvarchar(50);
	
	tblMandatory =
	SELECT
        ROW_NUMBER() OVER (ORDER BY "FieldName") AS "RowNum",
        "FieldName",
        "Description"
    FROM
	(
	    SELECT 'DocDate' AS "FieldName", N'Ngày' AS "Description" FROM DUMMY
	    UNION ALL SELECT 'DocNo', N'Số Phiếu' FROM DUMMY
	    UNION ALL SELECT 'TruckPlateNo', N'Biển Số đầu xe' FROM DUMMY
	    UNION ALL SELECT 'SOPONo', N'Số SO/PO' FROM DUMMY
	    UNION ALL SELECT 'CardCode', N'Mã Khách Hàng / Nhà cung cấp' FROM DUMMY
	    UNION ALL SELECT 'CardName', N'Khách Hàng / Nhà cung cấp' FROM DUMMY
	    UNION ALL SELECT 'InWeight', N'KL cân lần 1 / In weight' FROM DUMMY
	    UNION ALL SELECT 'InTime', N'Giờ cân lần 1' FROM DUMMY
	    UNION ALL SELECT 'OutWeight', N'KL cân lần 2 / Out weight' FROM DUMMY
	    UNION ALL SELECT 'OutTime', N'Giờ cân lần 2' FROM DUMMY
	    UNION ALL SELECT 'NetWeight', N'Net weight / Goods weight' FROM DUMMY
	    UNION ALL SELECT 'WeighingType', N'Kiểu cân' FROM DUMMY
	    UNION ALL SELECT 'WeigherName', N'Người cân' FROM DUMMY
	);
	tblDetailMandatory =
	SELECT
        ROW_NUMBER() OVER (ORDER BY "FieldName") AS "RowNum",
        "FieldName",
        "Description"
    FROM
	(
	    SELECT 'ItemCode' AS "FieldName", N'Mã hàng hóa' AS "Description" FROM DUMMY
	);
	
	CREATE LOCAL TEMPORARY TABLE #TBLHeader
	(
	    "DocDate" NVARCHAR(50),
	    "DocNo" NVARCHAR(50),
	    "TruckPlateNo" NVARCHAR(50),
	    "ContainerNo" NVARCHAR(50),
	    "TrailerPlateNo" NVARCHAR(50),
	    "ContainerType" NVARCHAR(50),
	    "SealNo" NVARCHAR(50),
	    "SOPONo" nvarchar(50),
	    SOPOEntry int, -- DocEntry SO/PO
		SOPOType nvarchar(20), -- ObjType 17/22
	    "CardCode" NVARCHAR(50),
	    "CardName" NVARCHAR(200),
	    "DriverName" NVARCHAR(200),
	    "InWeight" NVARCHAR(50),
	    "InTime" NVARCHAR(50),
	    "OutWeight" NVARCHAR(50),
	    "OutTime" NVARCHAR(50),
	    "TareWeight" NVARCHAR(50),
	    "GrossWeight" NVARCHAR(50),
	    "NetWeight" NVARCHAR(50),
	    "TruckRegisteredWeight" NVARCHAR(50),
	    "TrailerRegisteredWeight" NVARCHAR(50),
	    "ContainerRegisteredWeight" NVARCHAR(50),
	    "AlarmMessage" NVARCHAR(500),
	    "RegisterRemark" NVARCHAR(500),
	    "WeighingRemark" NVARCHAR(500),
	    "WeighingType" NVARCHAR(50),
	    "WeigherName" NVARCHAR(200)
	);
	
	CREATE LOCAL TEMPORARY TABLE #TBLDetails
	(
	    "DocNo" NVARCHAR(50),
	    "ItemCode" NVARCHAR(50),
	    "ItemName" NVARCHAR(200),
	    "ItemType" NVARCHAR(100),
	    "DeliveryTruckWeight" NVARCHAR(50),
	    "DeliveryGoodsWeight" NVARCHAR(50),
	    "DeliveryEmptyTruckWeight" NVARCHAR(50),
	    "PackingWeight" NVARCHAR(50),
	    "BagQuantity" NVARCHAR(50),
	    "PalletWeight" NVARCHAR(50),
	    "PalletQuantity" NVARCHAR(50)
	);
	
	------------- Lấy dữ liệu từ api vào 2 bảng tạm
	query := 'insert into #tblHeader(' || :headerFields || ')
	select top 1 ' || :headerFields || ' from #tbl';
	exec :query;
	
	select string_agg('"Details.' || Data || '"', ',') into query from split(:detailFields, ',');
	
	query := 'insert into #tblDetails("DocNo",' || :detailQuote || ')
	select "DocNo",' || :query || ' from #tbl';
	exec :query;
	
	exec 'drop table #tbl';
	-------------------------------------------

	------------- Validate -------------------
	if not exists(select 1 from #tblHeader) then
		t = select 2 as MsgCode, 'Dữ liệu không được cung cấp.' as Mess from dummy;
		return;
	end if;
	
	-- Các thông tin header bắt buộc
	SELECT COUNT(*)
	INTO cnt
	FROM :tblMandatory;

	for i in 1..:cnt do
		declare tempValue nvarchar(200);
		select "FieldName", "Description" into fieldName, description from :tblMandatory where "RowNum"=:i;
		exec 'select "' || :fieldName || '" from #tblHeader' into tempValue;
		if ifnull(:tempValue,'')='' then
			t = select 3 as MsgCode, :description || ' (' || :fieldName || ') không được để trống.' as Mess from dummy;
			return;
		end if;
	end for;
	
	select "DocNo" into docNo from #tblHeader;
	
	-- DocNo đã tồn tại
	if exists(select 1 from GOODSWEIGHT where DocNo=:docNo) then
		t = select 4 as MsgCode, 'Số phiếu ''' || :docNo || ''' đã tồn tại.' as Mess from dummy;
		return;
	end if;
	
	-- Số SO/PO không tồn tại
	select "SOPONo" into soPONo from #tblHeader;
	if exists(select 1 from OPOR where cast("DocNum" as nvarchar)=:soPONo and CANCELED='N') then
		update a set SOPOEntry=b."DocEntry", SOPOType='22'
		from #tblHeader a join OPOR b on a."SOPONo"=b."DocNum";
	elseif exists(select 1 from ORDR where cast("DocNum" as nvarchar)=:soPONo and CANCELED='N') then
		update a set SOPOEntry=b."DocEntry", SOPOType='17'
		from #tblHeader a join ORDR b on a."SOPONo"=b."DocNum";
	else
		t = select 5 as MsgCode, 'Số SO/PO không tồn tại.' as Mess from dummy;
		return;
	end if;
	
	-- Các thông tin details bắt buộc
	SELECT COUNT(*)
	INTO cnt
	FROM :tblDetailMandatory;

	for i in 1..:cnt do
		declare tempValue nvarchar(200);
		select "FieldName", "Description" into fieldName, description from :tblDetailMandatory where "RowNum"=:i;
		exec 'select "' || :fieldName || '" from #tblDetails' into tempValue;
		if ifnull(:tempValue,'')='' then
			t = select 5 as MsgCode, 'Details: ' || :description || ' (' || :fieldName || ') không được để trống.' as Mess from dummy;
			return;
		end if;
	end for;
	
	------------- Xong Validate ------------
	
	-- Insert Data
	update #tblHeader set "InTime"=replace("InTime",':',''), "OutTime"=replace("OutTime",':','');
	
	query := 'insert into GOODSWEIGHT(SOPOEntry,SOPOType,' || replace(:headerFields,'"','') || ',CreateDate,CreateTime)
	select SOPOEntry,SOPOType,' || :headerFields || ',current_date,to_char(now(),''hh24mi'') from #tblHeader';
	exec :query;
	
	update #tblDetails set "DocNo"=:docNo;
	
	query := 'insert into GOODSWEIGHT_DETAILS(DocNo,' || replace(:detailQuote,'"','') || ')
	select "DocNo",' || :detailQuote || ' from #tblDetails';
	exec :query;
	
	drop table #tblHeader;
	drop table #tblDetails;
	
	-- Trả kết quả success
	t = select 200 as MsgCode, :docNo as Mess from dummy;
	
end
