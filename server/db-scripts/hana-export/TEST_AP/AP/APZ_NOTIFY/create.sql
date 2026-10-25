CREATE  PROCEDURE apz_notify(
in objtype NVARCHAR(50) default '',
in transtype NCHAR(1) default '',
in num_of_key INT default 0,
in key_columns NVARCHAR(254) default '',
in obj_id nvarchar(500000) default ''-- Tra ve Docnum header
)
AS
BEGIN
	DECLARE error INT =0;
	DECLARE error_message NVARCHAR(254)='';
	
	declare empID int;
	declare healthIDCurrent int;
	declare medicalDate date; -- ngay kham suc khoe o table Offer (đi kèm với khám sức khỏe thực phẩm)
	---- Job Vacancy --> Offer: APZ_HJV5


IF (objtype = '700012' and transtype in ('U')) then 
		update  a
		set  "Status" = NULL
		from APZ_HJV5 a
		Where  "DocEntry"  = :obj_id and  ( ("U_FoodHealthCheck" Is NUll or  ifnull("U_FoodHealthCheck",0 ) in ( 1,2))--1: Cần kiểm tra --2: Chờ kết quả
												--Nguồn giơi thiêu  = 0 --> NV cty giơi thiêu, và không chon nhân viên--> K cho chuyển trạng thái Accept
											 or  (ifnull("U_RecruitmentSource",99)=0 and  ifnull(cast("U_PersonWhoIntroduced" as nvarchar),'') =''));
 	
	---- Update thong tin nguoi gioi thieu vao bang nhan vien OHEM 
 	update  a
	set   a."U_PersonWhoIntroduced" = (select offer."U_PersonWhoIntroduced"
										from  APZ_HJV6 hired
										
										inner join  APZ_HJV5 offer on offer."DocEntry" = hired."DocEntry"
			
			where ifnull(offer."U_RecruitmentSource",99) = 0 and   ifnull(cast(offer."U_PersonWhoIntroduced" as nvarchar),'') !=''
			 	        and   ifnull(cast(hired."empId" as nvarchar),'') !='' and hired."Status"  = 'H'
			 	        and hired."DocEntry"  = :obj_id),
			 	        
	-- Nguồn tuyen dung
	 a."U_RecruitmentSource" = (select offer."U_RecruitmentSource"
										from  APZ_HJV6 hired
										
										inner join  APZ_HJV5 offer on offer."DocEntry" = hired."DocEntry"
			
			where 
			 	         ifnull(cast(hired."empId" as nvarchar),'') !='' and hired."Status"  = 'H'
			 	        and hired."DocEntry"  = :obj_id),
			 	        
			 	        
	 --- Số giờ làm trong tuần: 40, 44, 48

	a."U_WorkingHours" = (select ifnull(offer."U_WorkingHours",40)
										from  APZ_HJV6 hired
										
										inner join  APZ_HJV5 offer on offer."DocEntry" = hired."DocEntry"
			
			where  
			 	            ifnull(cast(hired."empId" as nvarchar),'') !='' and hired."Status"  = 'H'
			 	        and hired."DocEntry"  = :obj_id),
			 	        
	 --- U_Expat: Đối tượng nhân viên : ILD's Expat,Supplier's Expat,ILD

	a."U_Expat" = (select offer."U_Expat"
										from  APZ_HJV6 hired
										
										inner join  APZ_HJV5 offer on offer."DocEntry" = hired."DocEntry"
			
			where   
			 	         ifnull(cast(hired."empId" as nvarchar),'') !='' and hired."Status"  = 'H'
			 	        and hired."DocEntry"  = :obj_id)
			 	  
	from OHEM a
	where a."empID"  = (select top 1 ("empId") from  APZ_HJV6 hired where hired."DocEntry"  = :obj_id );
  
  
--------- Insert thông tin khám sức khỏe thực phẩm cho nhân viên
if exists( select  "empId"  from  APZ_HJV6 hired where hired."DocEntry"  = :obj_id) then
		 select top 1 ("empId")  into empID  from  APZ_HJV6 hired where hired."DocEntry"  = :obj_id;
	 
	If(ifnull(:empID,0)  >0) then 
	 	select max("healthID") into healthIDCurrent  from  APZ_OHEH;
	 	
	 	--- Ngày kiểm tra sức khỏe
			If((:empID IS NULL)) then 
				medicalDate = CURRENT_DATE;
			Else
				 if exists (SELECT ifnull(offer."U_medicalDate",CURRENT_DATE)      

				 	From  APZ_HJV6 hired
					inner join  APZ_HJV5 offer on offer."DocEntry" = hired."DocEntry"
				 	
				 	
				 	where 							ifnull(offer."U_FoodHealthCheck",99) = 3
							 	        and   ifnull(cast(hired."empId" as nvarchar),'') !='' 
				 		 	        
							 	        and hired."DocEntry"  = :obj_id)  then ---

				
				 	SELECT ifnull(offer."U_medicalDate",CURRENT_DATE) INTO  medicalDate  

				 	From  APZ_HJV6 hired
					inner join  APZ_HJV5 offer on offer."DocEntry" = hired."DocEntry"
				 	
				 	
				 	where 							ifnull(offer."U_FoodHealthCheck",99) = 3
							 	        and   ifnull(cast(hired."empId" as nvarchar),'') !='' 
				 		 	        
							 	        and hired."DocEntry"  = :obj_id;
							 	        
				end if; ---
			End If;
	
       ----
	 	
	 	
	 	insert into APZ_OHEH ("healthID","kind","empID","medicalDate","U_NgayTaiKiem")-- id,kind, empID, ngay khám bệnh, ngày tái kiểm
		values(Ifnull(healthIDCurrent,0)+1,1,:empID,:medicalDate,  ADD_MONTHS(TO_DATE (medicalDate),6));
	End if;

end if;
	
------------------ End Thông tin khám Sức Khỏe Thực Phẩm
 
END IF;

----======================================Contract: Hop Dong========================================-----------



----======================================END Contract: Hop Dong===================================------------
	SELECT :error,:error_message from DUMMY;
END;


