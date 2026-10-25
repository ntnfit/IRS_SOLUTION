Create procedure ILD_R_IRS_CostCenter
(
	in Thang	int,
	in Nam		int,
	in CostCenter 	nvarchar(5000)
)
as
Begin

create local temporary table #tbl (
		--STT int,
		CostCenter nvarchar(50),
		RealSalary decimal(21,6),
		
		Petro   decimal(21,6),
		Meal  decimal(21,6),
		Phone	decimal(21,6),
		Phone_Adjustment decimal(21,6),
		ReferalBonus decimal(21,6),
		
		SI_EE decimal(21,6),
		HI_EE decimal(21,6),
		UI_EE decimal(21,6),
		PIT decimal(21,6),
		
		DependantIns_Deduct decimal(21,6), -- medical insurance for dependent deduction
		HouseRental decimal(21,6),
		AdvanceAmount decimal(21,6), --Advance salary deduction
		Union_EE decimal(21,6),
		
		OtherDeduction decimal(21,6),
		SI_ER decimal(21,6),
		SI_ER_05 decimal(21,6),
		HI_ER decimal(21,6),
		UI_ER decimal(21,6),
		Union_ER decimal(21,6),
		
		TotalCost decimal(21,6),
		Thang int,
		Nam int
		);


insert into #tbl(CostCenter, RealSalary,
		
				 Petro,
				 Meal, 
				 Phone,
				 Phone_Adjustment,
				 ReferalBonus,
				
				 SI_EE,
				 HI_EE,
				 UI_EE,
				 PIT,
				
				 DependantIns_Deduct,
				 HouseRental,
				 AdvanceAmount, 
				 Union_EE,
				 OtherDeduction,
				 SI_ER,SI_ER_05,
				 HI_ER,UI_ER,
				 Union_ER,
				
				TotalCost,Thang,Nam)
				
select  "CostCenter",
		Max("RealSalary") as "RealSalary",
		Max("Petro") as "Petro",
		Max("Meal") as "Meal",
		Max("Phone") as "Phone",-- hiện đang gắn  = 0
		Max("Phone_Adjustment") as  "Phone_Adjustment",
		
		Max("Refferalbonus") as "Refferalbonus",
		
		Sum("BHXH_NV") as "BHXH_NV", -- 8% SI (EE)
		Sum("BHYT_NV") as "BHYT_NV", -- 1.5% HI (EE)
		Sum("BHTN_NV") as "BHTN_NV", --1% UI (EE)		
		Max("ThueTNCN") as "ThueTNCN", -- PIT

		Max("DependantIns_Deduct") as "DependantIns_Deduct",
		Max("HousingRental") as "HousingRental" ,
		
		Max("AdvanceAmount")  as  "AdvanceAmount",
		Sum("CongDoan_NV") as "CongDoan_NV", 
		Max("OtherDeduction") as "OtherDeduction",
		
		Sum("BHXH_CTY") as "BHXH_CTY",--17% SI _ ER

		0 as  "SI_ER_05",
		Sum("BHYT_CTY") as "BHYT_CTY",
		Sum("BHTN_CTY") as "BHTN_CTY",
		Sum("CongDoan_CTY") as "CongDoan_CTY",--2% Trade Union fee_ER

		Max("TotalEmpCost") as  "TotalEmpCost",
		
	
		Max("Thang") as "Thang",
		Max("Nam") as "Nam"
		
		
From 
(
 
	 select   "TongBHNV","TongBHCTY"  ,
				"ThueTNCN",
				"RealSalary","DependantIns_Deduct","HousingRental","Petro",
				"Meal",
				"Phone",-- hiện đang gắn  = 0
				"Phone_Adjustment",
				"Refferalbonus",
				"AdvanceAmount",
				"OtherDeduction",
				"TotalEmpCost",
		 
			 
						case when "LoaiBaoHiem" like  '%BHXH%' then "BH_NV" else 0 end as "BHXH_NV",
						case when "LoaiBaoHiem" like  '%BHXH%' then "BH_CTY" else 0 end as "BHXH_CTY",
						
						case when "LoaiBaoHiem" like  '%BHYT%' then "BH_NV" else 0 end as "BHYT_NV",
						case when "LoaiBaoHiem" like  '%BHYT%' then "BH_CTY" else 0 end as "BHYT_CTY",
						
						
						case when "LoaiBaoHiem" like  '%BHTN%' then "BH_NV" else 0 end as "BHTN_NV",
						case when "LoaiBaoHiem" like  '%BHTN%' then "BH_CTY" else 0 end as "BHTN_CTY",
						
						
						case when "LoaiBaoHiem" like  '%UnionFee%' then "BH_NV" else 0 end as "CongDoan_NV",
						case when "LoaiBaoHiem" like  '%UnionFee%' then "BH_CTY" else 0 end as "CongDoan_CTY",
						
						"PhongBan",
						"CostCenter",
						  
				 
						Thang as "Thang",
						Nam as "Nam"
				 
				from
				(
						select Max("TongBHNV") as  "TongBHNV",
							   Max("TongBHCTY") as "TongBHCTY",
							   Max("ThueTNCN") as  "ThueTNCN",
	  
							   "LoaiBaoHiem",
							
							   Max("EmpAmount") as "BH_NV",
							   Max("CompAmount") as "BH_CTY",
							   
							   
							   Max("RealSalary") as "RealSalary",
							   Max("DependantIns_Deduct") as "DependantIns_Deduct",
							   Max("HousingRental") as "HousingRental",
							   Max("AdvanceAmount") as "AdvanceAmount",
							   Max("OtherDeduction") as "OtherDeduction",
							   Max("TotalEmpCost") as "TotalEmpCost",
							   Max("Petro") as "Petro",
							   Max("Meal") as "Meal",
							   Max("Phone") as "Phone",
							   Max("Phone_Adjustment") as "Phone_Adjustment",
							   Max("Refferalbonus") as "Refferalbonus",
							      "PhongBan",
							      "CostCenter"
						from (
						
								select "TotalDeduction" as "TongBHNV", 
										"U_Totalinsurance_ER"  as "TongBHCTY",
										"PersonalIncomeTax" as "ThueTNCN",
										luong."RealSalary" as "RealSalary",
										luong."U_DependantIns_Deduct" as "DependantIns_Deduct",
										luong."U_HousingRental" as  "HousingRental",
										luong."AdvanceAmount" as  "AdvanceAmount",
										luong."U_AlowQta_14" as "OtherDeduction",
										luong."U_TOTALEMPLOYEECOST" as "TotalEmpCost",
										luong."U_AlowQta_2" as "Petro",
										luong."U_AlowQta_3" as  "Meal",
										
										luong."U_AlowQta_11"   as "Phone",
										luong."U_AlowQta_1"    as "Phone_Adjustment",
										
										luong."U_Refferalbonus" as  "Refferalbonus",
									
								
								luong."empID", baohiem."Name" as "LoaiBaoHiem",baohiem."EmpAmount", baohiem."CompAmount"
									   ,pb."Name" as "PhongBan",
									   nv."CostCenter" as "CostCenter"
								from APZ_OSAL luong
								inner join OHEM nv on luong."empID"  = nv."empID"
								inner join OUDP Pb on nv."dept"  = pb."Code"
								left join (select "empID",  loaibh."Name", sum("EmpAmount") as "EmpAmount" ,  sum("CompAmount")   as "CompAmount"
											from APZ_SAL2 bh 
											inner join APZ_OHDT loaibh on bh."DeducCode"   = loaibh."Code"
											where "Period"  in (select "PeriodCode"
												   from APZ_OHWP 
												   where  "PYear"  = Nam  and  Right("PeriodCode",2)= Thang  )
											group by "empID",  loaibh."Name") baohiem on baohiem."empID" = luong."empID"
								
								where "Period" in (select "PeriodCode"
												   from APZ_OHWP 
												   where "PYear"  = Nam  and  Right("PeriodCode",2)= Thang  
												   
												   )
												   
								order by luong."empID"  
							) as Nguon1
						Group by "PhongBan","CostCenter", "LoaiBaoHiem"
						
						Order by "PhongBan","CostCenter") as Nguon2) 
						
						as Nguon3 
						
						Group by "CostCenter" 
						Order by "CostCenter";


	

 
-------------
		
		
select row_number() over (order by Thang,CostCenter asc) as "STT",* 
from #tbl
where 
 
(
					ifnull(:CostCenter, '') = '' 
					or cast (CostCenter as nvarchar) in (select "DATA" from split(:CostCenter,','))
)

;
drop table #tbl;
		
end;


