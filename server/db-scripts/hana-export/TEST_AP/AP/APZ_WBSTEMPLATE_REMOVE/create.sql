
CREATE PROCEDURE APZ_WBSTemplate_Remove(in code nvarchar(50) default '')
AS
BEGIN
	declare chkExists varchar(10) = '';
	select (select 1 from APZ_PRG1 where "WbsTemplate" = :code) into chkExists from DUMMY;
	if :chkExists <> '' then
		select 1 as "ErrorCode", 'Cannot remove name because it has been used' as "ErrorMessage" from DUMMY;
 		return;
 	end if;
 	
 	delete from APZ_OWBS where "Code" = :code;
 	delete from APZ_TASK where "Code" = :code;
 	delete from APZ_WBS1 where "Code" = :code;
 	
 	select 0, '' from DUMMY;
END;



