create procedure ILD_R_IRS_Traceability_Batchs(in itemCode nvarchar(50))
as
begin
	select distinct c."MnfSerial" "Batch", c."InDate" from IBT1 a join OPDN b on a."BaseType"=b."ObjType" and a."BaseEntry"=b."DocEntry"
	join OBTN c on a."ItemCode"=c."ItemCode" and a."BatchNum"=c."DistNumber"
	where a."ItemCode"=:itemCode and b.CANCELED='N' and c."MnfSerial" is not null
	order by c."InDate" desc;
end

