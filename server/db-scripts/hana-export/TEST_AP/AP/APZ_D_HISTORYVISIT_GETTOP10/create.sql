

CREATE PROCEDURE APZ_D_HistoryVisit_GetTop10 (IN storeCode nvarchar(100) DEFAULT '')
AS
BEGIN

	SELECT TOP 10 A.*, IFNULL((CAST((SELECT COUNT(*) FROM ORDR C WHERE a."StoreCode" = C."CardCode" AND C.U_GUID = A."Guid") AS integer) + 
	CAST((SELECT COUNT(*) FROM OINV D WHERE a."StoreCode" = D."CardCode" AND U_GUID = D.U_GUID) AS integer)), 0) AS "OrderNumber", 
	IFNULL("firstName", '') || ' ' || IFNULL("middleName", '') || ' ' || IFNULL("lastName", '') AS "U_fullName" 
	FROM APZ_D_VISIT A INNER JOIN OHEM B ON A."UserSign" = B."userId" WHERE A."StoreCode" = :storeCode ORDER BY "DocEntry" DESC;
	
END




