CREATE procedure USP_RPT_INVNT_02_Bkup (
	in fdate			timestamp,
	in tdate			timestamp,
	in groupID			int,
	in warehouse		nvarchar(20),
	in account			nvarchar(20),
	--in branch			int,
	in itemcode			nvarchar(5000),
	in showType			char(1), -- S: Summary; W: By Warehouse
	in cancel			int
)
as
begin
	-- TỔNG HỢP NHẬP XUẤT TỒN----------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		GrpCode			int,
		GrpName			nvarchar(100),
		ItemCode		nvarchar(50),
		ItemName		nvarchar(100),
		ItemUoM			nvarchar(50),
		Warehouse		nvarchar(20), 
		OBQty			numeric(21,6),
		OBAmt			numeric(21,6),
		INQty			numeric(21,6),
		INAmt			numeric(21,6),
		OUQty			numeric(21,6),
		OUAmt			numeric(21,6),
		EBQty			numeric(21,6),
		EBAmt			numeric(21,6),
		Note			nvarchar(100)
	);
	-------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tblWH (
		WHCode			nvarchar(20),
		WHName			nvarchar(100),
		WHAccount		nvarchar(20)
	);
	insert into #tblWH (WHCode,WHName,WHAccount) select "WhsCode","WhsName","BalInvntAc" from "OWHS" 
	where 
		
		(warehouse='' or "WhsCode"=warehouse)
		--and (branch in (-1,0) or "BPLid"=branch)	
		;
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
	from "OITM" a  
	inner join  "OITB" b  on  a."ItmsGrpCod"=b."ItmsGrpCod"   
	where  a."InvntItem"='Y'
	and (b."ItmsGrpCod"=groupID or groupID=0)
	and
			(
			IFNULL(:itemcode, '') = '' OR cast ("ItemCode" as nvarchar) IN (SELECT "DATA" FROM "SPLIT"(:itemcode, ','))
			--ifnull((select STRING_AGG( Code) from :itemcode), '') = '' OR cast (a."ItemCode" as nvarchar) IN (select Code from :itemcode)
			) ;
	--------------------------------------------------------------------------------------------------------------------------
	--RAW DATA ---------------------------------------------------------------------------------------------------------------
	create local temporary table #tblDATAAA (
		GrpCode			int,
		GrpName			nvarchar(100),
		ItemCode		nvarchar(50),
		ItemName		nvarchar(100),
		ItemUoM			nvarchar(50),
		Warehouse		nvarchar(20), 
		
		Amount			numeric(21,6),
		DocDate			timestamp,
		TransType		nvarchar(5),
		
		InQtyTotal		numeric(21,6), -- có sum đầu kỳ ở bên dưới
		InQty			numeric(21,6),
		
		OutQtyTotal		numeric(21,6),
		OutQty			numeric(21,6)
		
	);
	/* --OLD
	insert into #tblDATAAA (GrpCode,GrpName,ItemCode,ItemName,ItemUoM,Warehouse,InQtyTotal,Amount,DocDate,TransType,InQty,OutQtyTotal,OutQty)
	select c.GrpID,c.GrpName,c.ItemCode,c.ItemName,c.UoM,b.WHCode,IFNULL(a."InQty",0)-IFNULL(a."OutQty",0),a."TransValue",a."DocDate",a."TransType",a."InQty",-IFNULL(a."InQty",0)+IFNULL(a."OutQty",0),a."OutQty"
	from "OINM" a, #tblWH b, #tblITEM c, APZ_ODOC d
	where a."DocDate"<=tdate and a."Warehouse"=b.WHCode and a."ItemCode"=c.ItemCode and a."TransType" = d."ObjType"
	and a."CreatedBy" = d."DocEntry" 
	and ( :cancel = 2 or  ifnull(d."CANCELED", 'N') = 'N')  
 	and (account='' 
		or (ifnull(a."InvntAct",'')<>'' and a."InvntAct"=account) 
		or (ifnull(a."InvntAct",'')='' and c.GLMethod='C' and c.InventoryAcct=account)
		or (ifnull(a."InvntAct",'')='' and c.GLMethod='W' and b.WHAccount=account)
		);
	
	*/

	
	
	
	
	insert into #tblDATAAA (GrpCode,GrpName,ItemCode,ItemName,ItemUoM,Warehouse,InQtyTotal,Amount,DocDate,TransType,InQty,OutQtyTotal,OutQty)
	select c.GrpID,c.GrpName,c.ItemCode,c.ItemName,c.UoM,b.WHCode,IFNULL(a."InQty",0)-IFNULL(a."OutQty",0),a."TransValue",a."DocDate",a."TransType",a."InQty",-IFNULL(a."InQty",0)+IFNULL(a."OutQty",0),a."OutQty"
	from "OINM" a 
	inner join  #tblWH b on a."Warehouse"=b.WHCode
	inner join  #tblITEM c on a."ItemCode"=c.ItemCode
	inner join APZ_ODOC d on a."TransType" = d."ObjType"
	where a."DocDate"<=tdate         
	and a."CreatedBy" = d."DocEntry" 
	and ( :cancel = 2 or  ifnull(d."CANCELED", 'N') = 'N')  
 	and (account='' 
		or (ifnull(a."InvntAct",'')<>'' and a."InvntAct"=account) 
		or (ifnull(a."InvntAct",'')='' and c.GLMethod='C' and c.InventoryAcct=account)
		or (ifnull(a."InvntAct",'')='' and c.GLMethod='W' and b.WHAccount=account)
		);
	-- REPORT ----------------------------------------------------------------------------------------------------------------
	if showType='S' then
		update #tblDATAAA set Warehouse = '';
	end if;
/*
--- bo sung du lieu revaluation
 
if showType = 'W' then
	Revaluation = select   a."ItemCode",b."ItemName",c."ItmsGrpCod" as "GrpCode",c."ItmsGrpNam" as "GrpName",
							b."InvntryUom" as "ItemUoM",
							a."Warehouse",
							
					 sum(a."RevalTotal")   as  "RevalTotal"
				from OINM a
				inner join OITM b on a."ItemCode" = b."ItemCode"
				inner join OITB c on b."ItmsGrpCod"  = c."ItmsGrpCod"
				where a."TransType" = 162  
				and a."DocDate" between fdate  and  tdate 
				group by a."ItemCode",b."ItemName",c."ItmsGrpCod",c."ItmsGrpNam",b."InvntryUom",a."Warehouse";
				
	insert into  #tblDATAAA (ItemCode,ItemName,GrpCode,GrpName,ItemUoM,Warehouse,Amount)
	select *
	from :Revaluation;
	

end if;	
*/
	
	-- EB
	insert into #tbl (GrpCode,GrpName,ItemCode,ItemName,ItemUoM,Warehouse,EBQty,EBAmt)
	select GrpCode,GrpName,ItemCode,ItemName,ItemUoM,Warehouse,
		sum(InQtyTotal),sum(Amount)
	from #tblDATAAA
	 group by GrpCode,GrpName,ItemCode,ItemName,ItemUoM,Warehouse;
 
	-- OB & IN & OU
	update #tbl set OBQty = (select sum(InQtyTotal) from #tblDATAAA x where x.ItemCode=#tbl.ItemCode and x.Warehouse=#tbl.Warehouse and x.DocDate<fdate);
	update #tbl set OBAmt = (select sum(Amount) from #tblDATAAA x where x.ItemCode=#tbl.ItemCode and x.Warehouse=#tbl.Warehouse and x.DocDate<fdate);
	update #tbl set INAmt = (select sum(Amount) from #tblDATAAA x where x.ItemCode=#tbl.ItemCode and x.Warehouse=#tbl.Warehouse and x.DocDate between fdate and tdate and x.Amount>0);
	update #tbl set OUAmt = (select sum(-Amount) from #tblDATAAA x where x.ItemCode=#tbl.ItemCode and x.Warehouse=#tbl.Warehouse and x.DocDate between fdate and tdate and x.Amount<0);

	-- loai cac phieu cancel Nhap vao
	update x set INQty = (
						select sum(CASE when TransType in ('14','16','18','20','69','162') then InQtyTotal 
						else 
							CASE when TransType not in ('13','15','19','21') then InQty end 
						end
		) from #tblDATAAA where x.ItemCode = ItemCode and x.Warehouse=Warehouse and DocDate between fdate and tdate 
	) from #tbl x;
	-- loai cac phieu cancel Xuat ra
		update x set OUQty = (
						select sum(CASE when TransType in ('13','15','19','21') then OutQtyTotal 
						else 
							CASE when TransType not in ('14','16','18','20','69','162') then OutQty end 
						end
		) from #tblDATAAA where x.ItemCode = ItemCode and x.Warehouse=Warehouse and DocDate between fdate and tdate 
	) from #tbl x;
	

 
	select * from #tbl a;


	drop table #tbl;
	drop table #tblWH;
	drop table #tblITEM;
	drop table #tblDATAAA;
end

