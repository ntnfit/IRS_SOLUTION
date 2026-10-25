CREATE procedure USP_RPT_INVNT_01_Bkup (
	in tdate			timestamp,
	in warehouse		nvarchar(20),
	in itemcode			nvarchar(5000)
)
as
begin
	
	create local temporary table #tbl (
		GrpCode			int,
		GrpName			nvarchar(100),
		ItemCode		nvarchar(50),
		ItemName		nvarchar(100),
		ItemUoM			nvarchar(50),
		Warehouse		nvarchar(20), 
		Qty				numeric(21,6),
		Amount			numeric(21,6),
		Note			nvarchar(100)
	);
	-------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tblWH (
		WHCode			nvarchar(20),
		WHName			nvarchar(100)
	);
	insert into #tblWH (WHCode,WHName) select "WhsCode","WhsName" from "OWHS" where
	 --(branch in (-1,0) or "BPLid"=branch) and 
	 (warehouse='' or "WhsCode"=warehouse);
	------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tblITEM (
		ItemCode		nvarchar(50),
		ItemName		nvarchar(100),
		UoM				nvarchar(50),
		GrpID			int,
		GrpName			nvarchar(100)
	);
	insert into #tblITEM (ItemCode,ItemName,UoM,GrpID,GrpName) select a."ItemCode",a."ItemName",a."InvntryUom",b."ItmsGrpCod",b."ItmsGrpNam" from "OITM" a, "OITB" b
	 where a."ItmsGrpCod"=b."ItmsGrpCod" and a."InvntItem"='Y'
	 and
			(
			IFNULL(:itemcode, '') = '' OR cast ("ItemCode" as nvarchar) IN (SELECT "DATA" FROM "SPLIT"(:itemcode, ','))
			--ifnull((select STRING_AGG( Code) from :itemcode), '') = '' OR cast (a."ItemCode" as nvarchar) IN (select Code from :itemcode)
			)
	 
	 
	 ;
	 
	 
	 
	--------------------------------------------------------------------------------------------------------------------------
	insert into #tbl (GrpCode,GrpName,ItemCode,ItemName,ItemUoM,Warehouse,Qty,Amount)
	select c.GrpID,c.GrpName,c.ItemCode,c.ItemName,c.UoM,b.WHCode,sum(a."InQty"-a."OutQty"),sum(a."TransValue") 
	from "OINM" a, #tblWH b, #tblITEM c
	where a."DocDate"<=tdate and a."Warehouse"=b.WHCode and a."ItemCode"=c.ItemCode
	group by c.GrpID,c.GrpName,c.ItemCode,c.ItemName,c.UoM,b.WHCode
	having sum(a."InQty"-a."OutQty")<>0 or sum(a."TransValue")<>0;


	select * from #tbl a;

	drop table #tbl;
	drop table #tblWH;
	drop table #tblITEM;
end
