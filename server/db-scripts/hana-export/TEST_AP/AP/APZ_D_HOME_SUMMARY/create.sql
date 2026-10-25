

CREATE FUNCTION apz_d_home_summary
(
	IN userCode nvarchar(50),
	IN type char(1), 
	IN storeList ParaStrListType
)
returns table
(
	"ItemTotal" nvarchar(30), "ItemPerOrder" nvarchar(30), "RevenuePerOrder" nvarchar(30), "TotalRevenue" nvarchar(30), 
	"NumberStore" nvarchar(30), "NumberStoreNew" nvarchar(30), "NumberVisit" nvarchar(30), "NumberVisitHaveOrder" nvarchar(30), 
	"NumberOrder" nvarchar(30), "NumberOrderOpen" nvarchar(30), "DateType" char(1)
)
AS
BEGIN
	DECLARE userid INT;
	declare fdate datetime;
	DECLARE tdate datetime;
	
	declare ItemTotal decimal; 
	declare ItemPerOrder decimal; 
	declare RevenuePerOrder decimal;
	declare TotalRevenue decimal;
	declare NumberStore decimal;
	declare NumberStoreNew decimal;
	declare NumberVisit decimal;
	declare NumberVisitHaveOrder decimal;
	declare NumberOrder decimal;
	declare NumberOrderOpen decimal;
	
	declare tmpStore table ("CardCode" nvarchar(50), "CardName" nvarchar(254));
	declare tblData table ("DocEntry" int, "DocTotal" decimal, U_GUID nvarchar(100), "DocStatus" char(1));
	declare tblData_1 table ("DocEntry" int, "ItemCode" nvarchar(50));
	declare tblData_2 table ("DocEntry" int, CNT int);

	declare result table 
	(
		"ItemTotal" nvarchar(30), "ItemPerOrder" nvarchar(30), "RevenuePerOrder" nvarchar(30), "TotalRevenue" nvarchar(30), 
		"NumberStore" nvarchar(30), "NumberStoreNew" nvarchar(30), "NumberVisit" nvarchar(30), "NumberVisitHaveOrder" nvarchar(30), 
		"NumberOrder" nvarchar(30), "NumberOrderOpen" nvarchar(30), "DateType" char(1)
	);

	SELECT TOP 1 USERID INTO userid FROM OUSR WHERE USER_CODE = :userCode;
	
	IF :type = 'W' THEN 
		fdate := ADD_DAYS(current_date, -dayofweek(current_date) + 2);
		tdate := add_days(:fdate, 6);
	ELSE IF :type = 'M' THEN 
		fdate := add_days(last_day(add_months(current_date, -1)), 1);
		tdate := last_day(current_date);
	ELSE IF :type = 'Y' THEN 
		fdate := to_date(year(current_date) || '-01-01', 'yyyy-MM-dd');
		tdate := to_date(year(current_date) || '-12-31', 'yyyy-MM-dd');
	END IF;
	END IF;
	END IF;
	
	INSERT INTO  :tmpStore ("CardCode")
	select * from :storeList;
	
	INSERT INTO :tblData SELECT "DocEntry", "DocTotal", U_GUID, "DocStatus" FROM ORDR WHERE CANCELED = 'N' 
	--AND "UserSign" = :userid 
	AND "DocDate" BETWEEN :fdate AND :tdate AND "CardCode" IN (SELECT "CardCode" FROM :tmpStore);
	
	IF (SELECT COUNT(1) FROM :tblData) > 0 THEN 
		SELECT SUM("DocTotal") / COUNT("DocEntry") INTO RevenuePerOrder FROM :tblData;
		SELECT SUM("DocTotal") INTO TotalRevenue FROM :tblData;
		INSERT INTO :tblData_1 SELECT RDR1."DocEntry", "ItemCode" FROM :tblData, RDR1 WHERE :tblData."DocEntry" = RDR1."DocEntry" GROUP BY RDR1."ItemCode", RDR1."DocEntry";
		INSERT INTO :tblData_2 SELECT "DocEntry", COUNT("ItemCode") AS "CNT" FROM :tblData_1 GROUP BY "DocEntry";
		SELECT AVG(CNT) INTO ItemPerOrder FROM :tblData_2;
		SELECT COUNT(DISTINCT "ItemCode")  INTO ItemTotal FROM :tblData_1;
	END IF;
	
	select count(1) into NumberStore from :tmpStore;
	select count(1) into NumberStoreNew from OCRD H where H."CardCode" in (select "CardCode" from :tmpStore) and H."CreateDate" between :fdate and :tdate;
	select count(1) into NumberVisit from APZ_D_VISIT T0 where T0."StoreCode" in (select "CardCode" from :tmpStore) and T0."FinishedDate" between :fdate and :tdate;
	select count(1) into NumberVisitHaveOrder from :tblData T1 WHERE T1.U_GUID in (select "Guid" from APZ_D_VISIT T0 where T0."StoreCode" in (select "CardCode" from :tmpStore) and T0."FinishedDate" between :fdate and :tdate);
	select count(1) into NumberOrder from :tblData;
	select count(1) into NumberOrderOpen from :tblData where "DocStatus" = 'O';
	
	insert into :result select -- định dạng kiểu: 1 325 235 325,56
		IFNULL(NULLIF(replace(trim(to_nvarchar(:ItemTotal, replicate(' 999', ceil(length(floor(:ItemTotal)) / 3)) || '.00')), '.', ','), ''), '0') AS "ItemTotal",
		IFNULL(NULLIF(replace(trim(to_nvarchar(:ItemPerOrder, replicate(' 999', ceil(length(floor(:ItemPerOrder)) / 3)) || '.00')), '.', ','), ''), '0') AS "ItemPerOrder",
		IFNULL(NULLIF(replace(trim(to_nvarchar(:RevenuePerOrder, replicate(' 999', ceil(length(floor(:RevenuePerOrder)) / 3)))), '.', ','), ''), '0') AS "RevenuePerOrder",
		IFNULL(NULLIF(replace(trim(to_nvarchar(:TotalRevenue, replicate(' 999', ceil(length(floor(:TotalRevenue)) / 3)))), '.', ','), ''), '0') AS "TotalRevenue",
		IFNULL(NULLIF(replace(trim(to_nvarchar(:NumberStore, replicate(' 999', ceil(length(floor(:NumberStore)) / 3)))), '.', ','), ''), '0') AS "NumberStore",
		IFNULL(NULLIF(replace(trim(to_nvarchar(:NumberStoreNew, replicate(' 999', ceil(length(floor(:NumberStoreNew)) / 3)))), '.', ','), ''), '0') AS "NumberStoreNew",
		IFNULL(NULLIF(replace(trim(to_nvarchar(:NumberVisit, replicate(' 999', ceil(length(floor(:NumberVisit)) / 3)))), '.', ','), ''), '0') AS "NumberVisit",
		IFNULL(NULLIF(replace(trim(to_nvarchar(:NumberVisitHaveOrder, replicate(' 999', ceil(length(floor(:NumberVisitHaveOrder)) / 3)))), '.', ','), ''), '0') AS "NumberVisitHaveOrder",
		IFNULL(NULLIF(replace(trim(to_nvarchar(:NumberOrder, replicate(' 999', ceil(length(floor(:NumberOrder)) / 3)))), '.', ','), ''), '0') AS "NumberOrder",
		IFNULL(NULLIF(replace(trim(to_nvarchar(:NumberOrderOpen, replicate(' 999', ceil(length(floor(:NumberOrderOpen)) / 3)))), '.', ','), ''), '0') AS "NumberOrderOpen",
		:type "DateType" from dummy;
		
	return :result;

END;



