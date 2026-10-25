CREATE procedure ILD_LayPurchaseOrder
(
	in fdate date,
	in tdate date
)
as
begin
	select "DocNum","DocDate"
	from  OPOR
	where"CANCELED"  =  'N' and  "DocDate" between fdate and tdate;
end;

