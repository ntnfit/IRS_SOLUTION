CREATE procedure USP_RPT_INVNT_05 (
	in fdate			timestamp,
	in tdate			timestamp,
	in groupID			int,
	in warehouse		nvarchar(20),
	in procType			int,
	in itemcode			nvarchar(5000)
)
as
begin
	
	-- TỔNG HỢP KHO THEO NGHIỆP VỤ----------------------------------------------------------------------------------------------
	declare DocType nvarchar(20);
	declare AppObject int;
	declare TransferType char(1);
	
	select DocType,AppObj,Transfer into DocType,AppObject,TransferType from APZ_OBJECT_TYPE where ID=procType;
	----------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		GrpCode			int,
		GrpName			nvarchar(100),
		ItemCode		nvarchar(50),
		ItemName		nvarchar(100),
		ItemUoM			nvarchar(50),
		Qty				numeric(21,6),
		Amount			numeric(21,6),
		Note			nvarchar(100)
	);
	-------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tblWH (
		WHCode			nvarchar(20),
		WHName			nvarchar(100),
		WHAccount		nvarchar(20)
	);
	insert into #tblWH (WHCode,WHName,WHAccount) select "WhsCode","WhsName","BalInvntAc" from "OWHS" where 
	--(branch in (-1,0) or "BPLid"=branch) and 
	(warehouse='' or "WhsCode"=warehouse);
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
	where a."ItmsGrpCod"=b."ItmsGrpCod" and a."InvntItem"='Y'
	and (b."ItmsGrpCod"=groupID or groupID=0)
	and
			(
			IFNULL(:itemcode, '') = '' OR cast ("ItemCode" as nvarchar) IN (SELECT "DATA" FROM "SPLIT"(:itemcode, ','))
			--ifnull((select STRING_AGG( Code) from :itemcode), '') = '' OR cast (a."ItemCode" as nvarchar) IN (select Code from :itemcode)
			) 
	
	;
	
	-----------------------------

	create local temporary table #Cancel
	(
		DocEntry	nvarchar(30)
	);
	insert into #Cancel 
			  select "DocEntry" || ',' || "ObjType" from OINV where "CANCELED" in ('C','Y') and "DocDate" between :fdate and :tdate
	union all select "DocEntry" || ',' || "ObjType" from ORIN where "CANCELED" in ('C','Y') and "DocDate" between :fdate and :tdate 
	union all select "DocEntry" || ',' || "ObjType" from ODLN where "CANCELED" in ('C','Y') and "DocDate" between :fdate and :tdate
	union all select "DocEntry" || ',' || "ObjType" from ORDN where "CANCELED" in ('C','Y') and "DocDate" between :fdate and :tdate
	union all select "DocEntry" || ',' || "ObjType" from OPCH where "CANCELED" in ('C','Y') and "DocDate" between :fdate and :tdate
	union all select "DocEntry" || ',' || "ObjType" from ORPC where "CANCELED" in ('C','Y') and "DocDate" between :fdate and :tdate
	union all select "DocEntry" || ',' || "ObjType" from OPDN where "CANCELED" in ('C','Y') and "DocDate" between :fdate and :tdate
	union all select "DocEntry" || ',' || "ObjType" from ORPD where "CANCELED" in ('C','Y') and "DocDate" between :fdate and :tdate
	union all select "DocEntry" || ',' || "ObjType" from OIGN where "CANCELED" in ('C','Y') and "DocDate" between :fdate and :tdate
	union all select "DocEntry" || ',' || "ObjType" from OIGE where "CANCELED" in ('C','Y') and "DocDate" between :fdate and :tdate
	union all select "DocEntry" || ',' || "ObjType" from OWTR where "CANCELED" in ('C','Y') and "DocDate" between :fdate and :tdate
	;

	-----------------------------
	--RAW DATA ---------------------------------------------------------------------------------------------------------------
	create local temporary table #tblDATA (
		GrpCode			int,
		GrpName			nvarchar(100),
		ItemCode		nvarchar(50),
		ItemName		nvarchar(100),
		ItemUoM			nvarchar(50),
		DocType			nvarchar(20),
		Qty				numeric(21,6),
		Amount			numeric(21,6)
	);
	insert into #tblDATA (GrpCode,GrpName,ItemCode,ItemName,ItemUoM,DocType,Qty,Amount)
	select c.GrpID,c.GrpName,c.ItemCode,c.ItemName,c.UoM,a."TransType",a."InQty"-a."OutQty",a."TransValue"
	from "OINM" a, #tblWH b, #tblITEM c
	where a."DocDate" between fdate and tdate and a."Warehouse"=b.WHCode and a."ItemCode"=c.ItemCode
	and a."TransType"=DocType --and (a."ApplObj"=AppObject )
--	and case when TransferType='I' then a."InQty" end > 0
--	and case when TransferType='O' then a."OutQty" end > 0
	--and ((a."CreatedBy" || ',' || a."TransType") not in (select DocEntry from #Cancel))
	;
	if TransferType='I' then
		delete from #tblDATA where Qty<0;
	end if;
	if TransferType='O' then
		delete from #tblDATA where Qty>0;
	end if;
	-- REPORT ----------------------------------------------------------------------------------------------------------------
	insert into #tbl (GrpCode,GrpName,ItemCode,ItemName,ItemUoM,Qty,Amount)
	select GrpCode,GrpName,ItemCode,ItemName,ItemUoM,abs(sum(Qty)), abs(sum(Amount))
	from #tblDATA
	group by GrpCode,GrpName,ItemCode,ItemName,ItemUoM;
	-----------
	
	select * from #tbl a order by ItemCode;

	drop table #tbl;
	drop table #tblWH;
	drop table #tblITEM;
	drop table #Cancel;
end

