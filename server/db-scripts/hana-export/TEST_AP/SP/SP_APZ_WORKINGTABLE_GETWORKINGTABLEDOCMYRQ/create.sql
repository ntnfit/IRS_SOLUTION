
CREATE  PROCEDURE SP_APZ_WorkingTable_GetWorkingTableDocMyRQ(in UserCode nvarchar(50) default '')
as
BEGIN
	declare UserId int;
	select IFNULL((select USERID from ousr where USER_CODE = :UserCode), 0) into UserId from DUMMY;
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
			SELECT N'RequestDate',		N'Request Date',		'date',			120,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL FROM DUMMY
	        	UNION ALL
			SELECT N'Status',			N'Status',				'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL FROM DUMMY
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
			SELECT N'ObjType',			N'ObjType',				'int',			75,		-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
	        	UNION ALL
			SELECT N'Total',			N'Total',				'numeric',		100,		-1,			1,			'numeric',		'n3',			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY
	        	UNION ALL
			SELECT N'Currency',			N'Currency',			'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 NULL,	NULL,		 NULL FROM DUMMY;
					
	SELECT * FROM #tblStructure;
	
	SELECT DISTINCT CASE b."ObjType" WHEN 23 THEN 'Sales Quotation' 
									 WHEN 17 THEN 'Sales Order' 
									 WHEN 15 THEN 'Delivery' 
									 WHEN 16 THEN 'Return' 
									 WHEN 203 THEN 'A/R Down Payment' 
									 WHEN 13 THEN 'A/R Invoice' 
									 WHEN 165 THEN 'A/R Correction Invoice' 
									 WHEN 166 THEN 'A/R Correction Invoice Reversal' 
									 WHEN 14 THEN 'A/R Credit Memo' 
									 WHEN 132 THEN 'Correction Invoice' 
									 WHEN 1470000113 THEN 'Purchase Request' 
									 WHEN 540000006 THEN 'Purchase Quotation' 
									 WHEN 22 THEN 'Purchase Order' 
									 WHEN 20 THEN 'Goods Receipt PO' 
									 WHEN 21 THEN 'Goods Returns' 
									 WHEN 204 THEN 'A/P Down Payment' 
									 WHEN 18 THEN 'A/P Invoice' 
									 WHEN 163 THEN 'A/P Correction Invoice' 
									 WHEN 164 THEN 'A/P Correction Invoice Reversal' 
									 WHEN 19 THEN 'A/P Credit Memo' 
									 WHEN 59 THEN 'Goods Receipt' 
									 WHEN 60 THEN 'Goods Issue' 
									 WHEN 1250000001 THEN 'Inventory Transfer Request' 
									 WHEN 67 THEN 'Inventory Transfer' 
									 WHEN 310000001 THEN 'Inventory Opening Balance' 
									 WHEN 46 THEN 'Outgoing Payment' 
									 WHEN 1470000065 THEN 'Inventory Counting' 
									 WHEN 10000071 THEN 'Inventory Posting' 
									 WHEN 112 THEN 'Documents - Drafts' 
									 WHEN 140 THEN 'Payment Drafts' 
									 WHEN 1470000109 THEN 'Inventory Counting - Drafts' 
									 WHEN 1470000136 THEN 'Inventory Posting - Drafts' 
									 WHEN 1470000131 THEN 'Inventory Opening Balance - Drafts' ELSE '' END AS "DocType"
			, a."DraftEntry" AS "DocNo", d.USER_CODE AS "RequestedUser", a."CreateDate" AS "RequestDate"
			, CASE A."Status" WHEN '-' THEN 'Without' 
					WHEN 'W' THEN 'Pending' 
					WHEN 'Y' THEN 'Approved' 
					WHEN 'N' THEN 'Rejected' 
					WHEN 'P' THEN 'Generated'
					WHEN 'A' THEN 'Generated by Authorizer' 
					WHEN 'C' THEN 'Canceled' END AS "Status"
			, b."DocDueDate" AS "DueDate", b."DocDate" AS "PostingDate", b."TaxDate" AS "DocDate", a."Remarks" AS "Remarks", a."WddCode" AS "WddCode", b."ObjType" AS "ObjType"
			, CASE WHEN IFNULL(b."DocTotalFC", 0) > 0 THEN IFNULL(b."DocTotalFC", 0) ELSE IFNULL(b."DocTotal", 0) END AS "Total"
			, b."DocCur" AS "Currency" 
	FROM OWDD a INNER JOIN odrf b ON a."DraftEntry" = b."DocEntry" INNER JOIN WST1 c ON a."CurrStep" = c."WstCode" INNER JOIN OUSR d ON a."OwnerID" = d.USERID	
		where a."OwnerID"=:UserId and  a."DraftType"=112 and a."IsDraft" ='Y'  and b."DocStatus"<>'C' and b."WddStatus"<>'C'; -- Quang hn Yêu cầu
END



