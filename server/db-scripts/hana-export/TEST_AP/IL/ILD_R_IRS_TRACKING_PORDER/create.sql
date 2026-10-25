CREATE procedure ILD_R_IRS_Tracking_POrder
(
	in fdate	date,
	in tdate	date,
	in DocNumPO		varchar(250) -- ILD_LayPurchaseOrder

)
as
begin
create local temporary table #tbl
(
	RowNum	 		int,
	DocEntry_PO 	int,
	DocNum_PO		int,
	ItemCode		varchar(50),
	Dscription		Nvarchar(250),
	
	Quantity_PO		decimal(21,6),
	OpenQty_PO		decimal(21,6),
	Price_PO		decimal(21,6),
	Amount_PO		decimal(21,6),
	Rate_PO			decimal(21,6),
	
	DocDate_PO		varchar(10),
	CreateDate_PO	varchar(10),
	Status_PO		Nvarchar(15),
	
	DocEntry_GRPO	int,
	DocNum_GRPO  	int,
	Quantity_GRPO	decimal(21,6),
	Price_GRPO		decimal(21,6),
	Amount_GRPO		decimal(21,6),
	Rate_GRPO		decimal(21,6),
	DocDate_GRPO	varchar(10),
	Status_GRPO		Nvarchar(15)
	
);
	insert into #tbl (RowNum, DocEntry_PO,DocNum_PO,ItemCode,Dscription,
						Quantity_PO, OpenQty_PO,Price_PO,Amount_PO,Rate_PO,
						DocDate_PO,CreateDate_PO, Status_PO,
						-- grpo
						DocEntry_GRPO,DocNum_GRPO, Quantity_GRPO,Price_GRPO, Amount_GRPO,Rate_GRPO,
						DocDate_GRPO, Status_GRPO)
	select ROW_NUMBER() OVER( PARTITION BY  a."DocEntry",a."ItemCode",a."Dscription" 
						 ORDER BY a."DocEntry"  asc ) as "RowID",
						 
	a."DocEntry" as "DocEntry_PO",b."DocNum" as "DocNum_PO", a."ItemCode", a."Dscription" as "Dscription",
	
	a."Quantity" as "Quantity_PO", 
	a."OpenQty" as "OpenQty_PO", a."Price" as "Price_PO", 
	a."LineTotal" as "Amount_PO" , 
	a."Rate" as "Rate_PO",
	--
	TO_VARCHAR(TO_DATE(b."DocDate"),'DD/MM/YYYY') as "DocDate_PO", 
	TO_VARCHAR(TO_DATE(b."CreateDate"),'DD/MM/YYYY') as "CreateDate_PO", 
	(case when a."LineStatus" = 'C' then 'Closed' when a."LineStatus" = 'O' then 'Open' end) as "Status_PO",
	-- GRPO
	pdn1."DocEntry" as "DocEntry_GRPO","SoPhieu_GRPO","Quantity_GRPO","Price_GRPO","Amount_GRPO","Rate_GRPO",
	"DocDate_GRPO","Status_GRPO"
	--a."BaseLine" as "BaseLine_PO",
	--a."TargetType",a."TrgetEntry",a."ObjType",
	 --a."BaseEntry" as "BaseEntry_PO",
	-- a."BaseType" as "BaseType_PO",
	--a."LineNum" as "LineNum_PO" ,
	from POR1 a 
	inner join  OPOR b on a."DocEntry"  =  b."DocEntry"

---------- Join Dữ Liệu Nguôn 3 vơi Goods Receipt PO (GRPO)
	left join (select b."DocNum" as "SoPhieu_GRPO","ItemCode", a."DocEntry","BaseLine",
													a."BaseEntry", a."BaseType","TargetType","TrgetEntry" , 
											        "Quantity" as "Quantity_GRPO", 
											        "OpenQty" as "OpenQty_GRPO", 
											        "Price" as "Price_GRPO", "LineTotal" as "Amount_GRPO", 
											        "Rate" as "Rate_GRPO",
											        TO_VARCHAR(TO_DATE(b."DocDate"),'DD/MM/YYYY') as "DocDate_GRPO",
											        "LineNum" as "LineNumGRPO",
											        (case when "LineStatus" = 'C' then 'Closed' when "LineStatus" = 'O' then 'Open' end) as "Status_GRPO"
											        
	from PDN1 a 
	inner join  OPDN b on a."DocEntry"  =  b."DocEntry"
	where  "CANCELED"  =  'N'
	) pdn1  on   pdn1."ItemCode"  =  a."ItemCode" 
	and  pdn1."BaseLine"  =  a."LineNum"
	and a."DocEntry"  = pdn1."BaseEntry" and   a."TargetType" = 20 
	
	
	where  b."CANCELED"  =  'N' and  b."DocDate" between fdate and tdate
		  	and
				(
					ifnull(:DocNumPO, '') = '' 
					or cast (b."DocNum" as nvarchar) in (select "DATA" from split(:DocNumPO ,','))
				);
	--update open Po quantity  =0 ,chi giu lai dong cuoi
	update a
	set  Quantity_PO = 0,OpenQty_PO  =0
	from #tbl a
	where RowNum  not in (select max(RowNum) from #tbl b 
			where a.DocEntry_PO||a.ItemCode||a.Dscription = b.DocEntry_PO||b.ItemCode||b.Dscription );			
	--
	select RowNum "RowID",DocEntry_PO "DocEntry_PO",
			DocNum_PO "DocNum_PO",
			ItemCode "ItemCode",
			Dscription "Dscription",
			Quantity_PO "Quantity_PO",
			OpenQty_PO "OpenQty_PO",
			Price_PO "Price_PO",
			Amount_PO "Amount_PO",Rate_PO "Rate_PO",
			DocDate_PO "DocDate_PO",CreateDate_PO "CreateDate_PO",Status_PO "Status_PO",
			-- GRPO
			DocEntry_GRPO"DocEntry_GRPO",
			DocNum_GRPO "DocNum_GRPO",
			Quantity_GRPO "Quantity_GRPO",
			Price_GRPO "Price_GRPO",
			Amount_GRPO "Amount_GRPO",
			Rate_GRPO "Rate_GRPO",
			DocDate_GRPO "DocDate_GRPO",
			Status_GRPO "Status_GRPO" 
	from #tbl
	order by  DocNum_PO,RowNum,ItemCode;
	drop table #tbl;

end;

