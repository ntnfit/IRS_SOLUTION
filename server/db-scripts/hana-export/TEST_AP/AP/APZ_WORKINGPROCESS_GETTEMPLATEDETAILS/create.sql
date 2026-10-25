

CREATE PROCEDURE APZ_WorkingProcess_GetTemplateDetails(in wddCode int)
AS
BEGIN

	declare currSortId int;
	select b."SortId" into currSortId from OWDD a join WTM2 b on a."WtmCode" = b."WtmCode" and a."CurrStep" = b."WstCode" where a."WddCode" = :wddCode;
	
	create local temporary table #tbl
	(
		"WddCode" int,
		"WtmCode" int,
		"WtmName" nvarchar(20),
		"DocNo" int,
		"DocType" nvarchar(50),
		"UserId" int,
		"UserName" nvarchar(155),
		"CreateDate" nvarchar(10),
		"CreateTime" smallint,
		"Remarks" nvarchar(100),
		"WstCode" int,
		"WstName" nvarchar(30),
		"AuthId" int,
		"AuthName" nvarchar(155),
		"StatusCode" varchar(1),
		"Status" nvarchar(10),
		"UpdateDate" nvarchar(10),
		"UpdateTime" smallint,
		"AuthRemarks" nvarchar(100),
		"Total" decimal(19,6),
		"Currency" nvarchar(3),
		"SortId" smallint,
		"IsDraft" varchar(1)
	);
	
	insert into #tbl ("WddCode", "WtmCode", "WtmName", "DocNo", "DocType", "UserId", "UserName", "CreateDate", "CreateTime",
	"Remarks", "WstCode", "WstName", "AuthId", "AuthName", "StatusCode", "Status", "Total", "Currency", "SortId", "IsDraft")
	select a."WddCode", a."WtmCode", b."Name", a."DraftEntry",
	case a."ObjType" 
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
		else '' end "DocType", 
	a."OwnerID", d.U_NAME, 
	TO_VARCHAR(TO_DATE(a."CreateDate"), 'dd.MM.yyyy'), a."CreateTime", b."Remarks", 
	e."WstCode", e."SortId" || '. ' || f."Name", g."UserID", h.U_NAME, case when e."SortId" <= :currSortId then 'W' else 'S' end, 'Pending',
	case when IFNULL(l."DocTotalFC", 0) > 0 
		then IFNULL(l."DocTotalFC", 0) 
		else IFNULL(l."DocTotal", 0) end, 
	l."DocCur", e."SortId", a."IsDraft"
	from OWDD a 
	inner join OWTM b on a."WtmCode" = b."WtmCode"
	inner join OUSR d on a."OwnerID" = d.USERID
	inner join WTM2 e on a."WtmCode" = e."WtmCode"
	inner join OWST f on f."WstCode" = e."WstCode"
	inner join WST1 g on f."WstCode" = g."WstCode" 
	inner join OUSR h on g."UserID" = h.USERID
	inner join ODRF l on a."DraftEntry" = l."DocEntry"
	where a."WddCode" = :wddCode;
	
	
	update a set "StatusCode" = b."Status", "Status" = case when b."Status"  = 'W' then 'Pending' 
															 when b."Status"  = 'Y' then 'Approved' 
															 when b."Status"  = 'N' then 'Rejected'
														end,
				"UpdateDate" = TO_VARCHAR(TO_DATE(b."UpdateDate"), 'dd.MM.yyyy'), "UpdateTime" = b."UpdateTime", "AuthRemarks" = b."Remarks"
	from #tbl a, WDD1 b 
	where a."WddCode" = b."WddCode" and a."WstCode" = b."StepCode" and a."AuthId" = b."UserID";
	
	select * from #tbl;
	
END




