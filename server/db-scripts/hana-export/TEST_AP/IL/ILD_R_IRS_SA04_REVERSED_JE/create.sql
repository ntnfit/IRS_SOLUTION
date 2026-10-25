CREATE procedure ILD_R_IRS_SA04_Reversed_JE
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

declare DieuChinh table
(

	STT int,
	PostingDate date,
	GLAcc	nvarchar(15),
	Debit 	decimal(19,6),
	Credit 	decimal(19,6),
	Remark	nvarchar(250),
	SoPhieu_Delivery nvarchar(250),
	Remark_Header nvarchar(250) ,
	RowKey varchar(50),
	DO_ID int
);
-- Không lấy các phiếu có Posting Date = ngày cuối kỳ. Chỉ Insert các phiếu đảo vào ngày đầu tháng sau của kỳ đang chạy
-- Thêm 2 cột AR Tax Invoice, AR Invoice Date -> lấy phiếu AR Invoice

 --Year_Period, Period
call ILD_R_IRS_SA02_Reversed_COGS (Year_Period,Period,tab_result); 
create local temporary table #SA02 as (select round(sum(DO_SSCC_Amount),2) as TransValue,
											  round(sum(Adj_Reversed_COGS),2) as  Adj_Reversed_COGS, DO_ID,
											  MAX(DO_NO) as SoPhieu_Del
											 from :tab_result
											 group by  DO_ID 					   
									    );								    

select Last_day(nam||'-'||thang||'-01') as "LastDay",
		add_days (Last_day (nam||'-'||thang||'-01'),1) as "Next_DayOF_Next_Month" 
		into lastday,first_day_next_month
from dummy;
---
-- ====================== Thêm dữ liệu điều chỉnh: 
/*
	----Tài khoản nợ: 1571

		--- 1571: Cho phiếu đảo vào ngày 1 đầu tháng sau (PHIẾU ĐẢO)
		insert into :DieuChinh (STT,SoPhieu_Delivery,PostingDate,
								GLAcc, Debit, Credit, Remark, 
								remark_Header,RowKey)
		select 11,SoPhieu_Del, :first_day_next_month as "PostingDate",
				'1571', 0  as "Debit",TransValue as "Credit", 'Adjusting COGS ' ||:Thang||'.'||Nam || '_'||SoPhieu_Del||'_reverse',
				'Adjusting COGS ' || thang||'-'||nam || '_Reverse' as "remark_Header",DO_ID
		from #SA02;
		


	----Tài khoản có: 6321
	
	-- 6321: Cho phiếu đảo vào ngày 1 đầu tháng sau (PHIẾU ĐẢO)
		insert into :DieuChinh (STT,SoPhieu_Delivery,PostingDate,
								GLAcc, Debit, Credit, Remark, 
								remark_Header,RowKey)
		select 11,SoPhieu_Del, :first_day_next_month as "PostingDate",
				'6321', TransValue  as "Debit",0 as "Credit", 'Adjusting COGS '||:Thang||'.'||Nam || '_' ||SoPhieu_Del||'_reverse',
				'Adjusting COGS ' || thang||'-'||nam || '_Reverse' as "remark_Header",DO_ID
		from #SA02;
*/
-- ====================== Thêm dữ liệu điều chỉnh bổ sung: 
		
---=========  Cho phiếu đảo vào ngày 1 đầu tháng sau
	----Tài khoản có:1571  Cho phiếu đảo vào ngày 1 đầu tháng sau	
		insert into :DieuChinh (STT,SoPhieu_Delivery,PostingDate,GLAcc, Debit, Credit, Remark, 
					remark_Header,RowKey)
		select 22,SoPhieu_Del, :first_day_next_month,'1571', 0 as "Debit", Adj_Reversed_COGS as "Credit",
				'Additional adjustment for COGS ' ||:Thang||'.'||Nam || '_'  ||SoPhieu_Del || '_reverse',
		
				'Adjusting COGS ' || thang||'-'||nam || '_Reverse' as "remark_Header",DO_ID
		from #SA02
		where Adj_Reversed_COGS>0;
		
		--=== trương hop  Adj_Reversed_COGS < 0 (ÂM)
		insert into :DieuChinh (STT,SoPhieu_Delivery,PostingDate,GLAcc, Debit, Credit, Remark, 
					remark_Header,RowKey)
		select 22,SoPhieu_Del, :first_day_next_month,'1571', abs(Adj_Reversed_COGS) as "Debit", 0 as "Credit",
				 'Additional adjustment for COGS ' ||:Thang||'.'||Nam || '_'  ||SoPhieu_Del || '_reverse',
		
				'Adjusting COGS ' || thang||'-'||nam || '_Reverse' as "remark_Header",DO_ID
		from #SA02
		where Adj_Reversed_COGS<0;
	
		
	
	----Tài khoản Nợ:6321  Cho phiếu đảo vào ngày 1 đầu tháng sau	
		insert into :DieuChinh (STT,SoPhieu_Delivery,PostingDate,GLAcc, Debit, Credit, Remark, 
					remark_Header,RowKey)
		select 22,SoPhieu_Del, :first_day_next_month,'6322',Adj_Reversed_COGS as "Debit", 0 as "Credit",
		
		 'Additional adjustment for COGS ' ||:Thang||'.'||Nam || '_'  ||SoPhieu_Del || '_reverse',
		
				'Adjusting COGS ' || thang||'-'||nam || '_Reverse' as "remark_Header",DO_ID
		from #SA02
		where Adj_Reversed_COGS>0;


		insert into :DieuChinh (STT,SoPhieu_Delivery,PostingDate,GLAcc, Debit, Credit, Remark, 
					remark_Header,RowKey)
		select 22,SoPhieu_Del, :first_day_next_month,'6322',0 as "Debit", abs(Adj_Reversed_COGS) as "Credit",
		
		 'Additional adjustment for COGS ' ||:Thang||'.'||Nam || '_'  ||SoPhieu_Del || '_reverse',
		
				'Adjusting COGS ' || thang||'-'||nam || '_Reverse' as "remark_Header",DO_ID
		from #SA02
		where Adj_Reversed_COGS<0;
 
update :DieuChinh set DO_ID = RowKey ;
update :DieuChinh set RowKey = RowKey || STT || case when Debit > 0 then 'DSA04' else 'CSA04' end ||GLAcc ;

select "U_InvDate" as "PostingDate",dc.GLAcc as "GLAcc"
	,case when je."StornoToTr" > 0 then dc.Debit * -1 else dc.Debit end as "Debit"
	,case when je."StornoToTr" > 0 then dc.Credit * -1 else dc.Credit end as "Credit"
	,dc.Remark as "Remark",dc.Remark_Header as "Remark_Header",dc.SoPhieu_Delivery as "DO no." 
	,inv."U_InvNo" as "AR Tax Invoice", inv."U_InvDate" as "AR Invoice Date"
	,inv."DocNum" as "AR_DocNo",inv."DocEntry" as "AR_DocID"
	,je."Number" as "JE_DocNo", je."TransId" as "JE_DocID"
	,case 
		when je."StornoToTr" > 0 then 'Cancellation'
		when totr."StornoToTr" > 0 then 'Canceled'
	else null end as "JE_Status"
	,RowKey AS "RowKey"
from :DieuChinh dc
left join (
	select
		max(a."DocNum") "DocNum", max(a."U_InvNo") "U_InvNo" ,max(a."U_InvDate") "U_InvDate", max(b."BaseRef") "BaseRef"
		,b."DocEntry",max(b."BaseEntry") "BaseEntry"
	from OINV a
	inner join INV1 b ON b."DocEntry" = a."DocEntry"
	where a.CANCELED = 'N' and b."BaseType" = 15 group by b."DocEntry"
	) inv on inv."BaseRef" = dc.SoPhieu_Delivery
left join (
	select OJDT."Number",JDT1."Ref3Line",OJDT."TransId","StornoToTr" from OJDT INNER JOIN JDT1 ON OJDT."TransId" = JDT1."TransId" 
	where JDT1."Ref3Line" != '' and OJDT."TransType" = 30
	) je on je."Ref3Line" = dc.RowKey
left join (select OJDT."Number",OJDT."TransId","StornoToTr" from OJDT where "StornoToTr" > 0) totr on totr."StornoToTr" = je."TransId"
order by  dc.STT, dc.remark,je."TransId"  ;

---
drop table #SA02;
 
end;
