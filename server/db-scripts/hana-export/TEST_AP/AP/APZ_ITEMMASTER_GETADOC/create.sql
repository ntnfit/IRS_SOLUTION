
CREATE PROCEDURE APZ_ItemMaster_GetADoc(
in ItemCode nvarchar(50) default '')

as
BEGIN
	declare cloudImage nvarchar(500);
	select ifnull((select top 1 t."LinkThumbnails" from APZ_D_UUPIM t where t."ObjectCode" = :ItemCode and t."ObjectType" = '810039' order by t."UploadDate", t."UploadTime" DESC),'') into cloudImage from dummy;
	select top 1 *, :cloudImage as "CloudImage" FROM OITM WHERE "ItemType"<>'F' and  "ItemCode" = :ItemCode;
END



