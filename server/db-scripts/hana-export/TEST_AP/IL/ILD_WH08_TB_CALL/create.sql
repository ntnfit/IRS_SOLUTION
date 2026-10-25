CREATE procedure ILD_WH08_TB_CALL (
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

call ILD_WH08_TB (fdate,tdate,:groupID,:warehouse,:account,:itemcode,:cancel,TB);
select --* 
	DocDate 		"Ngày"
	,DocID			"DocID"
	,DocNum			"Số chứng từ nhập / xuất kho"
	,SoToKhai		"Số tờ khai"
	,Invoice		"Số Invoice"
	,InvoiceDate	"Ngày Invoice"
	,BPCode			"Mã khách hàng"
	,BPName			"Tên khách hàng"	
	,PO				"Đơn hàng"
	,ItemCode		"Mã NVL"
	,ItemName		"Tên NVL"
	,DocType		
	,DocNote		"Doc Note"
	,ItemUoM		"ĐVT"
	,MaMD			"Mã mục đích xuất nhập kho"
	,TenMD			"Mục đích xuất nhập kho"
	,INQty			"Số lượng Nhập"
	,INPrice		"Đơn giá nhập"	
	,INAmt			"Trị giá NT Nhập"
	,OUQty			"Số lượng Xuất"
	,OUPrice		"Đơn giá xuất"	
	,OUAmt			"Trị giá NT Xuất"
	
	,BalQty			"Số lượng Tồn cuối"
	,BalPrice		"Đơn giá Tồn cuối"
	,BalAmt			"Trị giá Tồn cuối"
	,Pos
	,Comments		"Diễn giải mục đích xuất nhập kho"
	,DebitAcc		"Tài khoản Nợ"
	,CreditAcc		"Tài khoản Có"
from $TB order by ItemCode,Pos,DocDate,DocTime,ID;

end;

