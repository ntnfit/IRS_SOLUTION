CREATE procedure ILD_BaoCaoChuyenVatTuChoKhoSX(
	in fdate		timestamp,
	in tdate		timestamp,
	in status		nvarchar(10),
	in itemcode		  nvarchar(10000)
		
)
as
begin
	
	
	create local temporary table #tbl(
		ProOrID				int,
		ProOrNo				int,
		DocDate				nvarchar(10),
		Status				nvarchar(100),
		MaterialCode		nvarchar(100),
		MaterialName		nvarchar(100),
		PlannedQty			int,
		IssuedQty			int,
		Whse				nvarchar(20),
		QtyInWhse			int,
		TransferredQty		int,
		TransferRequestQty	int
	
	
	);
	insert into #tbl (ProOrID,ProOrNo, Status, MaterialCode, MaterialName, PlannedQty, IssuedQty, Whse, QtyInWhse, TransferredQty,DocDate)
	select 
	a."DocEntry",
	a."DocNum",
	case 
	when a."Status" = 'R' then 'Released' 
	when a."Status" = 'L' then 'Close' 
	when a."Status" = 'P' then 'Planned' 
	end
	,b."ItemCode",b."ItemName",b."PlannedQty",b."IssuedQty",'M',0,0,to_nvarchar(a."PostDate",'dd/mm/yyyy')
	from OWOR a
		left join WOR1 b on b."DocEntry" = a."DocEntry"
		left join OITM c on c."ItemCode" = b."ItemCode" --and c."InvntItem" = 'Y'
		--left join ORSC d on d.
	where a."PostDate" >= :fdate and a."PostDate" <= :tdate 
	and 
	(
	IFNULL(:status, '') = '' OR cast (a."Status"  as nvarchar) in (SELECT "DATA" FROM "SPLIT"(:status, ','))
	)
	and 
	(
	IFNULL(:itemcode, '') = '' OR cast (a."ItemCode" as nvarchar) IN (SELECT "DATA" FROM "SPLIT"(:itemcode, ','))
	--ifnull((select STRING_AGG( Code) from :itemcode), '') = '' OR cast (a."ItemCode" as nvarchar) IN (select Code from :itemcode)
	)
	and( not exists (select "VisResCode" from ORSC where b."ItemCode" = "VisResCode"))
	and c."InvntItem" = 'Y' 
	
	--Resource
	;	
	--select top 1 "InvntItem",* from OITM where "ItemCode" = '1520100002'
	--select top 10 * from WOR1 where "DocEntry" = 36
	--select * from OWOR where "DocNum" = '6'
	update x set QtyInWhse = ifnull((select  sum("OnHand") from OITW where "ItemCode" = x.MaterialCode and "WhsCode" = 'M'),0)
	from #tbl x;
	
	update x set TransferredQty = ifnull((select  sum("Quantity") from WTR1 a inner join OWTR b on a."DocEntry"  = b."DocEntry" 
													where a."ItemCode" = x.MaterialCode and a."FromWhsCod" = 'A' 
														and a."WhsCode" = 'M' and ifnull("U_ProductionEntry",'') <> ''),0)
	from #tbl x;
	--select * from WOR1 where "DocEntry" = 38
	--select top 10 * from OINM  
	--select  sum("OnHand") from OITW where "ItemCode" = '1520100002'
	--select * from
	--PlannedQty - QtyInWhse - TransferredQty - IssuedQty 
	--select  top 1 * from WTR1
	
	update x set TransferRequestQty = 
									Case when PlannedQty - QtyInWhse - TransferredQty - IssuedQty < 0 then 0
									else PlannedQty - QtyInWhse - TransferredQty - IssuedQty end
	from #tbl x;
	

	
	--delete from #tbl where to_nvarchar(DocDate,'dd/mm/yyyy') < to_nvarchar(:fdate,'dd/mm/yyyy') and to_nvarchar(DocDate,'dd/mm/yyyy') > to_nvarchar(:tdate,'dd/mm/yyyy');
	select * from #tbl order by ProOrNo;
	drop table #tbl;
end

