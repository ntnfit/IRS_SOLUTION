
CREATE PROCEDURE APZ_BarcodeInventoryCounting_GetDataCounting(
	in barcodeString nvarchar(254),
	in whsCode nvarchar(50) default '',
	in binCode nvarchar(50) default '')
AS
BEGIN

	-------------------------------------- Lấy cấu trúc table ----------------------------------------------------------------------------------------
	create local temporary table #tempCounting as (SELECT TOP 0 b.*, CAST(NULL AS integer) AS "UgpCode", CAST(NULL AS nvarchar(50)) AS "InvUnitMsr", CAST(NULL AS decimal(19, 6)) AS "NumInInven" 
																						FROM (select 1 ID from DUMMY) a left Join APZ_BIC1 b ON 1 = 1);

	------------------------------------- Lấy dữ liệu batch trên hệ thống vào kiểm tra trạng thái trên kho -------------------------------------------
	INSERT INTO #tempCounting ("ItemCode", "ItemName", "DistNumber", "MnfSerial", "LotNumber", "ObjType", "BarCode", "CountDate", "CountTime", "LineStatus") 
		SELECT a."ItemCode", a."itemName", a."DistNumber", a."MnfSerial", a."LotNumber", a."ObjType", :barcodeString, CURRENT_DATE, HOUR(CURRENT_DATE) * 100 + MINUTE(CURRENT_DATE)
			, CASE WHEN b."MdAbsEntry" IS NOT NULL THEN 'O' ELSE 'N' END FROM (SELECT "ItemCode", "itemName", "DistNumber", "MnfSerial", "LotNumber", "ObjType", "AbsEntry" 
		FROM OBTN WHERE "DistNumber" = :barcodeString) AS a LEFT OUTER JOIN OBTQ b ON a."AbsEntry" = b."MdAbsEntry";

	------------------------------------- Update lại số lượng tồn kho của batch -----------------------------------------------------------------------
	UPDATE a SET "InWhsQty" = b."Instock" FROM #tempCounting a INNER JOIN (SELECT * FROM APZ_OITW_LOCATION WHERE "BinCode" = IFNULL(:binCode, '') AND "WhsCode" = :whsCode) AS b ON a."ItemCode" = b."ItemCode" AND a."DistNumber" = b."DistNumber";

	------------------------------------- Với những mã trạng thái là Ok mà không có tồng kho thì update trạng thái là thừa hàng ------------------------
	UPDATE #tempCounting SET "LineStatus" = 'S' WHERE "LineStatus" = 'O' AND IFNULL("InWhsQty", 0) = 0;

	------------------------------------- UPDATE các thông tin về UOM ----------------------------------------------------------------------------------
	--update a set InvUoM = 'Y', UomEntry = d.UomEntry, UomCode = c.UomCode, unitMsr = b.InvntryUom, NumPerMsr = 1, InvUnitMsr = b.InvntryUom, NumInInven = isnull(d.AltQty, 1) / ISNULL(d.BaseQty, 1) from #tempCounting a inner join OITM b ON a.ItemCode = b.ItemCode 
	--												left JOin OUOM c ON c.UomName = b.InvntryUom
	--											    left join UGP1 d on b.UgpEntry = d.UgpEntry and d.UomEntry = c.UomEntry
    select * from #tempCounting;
	
END;




