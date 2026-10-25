

CREATE PROCEDURE APZ_D_SaveComment
AS
BEGIN

	declare relatedEnt nvarchar(50) = '';
	
	create local temporary table #APZ_D_OCMCLocal
	(
		"Code" nvarchar(100),
		"Content" nvarchar(4000),
		"RelatedTyp" nvarchar(20),
		"RelatedEnt" nvarchar(100),
		"UserSign" int,
		"UserName" nvarchar(254) ,
		"CreatedTime" timestamp,
		"UpdatedTime" timestamp,
		"LogInstance" int
	);
	exec 'drop table #APZ_D_OCMCLocal;';
	exec 'create local temporary table #APZ_D_OCMCLocal as (select * from #APZ_D_OCMC);';
	
	relatedEnt := cast((select top 1 "RelatedEnt" from #APZ_D_OCMCLocal) as nvarchar);

	delete from APZ_D_OCMC where RelatedEnt = :relatedEnt;
	insert into APZ_D_OCMC("Code", "Content", "RelatedTyp", "RelatedEnt", "UserSign", "UserName", "CreatedTime", "UpdatedTime", "LogInstance") 
	select  a."Code", a."Content", 'oBusinessPartners', a."RelatedEnt", a."UserSign", a."UserName", ifnull(a."CreatedTime", current_date), 
	a."UpdatedTime", ((select ifnull(MAX("LogInstance"), 0) from APZ_D_OCMC) + ROW_NUMBER() OVER (ORDER BY "CreatedTime" ASC))
	from #APZ_D_OCMCLocal a;
	
	if ::ROWCOUNT <= 0
	then
		select '1470000155' as "ObjectType", 406 as "Code", 'Insert table APZ_D_OCMC fail' as "Message" from dummy;
		return; 
	else
		select '1470000155' as "ObjectType", 200 as "Code", '' as "Message" from dummy;
		return;
	end if;

END



