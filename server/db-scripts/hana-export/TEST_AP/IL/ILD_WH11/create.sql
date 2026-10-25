CREATE procedure ILD_WH11 (
	in fdate			timestamp,
	in tdate			timestamp,
	in groupID			int,
	in warehouse		nvarchar(20),
	in account			nvarchar(20),
	in itemcode			nvarchar(5000),
	in cancel			int
)
as
begin
	call ILD_WH09_TB (fdate,tdate,:groupID,:warehouse,:account,:itemcode,:cancel,TB);
	create local temporary table #RS  (
		ID				int,
		ItemCode		nvarchar(50),
		ItemName		nvarchar(100),
		ItemUoM			nvarchar(50),
		
		OBQty			numeric(21,6),
		OBAmt			numeric(21,6),
		
		INQty			numeric(21,6),
		INAmt			numeric(21,6),
		
		OUQty			numeric(21,6),
		OUAmt			numeric(21,6),
		BalQty			numeric(21,6),
		BalAmt			numeric(21,6)
	); 
	--
	insert into #RS (ID,ItemCode,ItemName,ItemUoM,OBQty,OBAmt,INQty,INAmt,OUQty,OUAmt,BalQty,BalAmt)
	select --* 
		ROW_NUMBER() OVER () 	"STT"
		,ItemCode		"Mã sản sản phẩm"
		,ItemName		"Tên sản phẩm"
		--,Comments		"Diễn giải mục đích xuất nhập kho"
		,ItemUoM		"ĐVT"
		
		,null
		,null
		
		,sum(INQty)			"Số lượng Nhập"
		,sum(INAmt)			"Trị giá NT Nhập"
		
		,sum(OUQty)			"Số lượng Xuất"
		,sum(OUAmt)			"Trị giá NT Xuất"
		
		,sum(BalQty)			"Số lượng Tồn cuối"
		,sum(BalAmt)			"Trị giá Tồn cuối"

		--,cast ((sum(BalAmt) / case when sum(BalQty) = 0 then null else sum(BalQty) end) as numeric(21,2))  "Đơn giá trung bình"
		
	from $TB
	where Pos = 3
	group by ItemCode,ItemName,ItemUoM
	;
	
	-- update dau ky
	update x set OBQty = (select sum(BalQty) from $TB where Pos  = 1 and ItemCode = x.ItemCode) from #RS x;	
	update x set OBAmt = (select sum(BalAmt) from $TB where Pos  = 1 and ItemCode = x.ItemCode) from #RS x;
	
	-- show
	select 
		ID 	"STT"
		,ItemCode		"Mã NVL"
		,ItemName		"Tên NVL"
		,ItemUoM		"ĐVT"
		
		,OBQty			"Số lượng Đầu kỳ"
		,OBAmt			"Trị giá Đầu kỳ"
		
		,INQty			"Số lượng Nhập"
		,INAmt			"Trị giá NT Nhập"
		
		,OUQty			"Số lượng Xuất"
		,OUAmt			"Trị giá NT Xuất"
		
		,BalQty			"Số lượng Tồn cuối"
		,BalAmt			"Trị giá Tồn cuối" 
		
		,cast (BalAmt / (case when BalQty = 0 then null else BalQty end) as numeric(21,2))  "Đơn giá trung bình"
	from #RS;
	
	drop table #RS;
end;

