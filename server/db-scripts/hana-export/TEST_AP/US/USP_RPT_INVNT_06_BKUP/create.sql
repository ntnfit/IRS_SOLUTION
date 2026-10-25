Create procedure USP_RPT_INVNT_06_Bkup (
	in fdate			timestamp,
	in tdate			timestamp,
	in itemcode			nvarchar(5000),
	in groupID			int,
	in fromWH			nvarchar(20),
	in warehouse		nvarchar(20),
	--in branch			int,
	in showType			char(1),	-- S: Summary by Item; D: Transaction Detail
	in cancel			int
)
as
begin
	
	-- TỔNG HỢP ĐIỀU CHUYỂN KHO NỘI BỘ----------------------------------------------------------------------------------------------
	----------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		GrpCode			int,
		GrpName			nvarchar(100),
		ItemCode		nvarchar(50),
		ItemName		nvarchar(100),
		ItemUoM			nvarchar(50),
		DocType			nvarchar(20),
		DocID			int,
		DocNum			int,
		DocDate			timestamp,
		JVNumber		nvarchar(50),
		FromWH			nvarchar(20),
		ToWH			nvarchar(20),
		Qty				numeric(21,6),
		Amount			numeric(21,6)
	);
	-------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tblFromWH (
		WHCode			nvarchar(20),
		WHName			nvarchar(100),
		WHAccount		nvarchar(20)
	);
	insert into #tblFromWH (WHCode,WHName,WHAccount) select "WhsCode","WhsName","BalInvntAc" from "OWHS" 
	where --(branch in (-1,0) or "BPLid"=branch) and 
	(fromWH='' or "WhsCode"=fromWH);
	create local temporary table #tblWH (
		WHCode			nvarchar(20),
		WHName			nvarchar(100),
		WHAccount		nvarchar(20)
	);
	insert into #tblWH (WHCode,WHName,WHAccount) select "WhsCode","WhsName","BalInvntAc" from "OWHS" where /*(branch in (-1,0) or "BPLid"=branch) and*/ (warehouse='' or "WhsCode"=warehouse);
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
	--RAW DATA ---------------------------------------------------------------------------------------------------------------
	create local temporary table #tblDATA (
		GrpCode			int,
		GrpName			nvarchar(100),
		ItemCode		nvarchar(50),
		ItemName		nvarchar(100),
		ItemUoM			nvarchar(50),
		DocType			nvarchar(20),
		DocID			int,
		DocNum			int,
		DocDate			timestamp,
		JVNumber		nvarchar(50),
		FromWH			nvarchar(20),
		ToWH			nvarchar(20),
		Qty				numeric(21,6),
		Amount			numeric(21,6)
	);
	insert into #tblDATA (GrpCode,GrpName,ItemCode,ItemName,ItemUoM,DocType,DocID,DocNum,DocDate,JVNumber,FromWH,ToWH,Qty,Amount)
	select c.GrpID,c.GrpName,c.ItemCode,c.ItemName,c.UoM,a."TransType",x."DocEntry",a."BASE_REF" as "DocNum",a."DocDate",e."U_JVNumber",x."FromWhsCod",x."WhsCode",a."InQty",a."TransValue"
	from "OINM" a,"WTR1" x , #tblWH b, #tblITEM c, #tblFromWH d, "OWTR" e--, APZ_ODOC f
	where a."DocDate" between fdate and tdate 
	and a."Warehouse"=b.WHCode and d.WHCode=a."Ref2" --and e."CANCELED" = 'N'
	and a."ItemCode"=c.ItemCode 
	
	and x."DocEntry"=e."DocEntry"
	and x."ObjType"=a."TransType" and x."DocEntry"=a."CreatedBy" and (x."LineNum"=a."DocLineNum") --and x."WhsCode"=a."Warehouse"
	--and a."TransType" = f."ObjType"
	--and a."CreatedBy" = f."DocEntry" 
	--and ( :cancel = 2 or  ifnull(f."CANCELED", 'N') = 'N') 
	
	
	
	
	
	
	
	;
	-- REPORT ----------------------------------------------------------------------------------------------------------------
	-----------
	if showType='S' then
		update #tblDATA set DocID=null, DocDate=null,JVNumber='';
	end if;

	insert into #tbl (GrpCode,GrpName,ItemCode,ItemName,ItemUoM,DocType,DocID,DocNum,DocDate,JVNumber,FromWH,ToWH,Qty,Amount)
	select GrpCode,GrpName,ItemCode,ItemName,ItemUoM,DocType,DocID,DocNum,DocDate,JVNumber,FromWH,ToWH,sum(Qty),sum(Amount) 
	from #tblDATA
	group by GrpCode,GrpName,ItemCode,ItemName,ItemUoM,DocType,DocID,DocNum,DocDate,JVNumber,FromWH,ToWH;
	
	select * from #tbl a order by DocDate,DocID,ItemCode;

	drop table #tbl;
	drop table #tblWH;
	drop table #tblITEM;
end

