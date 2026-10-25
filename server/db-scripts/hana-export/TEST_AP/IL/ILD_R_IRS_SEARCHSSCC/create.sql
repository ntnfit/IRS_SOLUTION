CREATE procedure ILD_R_IRS_SearchSSCC
(
	in FromDate date,
	in ToDate date,
	in DocType varchar(15),
	in SSCC		varchar(250)
)
as 
begin
create local temporary table #tbl
(
	WOID int,
	WONO varchar(50),
	SSCC varchar(50),
	ItemCode varchar(50),
	ItemName nvarchar(100),
	DocID int,
	DocNum varchar(50),
	DocType int
);
---
IF(:DocType  = '111') then -- All --> Search theo SSCC
	---Tìm kiếm theo SSCC 
	insert into #tbl (WOID,WONO, SSCC,ItemCode,ItemName,DocID,DocType)
	select distinct a."BsDocEntry" as "WOID",b."DocNum" as "WONO",a."BatchNum" as "SSCC",a."ItemCode",a."ItemName", a."BaseEntry" as "DocID",a."BaseType" as "DocType"
	from  IBT1 a
	left join OWOR b on a."BsDocEntry"= b."DocEntry"  
	WHERE  a."BatchNum" in (select "DATA" from split(:SSCC ,','));
	
	---
			update a
			set DocNum  =  b."DocNum"
			from #tbl a
			inner join OIGN b on a.DocID = b."DocEntry"
			where a.DocType =  59;
			--
			update a
			set DocNum  =  b."DocNum"
			from #tbl a
			inner join OIGE b on a.DocID = b."DocEntry"
			where a.DocType =  60;
			
			update a
			set DocNum  =  b."DocNum"
			from #tbl a
			inner join OWTQ b on a.DocID = b."DocEntry"
			where a.DocType =  '1250000001';
			--
			update a
			set DocNum  =  b."DocNum"
			from #tbl a
			inner join OWTR b on a.DocID = b."DocEntry"
			where a.DocType =  '67';
			--
			update a
			set DocNum  =  b."DocNum"
			from #tbl a
			inner join ORDN b on a.DocID = b."DocEntry"
			where a.DocType =  '16';
	---
	Else
		Begin
			If ( :DocType  = '59' or :DocType  = '60') then
			insert into #tbl (WOID,WONO, SSCC,ItemCode,ItemName,DocID,DocType)
			select  a."BsDocEntry" as "WOID",b."DocNum" as "WONO",a."BatchNum" as "SSCC",a."ItemCode",a."ItemName", a."BaseEntry" as "DocID",a."BaseType" as "DocType"
			from  IBT1 a
			inner join OWOR b on a."BsDocEntry"= b."DocEntry"  
			WHERE "BaseType" in ('59', '60') and "BsDocType"  = 202 	and "DocDate" between FromDate And ToDate;	
			--
			--
		
			update a
			set DocNum  =  b."DocNum"
			from #tbl a
			inner join OIGN b on a.DocID = b."DocEntry"
			where a.DocType =  59;
			--
			update a
			set DocNum  =  b."DocNum"
			from #tbl a
			inner join OIGE b on a.DocID = b."DocEntry"
			where a.DocType =  60;
			 ------ RETURN: SALES
			elseIf ( :DocType  = '16') then 
				insert into #tbl (WOID,WONO, SSCC,ItemCode,ItemName,DocID,DocType)
				select  a."BsDocEntry" as "WOID",b."DocNum" as "WONO",a."BatchNum" as "SSCC",a."ItemCode",a."ItemName", a."BaseEntry" as "DocID",a."BaseType" as "DocType"
				from  IBT1 a
				left join OWOR b on a."BsDocEntry"= b."DocEntry"  
				WHERE "BaseType" = '16'	and "DocDate" between FromDate And ToDate;	
			
						
				update a
				set DocNum  =  b."DocNum"
				from #tbl a
				inner join ORDN b on a.DocID = b."DocEntry"
				where a.DocType =  '16'; ---  KET THUC RETURN
				
				
		else -- Transfer request va transfer
			insert into #tbl (WOID,WONO, SSCC,ItemCode,ItemName,DocID,DocType)
			select  a."BsDocEntry" as "WOID",b."DocNum" as "WONO",a."BatchNum" as "SSCC",a."ItemCode",a."ItemName", a."BaseEntry" as "DocID",a."BaseType" as "DocType"
			from  IBT1 a
			left join OWOR b on a."BsDocEntry"= b."DocEntry"  
			WHERE "BaseType" in ('1250000001', '67')	and "DocDate" between FromDate And ToDate;	
			--
			--
		
			update a
			set DocNum  =  b."DocNum"
			from #tbl a
			inner join OWTQ b on a.DocID = b."DocEntry"
			where a.DocType =  '1250000001';
			--
			update a
			set DocNum  =  b."DocNum"
			from #tbl a
			inner join OWTR b on a.DocID = b."DocEntry"
			where a.DocType =  '67';
		
		
				End if;
		End;
END IF;

 
--
select * from #tbl 
--where DocType  = :DocType
order by  WONO, DocType;
--
drop table #tbl;
	
end;

