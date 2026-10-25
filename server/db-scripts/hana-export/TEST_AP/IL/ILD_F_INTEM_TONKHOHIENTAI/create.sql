CREATE procedure ILD_F_inTem_TonKhoHienTai
(
	SSCC_List table (SSCC Nvarchar(36)),
	out tblOut table
	(
		"DOCENTRY" int,
		"ItemCode" nvarchar(50),
		"ItemName" nvarchar(250),
		"Batch" nvarchar(50),
		"SSCC" nvarchar(50),
		"Quantity" decimal(19,6),
		"ManufacturingDate" date,
		"AdmissionDate" date,
		"ExpirationDate" date,
		"Unit" nvarchar(20),
		"NetContent" nvarchar(150),
		"Qty_Decimal" decimal(19,6),
		"Re_Manuf_date_title" nvarchar(150)
		
	)

)
as
Begin
	
		declare tbl table
		(
			"DOCENTRY" int,
			"ItemCode" nvarchar(50),
			"ItemName" nvarchar(250),
			"Batch" nvarchar(50),
			"SSCC" nvarchar(50),
			"Quantity" decimal(19,6),
			"ManufacturingDate" date,
			"AdmissionDate" date,
			"ExpirationDate" date,
			"Unit" nvarchar(20),
			"NetContent" nvarchar(150),
			"Qty_Decimal" decimal(19,6),
			"Re_Manuf_date_title" nvarchar(150)
		);
		--insert into :tblSSCC (STT,SSCC) select  "id","part" from "SPLITSTRING"(SSCC_List,',');
		
		
		
		insert into :tbl ("DOCENTRY","ItemCode","ItemName","Batch",
							"SSCC","Quantity","ManufacturingDate",
							"AdmissionDate","ExpirationDate","Unit",
							"NetContent","Qty_Decimal","Re_Manuf_date_title")
							
		Select distinct  0 as "DOCENTRY",c."ItemCode",b."ItemName", "MnfSerial" as "Batch",  
							c."DistNumber",d."OnHandQty", c."MnfDate",
							
							c."InDate" as  "AdmissionDate",
							c."ExpDate" as "ExpirationDate",
							b."InvntryUom" as "Unit",
							'' as "NetContent",
							1 as "Qty_Decimal",
							'Receiving date' as "Re_Manuf_date_title"
		from OBTN c  
		left join OITM b on c."ItemCode" = b."ItemCode"
		inner join   OBBQ d on c."AbsEntry" = d."SnBMDAbs"
		where
			 (
				ifnull((select STRING_AGG(SSCC) from :SSCC_List), '') = '' 
				or   c."DistNumber"  in (select SSCC from :SSCC_List)
			)
			and d."OnHandQty" >0;
		--
		tblOut  = select * from  :tbl;
End;
