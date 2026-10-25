CREATE PROCEDURE APZ_D_Sync_GetCommentCustomer(
	in userName nvarchar(50),
	in objectType nvarchar(50),
	in relatedTyp nvarchar(50)
)
AS
BEGIN
	-- set user id
	declare userId int;
	select (select FIRST_VALUE("USERID" order by "USERID") from OUSR where "USER_CODE" = :userName) into userId from DUMMY;
	-- Create temp table #tempLocal
	create local temporary table #tempLocalComment as (select '' as "CardCode" from dummy);
	exec 'drop table #tempLocalComment;';
	exec 'create local temporary table #tempLocalComment as (select * from "#temp");';
	
	SELECT *,"RelatedEnt" as "CardCode",
	--avatar
	(select top 1 "LinkThumbnails" from APZ_D_UUPIM where "UserSign" = :userId and "ObjectType" = :objectType) as "LinkThumbnails"
    FROM APZ_D_OCMC where "UserSign" = :userId and "RelatedEnt" in (select "CardCode" from #tempLocalComment) and "RelatedTyp" = :relatedTyp order by "CreatedTime" desc;
END



