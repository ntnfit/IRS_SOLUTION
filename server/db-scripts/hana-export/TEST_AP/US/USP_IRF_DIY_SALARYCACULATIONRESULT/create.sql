CREATE PROCEDURE USP_IRF_DIY_SalaryCaculationResult(
	in period 					nvarchar(50),
	in branch 					int,
	in empID 					nvarchar(4000),
	in status 					nvarchar(40000) default ''
)
AS
BEGIN
	declare fdate,tdate,startDate timestamp;
	declare hourperday int;
	declare expat int;
	select x."FromDate",x."ToDate",x."HourPerDay" into fdate,tdate,hourperday from APZ_OHWP x where x."PeriodCode" = :period;
	
	select "startDate"  into  startDate from OHEM where "empID"  =  :empID;
	select ifnull("U_Expat",999)  into  expat from OHEM where "empID"  =  :empID;
	

-- Thong tin nhan vien


	create local temporary table #tbl(
		
		empID int,
		PreviousGrossSalary numeric(19,6),
		AnnualLeave numeric(19,6),
		OtherLeave numeric(19,6),
		UnpaidLeave numeric(19,6), 
		AbsentSum numeric(19,6), -- Tổng các ngày nghỉ không lương (bao gồm cả sick leave, maternity leave)
		SickLeave numeric(19,6), -- Sum của các workcode có type là Sick leave với rate tương ứng.
		MaternityLeave numeric(19,6), --Nghỉ thai sản: Sum của các workcode có type là Maternity leave với rate tương ứng.
		
		
		SalaryIncreaseAdjusstment numeric(19,6), --Dieu chinh tang luong
		SalaryReductionAdjusstment numeric(19,6), -- Dieu chinh giam luong
		SalaryAdjusstment numeric(19,6),
		
		PerfomanceBonus numeric(19,6),
		Salary13 numeric(19,6),
		RefferalBonus  numeric(19,6),
		RealSalTotal 	numeric(19,6),
		OtherAllowance	  numeric(19,6),
		ConsultantAllowanceNet	  numeric(19,6),
		
		TotalRelief numeric(19,6),
		U_TotalAmount  numeric(19,6), -- Tổng số tiền bảo hiểm người thân

		U_GuaranteeBonus numeric(19,6),
		U_PPBonus numeric(19,6),
		U_SPPBonus numeric(19,6),
		U_MeritBonus numeric(19,6),

		TyGiaHRM numeric(19,6),
		TyGiaHRM_Sal numeric(19,6),
		LimitedAmt1 numeric(19,6), -- bao hiem xa hoi
		LimitedAmt2 numeric(19,6), -- bao hiem y te
		LimitedAmt3 numeric(19,6), -- bao hiem that nghiep
		LimitedAmt5 numeric(19,6), -- cong doan

		Seniormerit_1y numeric(19,6),
		Seniormerit_2y numeric(19,6),
		HousingRental_Taxable numeric(19,6),
		HousingRental numeric(19,6),
		TienGT	  numeric(19,6),
		HLuyenDienTapPCCC	  numeric(19,6),
		PIT_ER	  numeric(19,6),
		PIT_Final_Refund	  numeric(19,6),
		SHUI_ER_Adjust	  numeric(19,6),
		
		NightShift	  numeric(19,6),
		SHUI_EE_Adjustment numeric(19,6),
		AmtRelat numeric(19,6) -- lay tu salary parameter
		

);	

insert into #tbl (empId)
values (:empId);

update #tbl
set 
	 
     
	 /*PreviousGrossSalary  = (select top 1  "BasicSal"
							From APZ_OSAL bluong
							join APZ_OHWP kyluong on bluong."Period"  = kyluong."PeriodCode"
							
							where "empID"  = :empID 
							
							and cast(kyluong."ToDate" as date)  <  (select "ToDate" from APZ_OHWP where "PeriodCode"  = :period )
							
							
							and "BasicSal" <>  (select "BasicSal" from APZ_OSAL where  "empID"  = :empID  and "Period" = :period )
							
							order by "ToDate" Desc)*/
						
	PreviousGrossSalary  = (select max("SalTotal")
							From APZ_OHSI  
							 
							
							where "EmpId"  = :empID and cast("ValidTo" as date) <= tdate)
							
	------------------ Leave: Nghỉ	

	,SickLeave  =  		(Select sum("WorkTotal")
								From APZ_TSL1 a
								inner join APZ_OHWC b on a."WorkCode" = b."Code"
								Where  "Period" = :period and  "EmpId"  = :empID and b."WorkType" = 7 and b."Branch" =:branch) 	
														
							--	Thai sản
	,MaternityLeave  =  		(Select sum("WorkTotal")
								From APZ_TSL1 a
								inner join APZ_OHWC b on a."WorkCode" = b."Code"
								Where  "Period" = :period and  "EmpId"  = :empID and b."WorkType" = 8 and b."Branch" =:branch)	
														
							--						
    
	,AnnualLeave  =  		(Select SUm("Used")
								From APZ_TSL2 
								Where  "Period" = :period and  "EmpId"  = :empID  	)
														
							--
	
	,OtherLeave  =  		(Select sum("WorkTotal")
								From APZ_TSL1 a
								inner join APZ_OHWC b on a."WorkCode" = b."Code"
								Where  "Period" = :period and  "EmpId"  = :empID and b."WorkType" = 6 and b."Branch" =:branch)	
		--(Select sum("WorkTotal") From APZ_TSL1 	Where  "Period" = :period and  "EmpId"  = :empID and "WorkCode"  =  'O'	)	
								
								---

	,UnpaidLeave  =  		(Select sum("WorkTotal")
								From APZ_TSL1 a
								inner join APZ_OHWC b on a."WorkCode" = b."Code"
								Where  "Period" = :period and  "EmpId"  = :empID and b."WorkType" = 17 and b."Branch" =:branch)	
	--
	,AbsentSum  =  		(Select sum("WorkTotal")

								From APZ_TSL1 a
								inner join APZ_OHWC b on a."WorkCode" = b."Code"
								inner join APZ_OHWT c on b."WorkType" = c."Id"
								Where  "Period" = :period and  "EmpId"  = :empID and  c."Typ"  = 'A' and b."Branch" =:branch)								
								--- 
	
	,PerfomanceBonus =      (Select SUM("TotalAmount")
							From  APZ_HPT2  master
							inner join APZ_HPT1 detail on master."DocNum"  =  detail."DocNum"
							inner join APZ_OHPT loai  on loai."Code" = detail."PenalizationType"
							
							where 
								 master."Period" =  :period and
								   detail."EmpId"  = :empID
								
								  and loai."Type"  = 'B' and loai."Code"  = 7)
								  
	--- SHUI_EE_Adjustment
  	,SHUI_EE_Adjustment = (Select SUM("TotalAmount")
							From  APZ_HPT2  master
							inner join APZ_HPT1 detail on master."DocNum"  =  detail."DocNum"
							inner join APZ_OHPT loai  on loai."Code" = detail."PenalizationType"
							
							where 
								 master."Period" =  :period and
								   detail."EmpId"  = :empID
								
								  and loai."Type"  = 'P' and loai."Code" = 21)
		
		
								  
	--- điều chỉnh tăng lương: tổng điều chỉnh tăng - tổng điều chỉnh giảm
	,SalaryIncreaseAdjusstment = (Select SUM("TotalAmount")
							From  APZ_HPT2  master
							inner join APZ_HPT1 detail on master."DocNum"  =  detail."DocNum"
							inner join APZ_OHPT loai  on loai."Code" = detail."PenalizationType"
							
							where 
								 master."Period" =  :period and
								   detail."EmpId"  = :empID
								
								  and loai."Type"  = 'B' and loai."Code" = 6)
	--- điều chỉnh giảm lương: tổng điều chỉnh tăng - tổng điều chỉnh giảm
	,SalaryReductionAdjusstment = (Select SUM("TotalAmount")
							From  APZ_HPT2  master
							inner join APZ_HPT1 detail on master."DocNum"  =  detail."DocNum"
							inner join APZ_OHPT loai  on loai."Code" = detail."PenalizationType"
							
							where 
								 master."Period" =  :period and
								   detail."EmpId"  = :empID
								
								  and loai."Type"  = 'P' and loai."Code" = 14)
								 
 
	--- điều chỉnh lương: tổng điều chỉnh tăng - tổng điều chỉnh giảm
	,SalaryAdjusstment = ifnull((Select SUM("TotalAmount")
							From  APZ_HPT2  master
							inner join APZ_HPT1 detail on master."DocNum"  =  detail."DocNum"
							inner join APZ_OHPT loai  on loai."Code" = detail."PenalizationType"
							
							where 
								 master."Period" =  :period and
								   detail."EmpId"  = :empID
								
								  and loai."Type"  = 'B' and loai."Code" = 6) ,0)
								  
					     -
					     
					     ifnull((Select SUM("TotalAmount")
							From  APZ_HPT2  master
							inner join APZ_HPT1 detail on master."DocNum"  =  detail."DocNum"
							inner join APZ_OHPT loai  on loai."Code" = detail."PenalizationType"
							
							where 
								 master."Period" =  :period and
								   detail."EmpId"  = :empID
								
								  and loai."Type"  = 'P' and loai."Code" = 14) ,0)
	--- Lương tháng 13
			
	,Salary13 = case when month(:fdate)  < 12 then 0 --- Kỳ công tháng 12 mới tính lương tháng 13

				when month(:fdate)  = 12   -- Kỳ công đang tính là tháng 12 + thời gian làm viêc là >=12 tháng --> 1 tháng lương
				     and  ( DAYS_BETWEEN(cast (startDate as date),   cast (tDate as date)) / 30) >=12
			
			
				then (select  top 1 "BasicSal"
				  from APZ_OHSI
				  where  "EmpId"  = :empID and cast ("ValidTo" as date) >=  :TDate)
					
					
				when month(:fdate)  = 12   -- Kỳ công đang tính  12 + thời gian làm viêc CHUA DU  12 tháng --> 1 tháng lương
				     and  ( DAYS_BETWEEN(cast (startDate as date),   cast (tDate as date)) / 30) >=12
			
			
				 -- (So thang lam viec/ 12 ) * luong
				then (ifnull(( DAYS_BETWEEN(cast (startDate as date),   cast (tDate as date)) / 30),0)/12) *

				(select  top 1 "BasicSal" 
				  from APZ_OHSI
				  where  "EmpId"  = :empID and cast ("ValidTo" as date) >=  :TDate)		
					
											
 			else 0 end
 			-----Tiền thưởng giới thiệu


	,RefferalBonus  =   (Select SUM("TotalAmount")
							From  APZ_HPT2  master
							inner join APZ_HPT1 detail on master."DocNum"  =  detail."DocNum"
							
							
							where master."Period" =  :period
								  and  detail."EmpId"  = :empID
								  and detail."PenalizationType"  =  5)
								  
	,OtherAllowance  =   (Select SUM("TotalAmount")
							From  APZ_HPT2  master
							inner join APZ_HPT1 detail on master."DocNum"  =  detail."DocNum"
							
							
							where master."Period" =  :period
								  and  detail."EmpId"  = :empID
								  and detail."PenalizationType"  =  20)
	---
	,ConsultantAllowanceNet  =   (Select SUM("TotalAmount")
							From  APZ_HPT2  master
							inner join APZ_HPT1 detail on master."DocNum"  =  detail."DocNum"
							
							
							where master."Period" =  :period
								  and  detail."EmpId"  = :empID
								  and detail."PenalizationType"  =  18)
								  
	   -----


	,U_GuaranteeBonus  =   (Select SUM("TotalAmount")
							From  APZ_HPT2  master
							inner join APZ_HPT1 detail on master."DocNum"  =  detail."DocNum"
							
							
							where master."Period" =  :period
								  and  detail."EmpId"  = :empID
								  and detail."PenalizationType"  =  11)
								  
								  ---
	,U_PPBonus  =   (Select SUM("TotalAmount")
							From  APZ_HPT2  master
							inner join APZ_HPT1 detail on master."DocNum"  =  detail."DocNum"
							
							
							where master."Period" =  :period
								  and  detail."EmpId"  = :empID
								  and detail."PenalizationType"  =  12)
								  
	,U_SPPBonus  =   (Select SUM("TotalAmount")
							From  APZ_HPT2  master
							inner join APZ_HPT1 detail on master."DocNum"  =  detail."DocNum"
							
							
							where master."Period" =  :period
								  and  detail."EmpId"  = :empID
								  and detail."PenalizationType"  =  13)	
								  
	,U_MeritBonus  =   (Select SUM("TotalAmount")
							From  APZ_HPT2  master
							inner join APZ_HPT1 detail on master."DocNum"  =  detail."DocNum"
							
							
							where master."Period" =  :period
								  and  detail."EmpId"  = :empID
								  and detail."PenalizationType"  =  14)	
	,Seniormerit_1y  =   (Select SUM("TotalAmount")
							From  APZ_HPT2  master
							inner join APZ_HPT1 detail on master."DocNum"  =  detail."DocNum"
							
							
							where master."Period" =  :period
								  and  detail."EmpId"  = :empID
								  and detail."PenalizationType"  =  15)
	,Seniormerit_2y  =   (Select SUM("TotalAmount")
							From  APZ_HPT2  master
							inner join APZ_HPT1 detail on master."DocNum"  =  detail."DocNum"
							
							
							where master."Period" =  :period
								  and  detail."EmpId"  = :empID
								  and detail."PenalizationType"  =  16)	
								  
	,SHUI_ER_Adjust  =   (Select SUM("TotalAmount")
							From  APZ_HPT2  master
							inner join APZ_HPT1 detail on master."DocNum"  =  detail."DocNum"
							
							
							where master."Period" =  :period
								  and  detail."EmpId"  = :empID
								  and detail."PenalizationType"  =  25)							  
								  
	,PIT_ER  =   (Select SUM("TotalAmount")
							From  APZ_HPT2  master
							inner join APZ_HPT1 detail on master."DocNum"  =  detail."DocNum"
							
							
							where master."Period" =  :period
								  and  detail."EmpId"  = :empID
								  and detail."PenalizationType"  =  22)
	,PIT_Final_Refund = ifnull((Select SUM("TotalAmount")
							From  APZ_HPT2  master
							inner join APZ_HPT1 detail on master."DocNum"  =  detail."DocNum"
							inner join APZ_OHPT loai  on loai."Code" = detail."PenalizationType"
							
							where 
								 master."Period" =  :period and
								   detail."EmpId"  = :empID
								
								  and loai."Type"  = 'B' and loai."Code" = 24) ,0)
								  
					     -
					     
					     ifnull((Select SUM("TotalAmount")
							From  APZ_HPT2  master
							inner join APZ_HPT1 detail on master."DocNum"  =  detail."DocNum"
							inner join APZ_OHPT loai  on loai."Code" = detail."PenalizationType"
							
							where 
								 master."Period" =  :period and
								   detail."EmpId"  = :empID
								
								  and loai."Type"  = 'P' and loai."Code" = 23) ,0)
	-- Phụ cấp giới thiệu ứng viên
	,TienGT  =   (Select SUM("TotalAmount")
							From  APZ_HPT2  master
							inner join APZ_HPT1 detail on master."DocNum"  =  detail."DocNum"
							
							
							where master."Period" =  :period
								  and  detail."EmpId"  = :empID
								  and detail."PenalizationType"  =  2)
	--- Phụ cấp huấn luyện và diễn tập PCCC
	,HLuyenDienTapPCCC  =   (Select SUM("TotalAmount")
							From  APZ_HPT2  master
							inner join APZ_HPT1 detail on master."DocNum"  =  detail."DocNum"
							
							
							where master."Period" =  :period
								  and  detail."EmpId"  = :empID
								  and detail."PenalizationType"  =  17)	
	
     ,TyGiaHRM =  case when expat  = 1 then (select top 1  ifnull("U_TygiaMacDinh",1)  from "@TYGIA_HRM" where "Name" like '%' || YEAR(CURRENT_DATE) ||'%')
     				else 1 end

	,TyGiaHRM_Sal = case when expat  = 1 then (select top 1  ifnull("U_TygiaSal",1)  from "@TYGIA_HRM" where "Name" like '%' || YEAR(CURRENT_DATE) ||'%') else 1 end
	,U_TotalAmount= (select  ifnull("U_TotalAmount",0)  from "@RELATIVES_INS_LINE"	where "U_Workperiod"  =:period  and  "U_EmpID" = :empID)
	
	,LimitedAmt1 = 	(select ifnull( "LimitedAmt",0)  from APZ_HDT1 where "Code"  = 1)
	,LimitedAmt2 = 	(select ifnull( "LimitedAmt",0)  from APZ_HDT1 where "Code"  = 2)
	,LimitedAmt3 = 	(select ifnull( "LimitedAmt",0)  from APZ_HDT1 where "Code"  = 3)
	,LimitedAmt5 = 	(select ifnull( "LimitedAmt",0)  from APZ_HDT1 where "Code"  = 5)
	--(select ifnull(max("LimitedAmt"),29800000)  from APZ_HDT1)
	
	, NightShift  = (select   ifnull(sum(a."WorkTotal"),0)*8 as "NightShift"
						from APZ_TSL1 a -- bảng kê thưa từ APZ_IOD1: Dữ liệu châm công
						inner join APZ_IOD1 b on a."Period"  = b."Period" and a."EmpId" = b."empID" and a."WorkDay"  = b."WorkDay"
						inner join  APZ_OEWS c on b."ShiftID"  = c."ShiftID" 
						
						where ifnull(c."IsNightShift",'') = 'Y' and a."EmpId" = :empID and  a."Period" = :period  )
	
	--
	,AmtRelat  =  (select top 1 "AmtRelat" from APZ_OHSP  order by "ValidTo" desc)
	;
	
	 

select * from #tbl;
drop table #tbl;

END;

