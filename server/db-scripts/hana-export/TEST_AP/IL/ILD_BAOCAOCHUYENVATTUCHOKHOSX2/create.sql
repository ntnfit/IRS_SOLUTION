CREATE procedure ILD_BaoCaoChuyenVatTuChoKhoSX2(
	in fdate		timestamp,
	in tdate		timestamp,
	in status		nvarchar(10),
	in itemcode		  nvarchar(10000)
		
)
as
begin
	
	CREATE local temporary TABLE #Batch(
		DocEntry 	int,
		ItemCode 	nvarchar(50),
		DocType		nvarchar(20),
		DistNumber	nvarchar(50),
		Quantity	numeric(21,6), Qty2	numeric(21,6),
		DocLine		int,
		ExpDate		timestamp,
		MaKe		nvarchar(50)
	);
	
	-----------------------------------
	create local temporary table #tbl(
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
	insert into #tbl (ProOrNo, Status, MaterialCode, MaterialName, PlannedQty, IssuedQty, Whse, QtyInWhse, TransferredQty,DocDate)
	select 
	a."DocNum"
	,
	case 
	when a."Status" = 'R' then 'Released' 
	when a."Status" = 'L' then 'Close' 
	when a."Status" = 'P' then 'Planned' 
	end
	,b."ItemCode",b."ItemName",a."PlannedQty",b."IssuedQty",'M',0,0,to_nvarchar(a."PostDate",'dd/mm/yyyy')
	from OWOR a
		left join WOR1 b on b."DocEntry" = a."DocEntry"
		left join OITM c on c."ItemCode" = b."ItemCode"
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
	;	
	
	update x set QtyInWhse = ifnull((select  sum("OnHand") from OITW where "ItemCode" = x.MaterialCode and "WhsCode" = 'M'),0)
	from #tbl x;
	
	update x set TransferredQty = ifnull((select  sum("Quantity") from WTR1 where "ItemCode" = x.MaterialCode and "FromWhsCod" = 'A' and "WhsCode" = 'M'),0)
	from #tbl x;
	
	---- insert #batch
	insert into #Batch (DocEntry,ItemCode,DocType,DistNumber,Quantity,DocLine,ExpDate,MaKe, Qty2)
	SELECT a."DocEntry", a."ItemCode", a."DocType", c."DistNumber", b."Quantity", a."DocLine", c."ExpDate", 
			--t5."BinCode", t4."Quantity"
			null,null
	FROM "OITL" a 
		LEFT OUTER JOIN "ITL1" b ON a."LogEntry" = b."LogEntry" 
		LEFT OUTER JOIN "OBTN" c ON (c."SysNumber" = b."SysNumber" AND b."ItemCode" = c."ItemCode") 
		
		left JOIN OBTL T4 ON a."LogEntry"   = t4."ITLEntry" and T4."SnBMDAbs" = c."AbsEntry"
		left JOIN OBIN T5 ON T5."AbsEntry" = T4."BinAbs"
	WHERE (a."DocType" = '67' AND a."DocEntry" = 37)
			AND IFNULL(b."Quantity", 0) <> 0;
	
	
	
	
	
	
	
	
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
	--select * from #tbl order by ProOrNo;
	select * from #Batch ;--order by ProOrNo;
	drop table #tbl;
	drop table #Batch;
end

