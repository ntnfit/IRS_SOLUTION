CREATE procedure ILD_LAY_GRPONo
(
	
)
as
begin
	SELECT "DocNum" 
	FROM OPDN where "CANCELED"  = 'N'
	order by "DocNum";
end;

