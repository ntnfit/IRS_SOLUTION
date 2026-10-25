CREATE PROCEDURE ILD_R_IRS_ChiTietXuatNhapTonTheoBatch_Glive_Lan1 (
	in fdate			timestamp,
	in tdate			timestamp,
	in litemcode		nvarchar(5000),
	in lwhs				nvarchar(5000),
	in litemgroup		nvarchar(5000),
	in account			nvarchar(20),
	
	out result_out table (
		WhsCode				nvarchar(8)
		,WhsName			nvarchar(100)
		,WhsAddress			nvarchar(100)
		,ItemGroup			smallint
		,ItemGroupName		nvarchar(100)
		,ItemCode			nvarchar(50)
		,ItemName			nvarchar(100)

		,UoM				nvarchar(50)
		,SSCC				nvarchar(100)
		,Batch				nvarchar(100)
		,SSCC_Status		nvarchar(20)
		,ExpiredDate		timestamp
		,CreatedTime		nvarchar(15)
		,ProdDate			timestamp
		
		,OBQty				numeric(19,6) 
		,InQty				numeric(19,6)
		,OutQty				numeric(19,6)
		,EBQty				numeric(19,6)
		
		,Bin				nvarchar(100)
		,AdmissiongDate		timestamp
		,Father1			varchar(100)
		,AdmissionDate_Father1	date
		,ORIBatch_Del 		nvarchar(50)
	) 

)
AS
BEGIN
	declare  tbl table
	(
		WhsCode				nvarchar(8)
		,WhsName			nvarchar(100)
		,WhsAddress			nvarchar(100)
		,ItemGroup			smallint
		,ItemGroupName		nvarchar(100)
		,ItemCode			nvarchar(50)
		,ItemName			nvarchar(100)

		,UoM				nvarchar(50)
		,SSCC				nvarchar(100)
		,Batch				nvarchar(100)
		,SSCC_Status		nvarchar(20)
		,ExpiredDate		timestamp
		,CreatedTime		nvarchar(15)
		,ProdDate			timestamp
		
		,OBQty				numeric(19,6) 
		,InQty				numeric(19,6)
		,OutQty				numeric(19,6)
		,EBQty				numeric(19,6)
		
		,Bin				nvarchar(100)
		,AdmissiongDate		timestamp
		,Father1			varchar(100)
		,AdmissionDate_Father1	date
		,ORIBatch_Del 		nvarchar(50)
	);
 	call ILD_TruyXuatSSCCNhieuCap (tab_result); 
	create local temporary table #TruyXuatSSCC as (select * from :tab_result);

	
	 

	
-- Batch
	lcsql	= select d."WhsCode",d."WhsName", f."ItmsGrpCod",f."ItmsGrpNam",c."ItemCode",c."ItemName",c."InvntryUom",
			e."DistNumber", a."DocType", a."DocEntry", a."DocLine", a."DocDate",			
			
							case when b."Quantity" > 0 then b."Quantity" else 0 end as "InQty",
			 				case when b."Quantity" < 0 then -b."Quantity" else 0 end as "OutQty"
			 , e."InDate"
			 	
	from OITL a inner join ITL1 b on a."LogEntry"=b."LogEntry" and a."ItemCode"=b."ItemCode" 
				inner join OITM c on c."ItemCode"=b."ItemCode"
				left join OWHS d on d."WhsCode"=a."LocCode"
	
				left join OBTN e on e."ItemCode"=b."ItemCode" and e."AbsEntry"=b."MdAbsEntry"
				
				left join OITB f on f."ItmsGrpCod"=c."ItmsGrpCod"
				
				inner join oinm g on a."DocType" = g."TransType" and a."DocEntry" = g."CreatedBy" and a."DocLine" = g."DocLineNum" and a."LocCode" = g."Warehouse"

	where a."DocDate" <= :tdate  
	
		and ( (ifnull(:lwhs, '') = '') or (a."LocCode" in (select Data from split(:lwhs, ',')) ))
		
		and ( (ifnull(:litemcode, '') = '') or (c."ItemCode" in (select Data from split(:litemcode, ',')) ))
						
		and ( (ifnull(litemgroup, '') = '') or (cast(c."ItmsGrpCod" as nvarchar) in (select Data from split(litemgroup, ',')) )) 
	
		and (account='' 
		or (ifnull(g."InvntAct",'')<>'' and g."InvntAct"=account) 
		or (ifnull(g."InvntAct",'')='' and c."GLMethod"='C' and f."BalInvntAc"=account)
		or (ifnull(g."InvntAct",'')='' and c."GLMethod"='W' and d."BalInvntAc"=account)
		) ;


		
	insert into :tbl (WhsCode,WhsName, ItemGroup, ItemGroupName, ItemCode, ItemName, UoM, SSCC, AdmissiongDate)
	select distinct "WhsCode","WhsName", "ItmsGrpCod","ItmsGrpNam","ItemCode","ItemName","InvntryUom", "DistNumber", "InDate"		 	
	from :lcsql ;


	update x set OBQty = (select sum(ifnull("InQty",0) - ifnull("OutQty",0))
							from :lcsql
							where "DocDate" < :fdate and "ItemCode" = x.ItemCode and x.WhsCode = "WhsCode" and x.SSCC = "DistNumber")
	from :tbl x ;


	update x set InQty = (select SUM(CASE when "DocType" in ('14','16','18','20','69','162') then IFNULL("InQty",0)-IFNULL("OutQty",0) else CASE when "DocType" not in ('13','15','19','21') then "InQty" end end)
							from :lcsql
							where "DocDate" between :fdate and :tdate and
									"ItemCode" = x.ItemCode and x.WhsCode = "WhsCode" and x.SSCC = "DistNumber")
	from :tbl x ;

	update x set OutQty = (select SUM(CASE when "DocType" in ('13','15','19','21') then -IFNULL("InQty",0)+IFNULL("OutQty",0) else CASE when "DocType" not in ('14','16','18','20','69','162') then "OutQty" end end)
							from :lcsql
							where "DocDate" between :fdate and :tdate and
									"ItemCode" = x.ItemCode and x.WhsCode = "WhsCode" and x.SSCC = "DistNumber") 
	from :tbl x ;
	


	update :tbl set EBQty = ifnull(OBQty,0) + ifnull(InQty,0) - ifnull(OutQty,0) ;
	
	delete from :tbl where ifnull(OBQty,0) = 0 and ifnull(InQty,0) = 0 and ifnull(OutQty,0) = 0 and ifnull(EBQty,0) = 0  ;
	
--
	lcsql1 = select a."LocCode", c."ItemCode", e."DistNumber", T5."BinCode" 
				from OITL a 
				inner join ITL1 b on a."LogEntry"=b."LogEntry" and a."ItemCode"=b."ItemCode" and b."Quantity" <> 0
		
				left join OBTN e on e."ItemCode"=b."ItemCode" and e."SysNumber"=b."SysNumber"
				
				inner JOIN OBTL T4 ON a."LogEntry"   = t4."ITLEntry" and T4."SnBMDAbs" = e."AbsEntry"
				left JOIN OBIN T5 ON T5."AbsEntry" = T4."BinAbs"
				
				inner join OITM c on c."ItemCode"=b."ItemCode"

	where a."DocDate"<=:tdate	
	group by a."LocCode", c."ItemCode", e."DistNumber", T5."BinCode"
	having sum(case when b."Quantity" > 0 then t4."Quantity" else -t4."Quantity" end) > 0 ;
 	
 	update x set Bin = y."BinCode"
 	from :tbl x
 	inner join :lcsql1 y on x.WhsCode = y."LocCode" and  x.ItemCode = y."ItemCode" and  x.SSCC = y."DistNumber"
 	where  EBQty > 0 ;
 	

--	
	update x set  Batch = y."MnfSerial", SSCC_Status = y."LotNumber", ExpiredDate = y."ExpDate", ProdDate = ifnull(y."MnfDate",y."InDate"), 
	 	CreatedTime= case when LENGTH(y."U_CreatedTime")  = 1 then  '00:0'|| y."U_CreatedTime"
							 when LENGTH(y."U_CreatedTime")  = 2 then  '00:'|| y."U_CreatedTime"				
							 when LENGTH(y."U_CreatedTime")  = 3 then  '0'||left(y."U_CreatedTime",1) || ':' || right(y."U_CreatedTime",2) 
		 					 when LENGTH(y."U_CreatedTime")  = 4 then   left(y."U_CreatedTime",2) || ':' || right(y."U_CreatedTime",2)  
	 			else  '' end 
	from :tbl x inner join obtn y on x.ItemCode = y."ItemCode" and x.SSCC = y."DistNumber" ;
	-- Update SSCC Ông Nội, và ngày tạo 
if exists (select 1 from :tbl ) then
	begin

		 -- LV0
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join #TruyXuatSSCC b on   a.SSCC = b.SSCC_LV0
		 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = '' 
		 and ifnull(b.SSCC_LV4,'') = '' and ifnull(b.SSCC_LV3,'') = '' 
		 and  ifnull(b.SSCC_LV2,'') =''  and ifnull(b.SSCC_LV1,'') = ''and  ifnull(b.SSCC_LV0,'') <> '';
		 		 -- LV1
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join #TruyXuatSSCC b on   a.SSCC = b.SSCC_LV1
		 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = ''
		  and ifnull(b.SSCC_LV4,'') = '' and ifnull(b.SSCC_LV3,'') = '' and  ifnull(b.SSCC_LV2,'') =''  and  ifnull(b.SSCC_LV1,'') <> '';
		  		 -- LV2
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join #TruyXuatSSCC b on   a.SSCC = b.SSCC_LV2
		 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = '' 
		 and ifnull(b.SSCC_LV4,'') = '' and ifnull(b.SSCC_LV3,'') = ''   and  ifnull(b.SSCC_LV2,'') <> '';
		 		 -- LV3
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join #TruyXuatSSCC b on   a.SSCC = b.SSCC_LV3
		 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = '' and ifnull(b.SSCC_LV4,'') = '' and  ifnull(b.SSCC_LV3,'') <> '';
		 		 -- LV4
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join #TruyXuatSSCC b on   a.SSCC = b.SSCC_LV4
		 where ifnull(b.SSCC_LV6,'') = '' and ifnull(b.SSCC_LV5,'') = '' and  ifnull(b.SSCC_LV4,'') <> '';
		 		 -- LV5
		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join #TruyXuatSSCC b on   a.SSCC = b.SSCC_LV5
		 where ifnull(b.SSCC_LV6,'') = '' and  ifnull(b.SSCC_LV5,'') <> '';
		 		 -- LV6
		 		 update a
		 set a.Father1  = b.SSCC_OngNoi,
		 	 a.AdmissionDate_Father1 = b.Admission_Date_OngNoi
		 from :tbl a
		 inner join #TruyXuatSSCC b on   a.SSCC = b.SSCC_LV6
		 where ifnull(b.SSCC_LV6,'') <> '';


	
	end;
end if;
--- Update  SSCC Ông Nội, và ngày tạo  = chính nó nếu nó là kẻ FA

	-- Kiem tra co phai return hay khong (neu return se co gia tri U_ORIBatch_Del )
update a
set a.ORIBatch_Del = b."U_ORIBatch_Del",
	a.Father1 = b."U_ORIBatch_Del",
	a.AdmissionDate_Father1  = c."InDate"
from :tbl a
inner join OBTN b on a.SSCC = b."DistNumber"
inner join OBTN c on  c."DistNumber" = b."U_ORIBatch_Del"  
where ifnull(a.Father1,'') = '' and ifnull(b."U_ORIBatch_Del",'')  <> '';

 

	
update a 
set a.Father1  = SSCC,
	a.AdmissionDate_Father1 = AdmissiongDate
from  :tbl a
where ifnull(a.Father1,'') = '';
	
 
			
	
result_out =	select * from :tbl order by ItemCode;	
	--
	--drop table #tbl  ;
	drop table #TruyXuatSSCC;

END;

