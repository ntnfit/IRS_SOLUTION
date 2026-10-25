CREATE procedure ILD_R_QATruyXuatNguonGoc_ThanhPham 
(
	FromDate date, -- Start Date (ngay bắt đầu chạy lệnh sản xuất)
	ToDate date -- Start Date (ngay bắt đầu chạy lệnh sản xuất)
	--FGCode nvarchar(250) --ILD_LayItemThanhPham
	
)
as
begin
create local temporary table #productionOrder
(
	PO_Entry int
);
insert into   #productionOrder
select  "DocEntry"
from  OWOR
where "StartDate" between  Fromdate and Todate;



		Create local temporary table #tbl
		(
			POID int,
			POCode int,
			StartDate nvarchar(10),
			ItemCode nvarchar(50),
			ItemName nvarchar(100),
			UOM nvarchar(20),
			ItemGroup	nvarchar(50), --Thanhf pham or Rework, Nguyên liệu
			Level nvarchar(10), -- 1: Thanh phẩm, Rework, 2: Nguyên vật liệu
			GRCeiptID int,
			GIssueID int,
			Qty decimal(19,6),
			FGBatch nvarchar(5000),
			FGSSCC ntext,
			License nvarchar(50),
			
			Batch nvarchar(5000),
			SSCC ntext,
			
			Rate_License nvarchar(50),
			Sotokhai nvarchar(50)
		);
		
		insert into  #tbl (
										POID,POCode,StartDate,
										ItemCode,
										ItemName,
										UOM,
										ItemGroup,
										Level,
										GRCeiptID,
										Qty
										)
		select distinct
				a."DocEntry" as "POID",
				a."DocNum" as "POCode" ,
			   TO_VARCHAR(b."StartDate",'DD/MM/YYYY') as "PO_StartDate",
			   GRC."ItemCode",
			   GRC."Dscription",
			   itemTP."InvntryUom" as "DVT_TP",
			  
			   case when itemTP."ItmsGrpCod" =  120 then 'Thành phẩm' else 'Rework' end as "ItemGroup",
			   1 as "Level",
			   GRC."DocEntry" as "GRCeiptID",
			   GRC."Quantity" as "Qty"
			   
		from  OWOR a
		inner join WOR1 b on a."DocEntry"  = b."DocEntry"
		inner join #productionOrder aa on aa.PO_Entry  = a."DocEntry"
		left join IGN1 grc on  grc."BaseEntry"  = a."DocEntry" --Goods Receipt: 59
		inner join  OITM itemTP on itemTP."ItemCode" = GRC."ItemCode"
		where GRC."BaseType"  =  202  and itemTP."ItmsGrpCod"  in (120,121);
		
		--- Update: Batch: Thành Phẩm - Rework: 
		update a
		set FGBatch  =  (select  STRING_AGG(b."SuppSerial",',' ) from  OIBT b 
															 where   b."BaseType"= 59 and a.ItemCode = b."ItemCode" 
																					and a.GRCeiptID = b."BaseEntry" 
		   													group by b."ItemCode", b."BaseEntry" )
		from #tbl a;
		 
		--- Update SSCC: Thành Phẩm - Rework:
		update a
		set FGSSCC  =  (select  STRING_AGG(b."BatchNum",',' ) from  OIBT b 
															 where   b."BaseType"= 59 
															  and a.ItemCode = b."ItemCode" 
																					and a.GRCeiptID = b."BaseEntry" 
		   													group by b."ItemCode", b."BaseEntry" )
		from #tbl a;
		---=============================== Nguyên vật liệu =====
		insert into  #tbl (
										POID,
										POCode,
										StartDate,
										ItemCode,
										ItemName,
										UOM,
										ItemGroup,
										Level,-- Tự quy định: 2 (cấp con)
										GIssueID, -- Lấy từ Goods issue : 60
										Qty,
										License,
										Sotokhai
										)
		select distinct
				a."DocEntry" as "POID",
				a."DocNum" as "POCode" ,
				TO_VARCHAR(b."StartDate",'DD/MM/YYYY') as "PO_StartDate",
				c."ItemCode",
			    c."Dscription",
			    itemNVL."InvntryUom" as "DVT",
			   
			   	case when itemNVL."ItmsGrpCod" =  113 then 'Nguyên liệu, vật liệu chính' else 'Nguyên liệu, vật liệu phụ' end as "ItemGroup",
			    2 as "Level",
			    c."DocEntry" as  "GIssueID",
			    c."Quantity" as "Qty",
			    d."U_License",
			    d."U_SoToKhai"
			   
			   
		from  OWOR a
		inner join WOR1 b on a."DocEntry"  = b."DocEntry"
		inner join #productionOrder aa on aa.PO_Entry  = a."DocEntry"
		left join IGE1 c on  c."BaseEntry"  = a."DocEntry" --Goods Issue: 60
		inner join OIGE d on d."DocEntry"  = c."DocEntry"
		inner join  OITM itemNVL on itemNVL."ItemCode" = c."ItemCode"
		where c."BaseType"  =  202  
		 and itemNVL."ItmsGrpCod"  in (113,114, 121);-- NVL chính, phụ, rework
		
		-----------------------
		--- Update: Batch: Nguyên vật liệu - Rework: 
		update a
		set Batch  =  (select  STRING_AGG(b."SuppSerial",',' ) from  OIBT b 
															 where   b."BaseType"= 60 and a.ItemCode = b."ItemCode" 
																					and a.GIssueID = b."BaseEntry" 
		   													group by b."ItemCode", b."BaseEntry" )
		from #tbl a;
		 
		--- Update SSCC:  Nguyên vật liệu - Rework: 
		update a
		set SSCC  =  (select  STRING_AGG(b."BatchNum",',' ) from  OIBT b 
															 where   b."BaseType"= 60 
															  and a.ItemCode = b."ItemCode" 
																					and a.GIssueID = b."BaseEntry" 
		   													group by b."ItemCode", b."BaseEntry" )
		from #tbl a;
		 
		---
		
select  POID "POID",
		POCode "POCode",
		StartDate "StartDate",
		ItemCode "ItemCode",
		ItemName "ItemName",
		UOM UOM,
		ItemGroup "ItemGroup",
		Level "Level",
		
		case when Level  =1 then GRCeiptID  end as "GRCeipt ID",
		case when Level  =2 then GIssueID   end as "GIssue ID",
		Qty "Qty",
		case when Level  =1 then FGBatch else Batch end as "Batch",
		case when Level  =1 then FGSSCC else SSCC end as "SSCC",
		--License "License",
		--Rate_License "Rate_License",
		Sotokhai "Sotokhai"
from #tbl a
order by POID,StartDate,Level;
		
drop table #tbl;
drop table #productionOrder;
 

end;

