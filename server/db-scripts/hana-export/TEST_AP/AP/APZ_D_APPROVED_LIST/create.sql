
CREATE PROCEDURE APZ_D_Approved_List(in userName nvarchar(100) default '-1')
AS
BEGIN
	
	declare userId int;
	declare tblObjectType table("ObjType" int,"Name" nvarchar(500));
	declare tbl table (
			"Name" nvarchar(500)
			,"PendingQuantity" int
			,"ApprovedQuantity" int
			,"AllQuantity" int
			,"ObjType" nvarchar(20)
		);
	declare tmpApproval table (
		"WddCode" int, 
		"WtmCode" int,
		"WtmName" nvarchar(100),
		"CardCode" nvarchar(15),
		"CardName" nvarchar(100),
		"OwnerId" int,
		"DocNo" nvarchar(50),
		"ObjType" int,
		"DocType" nvarchar(100),
		"DocDate" datetime,
		"DocDueDate" datetime,
		"DocTotal" decimal(19,6),
		"DocCur" varchar(10),
		"IsDraft" char(1),
		"UserId" int,
		"UserCode" varchar(10),
		"UserName" nvarchar(50),
		"UpdateDate" datetime,
		"UpdateTime" int,
		"Status" char(1),
		"Remarks" nclob,
		"RequestType" char(1),--dùng để phân biệt my request và approval request
		"ItemCheck" varchar(5),
		"ProcesStat" varchar(5));
	
	declare tblActiveObjectType table ("ObjType" int);
	
	select ifnull(((select top 1 USERID from OUSR where USER_CODE = :userName)),-1) into userId from DUMMY;
	
	insert into :tblObjectType
		select 23,N'Sales Quotation' from DUMMY
			UNION ALL
		select 17,N'Sales Order' from DUMMY
			UNION ALL
		select 15, N'Delivery' from DUMMY
			UNION ALL
		select 16, N'Return' from DUMMY
			UNION ALL
		select 203 , N'A/R Down Payment' from DUMMY
			UNION ALL
		select 13 , N'A/R Invoice' from DUMMY
			UNION ALL
		select 165 , N'A/R Correction Invoice' from DUMMY
			UNION ALL
		select 166 , N'A/R Correction Invoice Reversal' from DUMMY
			UNION ALL
		select 14 , N'A/R Credit Memo' from DUMMY
			UNION ALL
		select 132, N'Correction Invoice' from DUMMY
			UNION ALL
		select 1470000113 , N'Purchase Request' from DUMMY
			UNION ALL
		select 540000006 , N'Purchase Quotation' from DUMMY
			UNION ALL
		select 22 , N'Purchase Order' from DUMMY
			UNION ALL
		select 20 , N'Goods Receipt PO' from DUMMY
			UNION ALL
		select 21 , N'Goods Returns' from DUMMY
			UNION ALL
		select 204 , N'A/P Down Payment' from DUMMY
			UNION ALL
		select 18 , N'A/P Invoice' from DUMMY
			UNION ALL
		select 163 ,N'A/P Correction Invoice' from DUMMY
			UNION ALL
		select 164 , N'A/P Correction Invoice Reversal' from DUMMY
			UNION ALL
		select 19 , N'A/P Credit Memo' from DUMMY
			UNION ALL
		select 59 , N'Goods Receipt' from DUMMY
			UNION ALL
		select 60 , N'Goods Issue' from DUMMY
			UNION ALL
		select 1250000001 , N'Inventory Transfer Request' from DUMMY
			UNION ALL
		select 67 , N'Inventory Transfer' from DUMMY
			UNION ALL
		select 310000001 , N'Inventory Opening Balance' from DUMMY
			UNION ALL
		select 46 , N'Outgoing Payment' from DUMMY
			UNION ALL
		select 1470000065 , N'Inventory Counting' from DUMMY
			UNION ALL
		select 10000071 , N'Inventory Posting' from DUMMY
			UNION ALL
		select 112 , N'Documents - Drafts' from DUMMY
			UNION ALL
		select 140 , N'Payment Drafts' from DUMMY
			UNION ALL
		select 1470000109 , N'Inventory Counting - Drafts' from DUMMY
			UNION ALL
		select 1470000136 , N'Inventory Posting - Drafts' from DUMMY
			UNION ALL
		select 1470000131 , N'Inventory Opening Balance - Drafts' from DUMMY;
		
	INSERT INTO :tblActiveObjectType (SELECT DISTINCT t2."TransType" 
	FROM OWTM t0 
	    INNER JOIN WTM2 t1 ON t0."WtmCode" = t1."WtmCode" 
	    INNER JOIN WTM3 t2 ON t0."WtmCode" = t2."WtmCode" 
	    INNER JOIN WST1 t3 ON t1."WstCode" = t3."WstCode"
	WHERE t3."UserID" = :userid AND t0."Active" = 'Y');
	
	call APZ_Function_GetApprovalTemplateData(:userid, 'All', 'P', result);
	
	INSERT INTO :tmpApproval ("WddCode","WtmCode", "WtmName", "CardCode", "CardName", "OwnerId", "DocNo", "ObjType", "DocType", "DocDate", "DocDueDate", "DocTotal", "DocCur", "IsDraft", "UserId", "UserCode", "UserName", "UpdateDate", "UpdateTime", "Status", "Remarks", "RequestType", "ItemCheck","ProcesStat")
		select * from :result;
		
	insert into :tbl ("Name","PendingQuantity","ApprovedQuantity","AllQuantity","ObjType")
	select a."Name", 
	(select count(*) from :tmpApproval t0 inner join (select distinct "WddCode" from WDD1 where "UserID" = :userId and "Status" = 'W') t1 
	on t0."WddCode" = t1."WddCode"  where t0."ObjType" = a."ObjType") as "PendingQuantity",
	(select count(*) from :tmpApproval t0 inner join (select distinct wd1."WddCode" from WDD1 wd1 where wd1."UserID" = :userId 
	and 'W' not in (select "Status" from WDD1 where "WddCode" = wd1."WddCode" and "UserID" = :userId)) t1 
	on t0."WddCode" = t1."WddCode"  where t0."ObjType" = a."ObjType") as "ApprovedQuantity",
	(select count(*) from :tmpApproval t0 where t0."ObjType" = a."ObjType") as "AllQuantity", a."ObjType"
	from :tblObjectType a;

	/*INSERT INTO :tbl ("Name", "Quantity", "ObjType") (SELECT b."Name", 
    CASE 
        WHEN a."Quantity" IS NULL THEN 0 
        ELSE a."Quantity" 
    END, b."ObjType" 
	FROM :tblObjectType b
    LEFT OUTER JOIN (SELECT DISTINCT ("ObjType") AS "ObjType", COUNT("WddCode") AS "Quantity" 
    FROM :tmpApproval 
    WHERE "Status" = 'W' 
    GROUP BY "ObjType") AS a ON a."ObjType" = b."ObjType");	*/
    
	select a."Name",    
	CASE 
        WHEN a."PendingQuantity" IS NULL THEN 0 
        ELSE a."PendingQuantity" END "PendingQuantity",
    CASE 
        WHEN a."ApprovedQuantity" IS NULL THEN 0 
        ELSE a."ApprovedQuantity" END "ApprovedQuantity",
    CASE 
        WHEN a."AllQuantity" IS NULL THEN 0 
        ELSE a."AllQuantity" END "AllQuantity",   
    a."ObjType"
    from :tbl a;
		
END




