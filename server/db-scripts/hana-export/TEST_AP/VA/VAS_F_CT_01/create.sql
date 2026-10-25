CREATE procedure VAS_F_CT_01 (
	in DocID	int
)
as
begin
	declare bplID int;
	declare DecQty, DecPrice int;
	declare DateSep, ThousSep nvarchar(10);
	declare num1, num2 numeric(21,6);
	declare tCompName, tCompAddr, tCompTel, tCompFax, tCompWebsite, tCompEmail nvarchar(100);
	
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
		, ItemCode		nvarchar(50)
		, ItemName		nvarchar(100)
		, UoM			nvarchar(50)
		, WhsCode		nvarchar(20)
		, WhsName		nvarchar(100)
		, OnBookQty		numeric(21,6)
		, ActualQty		numeric(21,6)
		, DiffQty		numeric(21,6)
		, Deci_OnBook	int
		, Deci_Actual	int
		, Deci_Diff		int
		, DateTitle		nvarchar(100)
		, Logo			nvarchar(100)
		, Note 			nvarchar(1000)
	);	

	-- REPORT ----------------------------------------------------------------------------------------------------------------
	insert into #tbl (CountDate,CountTime,ItemCode,ItemName,WhsCode,OnBookQty,ActualQty,DiffQty,Note)
	select a."CountDate",a."Time",b."ItemCode",b."ItemDesc",b."WhsCode",b."InWhsQty",b."CountQty",b."Difference",a."Remarks" from "OINC" a, "INC1" b where a."DocEntry"=b."DocEntry" and a."DocEntry"=DocID;
	
	update #tbl set CountTime = RIGHT('0'||CountTime,4);

	update #tbl set 
		FormNo 		= 'Mẫu số 05 - VT', 
		IssueNote 	= '(Ban hành theo Thông tư số: 200/2014/TT-BTC ngày 22/12/2014 của BTC)', 
		RptName 	= 'BIÊN BẢN KIỂM KÊ VẬT TƯ, CÔNG CỤ, SẢN PHẨM, HÀNG HOÁ',
		PeriodTitle = 'Thời gian kiểm kê: ' || LEFT(CountTime,2) || 'h' || RIGHT(CountTime,2) || '  ngày ' || to_varchar(CountDate,'DD/MM/YYYY')
		;
	update x set CompName = y."CompnyName", CompAdd = y."CompnyAddr", UoM = z."InvntryUom", WhsName = t."WhsName" from #tbl x, "OADM" y, "OITM" z, "OWHS" t where x.ItemCode=z."ItemCode" and x.WhsCode=t."WhsCode";
	
	update #tbl set Deci_OnBook = case when Floor(OnBookQty)<>OnBookQty then 2 else 0 end;
	update #tbl set Deci_Actual = case when Floor(ActualQty)<>ActualQty then 2 else 0 end;
	update #tbl set Deci_Diff = case when Floor(DiffQty)<>DiffQty then 2 else 0 end;
	
	select * from #tbl;
	drop table #tbl;
end

