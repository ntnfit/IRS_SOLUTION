CREATE procedure ILD_R_IRS_MRPSanXuat
(
	FromDate date,
	ToDate date
)
As
Begin
	Create local temporary table #SO
	(
		ItemCode nvarchar(50),
		ItemName nvarchar(150),
		Quantity decimal(19,6)
	);
	
	insert into #SO
	select b."ItemCode",b."Dscription",
			sum("Quantity") as "SoLuongYeuCau"
	from ORDR a
	inner join RDR1 b on a."DocEntry"  = b."DocEntry"
	inner join OITM c on b."ItemCode" =  c."ItemCode"
--inner join OITB d on d."ItmsGrpCod"  = c."ItmsGrpCod"
	where "DocStatus" = 'O' and "ItmsGrpCod" = 120 and "CANCELED"  =  'N' and a."DocDate" between FromDate and ToDate
	group by b."ItemCode",b."Dscription";
--- 
	create local temporary table #tbl
	(
		ItemCode nvarchar(50),
		ItemName nvarchar(150),
		SoLuongYeuCau decimal(19,6),
		SLTonKhoTP decimal(19,6),
		SLTonKhoSX decimal(19,6),
		
		SLTrenLSX decimal(19,6),
		SoLuongKhaDung decimal(19,6),
		
		SoLuongDatThem decimal(19,6)
	);
	
	insert into  #tbl (ItemCode,ItemName,
						SoLuongYeuCau,
						SLTonKhoTP,
						SLTonKhoSX,
						
						SLTrenLSX
							)
	select a.ItemCode,a.ItemName,ifnull(a.Quantity,0),ifnull(b."SLTonKhoTP",0),ifnull(c."SLTonKhoSX",0), ifnull(d."PlannedQty",0)
	from #SO a
	left join  (select "ItemCode","OnHand" as "SLTonKhoTP" from OITW where "WhsCode" = 'C') b on b."ItemCode" = a.ItemCode
	left join  (select "ItemCode","OnHand" as "SLTonKhoSX" from OITW where "WhsCode" = 'N') c on c."ItemCode" = a.ItemCode
	left join (select "ItemCode",Sum("PlannedQty") as "PlannedQty" 
				from  OWOR 
				where "Status"  = 'R' 
				group by  "ItemCode" ) d on d."ItemCode"  = a.ItemCode;
---- So luong kha dung
	update  a
	set SoLuongKhaDung  = SLTonKhoTP + SLTonKhoSX + SLTrenLSX + SoLuongYeuCau
	from #tbl a;
	
---- So luong Dat them
	update  a
	set SoLuongDatThem  =    SoLuongYeuCau - SoLuongKhaDung
	from #tbl a;
---
	
	select CURRENT_DATE "Ngay",
		   a.ItemCode "MaHang",
		   a.ItemName "TenHang",
		   a.SoLuongYeuCau "SoLuongYeuCau",
		   a.SLTonKhoTP "SLTonKhoTP",
		   a.SLTonKhoSX "SLTonKhoSX",
		   
		   a.SLTrenLSX "SLTrenLSX",
		   a.SoLuongKhaDung "SoLuongKhaDung",
		   case when a.SoLuongDatThem <0 then 0 else  a.SoLuongDatThem end as "SoLuongDatThem" 
		   
	from #tbl a;
---
	 
	
	drop table #SO;
	drop table #tbl;
End;

