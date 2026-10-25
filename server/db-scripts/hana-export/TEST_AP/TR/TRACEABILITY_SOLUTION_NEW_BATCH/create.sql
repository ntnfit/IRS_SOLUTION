create procedure TRACEABILITY_SOLUTION_NEW_Batch(in itemCode nvarchar(50))
as
begin
	select distinct c."MnfSerial" "Batch", c."InDate"
	from IBT1 a
	join OBTN c on a."ItemCode"=c."ItemCode" and a."BatchNum"=c."DistNumber"
	where a."BaseType"=59 and a."BsDocType"=202 and a."ItemCode"=:itemCode and c."MnfSerial" is not null
	order by c."InDate" desc;
end

