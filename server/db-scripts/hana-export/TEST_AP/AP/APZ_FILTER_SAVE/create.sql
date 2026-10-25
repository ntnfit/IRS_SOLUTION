
CREATE procedure apz_filter_save(
	in Code nvarchar(100) default '',
	in Name nvarchar(254) default '',
	in ObjType nvarchar(20) default '',
	in FilterText nclob default null)
as
begin
	declare checkCount integer = 0;
	select IFNULL((select count(1) from APZ_FILTER where "Code"=:Code),0) into checkCount from DUMMY;
	if checkCount = 1
	then
		UPDATE APZ_FILTER SET "Name" = :Name, "ObjType" = :ObjType, "FilterText" = :FilterText WHERE "Code" = :Code;
	else
		INSERT INTO APZ_FILTER ("Code", "Name", "ObjType", "FilterText", "Valid") VALUES (:Code, :Name, :ObjType, :FilterText, 1);
	end IF;

end




