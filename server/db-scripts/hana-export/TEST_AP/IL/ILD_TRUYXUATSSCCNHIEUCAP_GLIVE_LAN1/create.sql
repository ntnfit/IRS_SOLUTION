CREATE procedure ILD_TruyXuatSSCCNhieuCap_Glive_Lan1
(
	out tab_result table
	(
		SSCC_OngNoi varchar(150),
		Admission_Date_OngNoi date,
		
		SSCC_LV0 varchar(150),
		Admission_Date_LV0 date,
		
		SSCC_LV1 varchar(150),
		Admission_Date_LV1 date,
		
		SSCC_LV2 varchar(150),
		Admission_Date_LV2 date,
		
		SSCC_LV3 varchar(150),
		Admission_Date_LV3 date,
		
		SSCC_LV4 varchar(150),
		Admission_Date_LV4 date,
		
		SSCC_LV5 varchar(150),
		Admission_Date_LV5 date,
		
		SSCC_LV6 varchar(150),
		Admission_Date_LV6 date
	)
	
)
as
begin
create local temporary table #tbl as (select * from :tab_result);
		SSCC_DauTien = select distinct "U_ORIBatch" as "SSCC" from OBTN
where ifnull("U_ORIBatch",'') <> '';

SSCC_DauTien_Admission_Date = select a.*, b."InDate" as "Admission_Date_OngNoi"
from :SSCC_DauTien a
inner join OBTN b on a."SSCC"  = b."DistNumber";


Lv0=select b."SSCC" as "SSCC_OngNoi",b."Admission_Date_OngNoi",
	 a."DistNumber" as "SSCC_Lv0", a."InDate" as "Admission_Date_LV0"
from  OBTN a
inner join :SSCC_DauTien_Admission_Date b on a."U_ORIBatch"  = b."SSCC"
order by b."SSCC";

---
if exists (select 1 from :Lv0) then
	begin
		 insert into #tbl (SSCC_OngNoi,Admission_Date_OngNoi,
		 					Admission_Date_LV0,SSCC_LV0)
		 select "SSCC_OngNoi",
				 "Admission_Date_OngNoi",
				 "Admission_Date_LV0",
				 "SSCC_Lv0"
		 from :Lv0;
			 
	end;

end if;
---

LV1 = select a."SSCC_OngNoi",a."Admission_Date_OngNoi",
 			 b."DistNumber" as "SSCC_Lv1", b."InDate" as "Admission_Date_LV1"
	 from :LV0 a
	 inner join OBTN b on a."SSCC_Lv0" = b."U_ORIBatch" ;
	 
-- Insert LV1
if exists (select 1 from :Lv1) then
	begin
		 insert into #tbl (SSCC_OngNoi,Admission_Date_OngNoi,
		 					
		 					Admission_Date_LV0,SSCC_LV0,
		 					Admission_Date_LV1,SSCC_LV1
		 					)
		 select  a."SSCC_OngNoi",
				 a."Admission_Date_OngNoi",
				 
				 c."Admission_Date_LV0",c."SSCC_Lv0",
				 
				 a."Admission_Date_LV1",a."SSCC_Lv1"
		 from :Lv1 a
		 inner join  OBTN b on a."SSCC_Lv1"  = b."DistNumber"
		 inner join :Lv0 c on b."U_ORIBatch" = c."SSCC_Lv0";
		---
		LV2 = select a."SSCC_OngNoi",a."Admission_Date_OngNoi",
 			 b."DistNumber" as "SSCC_Lv2", b."InDate" as "Admission_Date_LV2"
			 from :LV1 a
			 left join OBTN b on a."SSCC_Lv1" = b."U_ORIBatch" ;
	end;

end if;

--- Insert LV2
if exists (select 1 from :LV2) then
	begin
		 insert into #tbl (SSCC_OngNoi,Admission_Date_OngNoi,
		 					
		 					Admission_Date_LV0,SSCC_LV0,
		 					Admission_Date_LV1,SSCC_LV1,
		 					Admission_Date_LV2,SSCC_LV2
		 					)
		 select  a."SSCC_OngNoi",
				 a."Admission_Date_OngNoi",
				 
				 b.Admission_Date_LV0,b.SSCC_Lv0,
				 b.Admission_Date_LV1,b.SSCC_Lv1,
				 a."Admission_Date_LV2",a."SSCC_Lv2"
		 from :Lv2 a
		 inner join  #tbl b on a."SSCC_OngNoi" = b.SSCC_OngNoi and a."SSCC_Lv2"  = b.SSCC_LV1;
		 
		  
		--
		LV3 = select a."SSCC_OngNoi",a."Admission_Date_OngNoi",
 			 b."DistNumber" as "SSCC_Lv3", b."InDate" as "Admission_Date_LV3"
			 from :LV2 a
			 left join OBTN b on a."SSCC_Lv2" = b."U_ORIBatch" ;
			 
	end;

end if;

if exists (select 1 from :LV3) then
	begin
		 insert into #tbl (SSCC_OngNoi,Admission_Date_OngNoi,
		 					
		 					Admission_Date_LV0,SSCC_LV0,
		 					Admission_Date_LV1,SSCC_LV1,
		 					Admission_Date_LV2,SSCC_LV2,
		 					Admission_Date_LV3,SSCC_LV3
		 					)
		 select  a."SSCC_OngNoi",
				 a."Admission_Date_OngNoi",
				 
				 b.Admission_Date_LV0,b.SSCC_Lv0,
				 b.Admission_Date_LV1,b.SSCC_Lv1,
				 b.Admission_Date_LV2,b.SSCC_Lv2,
				 a."Admission_Date_LV3",a."SSCC_Lv3"
		 from :Lv3 a
		 inner join  #tbl b on a."SSCC_OngNoi" = b.SSCC_OngNoi and a."SSCC_Lv3"  = b.SSCC_LV2;
		
		--
		LV4 = select a."SSCC_OngNoi",a."Admission_Date_OngNoi",
 			 b."DistNumber" as "SSCC_Lv4",b."InDate" as "Admission_Date_LV4"
			 from :LV3 a
			 left join OBTN b on a."SSCC_Lv3" = b."U_ORIBatch" ;
			 
	end;

end if;

if exists (select 1 from :LV4) then
	begin
		 insert into #tbl (SSCC_OngNoi,Admission_Date_OngNoi,
		 					
		 					Admission_Date_LV0,SSCC_LV0,
		 					Admission_Date_LV1,SSCC_LV1,
		 					Admission_Date_LV2,SSCC_LV2,
		 					Admission_Date_LV3,SSCC_LV3,
		 					Admission_Date_LV4,SSCC_LV4
		 					)
		 select  a."SSCC_OngNoi",
				 a."Admission_Date_OngNoi",
				 
				 b.Admission_Date_LV0,b.SSCC_Lv0,
				 b.Admission_Date_LV1,b.SSCC_Lv1,
				 b.Admission_Date_LV2,b.SSCC_Lv2,
				 b.Admission_Date_LV3,b.SSCC_Lv3,
				 a."Admission_Date_LV4",a."SSCC_Lv4"
		 from :Lv4 a
		 inner join  #tbl b on a."SSCC_OngNoi" = b.SSCC_OngNoi and a."SSCC_Lv4"  = b.SSCC_LV3;
	---
		LV5 = select a."SSCC_OngNoi",a."Admission_Date_OngNoi",
 			 b."DistNumber" as "SSCC_Lv5",b."InDate" as "Admission_Date_LV5"
			 from :LV4 a
			 left join OBTN b on a."SSCC_Lv4" = b."U_ORIBatch" ;
			 
	end;

end if;

if exists (select 1 from :LV5) then
	begin
		 insert into #tbl (SSCC_OngNoi,Admission_Date_OngNoi,
		 					
		 					Admission_Date_LV0,SSCC_LV0,
		 					Admission_Date_LV1,SSCC_LV1,
		 					Admission_Date_LV2,SSCC_LV2,
		 					Admission_Date_LV3,SSCC_LV3,
		 					Admission_Date_LV4,SSCC_LV4,
		 					Admission_Date_LV5,SSCC_LV5
		 					)
		 select  a."SSCC_OngNoi",
				 a."Admission_Date_OngNoi",
				 
				 b.Admission_Date_LV0,b.SSCC_Lv0,
				 b.Admission_Date_LV1,b.SSCC_Lv1,
				 b.Admission_Date_LV2,b.SSCC_Lv2,
				 b.Admission_Date_LV3,b.SSCC_Lv3,
				 b.Admission_Date_LV4,b.SSCC_Lv4,
				 a."Admission_Date_LV5",a."SSCC_Lv5"
		 from :Lv5 a
		 inner join  #tbl b on a."SSCC_OngNoi" = b.SSCC_OngNoi and a."SSCC_Lv5"  = b.SSCC_LV4;
		--
		LV6 = select a."SSCC_OngNoi",a."Admission_Date_OngNoi",
 			 b."DistNumber" as "SSCC_Lv6",b."InDate" as "Admission_Date_LV6"
			 from :LV5 a
			 left join OBTN b on a."SSCC_Lv5" = b."U_ORIBatch" ;
			 
	end;

end if;


if exists (select 1 from :Lv6) then
	begin
		 insert into #tbl (SSCC_OngNoi,Admission_Date_OngNoi,
		 					
		 					Admission_Date_LV0,SSCC_LV0,
		 					Admission_Date_LV1,SSCC_LV1,
		 					Admission_Date_LV2,SSCC_LV2,
		 					Admission_Date_LV3,SSCC_LV3,
		 					Admission_Date_LV4,SSCC_LV4,
		 					Admission_Date_LV5,SSCC_LV5,
		 					Admission_Date_LV6,SSCC_LV6
		 					)
		 select  a."SSCC_OngNoi",
				 a."Admission_Date_OngNoi",
				 
				 b.Admission_Date_LV0,b.SSCC_Lv0,
				 b.Admission_Date_LV1,b.SSCC_Lv1,
				 b.Admission_Date_LV2,b.SSCC_Lv2,
				 b.Admission_Date_LV3,b.SSCC_Lv3,
				 b.Admission_Date_LV4,b.SSCC_Lv4,
				  b.Admission_Date_LV5,b.SSCC_Lv5,
				 a."Admission_Date_LV6",a."SSCC_Lv6"
		 from :Lv6 a
		 inner join  #tbl b on a."SSCC_OngNoi" = b.SSCC_OngNoi and a."SSCC_Lv6"  = b.SSCC_LV5;
			 
	end;

end if;
delete from #tbl where ifnull(SSCC_LV0,'') ='';

tab_result =select  * from #tbl;


drop table #tbl;
end

