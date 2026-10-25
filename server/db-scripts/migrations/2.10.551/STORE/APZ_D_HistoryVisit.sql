
CALL APZ_EXISTSPROC('APZ_D_HistoryVisit');

GO


CREATE PROCEDURE APZ_D_HistoryVisit
(
	IN fromDate nvarchar(100) DEFAULT null,
	IN toDate nvarchar(100) DEFAULT null,
	IN userSign int DEFAULT -1,
	IN pageIndex int DEFAULT 1
)
AS
BEGIN
	declare lowLimit int;
	declare upLimit int;
	
	lowLimit := 10 * :pageIndex - 10;
	upLimit = :pageIndex * 10;

	CREATE LOCAL TEMPORARY TABLE #tblLocal ("CardCode" nvarchar(15));
	exec 'insert into #tblLocal select * from "#tbl"';

	SELECT * FROM (SELECT ROW_NUMBER() OVER ( ORDER BY A."FinishedDate" DESC) AS "RowNum", A.*, 
	IFNULL(B."LinkThumbnails", IFNULL(B."LinkDownload", (SELECT TOP 1 "DefaultAvatar" FROM APZ_D_OAPC WHERE "AppName" = 'i-DMS'))) AS "LinkThumbnails", 
	IFNULL(B."LinkDownload", (SELECT TOP 1 "DefaultAvatar" FROM APZ_D_OAPC WHERE "AppName" = 'i-DMS')) AS "LinkDownload", 
	IFNULL((CAST((SELECT COUNT(*) FROM ORDR C WHERE a."StoreCode" = C."CardCode" AND C."U_GUID" = A."Guid") AS integer) + CAST((SELECT COUNT(*) 
		FROM OINV D WHERE a."StoreCode" = D."CardCode" AND A."Guid" = D."U_GUID") AS integer)), 0) AS "OrderNumber", 
	IFNULL(CAST(E."firstName" AS nvarchar(50)), '') || ' ' || IFNULL(CAST(E."middleName" AS nvarchar(50)), '') || ' ' || IFNULL(CAST(E."lastName" AS nvarchar(50)), '') AS "U_fullName", 
	CASE WHEN IFNULL((CAST((SELECT COUNT(*) FROM ORDR C WHERE a."StoreCode" = C."CardCode" AND C."U_GUID" = A."Guid") AS integer) + CAST((SELECT COUNT(*) 
		FROM OINV D WHERE a."StoreCode" = D."CardCode" AND A."Guid" = D."U_GUID") AS integer)), 0) > 0 THEN 'Y' ELSE 'N' END AS "HaveOrder" 
	FROM APZ_D_VISIT A LEFT OUTER JOIN APZ_D_UPIM B ON A."StoreCode" = B."CardCode" AND B."IsAvatar" = 'Y' INNER JOIN OHEM E ON A."UserSign" = E."userId" 
	WHERE ((ifnull(:fromDate,'') <> '' and :fromDate = :toDate AND CAST(A."FinishedDate" AS date) = :fromDate) OR ((ifnull(:fromDate,'') = '' OR CAST(A."FinishedDate" AS date) >= :fromDate) 
	AND (ifnull(:toDate,'') = '' OR CAST(A."FinishedDate" AS date) <= :toDate))) AND ((:userSign IS NULL OR :userSign = -1) OR A."UserSign" = :userSign) 
	AND ((NOT EXISTS (SELECT 1 FROM #tblLocal)) OR A."StoreCode" IN (SELECT "CardCode" FROM #tblLocal))) AS B WHERE (B."RowNum" > :lowLimit AND B."RowNum" <= :upLimit);
END