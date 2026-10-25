
CALL APZ_EXISTSPROC('APZ_D_PromotionRegister_GetAttachment');

GO

CREATE  PROCEDURE APZ_D_PromotionRegister_GetAttachment(in type nvarchar(20) default 'list_promotion', in usercode nvarchar(20), in cardCode nvarchar(50), in promotionCode nvarchar(100), in groupCode nvarchar(100))
AS
BEGIN
	declare userId int;
	select ifnull((select top 1 USERID from OUSR where USER_CODE = :usercode), -1) into userid from dummy;
	
	if :type = 'list_promotion'
	THEN
	--type để phục vụ việc thêm sửa xóa
		select a.*,null as "Type",(select "LinkDownload" from (select "LinkDownload", row_number() over(order by "Code") id from APZ_D_UPIM where cast("Code" as nvarchar(50)) = a."AttachmentId") x where id = 1) as "Uri" 
						from APZ_D_PMO5 a where a."CardCode" = :cardCode;
	else
		select a.*,null as "Type",(select "LinkDownload" from (select "LinkDownload", row_number() over(order by "Code") id from APZ_D_UPIM where cast("Code" as nvarchar(50)) = a."AttachmentId") x where id = 1) as "Uri" 
				from APZ_D_PMO5 a where a."Code" = :promotionCode 
						and ((select top 1 ifnull("PakageSplit",'N') from APZ_OPPR where "Code" = :promotionCode) = 'N' 
						or ifnull(nullif(a."GroupCode",''),'') = ifnull(nullif(:groupCode,''),''));
	end IF;
END