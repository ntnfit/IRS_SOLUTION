CREATE procedure ILD_Lay_UDCODEQA
as
begin
-- Loai phiếu
select "Code","Name" from "@QC_UDCODE"  where "U_Active"  = 'Y'

union all
select '999' as  "Code",N' Not Yet UDCode' as "Name" from dummy

order by "Code";

end;

