CREATE procedure ILD_LayDonViTinh
as
begin
	 Select  "U_Code","U_Name" from "@UOM_LIST_LINE" order by "U_STT";
end;
