
CREATE PROCEDURE APZ_Process_SaveNotiSetting (
	in usercode nvarchar(25), in typeday nvarchar(25), in fromday int, in today int, in pickday nvarchar(100), in typetime nvarchar(25), in fromtime int,
	in totime int,in specifynotification int,in typespecify nvarchar(25))
as
begin
	-------------------------- Biến dùng để check 0: False - > 0: TRUE -----------------------
	declare checkCount integer = 0;
	select IFNULL((select count(1) from APZ_NOSE where "UserCode"=:usercode),0) into checkCount from DUMMY;
	if :checkCount = 0
	then
		INSERT INTO APZ_NOSE VALUES (:usercode, :typeday, :fromday, :today, :pickday, :typetime, :fromtime, :totime, :specifynotification, :typespecify, current_date);
	else
		UPDATE APZ_NOSE SET "TypeDay" = :typeday, "FromDay" = :fromday, "ToDay" = :today, "PickDay" = :pickday, "TypeTime" = :typetime, "FromTime" = :fromtime, "ToTime" = :totime, "SpecifyNotification" = :specifynotification, "TypeSpecify" = :typespecify, "TimeSetting" = current_date WHERE "UserCode" = :usercode;
	end if;
end;



