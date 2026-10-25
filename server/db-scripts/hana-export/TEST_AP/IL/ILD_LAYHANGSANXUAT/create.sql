CREATE procedure ILD_LayHangSanXuat
as
begin
select   "FirmCode", "FirmName"   from   OMRC where "FirmCode"  <> -1;
end;

