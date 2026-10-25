
create procedure APZ_AdditionalGeneralSettings_GetData
as
begin
	select * from APZ_OADM;
	select * from APZ_ATC2;
	select * from APZ_D_OAPC;
end;




