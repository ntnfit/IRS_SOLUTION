CREATE procedure ILD_R_IRS_SA03_JE_Reversed_COGS
(
	in Year_Period int, --ILD_LAYYEAR_PERIOD
	in Period nvarchar(20) --ILD_LAYPERIOD @YEAR_PERIOD
)
as
begin

--declare period_code nvarchar(20) = '2024-05';
declare lastday, first_day_next_month date; 
declare thang char(2)  = right (:Period,2);
declare nam char(4)  = left (:Period,4);
/*
declare tbl_GUI table 
(
	SoPhieu_Del nvarchar(20),
	TransValue decimal (19,6),
	Adj_Reversed_COGS decimal (19,6)
);
*/
declare DieuChinh table
(

	STT int,
	PostingDate date,
	GLAcc	nvarchar(15),
	Debit 	decimal(19,6),
	Credit 	decimal(19,6),
	Remark	nvarchar(250),
	SoPhieu_Delivery nvarchar(250),
	Remark_Header nvarchar(250) 
);
 --Year_Period, Period
call ILD_R_IRS_SA02_Reversed_COGS (Year_Period,Period,tab_result); 
create local temporary table #SA02 as (select round(sum(DO_SSCC_Amount),2) as TransValue,
											  round(sum(Adj_Reversed_COGS),2) as  Adj_Reversed_COGS,
											   DO_NO as SoPhieu_Del,max(ifnull(U_Term,'')) as U_Term
											 from :tab_result
											 group by  DO_NO	 					   
									    );
									    
select Last_day (nam||'-'||thang||'-01') as "LastDay",
		add_days (Last_day (nam||'-'||thang||'-01'),1) as "Next_DayOF_Next_Month" 
		into lastday,first_day_next_month
from dummy;
---

-- ====================== Thêm dữ liệu điều chỉnh: 
		----Tài khoản nợ: 1571
			insert into :DieuChinh (STT,SoPhieu_Delivery,PostingDate,GLAcc, Debit, Credit, Remark, 
						remark_Header)
			select 1,SoPhieu_Del, :lastday,'1571', TransValue as "Debit",0 as "Credit", 'Adjusting COGS '||:Thang||'.'||Nam || '_' ||SoPhieu_Del,
					'Adjusting COGS ' || thang||'-'||nam as "remark_Header"
			from #SA02 where ifnull(U_Term,'') != 'FOB';
			
			--- 1571: Cho phiếu đảo vào ngày 1 đầu tháng sau (PHIẾU ĐẢO)
			insert into :DieuChinh (STT,SoPhieu_Delivery,PostingDate,
									GLAcc, Debit, Credit, Remark, 
									remark_Header)
			select 11,SoPhieu_Del, :first_day_next_month as "PostingDate",
					'1571', 0  as "Debit",TransValue as "Credit", 'Adjusting COGS ' ||:Thang||'.'||Nam || '_'||SoPhieu_Del||'_reverse',
					'Adjusting COGS ' || thang||'-'||nam || '_Reverse' as "remark_Header"
			from #SA02 where ifnull(U_Term,'') != 'FOB';
			
	
		
		----Tài khoản có: 6321
			insert into :DieuChinh (STT,SoPhieu_Delivery,PostingDate,GLAcc, Debit, Credit, Remark, remark_Header)
			select 1,SoPhieu_Del, :lastday,'6321', 0,TransValue, 'Adjusting COGS '||:Thang||'.'||Nam || '_' ||SoPhieu_Del,
				  'Adjusting COGS ' || thang||'-'||nam as "remark_Header"
			from #SA02 where ifnull(U_Term,'') != 'FOB';
			
		-- 6321: Cho phiếu đảo vào ngày 1 đầu tháng sau (PHIẾU ĐẢO)
			insert into :DieuChinh (STT,SoPhieu_Delivery,PostingDate,
									GLAcc, Debit, Credit, Remark, 
									remark_Header)
			select 11,SoPhieu_Del, :first_day_next_month as "PostingDate",
					'6321', TransValue  as "Debit",0 as "Credit", 'Adjusting COGS '||:Thang||'.'||Nam || '_' ||SoPhieu_Del||'_reverse',
					'Adjusting COGS ' || thang||'-'||nam || '_Reverse' as "remark_Header"
			from #SA02 where ifnull(U_Term,'') != 'FOB';
	
-- ====================== Thêm dữ liệu điều chỉnh bổ sung: 
		----Tài khoản nợ: 1571
			insert into :DieuChinh (STT,SoPhieu_Delivery,PostingDate,GLAcc, Debit, Credit, Remark, 
						remark_Header)
			select 2,SoPhieu_Del, :lastday,'1571', Adj_Reversed_COGS,0, 'Additional adjustment for COGS ' ||:Thang||'.'||Nam || '_' ||SoPhieu_Del,
			
					'Adjusting COGS ' || thang||'-'||nam as "remark_Header"
			from #SA02
			where Adj_Reversed_COGS>0 ;
			--and ifnull(U_Term,'') != 'FOB';
			
			--=== trương hop  Adj_Reversed_COGS < 0 (ÂM)
			insert into :DieuChinh (STT,SoPhieu_Delivery,PostingDate,GLAcc, Debit, Credit, Remark, 
						remark_Header)
			select 2,SoPhieu_Del, :lastday,'1571', 0,abs(Adj_Reversed_COGS), 'Additional adjustment for COGS ' ||:Thang||'.'||Nam || '_'  ||SoPhieu_Del,
			
					'Adjusting COGS ' || thang||'-'||nam as "remark_Header"
			from #SA02
			where Adj_Reversed_COGS<0;
			-- and ifnull(U_Term,'') = 'FOB';
			
	
---=========  Cho phiếu đảo vào ngày 1 đầu tháng sau
		----Tài khoản có:1571  Cho phiếu đảo vào ngày 1 đầu tháng sau	
			insert into :DieuChinh (STT,SoPhieu_Delivery,PostingDate,GLAcc, Debit, Credit, Remark, 
						remark_Header)
			select 22,SoPhieu_Del, :first_day_next_month,'1571', 0 as "Debit", Adj_Reversed_COGS as "Credit",
					'Additional adjustment for COGS ' ||:Thang||'.'||Nam || '_'  ||SoPhieu_Del || '_reverse',
			
					'Adjusting COGS ' || thang||'-'||nam || '_Reverse' as "remark_Header"
			from #SA02
			where Adj_Reversed_COGS>0 and ifnull(U_Term,'') != 'FOB'  ;
			
			--=== trương hop  Adj_Reversed_COGS < 0 (ÂM)
			insert into :DieuChinh (STT,SoPhieu_Delivery,PostingDate,GLAcc, Debit, Credit, Remark, 
						remark_Header)
			select 22,SoPhieu_Del, :first_day_next_month,'1571', abs(Adj_Reversed_COGS) as "Debit", 0 as "Credit",
					 'Additional adjustment for COGS ' ||:Thang||'.'||Nam || '_'  ||SoPhieu_Del || '_reverse',
			
					'Adjusting COGS ' || thang||'-'||nam || '_Reverse' as "remark_Header"
			from #SA02
			where Adj_Reversed_COGS<0 and ifnull(U_Term,'') != 'FOB';
		
		----Tài khoản có: 6322
			insert into :DieuChinh (STT,SoPhieu_Delivery,PostingDate,GLAcc, Debit, Credit, Remark, 
						remark_Header)
			select 2,SoPhieu_Del, :lastday,'6322', 0,Adj_Reversed_COGS,  'Additional adjustment for COGS ' ||:Thang||'.'||Nam || '_'  ||SoPhieu_Del,
			
					'Adjusting COGS ' || thang||'-'||nam as "remark_Header"
			from #SA02
			where Adj_Reversed_COGS>0 ;
			--and ifnull(U_Term,'') != 'FOB';
			
			--=== trương hop  Adj_Reversed_COGS < 0 (ÂM)
			insert into :DieuChinh (STT,SoPhieu_Delivery,PostingDate,GLAcc, Debit, Credit, Remark, 
						remark_Header)
			select 2,SoPhieu_Del, :lastday,'6322', abs(Adj_Reversed_COGS),0,  'Additional adjustment for COGS ' ||:Thang||'.'||Nam || '_'  ||SoPhieu_Del,
			
					'Adjusting COGS ' || thang||'-'||nam as "remark_Header"
			from #SA02
			where Adj_Reversed_COGS<0 ;
			--and ifnull(U_Term,'') = 'FOB';
		
		----Tài khoản Nợ:6321  Cho phiếu đảo vào ngày 1 đầu tháng sau	
			insert into :DieuChinh (STT,SoPhieu_Delivery,PostingDate,GLAcc, Debit, Credit, Remark, 
						remark_Header)
			select 22,SoPhieu_Del, :first_day_next_month,'6322',Adj_Reversed_COGS as "Debit", 0 as "Credit",
			
			 'Additional adjustment for COGS ' ||:Thang||'.'||Nam || '_'  ||SoPhieu_Del || '_reverse',
			
					'Adjusting COGS ' || thang||'-'||nam || '_Reverse' as "remark_Header"
			from #SA02
			where Adj_Reversed_COGS>0 and ifnull(U_Term,'') != 'FOB';
	
	
			insert into :DieuChinh (STT,SoPhieu_Delivery,PostingDate,GLAcc, Debit, Credit, Remark, 
						remark_Header)
			select 22,SoPhieu_Del, :first_day_next_month,'6322',0 as "Debit", abs(Adj_Reversed_COGS) as "Credit",
			
			 'Additional adjustment for COGS ' ||:Thang||'.'||Nam || '_'  ||SoPhieu_Del || '_reverse',
			
					'Adjusting COGS ' || thang||'-'||nam || '_Reverse' as "remark_Header"
			from #SA02
			where Adj_Reversed_COGS<0 and ifnull(U_Term,'') != 'FOB';


select PostingDate,GLAcc,Debit,Credit,Remark,Remark_Header,SoPhieu_Delivery as "DO no." from :DieuChinh 
order by     STT,  remark   ;
--select  * from :tbl_GUI;
-- dữ liệu Additional adjustment: điều chỉnh bổ sung
---
drop table #SA02;
 
end;
