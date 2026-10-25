CREATE procedure ILD_LaySSCC_TonKhoHienTai
as
begin
	select  b."DistNumber" as "SSCC","OnHandQty"
	from OBBQ a
	inner join OBTN b on a."SnBMDAbs"  = b."AbsEntry"
	where "OnHandQty" >0;
end
