

CREATE PROCEDURE APZ_D_UploadFile_GetFileByOjectType(in objectType nvarchar(50) default '', in objectCode nvarchar(50) default '')
AS
BEGIN

	select * from APZ_D_UUPIM 
	where (ifnull(:objectType, '') = '' or "ObjectType" = :objectType) 
	and (ifnull(:objectCode, '') = '' or "ObjectCode" = :objectCode)
	order by "UploadDate" desc;

END



