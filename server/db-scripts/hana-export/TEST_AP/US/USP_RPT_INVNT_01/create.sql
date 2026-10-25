CREATE procedure USP_RPT_INVNT_01 (
	in tdate			timestamp,
	in warehouse		nvarchar(20),
	in itemcode			nvarchar(5000)
)
as
begin
--- FGIT
	declare pwhfgit 		nvarchar(20);
	DECLARE FgitTemp TABLE (
			DO_ID			int,
			DO_NO			int,
			ItemCode		nvarchar(50),
			ItemName		nvarchar(100),
			GrpCode			int,
			GrpName			nvarchar(100),	
			ItemUoM			nvarchar(50),		
			Warehouse		nvarchar(20),		
			Qty				numeric(21,6),
			Amount			numeric(21,6)
		);
	declare DOJE table (
			DO_ID			int,			
			ItemCode		nvarchar(50),	
			Amount			numeric(21,6)
	);	
	create local temporary table #tbl (
		GrpCode			int,
		GrpName			nvarchar(100),
		ItemCode		nvarchar(50),
		ItemName		nvarchar(100),
		ItemUoM			nvarchar(50),
		Warehouse		nvarchar(20), 
		Qty				numeric(21,6),
		Amount			numeric(21,6),
		Unitprice		numeric(21,6),
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

-- Thêm danh sách hàng gửi đi bán.01/2026
-- Amount = giá trị DO + cột Adj_Reversed_COGS trên báo cáo SA02, và loại trừ khi EU làm AR invoice
-- Lay du lieu phieu Delivery co U_Term = 'FOB' theo todate cua WH01, cong them gia tri AdjReversed neu da Confirm SA02
-- Chi lay delivery co ngay hoa don > ngay xem bao cao.
	select case when :warehouse is null or :warehouse = '' then 'FGIT' else :warehouse end into pwhfgit from dummy ;
	if (upper(:pwhfgit) like '%FGIT%') then -- XU LY LOC THEO WH
		insert into :FgitTemp (DO_ID,DO_NO,ItemCode,ItemName,Qty,Amount)
		select fg."DocEntry",max(fg."DocNum") "DocNum" ,fg."ItemCode",max(fg."ItemName") as "ItemName"
			,sum(fg."Quantity") as Qty, sum(ifnull(fg.amount,0)) as amount
		from (	
			select a."DocEntry",b."DocNum",a."ItemCode",a."Dscription" as "ItemName" ,a."LineNum"		
			,a."Quantity" as "Quantity", ifnull(k.ADJ_REVERSED,0) as amount
			from DLN1 a
			
			left join ODLN b on b."DocEntry" = a."DocEntry"
			
			inner join #tblITEM j on a."ItemCode"=j.ItemCode
			
			left join INV1 c on c."BaseEntry" =  a."DocEntry" and c."BaseLine" = a."LineNum" and c."BaseType" = 15
			
			left join OINV d on d."DocEntry" = c."DocEntry"	and d."CANCELED"  = 'N'	
				
			left join (select a.itemcode, MAX(a.DO_LINENUM) DO_LINENUM, a.DO_NO,a.DO_ID
							,MAX(a.whscode) "WhsCode" ,sum(A.ADJ_REVERSED_COGS) ADJ_REVERSED
						from IRSSA02 a
						
						left join OFPR p on p."Code" = a.PERIOD
						
						where p."T_RefDate" <= :tdate 
						--and a.CONFIRM_NUMBER = 1.chua su dung	
						group by a.ITEMCODE,a.DO_NO,a.DO_ID,a.PERIOD
					) k on k.DO_ID = b."DocEntry" and k.itemcode = a."ItemCode" and k.DO_LINENUM = a."LineNum"
					
			where 
				b."CANCELED"  = 'N' and b."U_Term" = 'FOB' and b."DocDate" <= :tdate 
				and :tdate < case when d."DocDate" is null then add_days(:tdate,1) else  d."DocDate" end 
		) fg 
		group by "DocEntry","ItemCode" ;
	
		update a	
		set a.GrpCode = i."ItmsGrpCod", a.GrpName = j."ItmsGrpNam",a.ItemUoM = i."SalUnitMsr",a.Warehouse = 'FGIT'	
		from :FgitTemp a
		
		left join OITM i ON i."ItemCode" = a.ItemCode
		
		left join OITB j on j."ItmsGrpCod" = i."ItmsGrpCod";
		
--- Cap nhat gia tri COST va UnitCost
		insert into :DOJE
		select  a."CreatedBy" AS "BaseEntry",a."ItemCode",-sum(a."TransValue") as "TransValue"  
		from OINM a
				
		inner join :FgitTemp b on b.DO_ID = a."CreatedBy" and b.itemcode = a."ItemCode" 
				--and a."Warehouse"=b.Warehouse
		where  
			a."DocDate"<=tdate and substring(a."CostAct",0,3) in ('632','157') and a."TransType" = 15
					
		group by   a."ItemCode",a."CreatedBy", b.DO_ID;
					
-- COST = Tong gia tri DO / (Tong so luong DO * So luong tung SSCC)				
		update a
		set a.Amount  = a.Amount + ifnull(b.Amount,0)
		from :FgitTemp a
		inner join :DOJE b on a.DO_ID =  b.DO_ID and a.ItemCode = b.ItemCode;
	
		insert into #tbl (GrpCode,GrpName,ItemCode,ItemName,ItemUoM,Warehouse,Qty,Amount)
		select GrpCode,GrpName,itemcode,ItemName,ItemUoM,Warehouse,sum(QTY) QTY,SUM(AMOUNT) AMOUNT
		from :FgitTemp dn	
		group by GrpCode,GrpName,itemcode,ItemName,ItemUoM,Warehouse;
		
	end if;
   --
	update #tbl set Unitprice  =  ifnull(amount,0)/NULLIF (Qty, NULL) where Qty != 0;
	
	--
	select * from #tbl a ORDER by GrpCode,GrpName,ItemCode,ItemName,ItemUoM,Warehouse ;

	drop table #tbl;
	drop table #tblWH;
	drop table #tblITEM;
end
