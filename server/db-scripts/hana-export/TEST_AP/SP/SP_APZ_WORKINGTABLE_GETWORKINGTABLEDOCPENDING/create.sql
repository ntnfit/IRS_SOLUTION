
CREATE  PROCEDURE SP_APZ_WorkingTable_GetWorkingTableDocPending(in UserCode nvarchar(50) default '')
as
BEGIN
	declare UserId int;
	select ifnull((select userid from ousr where user_code=:UserCode),0) into UserId from DUMMY;
	
	---- Khai báo bảng Structure
	CREATE LOCAL TEMPORARY TABLE #tblStructure
	(
	     "ColumnName" NVARCHAR(50) NOT NULL
	    ,"Caption" NVARCHAR(100) NULL
	    ,"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar' --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
		,"SubType" VARCHAR(20) NULL			--none phone percentage address image measurement price quantity rate sum time
	    ,"Width" INT NULL DEFAULT 75
	    ,"GroupIndex" INT NULL DEFAULT -1	-- dùng cho grid  default -1
	    ,"Visible" smallint NULL DEFAULT 1
	    ,"FormatType" NVARCHAR(20) NULL   -- numeric, datetime/ 
	    ,"DisplayFormat" NVARCHAR(20) NULL  -- '{0:n2}'
	    ,"ReadOnly" smallint NULL DEFAULT 0
	    ,"AllowEdit" smallint NULL DEFAULT 1
	    ,"AllowMove" smallint NULL DEFAULT 1
	    ,"AllowSize" smallint NULL DEFAULT 1
	    ,"AllowSort" smallint NULL DEFAULT 1
	    ,"Fixed" VARCHAR(1) NULL	-- L:Left / R:Right
		-- Summary Item
	    ,"SummaryType" VARCHAR(10) NULL	-- sum,average...
		-- Group Summary
	    ,"GroupSummaryType" VARCHAR(10) NULL	-- sum,average...
		--,Parent BIT NULL	-- dùng cho treelist
		--,[Key] BIT NULL		-- dùng cho treelist (identity)
	);
	INSERT INTO #tblStructure ("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
		SELECT N'DocType',			N'Doc Type',			'nvarchar',		110,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL FROM DUMMY
			UNION ALL
		SELECT N'RequestedUser',		N'Requested From User',	'nvarchar',		120,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL FROM DUMMY
			UNION ALL
		SELECT N'RequestDate',		N'Request Date',		'date',			75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL FROM DUMMY
			UNION ALL
		SELECT N'Status',			N'Status',				'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
			UNION ALL
		SELECT N'DocNo',				N'Doc No',				'int',			50,		-1,			1,			NULL,		NULL,			1,		 1,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
			UNION ALL
		SELECT N'PostingDate',		N'Posting Date',		'date',			75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
			UNION ALL
		SELECT N'DocDate',			N'Doc Date',			'date',			75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
			UNION ALL
		SELECT N'DueDate',			N'DueDate',				'date',			75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
			UNION ALL
		SELECT N'Remarks',			N'Remarks',				'nvarchar',		220,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
			UNION ALL
		SELECT N'WddCode',			N'WddCode',				'int',			75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
			UNION ALL
		SELECT N'ObjType',			N'ObjType',				'int',			75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY;
					
	SELECT * FROM #tblStructure;
	
	create local temporary table #tmpAlert as (SELECT T1."KeyStr", T1."Code", T2."UserSign", T2."WasRead" FROM oalr T0
													INNER JOIN alr3 T1 ON T0."Code" = T1."Code" AND T1."ObjType" = 122 INNER JOIN oaib T2 ON T2."AlertCode" = T0."Code" AND T2."UserSign" = 1 WHERE T0."Type" = 'M');
	
	select 
		case b."ObjType" 
			when 23 then 'Sales Quotation'
			when 17 then 'Sales Order'
			when 15 then 'Delivery'
			when 16 then 'Return'
			when 203 then 'A/R Down Payment'
			when 13 then 'A/R Invoice'
			when 165 then 'A/R Correction Invoice'
			when 166 then 'A/R Correction Invoice Reversal'
			when 14 then 'A/R Credit Memo'
			when 132 then 'Correction Invoice'
			when 1470000113 then 'Purchase Request'
			when 540000006 then 'Purchase Quotation'
			when 22 then 'Purchase Order'
			when 20 then 'Goods Receipt PO'
			when 21 then 'Goods Returns'
			when 204 then 'A/P Down Payment'
			when 18 then 'A/P Invoice'
			when 163 then 'A/P Correction Invoice'
			when 164 then 'A/P Correction Invoice Reversal'
			when 19 then 'A/P Credit Memo'
			when 59 then 'Goods Receipt'
			when 60 then 'Goods Issue' 
			when 1250000001 then 'Inventory Transfer Request'
			when 67 then 'Inventory Transfer'
			when 310000001 then 'Inventory Opening Balance'
			when 46 then 'Outgoing Payment'
			when 1470000065 then 'Inventory Counting'
			when 10000071 then 'Inventory Posting'
			when 112 then 'Documents - Drafts'
			when 140 then 'Payment Drafts'
			when 1470000109 then 'Inventory Counting - Drafts' 
			when 1470000136 then 'Inventory Posting - Drafts'
			when 1470000131 then 'Inventory Opening Balance - Drafts'
			else ''	end "DocType",
		b."DocEntry" "DocNo",
		c."U_NAME" as "RequestedUser",
		a."CreateDate" "RequestDate",
		case b."WddStatus"
			when 'W' then 'Pending' 
			when 'Y' then 'Approved' 
			when 'N' then 'Rejected' 
			when 'P' then 'Generated' 
			when 'A' then 'Generated by Authorizer' 
			when 'C' then 'Canceled' end "Status",
		b."DocDueDate" as "DueDate",
		b."DocDate" as "PostingDate",
		b."TaxDate" as "DocDate",
		a."Remarks" as "Remarks",
		a."WddCode" as "WddCode",
		b."ObjType" as "ObjType",
		(select max("Code") "AlertCode" From #tmpAlert where CompareSplit(a."WddCode", #tmpAlert."KeyStr", ' ') = 1) "AlertCode",
		(select "WasRead" From #tmpAlert where "Code" = (select max("Code") "AlertCode" From #tmpAlert where CompareSplit(a."WddCode", #tmpAlert."KeyStr", ' ') = 1)) "WasRead"
	from OWDD a inner join ODRF b on a."DraftEntry"=b."DocEntry"
		inner join OUSR c on a."OwnerID" = c."USERID"
	where  a."Status"='W' and a."DraftType"=112 and b.canceled='N' and b."WddStatus"<>'C' and b."DocStatus"<>'C'
		and exists(select 1 from WDD1 T3 where T3."UserID"=:UserId and T3."StepCode"=a."CurrStep" and T3."WddCode" = a."WddCode" AND T3."Status"='W');
END



