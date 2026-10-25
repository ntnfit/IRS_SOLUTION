

CREATE PROCEDURE APZ_WorkingProcess_SaveMenu(in userId int)
AS
BEGIN

	declare maxId int = -1;

	create local temporary table #dtMenuLocal
	(
		"Id" int null,
		"UserId" int null,
		"DocType" nvarchar(254) null,
		"MnuType" char(1) null,
		"Visible" char(1),
		"Remarks" nvarchar(254) null
	);
	exec 'drop table #dtMenuLocal;';
	exec 'create local temporary table #dtMenuLocal as (select * from "#dtMenu");';
		
	delete from APZ_WPS1 where "UserId" = :userId;	
		
	select IFNULL((select MAX("Id") from APZ_WPS1), 1) into maxId from dummy;	
	
	insert into APZ_WPS1("Id", "UserId", "DocType", "MnuType", "Visible", "Remarks")
	select ROW_NUMBER() OVER(ORDER BY "DocType") + :maxId, :userId, "DocType", "MnuType", 'Y', "Remarks"
	from #dtMenuLocal a
	where "DocType" not in (select "DocType" from APZ_WPS1 where "UserId" = :userId and "MnuType" = a."MnuType");

	select '' from dummy;
END



