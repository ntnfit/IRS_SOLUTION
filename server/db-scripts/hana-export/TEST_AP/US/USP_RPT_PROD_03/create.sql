CREATE procedure USP_RPT_PROD_03 (
	in fdate			timestamp,
	in tdate			timestamp,
	in groupID			int,
	in warehouse		nvarchar(20),
	in showtype			char(1), -- D/W/M/Y: Daily/Weekly/Monthly/Yearly
	in branch			int
)
as
begin
	declare tmpdate timestamp;	
	-- KẾ HOẠCH & THỰC TẾ XUẤT NVL----------------------------------------------------------------------------------------------
	----------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		GrpCode			int,
		GrpName			nvarchar(100),
		ItemCode		nvarchar(50),
		ItemName		nvarchar(100),
		ItemUoM			nvarchar(50),
		TypeID			int,
		TypeNote		nvarchar(50),
		PeriodCode		nvarchar(20),
		DocDate			timestamp,
		Qty				numeric(21,6),
		Note			nvarchar(100),
		
		TotalPlan		numeric(21,6),
		TotalIssue		numeric(21,6),
		TotalOpen		numeric(21,6)
	);
	-------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tblWH (
		WHCode			nvarchar(20),
		WHName			nvarchar(100),
		WHAccount		nvarchar(20)
	);
	insert into #tblWH (WHCode,WHName,WHAccount) select "WhsCode","WhsName","BalInvntAc" from "OWHS" where (branch in (-1,0) or "BPLid"=branch) and (warehouse='' or "WhsCode"=warehouse);
	------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tblITEM (
		ItemCode		nvarchar(50),
		ItemName		nvarchar(100),
		UoM				nvarchar(50),
		InventoryAcct	nvarchar(20),
		GLMethod		char(1),
		GrpID			int,
		GrpName			nvarchar(100)
	);
	insert into #tblITEM (ItemCode,ItemName,UoM,GrpID,GrpName,GLMethod,InventoryAcct) 
	select a."ItemCode",a."ItemName",a."InvntryUom",b."ItmsGrpCod",b."ItmsGrpNam",a."GLMethod",case when a."GLMethod"='C' then b."BalInvntAc" end
	from "OITM" a, "OITB" b 
	where a."ItmsGrpCod"=b."ItmsGrpCod" and a."ItemType"='I';
	-- NOTE-------------------------------------------------------------------------------------------------------------------
	create local temporary table #tblNOTE (
		TypeID			int,
		TypeNote		nvarchar(50)
	);
	insert into #tblNOTE (TypeID,TypeNote) select 1,'1. Plan' from dummy;
	insert into #tblNOTE (TypeID,TypeNote) select 2,'2. Issued' from dummy;
	insert into #tblNOTE (TypeID,TypeNote) select 3,'3. Open' from dummy;
	-- PRODUCTION 
	create local temporary table #tblPRODUCTION (
		DocID			int,
		DueDate			timestamp
	);
	insert into #tblPRODUCTION (DocID,DueDate) select "DocEntry","DueDate" from "OWOR" where "Type"='S';
	--RAW DATA ---------------------------------------------------------------------------------------------------------------
	create local temporary table #tblDATA (
		GrpCode			int,
		GrpName			nvarchar(100),
		ItemCode		nvarchar(50),
		ItemName		nvarchar(100),
		ItemUoM			nvarchar(50),
		TypeID			int,
		TypeNote		nvarchar(50),
		PeriodCode		nvarchar(20),
		DocDate			timestamp,
		Qty				numeric(21,6),
		SignNo			int,
		Note			nvarchar(100)
	);
	-- REPORT ----------------------------------------------------------------------------------------------------------------
	-- Plan
	insert into #tblDATA (GrpCode,GrpName,ItemCode,ItemName,ItemUoM,TypeID,TypeNote,DocDate,Qty,SignNo)
	select a.GrpID,a.GrpName,a.ItemCode,a.ItemName,a.UoM,b.TypeID,b.TypeNote,d.DueDate,c."PlannedQty",1
	from #tblNOTE b
	inner join #tblITEM a on 1=1
	inner join "WOR1" c on (b.TypeID=1 and c."ItemCode"=a.ItemCode)
	inner join #tblWH w on w.WHCode=c."wareHouse"
	inner join #tblPRODUCTION d on (d.DocID=c."DocEntry" and d.DueDate between fdate and tdate);
	-- Issued
	insert into #tblDATA (GrpCode,GrpName,ItemCode,ItemName,ItemUoM,TypeID,TypeNote,DocDate,Qty,SignNo)
	select a.GrpID,a.GrpName,a.ItemCode,a.ItemName,a.UoM,b.TypeID,b.TypeNote,e."DocDate",c."Quantity",-1
	from #tblNOTE b
	inner join #tblITEM a on 1=1
	inner join "IGE1" c on (b.TypeID=2 and c."BaseType"='202' and c."ItemCode"=a.ItemCode)
	inner join "OIGE" e on (e."DocEntry"=c."DocEntry" and e."DocDate" between fdate and tdate)
	inner join #tblWH w on w.WHCode=c."WhsCode"
	inner join #tblPRODUCTION d on d.DocID=c."BaseEntry";
	-- Open
	insert into #tblDATA (GrpCode,GrpName,ItemCode,ItemName,ItemUoM,TypeID,DocDate,Qty)
	select GrpCode,GrpName,ItemCode,ItemName,ItemUoM,3,DocDate,sum(Qty*SignNo) 
	from #tblDATA
	group by GrpCode,GrpName,ItemCode,ItemName,ItemUoM,DocDate;
	
	-- REPORT
	delete from #tblITEM where ItemCode not in (select x.ItemCode from #tblDATA x);
	
	insert into #tbl (GrpCode,GrpName,ItemCode,ItemName,ItemUoM,TypeID,TypeNote,DocDate,Qty)
	select b.GrpID,b.GrpName,b.ItemCode,b.ItemName,b.UoM,a.TypeID,a.TypeNote,c.DocDate,c.Qty
	from #tblNOTE a
	left join #tblITEM b on 1=1
	left join #tblDATA c on (c.TypeID=a.TypeID and c.ItemCode=b.ItemCode);
	
	select min(DocDate) into tmpdate from #tbl where Qty<>0;
	update #tbl set DocDate = tmpdate where DocDate is null;
	update #tbl set Qty = null where Qty=0;
	
	if showtype='D' then
		update #tbl set PeriodCode = to_char(DocDate,'YYYY.MM.DD');
	end if;
	if showtype='W' then
		update #tbl set PeriodCode = ISOWEEK(DocDate) ;
						 -- CONCAT(CONCAT(WEEK(DocDate),'.'),YEAR(DocDate));
	end if;
	if showtype='M' then
		update #tbl set PeriodCode = to_char(DocDate,'YYYY.MM');
	end if;
	if showtype='Y' then
		update #tbl set PeriodCode = to_char(DocDate,'YYYY');
	end if;
	
	update x set TotalPlan = (select sum(Qty) from #tbl y where x.ItemCode = y.ItemCode and y.TypeID = 1 ),
					TotalIssue = (select sum(Qty) from #tbl y where x.ItemCode = y.ItemCode and y.TypeID = 2 ),
					TotalOpen = (select sum(Qty) from #tbl y where x.ItemCode = y.ItemCode and y.TypeID = 3 )
	from #tbl x ;
	
	select * from #tbl a order by GrpName,ItemCode,TypeID;

	drop table #tbl;
	drop table #tblWH;
	drop table #tblITEM;
	drop table #tblDATA;
	drop table #tblNOTE;
end

