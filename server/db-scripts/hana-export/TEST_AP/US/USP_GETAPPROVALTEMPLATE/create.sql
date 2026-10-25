
CREATE  PROCEDURE USP_GetApprovalTemplate(
in UserName varchar(50),
in ObjType INT,
in DocEntry INT default 0,
in ViewMode CHAR(1) default '3',
in DraftKey INT default 0)
AS
BEGIN
	DECLARE wddstatus CHAR(1) ='';
	IF :ObjType = 24 or :ObjType = 46
	then
		select ifnull((SELECT "WddStatus" FROM odrf WHERE "DocEntry"=:DraftKey),'') into wddstatus from dummy;
	END	if;
	
	create local temporary table #APZ_GetApprovalTemplate
	    ("ReqId" integer, "WtmCode" integer, "StageId" integer, "Name" nvarchar(200), "ReceiptId" integer, "ReceiptName" nvarchar(200), "Description" nvarchar(100),
	    "Remarks" nvarchar(2000), "QueryId" integer, "QueryName" nvarchar(200), "QueryString" nvarchar(5000), "Always" smallint);

	INSERT INTO #APZ_GetApprovalTemplate ("ReqId", "WtmCode", "StageId", "Name", "ReceiptId", "ReceiptName", "Description", "Remarks", "QueryId", "QueryName", "QueryString", "Always") 
	SELECT B."UserID" AS "ReqId", A."WtmCode", WST1."WstCode", A."Name", OUSR.USERID, OUSR.USER_CODE, A."Remarks", '' AS "Remarks", F."QueryId", G."QName", G."QString", CASE WHEN E."CondId" IS NULL AND F."QueryId" IS NULL THEN 1 ELSE 0 END 
	FROM OWTM A
	        INNER JOIN WTM1 B
	        LEFT JOIN OUSR U ON B."UserID" = U."USERID" ON A."WtmCode" = B."WtmCode"
	        INNER JOIN WTM2 C
	        INNER JOIN WST1
	        LEFT JOIN OUSR ON WST1."UserID" = OUSR."USERID" ON WST1."WstCode" = C."WstCode" ON A."WtmCode" = C."WtmCode"
	        INNER JOIN WTM3 D ON A."WtmCode" = D."WtmCode"
	        LEFT JOIN WTM4 E ON A."WtmCode" = E."WtmCode"
	        LEFT JOIN WTM5 F
	        LEFT JOIN OUQR G ON F."QueryId" = G."IntrnalKey" ON A."WtmCode" = F."WtmCode"
	        WHERE A."Active" = 'Y'
	            AND U."USER_CODE" = :UserName
	            AND D."TransType" = :ObjType
				AND ((:ViewMode='3' AND :wddstatus<>'Y') OR (:ViewMode='2' AND A."AppOnUpd"='Y'))
				AND C."SortId" = 1;
	
	SELECT *   FROM #APZ_GetApprovalTemplate;

END




