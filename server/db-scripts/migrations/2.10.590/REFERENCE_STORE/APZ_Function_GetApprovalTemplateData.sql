
CALL APZ_EXISTSPROC('APZ_Function_GetApprovalTemplateData');

GO


CREATE PROCEDURE APZ_Function_GetApprovalTemplateData(in userId int, status varchar(10), requestType char(1) default '', out result table
	(
		"WddCode" int, 
		"WtmCode" int,
		"WtmName" nvarchar(254),
		"CardCode" nvarchar(15),
		"CardName" nvarchar(254),
		"OwnerId" int,
		"DocNo" varchar(50),
		"ObjType" int,
		"DocType" varchar(100),
		"DocDate" datetime,
		"DocDueDate" datetime,
		"DocTotal" decimal(21,6),
		"DocCur" varchar(10),
		"IsDraft" char(1),
		"UserId" int,
		"UserCode" varchar(10),
		"UserName" nvarchar(50),
		"UpdateDate" datetime,
		"UpdateTime" int,
		"Status" char(1),
		"Remarks" nclob,
		"RequestType" char(1),
		"ItemCheck" char(5),
		"ProcesStat" char(5)
		)
	)
AS
BEGIN
	
	declare mainCur nvarchar(3);
	
	--Khai báo bảng danh sách approval lists
	create local temporary table #tblList
	(
		"WddCode" int, 
		"WtmCode" int,
		"WtmName" nvarchar(254),
		"CardCode" nvarchar(15),
		"CardName" nvarchar(254),
		"OwnerId" int,
		"DocNo" varchar(50),
		"ObjType" int,
		"DocType" varchar(100),
		"DocDate" datetime,
		"DocDueDate" datetime,
		"DocTotal" decimal(21,6),
		"DocCur" varchar(10),
		"IsDraft" char(1),
		"UserId" int,
		"UserCode" varchar(10),
		"UserName" nvarchar(50),
		"UpdateDate" datetime,
		"UpdateTime" int,
		"Status" char(1),
		"Remarks" nclob,
		"RequestType" char(1),
		"ItemCheck" char(5),
		"ProcesStat" char(5)
	);
	
	--Khai báo danh sách bảng object type
	create local temporary table #tblObjects
	(
		"ObjId" int, 
		"ObjName" nvarchar(254)
	);
	
	--Khai báo danh sách bảng object type
	insert into #tblObjects("ObjId", "ObjName") values(23, 'Sales Quotation');
	insert into #tblObjects("ObjId", "ObjName") values(17, 'Sales Order');
	insert into #tblObjects("ObjId", "ObjName") values(15, 'Delivery');
	insert into #tblObjects("ObjId", "ObjName") values(16, 'Return');
	insert into #tblObjects("ObjId", "ObjName") values(203, 'A/R Down Payment');
	insert into #tblObjects("ObjId", "ObjName") values(13, 'A/R Invoice');
	insert into #tblObjects("ObjId", "ObjName") values(165, 'A/R Correction Invoice');
	insert into #tblObjects("ObjId", "ObjName") values(166, 'A/R Correction Invoice Reversal');
	insert into #tblObjects("ObjId", "ObjName") values(14, 'A/R Credit Memo');
	insert into #tblObjects("ObjId", "ObjName") values(132, 'Correction Invoice');
	insert into #tblObjects("ObjId", "ObjName") values(1470000113, 'Purchase Request');
	insert into #tblObjects("ObjId", "ObjName") values(540000006, 'Purchase Quotation');
	insert into #tblObjects("ObjId", "ObjName") values(22, 'Purchase Order');
	insert into #tblObjects("ObjId", "ObjName") values(20, 'Goods Receipt PO');
	insert into #tblObjects("ObjId", "ObjName") values(21, 'Goods Returns');
	insert into #tblObjects("ObjId", "ObjName") values(204, 'A/P Down Payment');
	insert into #tblObjects("ObjId", "ObjName") values(18, 'A/P Invoice');
	insert into #tblObjects("ObjId", "ObjName") values(163, 'A/P Correction Invoice');
	insert into #tblObjects("ObjId", "ObjName") values(164, 'A/P Correction Invoice Reversal');
	insert into #tblObjects("ObjId", "ObjName") values(19, 'A/P Credit Memo');
	insert into #tblObjects("ObjId", "ObjName") values(59, 'Goods Receipt');
	insert into #tblObjects("ObjId", "ObjName") values(60, 'Goods Issue' );
	insert into #tblObjects("ObjId", "ObjName") values(1250000001, 'Inventory Transfer Request');
	insert into #tblObjects("ObjId", "ObjName") values(67, 'Inventory Transfer');
	insert into #tblObjects("ObjId", "ObjName") values(234000031, 'Return Request');
	insert into #tblObjects("ObjId", "ObjName") values(310000001, 'Inventory Opening Balance');
	insert into #tblObjects("ObjId", "ObjName") values(46, 'Outgoing Payment');
	insert into #tblObjects("ObjId", "ObjName") values(1470000065, 'Inventory Counting');
	insert into #tblObjects("ObjId", "ObjName") values(10000071, 'Inventory Posting');
	insert into #tblObjects("ObjId", "ObjName") values(112, 'Documents - Drafts');
	insert into #tblObjects("ObjId", "ObjName") values(140, 'Payment Drafts');
	insert into #tblObjects("ObjId", "ObjName") values(1470000109, 'Inventory Counting - Drafts');
	insert into #tblObjects("ObjId", "ObjName") values(1470000136, 'Inventory Posting - Drafts');
	insert into #tblObjects("ObjId", "ObjName") values(1470000131, 'Inventory Opening Balance - Drafts');

	select "MainCurncy" into mainCur from OADM;
	--Lấy danh sách document cần được phê duyệt theo User(My Request)
	if :requestType = 'M' then
		insert into #tblList("WddCode", "WtmCode", "WtmName", "CardCode", "CardName", "OwnerId", "DocNo", "ObjType", "DocType", "DocDate", "DocDueDate", "DocTotal", "DocCur", "IsDraft", "UserId", "UserCode", "UserName", "UpdateDate", "UpdateTime", "Status", "Remarks", "RequestType", "ItemCheck","ProcesStat")
		select distinct a."WddCode", a."WtmCode", c."Name", e."CardCode", e."CardName", a."OwnerID", a."DraftEntry", a."ObjType", IFNULL(d."ObjName", ''), a."DocDate", e."DocDueDate",
		case when e."DocCur" = :mainCur then e."DocTotal" else e."DocTotalFC" end, 
		e."DocCur", a."IsDraft", a."OwnerID", b.USER_CODE, b.U_NAME, a."CreateDate", a."CreateTime", a."Status", a."Remarks", :requestType, 'False', a."ProcesStat"
		from OWDD a
		inner join OUSR b on a."OwnerID" = b.USERID
		inner join OWTM c on a."WtmCode" = c."WtmCode"
		inner join #tblObjects d on a."ObjType" = d."ObjId"
		inner join ODRF e on a."DraftEntry" = e."DocEntry"
		where a."OwnerID" = :userId and e."DocStatus"='O' and (
			-- get all
			:status = 'All' or
			-- approval
			:status <> 'C' and (:status = 'A' or a."Status" = :status) and a."ProcesStat" in ('W', 'Y', 'N')
			or
			-- created
			:status = 'C' and a."ProcesStat" in ('A', 'P')
		);
		
		-- trường hợp phiếu đi vào nhiều quy trình duyệt
		if :status = 'A' then
			create local temporary table #tbl as (select "DocNo", min("Status") "Status" from #tblList group by "DocNo");
			update a set "Status" = b."Status" from #tblList a, #tbl b where a."DocNo" = b."DocNo";
		elseif :status = 'Y' then
			delete from #tblList where "DocNo" in (select "DraftEntry" from OWDD where "DraftType" = 112 and "Status" <> 'Y');
		end if;
		
	elseif :requestType = 'P' then
		--Lấy danh sách các document phải phê duyệt theo User(Approval Request)
		insert into #tblList("WddCode", "WtmCode", "WtmName", "CardCode", "CardName", "OwnerId", "DocNo", "ObjType", "DocType", "DocDate", "DocDueDate", "DocTotal", "DocCur", "IsDraft", "UserId", "UserCode", "UserName", "UpdateDate", "UpdateTime", "Status", "Remarks", "RequestType", "ItemCheck","ProcesStat")
		select distinct a."WddCode", a."WtmCode", e."Name", g."CardCode", g."CardName", a."OwnerID", a."DraftEntry", a."ObjType", f."ObjName", a."DocDate", g."DocDueDate",
		case when g."DocCur" = :mainCur then g."DocTotal" else g."DocTotalFC" end, g."DocCur", a."IsDraft", a."OwnerID", b.USER_CODE, b.U_NAME, a."CreateDate", a."CreateTime", a."Status", ifnull(a."Remarks", ''), :requestType, 'False', a."ProcesStat"
		from OWDD a
		inner join OUSR b on a."OwnerID" = b.USERID
		inner join WDD1 c on a."WddCode" = c."WddCode"
		--inner join OUSR d on c."UserID" = d.USERID
		inner join OWTM e on e."WtmCode" = a."WtmCode"
		inner join #tblObjects f on a."ObjType" = f."ObjId"
		inner join ODRF g on a."DraftEntry" = g."DocEntry"
		where c."UserID" = :userId and g."DocStatus"='O' and (
			-- get all
			:status = 'All' or
			-- approval
			:status <> 'C' and (:status = 'A' or c."Status" = :status) and a."ProcesStat" in ('W', 'Y', 'N') and (status <> 'W' or exists(select 1 from WST1 where "WstCode"=a."CurrStep" and "UserID"=:userID))
			or
			-- created
			:status = 'C' and a."ProcesStat" in ('A', 'P')
		);
	end if;

	--Hiển thị dữ liệu
	if :requestType = 'P'
	then
		result = select * from #tblList order by "DocDate" asc;
	elseif :requestType = 'M'
	then
		result = select * from #tblList order by "IsDraft" desc, "DocDate" asc;
	end if;
	
	drop table #tblList;
	drop table #tblObjects;
	
END