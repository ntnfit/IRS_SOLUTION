CREATE PROCEDURE APZ_D_Image_GetListItem(
	in fromCode nvarchar(50),
	in toCode nvarchar(50),
	in itemGroup nvarchar(4000)
)
AS
BEGIN
	create local temporary table #tblData as (select top 0 a.*, a."UploadDate"  as "UploadOnlyDate",ROW_NUMBER() OVER (PARTITION BY "ObjectCode","ObjectType" ORDER BY a."UploadDate" DESC,a."UploadTime" DESC) AS "rn"
	    from APZ_D_UUPIM a  );
	insert into #tblData 
		select a.*, a."UploadDate"  as "UploadOnlyDate",ROW_NUMBER() OVER (PARTITION BY "ObjectCode","ObjectType" ORDER BY a."UploadDate" DESC,a."UploadTime" DESC) AS "rn"
	    from APZ_D_UUPIM a
	     where a."ObjectType" = '810039';
	
	select  b."ItemCode",b."ItemName",b."ItmsGrpCod",a.* from #tblData a right join OITM b on a."ObjectCode" = b."ItemCode"
	
	where  (:fromCode != '' and :toCode != ''
	 AND "ItemCode" between :fromCode and :toCode OR :fromCode = '' OR :toCode = '')
	  AND  (:itemGroup = '' OR (:itemGroup = 'N' and ifnull(cast("ItmsGrpCod" as nvarchar(50) ),'') = '') or (:itemGroup != '' AND ((cast("ItmsGrpCod" as nvarchar(50)) in (select data as "grp" from split(:itemGroup,';'))) OR :itemGroup = 'A')))
  
 and ifnull("rn",0) <=1
   order by "ItmsGrpCod","ItemCode" ASC, "UploadOnlyDate" DESC,"UploadTime" ASC;
drop table #tblData;
END



