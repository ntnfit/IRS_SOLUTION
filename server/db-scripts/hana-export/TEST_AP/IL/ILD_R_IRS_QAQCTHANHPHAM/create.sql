CREATE procedure ILD_R_IRS_QAQCThanhPham
(
	FromDate date,
	ToDate date
)
as
Begin

	Create local temporary table #tbl 
	(
		ProID int,
		ProNo int,
		ProductCode nvarchar(50),
		ProductName nvarchar(100),
		
		ReceiptID int,
		ReceiptNo int,
		ReceiptDate date,
		Qty decimal(19,6),
		SSCC nvarchar(20),
		Batch nvarchar(50)
		
	);
	insert into #tbl (ProID,ProNo, ProductCode, ProductName,
						ReceiptID,ReceiptNo, ReceiptDate,Qty,
						SSCC)
	
	select a."DocEntry" as "ProID",
		   a."DocNum" as "ProNo",
		   b."ItemCode" as "ProductCode",
		   b."ItemName" as "ProductName",
		   
		   b."ReceiptID",
		   b."ReceiptNo",
		   b."ReceiptDate",
		   b."Quantity" as "Qty",
		   
		   b."SSCC"
		   
	from  OWOR a
	left join ( select "ItemCode","ItemName","BaseEntry" as "ReceiptID", "BaseNum" as "ReceiptNo" , "DocDate" as "ReceiptDate", 
					"Quantity", "BatchNum" as "SSCC", "BsDocEntry"
				from IBT1 
				where  "BaseType"  =  59 and "BsDocType"  = 202 ) b on b."BsDocEntry"  =   a."DocEntry" -- and b."ItemCode"  = a."ItemCode" 
				
				 
			
	where  a."StartDate" between FromDate and Todate  and ifnull("ReceiptDate",'') <> ''   
		   and a."Status"  <> 'C';
	
--- Update thông tin Batch
	update a
	set  a.Batch= b."SuppSerial"
	from #tbl a
	inner join OIBT b on a.SSCC  =  b."BatchNum";
	
select ProID, ProNo,
		   ReceiptDate, ProductCode, ProductName,
		    ReceiptID,  ReceiptNo, Qty, SSCC, Batch
	from #tbl;
	
	drop table #tbl;

End;

