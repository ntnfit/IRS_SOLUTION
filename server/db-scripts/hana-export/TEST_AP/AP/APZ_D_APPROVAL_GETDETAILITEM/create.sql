
CREATE PROCEDURE APZ_D_Approval_GetDetailItem
(
	IN userCode NVARCHAR(100),
	IN status varchar(10),
	IN requestType char(1) DEFAULT '',
	IN wddEntry INT DEFAULT -1
)
AS
BEGIN
	DECLARE userId INT = ifnull((SELECT TOP 1 USERID FROM OUSR WHERE USER_CODE = :userCode), -1);
	DECLARE rowIndex int = 0;
	DECLARE rowCount int = 0;
	DECLARE maxId int = 0;
	DECLARE maxDate date = null;
	DECLARE maxTime int = -1;
	DECLARE curStatus varchar(1) = '';
	DECLARE maxReq int = 0;
	DECLARE maxRejReq int = 0;
	DECLARE apprNum int = 0;
	DECLARE rejNum int = 0;

	DECLARE tblList TABLE 
	(
		"WddCode" int, 
		"WtmCode" int,
		"WtmName" nvarchar(100),
		"CardCode" nvarchar(15),
		"CardName" nvarchar(100),
		"OwnerID" int,
		"DocNo" nvarchar(50),
		"ObjType" int,
		"DocType" nvarchar(100),
		"DocDate" varchar(50),
		"DocDueDate" varchar(50),
		"DocTotal" decimal(19,6),
		"DocCur" varchar(10),
		"IsDraft" char(1),
		"UserId" int,
		"UserCode" varchar(10),
		"UserName" nvarchar(50),
		"UpdateDate" varchar(80),
		"UpdateTime" int,
		"Status" char(1),
		"Remarks" nclob,
		"RequestType" char(1),--dùng để phân biệt my request và approval request
		"ItemCheck" varchar(5),
		"OwnerName" NVARCHAR(500),
		"CurrentStatus" nvarchar(5),
		"CurrentStepRemarks" nvarchar(100),
		"CurrentStep" int
	);

	DECLARE tblWddCode Table("WddCode" int);
	
	DECLARE tblUpdate TABLE 
	(
		"WddCode" int, 
		"UserId" int,
		"UserCode" varchar(10),
		"UserName" nvarchar(50),
		"UpdateDate" varchar(50),
		"UpdateTime" int,
		"Status" char(1),
		"Remarks" nclob
	);

	DECLARE tblObjects TABLE
	(
		"ObjId" int,
		"ObjName" varchar(200)
	);

	insert into :tblObjects("ObjId", "ObjName") values(23, 'Sales Quotation');
	insert into :tblObjects("ObjId", "ObjName") values(17, 'Sales Order');
	insert into :tblObjects("ObjId", "ObjName") values(15, 'Delivery');
	insert into :tblObjects("ObjId", "ObjName") values(16, 'Return');
	insert into :tblObjects("ObjId", "ObjName") values(203, 'A/R Down Payment');
	insert into :tblObjects("ObjId", "ObjName") values(13, 'A/R Invoice');
	insert into :tblObjects("ObjId", "ObjName") values(165, 'A/R Correction Invoice');
	insert into :tblObjects("ObjId", "ObjName") values(166, 'A/R Correction Invoice Reversal');
	insert into :tblObjects("ObjId", "ObjName") values(14, 'A/R Credit Memo');
	insert into :tblObjects("ObjId", "ObjName") values(132, 'Correction Invoice');
	insert into :tblObjects("ObjId", "ObjName") values(1470000113, 'Purchase Request');
	insert into :tblObjects("ObjId", "ObjName") values(540000006, 'Purchase Quotation');
	insert into :tblObjects("ObjId", "ObjName") values(22, 'Purchase Order');
	insert into :tblObjects("ObjId", "ObjName") values(20, 'Goods Receipt PO');
	insert into :tblObjects("ObjId", "ObjName") values(21, 'Goods Returns');
	insert into :tblObjects("ObjId", "ObjName") values(204, 'A/P Down Payment');
	insert into :tblObjects("ObjId", "ObjName") values(18, 'A/P Invoice');
	insert into :tblObjects("ObjId", "ObjName") values(163, 'A/P Correction Invoice');
	insert into :tblObjects("ObjId", "ObjName") values(164, 'A/P Correction Invoice Reversal');
	insert into :tblObjects("ObjId", "ObjName") values(19, 'A/P Credit Memo');
	insert into :tblObjects("ObjId", "ObjName") values(59, 'Goods Receipt');
	insert into :tblObjects("ObjId", "ObjName") values(60, 'Goods Issue' );
	insert into :tblObjects("ObjId", "ObjName") values(1250000001, 'Inventory Transfer Request');
	insert into :tblObjects("ObjId", "ObjName") values(67, 'Inventory Transfer');
	insert into :tblObjects("ObjId", "ObjName") values(310000001, 'Inventory Opening Balance');
	insert into :tblObjects("ObjId", "ObjName") values(46, 'Outgoing Payment');
	insert into :tblObjects("ObjId", "ObjName") values(1470000065, 'Inventory Counting');
	insert into :tblObjects("ObjId", "ObjName") values(10000071, 'Inventory Posting');
	insert into :tblObjects("ObjId", "ObjName") values(112, 'Documents - Drafts');
	insert into :tblObjects("ObjId", "ObjName") values(140, 'Payment Drafts');
	insert into :tblObjects("ObjId", "ObjName") values(1470000109, 'Inventory Counting - Drafts');
	insert into :tblObjects("ObjId", "ObjName") values(1470000136, 'Inventory Posting - Drafts');
	insert into :tblObjects("ObjId", "ObjName") values(1470000131, 'Inventory Opening Balance - Drafts');

	--Lấy danh sách các document phải phê duyệt theo User (Approval Request)
	INSERT INTO :tblList ("WddCode", "WtmCode", "WtmName", "CardCode", "CardName", "OwnerID", "DocNo", "ObjType", "DocType", 
	"DocDate", "DocDueDate", "DocTotal", "DocCur", "IsDraft", "UserId", "UserCode", "UserName", "UpdateDate", "UpdateTime", 
	"Status", "Remarks", "RequestType", "ItemCheck", "OwnerName", "CurrentStatus", "CurrentStepRemarks", "CurrentStep")
	SELECT DISTINCT a."WddCode", a."WtmCode", e."Name", ifnull(g."CardCode",''), ifnulL(g."CardName",''), a."OwnerID", CAST(a."DraftEntry" AS varchar), a."ObjType", f."ObjName", 
    CAST(g."DocDate" AS varchar), CAST(g."DocDueDate" AS varchar), 
    CASE 
        WHEN IFNULL(g."DocTotalFC", 0) > 0 THEN IFNULL(g."DocTotalFC", 0) 
        ELSE IFNULL(g."DocTotal", 0) 
    END, g."DocCur", a."IsDraft", a."OwnerID", b.USER_CODE, b.U_NAME, CAST(a."CreateDate" AS varchar), 
    a."CreateTime", a."Status", IFNULL(c."Remarks", ''), :requestType, 'False', b."U_NAME", a."ProcesStat",
     --current step
    (select FIRST_VALUE("Remarks" order by "SortId" desc) "Remarks"
			from WTM2 where "WtmCode" = a."WtmCode" and "WstCode" = 
			(select FIRST_VALUE("StepCode" order by "SortId" desc) "WstCode" from WDD1 where "WddCode" = a."WddCode")) "CurrentStepRemarks",
            (select FIRST_VALUE("StepCode" order by "SortId" desc) "WstCode" from WDD1 where "WddCode" = a."WddCode") "CurrentStep"
	FROM OWDD a 
    INNER JOIN OUSR b ON a."OwnerID" = b.USERID 
    INNER JOIN WDD1 c ON a."WddCode" = c."WddCode" 
    INNER JOIN OUSR d ON c."UserID" = d.USERID 
    INNER JOIN OWTM e ON e."WtmCode" = a."WtmCode" 
    INNER JOIN :tblObjects f ON a."ObjType" = f."ObjId" 
    INNER JOIN ODRF g ON a."DraftEntry" = g."DocEntry" 
	WHERE c."UserID" = :userId AND c."Status" IN ('W','Y','N') AND (:wddEntry IS NULL OR :wddEntry = -1 OR a."WddCode" = :wddEntry);

		
	--Lấy các document đã phê duyệt mới nhất(Cập nhật user vừa phê duyệt vào document)
	INSERT INTO :tblWddCode ("WddCode") SELECT DISTINCT "WddCode" FROM :tblList WHERE "Status" <> 'C' AND "IsDraft" = 'Y';
	
	rowCount = ifnull((select count(1) from :tblList), 0);
	while :rowIndex <= :rowCount do

		DECLARE wddCode INT = ifnull((select top 1 "WddCode" from :tblWddCode), 0);
		DELETE FROM :tblUpdate;
	
		select top 1 "UpdateDate", "UpdateTime" into maxDate, maxTime default null, null from WDD1 where "WddCode" = :wddCode and "Status" in ('Y', 'N') order by "UpdateDate" desc;

		INSERT INTO :tblUpdate ("WddCode", "UserId", "UserCode", "UserName", "UpdateDate", "UpdateTime", "Status", "Remarks") 
		SELECT a."WddCode", a."UserID", b.USER_CODE, b.U_NAME, CAST(a."UpdateDate" AS varchar), a."UpdateTime", "Status", "Remarks" 
		FROM WDD1 a INNER JOIN OUSR b ON a."UserID" = b.USERID 
		WHERE "WddCode" = :wddCode AND "Status" IN ('Y','N') AND a."UpdateDate" = :maxDate AND a."UpdateTime" = :maxTime;

		if not exists(select 1 from :tblUpdate) then
			INSERT INTO :tblUpdate ("WddCode", "UserId", "UserCode", "UserName", "UpdateDate", "UpdateTime", "Status", "Remarks") 
			SELECT a."WddCode", a."OwnerID", b.USER_CODE, b.U_NAME, CAST(a."CreateDate" AS varchar), a."CreateTime", "Status", "Remarks" 
			FROM OWDD a INNER JOIN OUSR b ON a."OwnerID" = b.USERID 
			WHERE a."WddCode" = :wddCode AND a."Status" = 'W';
		end if;
		
		select "MaxReqr", "MaxRejReqr" into maxReq, maxRejReq default null, null from OWDD where "WddCode" = :wddCode;
		select count(1) into apprNum default 0 from WDD1 where "WddCode" = :wddCode and "Status" = 'Y';
		select count(1) into rejNum default 0 from WDD1 where "WddCode" = :wddCode and "Status" = 'N';
		
		if (:apprNum >= :maxReq and :rejNum > 0) or (:apprNum <= :maxReq and :rejNum > 0) then
			curStatus = 'N';
		elseif :apprNum < :maxReq then
			curStatus = 'W';
		else
			curStatus = '';
		end if;
		
		UPDATE a SET a."WddCode" = b."WddCode", a."UserId" = b."UserId", a."UserCode" = b."UserCode", a."UserName" = b."UserName",
		a."Status" = CASE :curStatus WHEN '' THEN a."Status" ELSE :curStatus END,
		a."UpdateDate" = b."UpdateDate", a."UpdateTime" = b."UpdateTime", a."Remarks" = b."Remarks",
		a."DocDate"= a."DocDate",a."DocDueDate"=a."DocDueDate"
		FROM :tblList a INNER JOIN :tblUpdate b ON a."WddCode" = b."WddCode";

		delete from :tblWddCode where "WddCode" = :wddCode;
		rowIndex = :rowIndex + 1;

	end while;

	--Hiển thị dữ liệu
	select * from :tblList where ifnull("Status", '') <> '' and "IsDraft" = 'Y' order by "UpdateDate" desc, "UpdateTime" desc;

END

