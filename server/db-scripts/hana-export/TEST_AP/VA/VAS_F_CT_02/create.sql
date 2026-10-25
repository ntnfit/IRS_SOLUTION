CREATE procedure VAS_F_CT_02 (
	in DocID	int
)
as
begin
	declare bplID int;
	declare DecQty, DecPrice int;
	declare DateSep, ThousSep nvarchar(10);
	declare num1, num2 numeric(21,6);
	declare tCompName, tCompAddr, tCompTel, tCompFax, tCompWebsite, tCompEmail nvarchar(100);
	declare tDocDate timestamp;
	
	select "CountDate" into tDocDate from OINC where "DocEntry"=DocID;
	
	----------------------------------------------------------------------------------------------------------------------------
	create local temporary table #tbl (
		FormNo			nvarchar(50)
		, IssueNote		nvarchar(100)
		, RptName		nvarchar(200)
		, PeriodTitle	nvarchar(100)
		, CompName		nvarchar(200)
		, CompAdd		nvarchar(300)
		, CountDate		timestamp
		, CountTime		nvarchar(10)
		, LineNum		int
		, ItemCode		nvarchar(50)
		, ItemName		nvarchar(100)
		, Manufacturer	nvarchar(100)
		, BatchNum		nvarchar(100)
		, QuyCach		nvarchar(100)
		, UoM			nvarchar(50)
		, ProdDate		timestamp
		, ExpiredDate	timestamp
		, WhsCode		nvarchar(20)
		, WhsName		nvarchar(100)
		, BinCode		nvarchar(100)
		, OnBookQty		numeric(21,6)
		, ActualQty		numeric(21,6)
		, DiffQty		numeric(21,6)
		, DiffQty2		numeric(21,6)
		, ToanVenBB		nvarchar(30)
		, CamQuanCL		nvarchar(30)
		, LineNote		nvarchar(100)
		, Deci_OnBook	int
		, Deci_Actual	int
		, Deci_Diff		int
		, DateTitle		nvarchar(100)
		, Logo			nvarchar(100)
		, Note 			nvarchar(1000)
	);	
	
	-- Tồng kho theo BATCH
	tblBIN_BATCH_DETAIL = 	
				select a."BinAbs",d."BinCode",b."ItemCode",b2."DistNumber",b1."Quantity",b."LocCode",b."DocDate",b."DocType",b."DocEntry",b."DocLine"
				from OITL b
				inner join ITL1 b1 on (b1."LogEntry"=b."LogEntry" and b1."ItemCode"=b."ItemCode")
				inner join OBTN b2 on (b2."SysNumber"=b1."SysNumber" and b2."ItemCode"=b1."ItemCode")
				left join OBTL a on b."LogEntry"=a."ITLEntry" and b1."MdAbsEntry"=a."SnBMDAbs"
				left join OBIN d on d."AbsEntry"=a."BinAbs"
				;
	tblBIN_BATCH = select "ItemCode","DistNumber","LocCode","BinAbs",ifnull("BinCode",'') as "BinCode",sum("Quantity") as "Quantity" from :tblBIN_BATCH_DETAIL where "DocDate"<=tDocDate group by "ItemCode","DistNumber","LocCode","BinAbs",ifnull("BinCode",'');
	
	tbl_BOOK = 
	select a."LineNum",a."ItemCode",a."WhsCode",case when c."ManBtchNum"='Y' then b."BinCode" else d."BinCode" end as "BinCode",b."DistNumber",case when c."ManBtchNum"='Y' then b."Quantity" else a."InWhsQty" end as "BookQty", cast(0 as numeric(19,6)) as "CountQty"
	from INC1 a
	left join OITM c on c."ItemCode"=a."ItemCode"
	left join :tblBIN_BATCH b on (b."ItemCode"=a."ItemCode" and b."LocCode"=a."WhsCode" and b."BinAbs"=a."BinEntry")
	left join OBIN d on d."AbsEntry"=a."BinEntry"
	where a."DocEntry"=DocID
	;
	
	tbl_BATCH = 
	select "ItemCode","ObjType","AbsEntry","DistNumber","MnfDate","ExpDate" from OBTN UNION ALL
	select "ItemCode","ObjType","AbsEntry","DistNumber","MnfDate","ExpDate" from ODBN;
	
	tbl_COUNTED = 
	select a."LineNum",a."ItemCode",a."WhsCode",b."BinCode",e."DistNumber", cast(0 as numeric(19,6)) as "BookQty",case when c."ManBtchNum"='Y' then d."Quantity" else a."CountQty" end as "CountQty"
	from INC1 a
	left join OBIN b on b."AbsEntry"=a."BinEntry"
	left join OITM c on c."ItemCode"=a."ItemCode"
	left join INC3 d on (d."LineNum"=a."LineNum")
	left join :tbl_BATCH e on (e."ItemCode"=d."ItemCode" and e."AbsEntry"=d."ObjAbs")
	where a."DocEntry"=DocID;
	
	-- REPORT ----------------------------------------------------------------------------------------------------------------
	insert into #tbl (LineNum,ItemCode,BatchNum,WhsCode,BinCode,OnBookQty,ActualQty)
	select T0."LineNum",T0."ItemCode",ifnull(T0."DistNumber",''), ifnull(T0."WhsCode",''),ifnull(T0."BinCode",''),sum(T0."BookQty"),sum(T0."CountQty") 
	from (select * from :tbl_BOOK a union all select * from :tbl_COUNTED b) T0
	group by T0."LineNum",T0."ItemCode",ifnull(T0."DistNumber",''), ifnull(T0."WhsCode",''),ifnull(T0."BinCode",'');
	
	update x set CountDate = y."CountDate", CountTime = y."Time" from #tbl x inner join OINC y on y."DocEntry"=DocID;
	update x set ItemName = y."ItemName", Manufacturer = z."FirmName", UoM = y."InvntryUom" from #tbl x inner join OITM y on y."ItemCode"=x.ItemCode left join OMRC z on z."FirmCode"=y."FirmCode";
	update x set ProdDate = y."MnfDate", ExpiredDate = y."ExpDate" from #tbl x inner join :tbl_BATCH y on y."ItemCode"=x.ItemCode and y."DistNumber"=x.BatchNum;
	
	update #tbl set CountTime = RIGHT('0'||CountTime,4);

	update #tbl set DiffQty = ifnull(OnBookQty,0) - ifnull(ActualQty,0) where ifnull(OnBookQty,0) > ifnull(ActualQty,0);
	update #tbl set DiffQty2 = ifnull(ActualQty,0) - ifnull(OnBookQty,0) where ifnull(OnBookQty,0) < ifnull(ActualQty,0);
	
	update #tbl set 
		FormNo 		= 'BM/KT/005', 
		RptName 	= 'BIÊN BẢN KIỂM KÊ HÀNG HOÁ',
		PeriodTitle = '1. Thời điểm kiểm kê: ' || LEFT(CountTime,2) || 'h' || RIGHT(CountTime,2) || '  ngày ' || to_varchar(CountDate,'DD')  || '  tháng ' || to_varchar(CountDate,'MM')  || '  năm ' || to_varchar(CountDate,'YYYY')
		;
	update x set CompName = y."CompnyName", CompAdd = y."CompnyAddr", WhsName = t."WhsName" from #tbl x, "OADM" y, "OWHS" t where x.WhsCode=t."WhsCode";
	
	update #tbl set Deci_OnBook = case when Floor(OnBookQty)<>OnBookQty then 2 else 0 end;
	update #tbl set Deci_Actual = case when Floor(ActualQty)<>ActualQty then 2 else 0 end;
	update #tbl set Deci_Diff = case when Floor(DiffQty)<>DiffQty then 2 else 0 end;
	
	select * from #tbl order by LineNum;
	drop table #tbl;
end

