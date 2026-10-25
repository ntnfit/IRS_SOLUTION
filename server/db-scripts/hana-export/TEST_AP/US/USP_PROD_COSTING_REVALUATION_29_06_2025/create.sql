Create procedure USP_PROD_COSTING_REVALUATION_29_06_2025(
	in period 	nvarchar(20), 
	in PrdLine 	nvarchar(1000), 
	in branch 	int
)
as
begin
	declare strNote nvarchar(100);
	declare fdate, tdate date;
	--
	declare ChenhLech_AllocToStock table
	(
			ItmCode nvarchar(50),
			RevAmount numeric(28,10),
			
			Diff numeric(28,10)
	);
	--
	select TO_NVARCHAR("T_RefDate",'MM/yyyy') into strNote from "OFPR" where "Code"=period;
	if (select count(1) from "OMRV" where "U_PeriodCost"=period)>0 then
		select 'Đã tính giá thành tháng ' || strNote as "Note" from dummy;
	ElseIf (select count(1) from "@COSTING_H" a ,"@COSTING_L2" b where a."DocEntry"  =b."DocEntry" and "U_Period"=period)>0 then -- lay du lieu UDO
		--================ lay du lieu tu UDO COSTING 
			--trường hợp này là do chạy costing ngoài giờ hỗ trợ, nên tạo udo này để iLD có thể chủ động copy paste dữ liệu vào 
		DELETE FROM "TBL_COSTING_REVALUATION" WHERE PerCode=period and ifnull(BrchID,0)=branch;

		insert into "TBL_COSTING_REVALUATION"(PerCode,BrchID,DocDate, ItmCode, ItmName, WHCode, WHEBQty, SalesQty, 
												AllocRate, QtyInWhse, RevAmount, IncAct, DecAct, GroupKey)
												
		Select :period,:branch,"U_DocDate", "U_ItmCode", "U_ItmName", "U_WHCode", "U_WHEBQty", "U_SalesQty", 
				"U_AllocRate", "U_QtyInWhse", "U_RevAmount", "U_IncAct", "U_DecAct", "U_GroupKey"

		from "@COSTING_H" a
		inner join "@COSTING_L2" b on a."DocEntry"  = b."DocEntry" and a."U_Period" = :period and a."Canceled" = 'N'
		order by "LineId";
		---
		select DocDate 											"DocDate",
			   ItmCode 											"ItmCode", -- Bat buoc ten nay
			   ItmName 										 	"ItemName",
			   WHCode 										 	"WhsCode",
			   
			   WHEBQty    		 		 						"End Qty",
			   SalesQty						   	 			 	"Delivery Qty",
			   cast(AllocRate  as decimal(28,2))				"Rate", --TO_CHAR(AllocRate ,'999,999,990.99')  
			   QtyInWhse										"Qty",
									 
			   cast(RevAmount 	as decimal(28,2))			   	"Amount", -- Bat buoc ten nay
			   IncAct 											 "CredAcct", 
			   DecAct 											 "DebAcct",
			   GroupKey 								     	"GroupKey"
		from TBL_COSTING_REVALUATION 
		where PerCode =:period;


	elseif (select count(1) from "OMRV" where "U_PeriodCost"=period)=0 then
		
		select "F_RefDate", "T_RefDate" into fdate,tdate from "OFPR" where "Code"=period;
	
	if (TO_DATE(tdate,'YYYY-MM-DD') > TO_DATE(current_date,'YYYY-MM-DD')) then
		tdate = current_date;
	end if;
	-- Truyền biến Fdate và Tdate vào không được --> Tạo table tạm
	 Tbl_F_Tdate = select "F_RefDate", "T_RefDate"   from "OFPR" where "Code"=period; 
	 -- Wh07 -- Lay du lieu ton kho EndQty Theo SSCC
	Call ILD_R_IRS_CHITIETXUATNHAPTONTHEOBATCH ('2020-01-01',current_date,'','','','',result_out_wh07);
	
	create local temporary table #wh07 as ( select  sum(a."EBQTY") as "EBQTY" , a."ITEMCODE", a."WHSCODE"
													from :result_out_wh07 a
													inner join OITM b on a."ITEMCODE"  = b."ItemCode" and b."ItmsGrpCod"  IN (120,121,127)
													where   a."ADMISSIONDATE_FATHER1"   between (select "F_RefDate" from :Tbl_F_Tdate)
																						and (select "T_RefDate" from :Tbl_F_Tdate)		 
													group by a."ITEMCODE", a."WHSCODE"
													Having sum(a."EBQTY")>0 );
	
	create local temporary table #tbl (
			STT				int,
			RowNum			int,
			DocDate 		date, 
			ItmCode 		nvarchar(50), 
			ItmName 		nvarchar(100), 
			WHCode 			nvarchar(10), 
			SalesQty		numeric(28,10),
			AllocRate		numeric(28,10),
			
			EBQty			numeric(28,10), -- Tổng tồn tất cả kho, tính từ Costing
			WHEBQty			numeric(28,10),
			WHEBAmt			numeric(28,10),
			
			Qty 			numeric(28,10), 
			RevQty 			numeric(28,10), 
			RevAmount 		numeric(28,10), 
			IncAct 			nvarchar(20), 
			DecAct 			nvarchar(20), 
			
			ALLOCTOSTOCK 	numeric(28,10),
			ALLOCTOCOGS 	numeric(28,10),
			ALLOCTOCOGS_6329 numeric(28,10),
			
			Allocate_Sale_Sample  	numeric(28,10),
			Allocate_Test_Sample 	numeric(28,10),
			
			Project 		nvarchar(20), 
			OcrCode 		nvarchar(20), 
			OcrCode2 		nvarchar(20), 
			OcrCode3 		nvarchar(20), 
			OcrCode4 		nvarchar(20), 
			OcrCode5 		nvarchar(20), 
			GroupKey 		nvarchar(10) default '1',
			QtyInWhse 		decimal(19,6)
		);

		--
		Costing_Total = select FGCode,FGName,FGCmpltQty,TotalDiff,ALLOCTOSTOCK,ALLOCTOCOGS,ALLOCTOCOGS_6329,PRDTYPE,EBQty, SalesQty, 
								 Allocate_Sale_Sample,
								Allocate_Test_Sample 		 
						from TBL_COSTING_TOTAL
						where PERCODE  = :period;
						
		--- Insert dữ liệu tồn kho từ wh07 và Tbl Costing
		insert into #tbl (STT,RowNum,DocDate,ItmCode,ItmName,WHCode,EBQty,WHEBQty,Qty,
								RevAmount,IncAct,DecAct,
								ALLOCTOSTOCK , ALLOCTOCOGS,ALLOCTOCOGS_6329,
								
								Allocate_Sale_Sample, Allocate_Test_Sample )
		select ROW_NUMBER() OVER () AS STT,1,tdate,a.FGCode,a.FGName, b."WHSCODE",a."EBQTY" as "EBTotal", b."EBQTY",1 as "Qty",
						a.ALLOCTOCOGS as  "RevAmount",'1548','1548',
						
						a.ALLOCTOSTOCK,
						a.ALLOCTOCOGS,
						a.ALLOCTOCOGS_6329,
						
						a.Allocate_Sale_Sample,
						a.Allocate_Test_Sample
						
		
		from :Costing_Total a
		inner join #wh07 b on a.FGCode = b."ITEMCODE";
		
		--Phân bổ chênh lệch sang giá vốn 632 theo mã hàng: Lấy Alloc COGS <> 0, kho ảo: COGSFG
		insert into #tbl (RowNum,DocDate,ItmCode,ItmName,WHCode, SalesQty,Qty,RevAmount,IncAct,DecAct)
		select 0,tdate,FGCode,FGName,'COGSFG',SalesQty,1 as "Qty",
						round (ALLOCTOCOGS, 2) as "RevAmount", '1548','1548'
		from :Costing_Total
		where ALLOCTOCOGS <> 0  ;
		--Phân bổ chênh lệch sang giá vốn 632 theo mã hàng: ALLOCTOCOGS_6329 <> 0, kho ảo: COGSFG
		insert into #tbl (RowNum,DocDate,ItmCode,ItmName,WHCode, SalesQty,Qty,RevAmount,IncAct,DecAct)
		select 0,tdate,FGCode,FGName,'COGS_WIP',SalesQty,1 as "Qty",
						round (ALLOCTOCOGS_6329, 2) as "RevAmount", '1548','1548'
		from :Costing_Total
		where ALLOCTOCOGS_6329 <> 0  ;
		
		
		-- Phân bổ chênh lệch sang giá vốn 632 theo mã hàng: Lấy Allocate_Sale_Sample <> 0, kho ảo: S1
		insert into #tbl (RowNum,DocDate,ItmCode,ItmName,WHCode, SalesQty,Qty,RevAmount,IncAct,DecAct)
		select 0,tdate,FGCode,FGName,'S1',SalesQty,1 as "Qty",
						round (Allocate_Sale_Sample, 2) as "RevAmount", '1548','1548'
		from :Costing_Total
		where Allocate_Sale_Sample <> 0;
		--Phân bổ chênh lệch sang giá vốn 632 theo mã hàng: Lấy Allocate_Test_Sample <> 0, kho ảo: S2
		insert into #tbl (RowNum,DocDate,ItmCode,ItmName,WHCode, SalesQty,Qty,RevAmount,IncAct,DecAct)
		select 0,tdate,FGCode,FGName,'S2',SalesQty,1 as "Qty",
						round (Allocate_Test_Sample, 2) as "RevAmount", '1548','1548'
		from :Costing_Total
		where Allocate_Test_Sample <> 0;
		
		
		-- update rate : Tỷ lệ = End Qty từng kho/ Tổng Qty của Item
		update #tbl
		set AllocRate  =  WHEBQty/ (EBQty)
		where EBQty <> 0;
--===================Xử lý chênh lệch số lẻ rate (AllocRate)
		create local temporary table #RateDiff
		(
			ItemCode varchar(50),
			AllocRate	numeric(28,10),
			Diff        numeric(28,10)
			
		);
		Insert into #RateDiff (ItemCode, AllocRate,Diff)
 		select  a.ItmCode,
  
   				TO_CHAR((select  sum(c.AllocRate)    from #tbl c where c.ItmCode = a.ItmCode  )  ,'999,999,990.99'),
 				1- ( TO_DECIMAL(TO_CHAR((select  sum(c.AllocRate)    from #tbl c where c.ItmCode = a.ItmCode),'999,999,990.99'))  ) 
		from  #tbl a
		where a.AllocRate  = (select max(b.AllocRate) from #tbl b where a.ItmCode = b.ItmCode); 
		
	if exists (select 1 from #RateDiff) then

		update a
		set a.AllocRate = a.AllocRate  +  b.Diff
		from  #tbl a
		inner join  #RateDiff b on a.ItmCode = b.ItemCode
		where a.AllocRate || a.STT  = (select max(c.AllocRate)|| max(c.STT) from #tbl c where a.ItmCode = c.ItmCode);
	end if;
-------------------------------------------------------
		
		-- update RevAmount
		update #tbl 
		set  RevAmount  =   round(ALLOCTOSTOCK  *  AllocRate,2)
		where WHCode  not in ( 'COGSFG', 'COGS_WIP','S1','S2');
		
--===================Xử lý chênh lệch số lẻ do phân bổ
		--- Rev Amount(Allocate Amount)
		update a
		set RevAmount  = a.RevAmount  + ifnull(((select b.ALLOCTOSTOCK  from :Costing_Total b where a.ItmCode  = b.FGCode  ) 
									   
											- (select sum(c.RevAmount) from  #tbl c where c.ItmCode = a.ItmCode and c.WHCode not in ( 'COGSFG','COGS_WIP','S1','S2'))),0)
		from  #tbl a
		where  a.WHCode not in ( 'COGSFG','COGS_WIP','S1','S2')  
				and cast(a.RevAmount||a.STT as nvarchar)  =  (select cast( max(d.RevAmount)||max(d.STT)  as nvarchar)from  #tbl d where a.ItmCode  = d.ItmCode and d.WHCode not in ( 'COGSFG','COGS_WIP','S1','S2'));
				
		---  End Qty
		update a
		set EBQty  = a.EBQty  + ifnull(((select b.EBQty  from :Costing_Total b where a.ItmCode  = b.FGCode  ) 
											- (select sum(c.EBQty) from  #tbl c where c.ItmCode = a.ItmCode and c.WHCode not in ( 'COGSFG','COGS_WIP','S1','S2'))),0)
		from  #tbl a
		where  a.WHCode not in ( 'COGSFG','COGS_WIP','S1','S2')   
				and cast(a.EBQty||a.STT as nvarchar) =  (select cast(max(d.EBQty)|| max (d.STT) as nvarchar) from  #tbl d where a.ItmCode  = d.ItmCode and d.WHCode not in ( 'COGSFG','COGS_WIP','S1','S2') );


		--
		insert into  :ChenhLech_AllocToStock (ItmCode, RevAmount)
		select ItmCode, sum(RevAmount) 
		from  #tbl a
		where  WHCode  not in ('S1','S2','COGSFG','COGS_WIP') 
		group by ItmCode;
		--
		update a
		set a.diff  = CASE  WHEN b.ALLOCTOSTOCK > 0 AND a.RevAmount < 0 THEN (b.ALLOCTOSTOCK + ABS(a.RevAmount))
        					ELSE (b.ALLOCTOSTOCK - a.RevAmount) END 
		from  :ChenhLech_AllocToStock a
		inner join  (select sum(ALLOCTOSTOCK) ALLOCTOSTOCK,FGCode 
							from :Costing_Total  group by FGCode) b on a.ItmCode  = b.FGCode;
		
		--
		update a
		set a.RevAmount  = a.RevAmount + b.diff
		from #tbl a
		inner join  :ChenhLech_AllocToStock b on a.ItmCode = b.ItmCode
		where b.diff <> 0 and
				a.WHCode not in ( 'COGSFG','COGS_WIP','S1','S2')  
				and a.STT   = (select max(d.STT)  
																from  #tbl d 
																where a.ItmCode  = d.ItmCode 
																and d.WHCode not in ( 'COGSFG','COGS_WIP','S1','S2'));
		-- CAN KIEM TRA LAI
	/*
		update a
		set  RevAmount  = RevAmount  +  (- 0.01)
		from #tbl a
		
		where ItmCode = '12000062'  
			and  a.STT  =  (select max(d.STT) from  #tbl d where a.ItmCode  = d.ItmCode 
											and d.WHCode not in ( 'COGSFG','COGS_WIP','S1','S2')  );
	*/
		--LƯU LẠI KẾT QUẢ VÀ PHÂN BỔ
		DELETE FROM "TBL_COSTING_REVALUATION" WHERE PerCode=period and ifnull(BrchID,0)=branch;
		-- Update  QtyInWhse
		QtyInWhse = select "ItemCode","WhsCode", sum("OnHandQty") as "OnHandQty"
					 from   OBBQ 
					 where "OnHandQty" >0
					 group by "ItemCode","WhsCode";
		
		update a
		set QtyInWhse  = b."OnHandQty"
		from #tbl a
		inner join  :QtyInWhse b on a.ItmCode  = b."ItemCode" and a.WHCode  = b."WhsCode";
		---
		insert into "TBL_COSTING_REVALUATION"(PerCode,BrchID,ItmCode,ItmName,WHCode,WHEBQty,RevAmount,IncAct,DecAct,
												AllocRate,
												GroupKey, QtyInWhse)
		select period,branch,ItmCode,ItmName,WHCode,WHEBQty,RevAmount,IncAct,DecAct,
			   AllocRate, 
			   GroupKey, QtyInWhse								
		
		 from #tbl;
		
		----
		update  a
		set QtyInWhse =  1
		from  #tbl a
		where ifnull(QtyInWhse,0) = 0 and WHCode  in ('S1','S2','COGSFG','COGS_WIP'); 
		---
		select DocDate 											"DocDate",
			   ItmCode 											"ItmCode", -- Bat buoc ten nay
			   ItmName 										 	"ItemName",
			   WHCode 										 	"WhsCode",
			   
			   WHEBQty    		 		 						"End Qty",
			   SalesQty						   	 			 	"Delivery Qty",
			   cast(AllocRate  as decimal(28,2))				"Rate", --TO_CHAR(AllocRate ,'999,999,990.99')  
			   QtyInWhse										"Qty",
									 
			   cast(RevAmount 	as decimal(28,2))			   	"Amount", -- Bat buoc ten nay
			   IncAct 											 "CredAcct", 
			   DecAct 											 "DebAcct",
			   GroupKey 								     	"GroupKey"
		from #tbl 
		
		order by  ItmCode, RowNum;
		drop table #tbl;
		drop table #wh07;
		drop table #RateDiff;
	end if;
end
