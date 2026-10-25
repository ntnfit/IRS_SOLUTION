

CREATE PROCEDURE APZ_Scheduling_GetDataProductionOrder
(
	IN itemCode nvarchar(50),
	version nvarchar(55) DEFAULT '',
	inputType char(1) DEFAULT 'S', ------------------------- Kiểm tra đầu vào của source để gen lệnh: S- schedule/ C- capacity -------------
	sourceId int DEFAULT -1,		  ------------------------- Source Id của capacity để lấy danh sách resource ------------------------------
	line int DEFAULT -1		
)
AS
BEGIN

	declare capacityProId int;
	declare whs nvarchar(50) = '';
	capacityProId = iFnull((SELECT "ProId" FROM APZ_CGRC WHERE :inputType = 'C' AND "GrcId" = :sourceId AND "Line" = :line), - 1);
	
	CREATE LOCAL TEMPORARY TABLE #temptable
							(
								"Type" int null
								, "Father" nvarchar(50) null
								, "Code" nvarchar(50) null
								, "Quantity" numeric(19,6) null
								, "Warehouse" nvarchar(50) null
								, "WipActCode" nvarchar(50) null
								, "Price" numeric(19,6) null
								, "Currency" nvarchar(10) null
								, "PriceList" int null
								, "IssueMthd" nvarchar(50) null
								, "Uom" nvarchar(50) null
								, "Comment" nvarchar(254) null
								, "OcrCode" nvarchar(50) null
								, "OcrCode2" nvarchar(50) null
								, "OcrCode3" nvarchar(50) null
								, "OcrCode4" nvarchar(50) null
								, "OcrCode5" nvarchar(50) null
								, "Project" nvarchar(50) null
								, "LineText" ntext null
								, "U_OpeCode" nvarchar(50) null
								, "U_StartTime" int null
								, "U_EndTime" int null
								, "StartDate" date null
								, "EndDate" date null
								, "U_Shift" nvarchar(50) null
								, "U_OriginLine" int null
							);

	if ifnull(:version, '') = '' then
		
		whs = ifnull((select "ToWH" from OITT where "Code" = :itemCode), '');
		insert into #temptable
		SELECT a."Type", a."Father", a."Code", a."Quantity" / b."Qauntity" AS "Quantity", a."Warehouse", a."WipActCode", a."Price", a."Currency", a."PriceList", a."IssueMthd", a."Uom", a."Comment", a."OcrCode", a."OcrCode2",
						a."OcrCode3", a."OcrCode4", a."OcrCode5", a."Project", a."LineText", NULL AS "U_OpeCode", NULL AS "U_StartTime", NULL AS "U_EndTime", NULL AS "StartDate", NULL AS "EndDate", NULL AS "U_Shift", NULL AS "U_OriginLine" FROM ITT1 a INNER JOIN OITT b ON a."Father" = b."Code" WHERE b."Code" = :itemCode;

	else
		whs = ifnull((select "ToWH" from APZ_OITT where "Code" = :itemCode and "Version" = :version), '');
		insert into #temptable
		SELECT a."Type", a."Father", a."Code", a."Quantity" / b."Qauntity" AS "Quantity", a."Warehouse", a."WipActCode", a."Price", a."Currency", a."PriceList", a."IssueMthd", a."Uom", a."Comment", a."OcrCode", a."OcrCode2",
						a."OcrCode3", a."OcrCode4", a."OcrCode5", a."Project", a."LineText", NULL AS "U_OpeCode", NULL AS "U_StartTime", NULL AS "U_EndTime", NULL AS "StartDate", NULL AS "EndDate", NULL AS "U_Shift", NULL AS "U_OriginLine" FROM APZ_ITT1 a INNER JOIN APZ_OITT b ON a."Father" = b."BomVerID" WHERE b."Code" = :itemCode AND b."Version" = :version;

	
	END if;

	insert into #temptable
	SELECT 290, :itemCode, "ResCode", "TimeRun" / CASE WHEN IFNULL("Qty", 0) = 0 THEN 1 ELSE "Qty" END, :whs, NULL, NULL, NULL, NULL, 'M', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, "OpeCode", "StartTime", "EndTime", "WorkDay", "EndWorkDay", "Shift", "Line" FROM APZ_CGRC WHERE :inputType = 'C' AND "GrcId" = :sourceId AND "ProId" = :capacityProId;

	select * from #temptable;
END



