
CREATE  PROCEDURE APZ_D_Promotion_SaveAttachment
AS
BEGIN

	create local temporary table #APZ_D_PMO5Data as (select top 0 *, null "PakageNo", null "Type" from APZ_D_PMO5);
	exec 'drop table #APZ_D_PMO5Data';
	exec 'create local temporary table #APZ_D_PMO5Data as (select * from #APZ_D_PMO5);';
	--xóa dữ liệu
	DELETE FROM APZ_D_PMO5 a
				where exists(select 1 from #APZ_D_PMO5Data b where a."Code" = b."Code" and ifnull(a."CardCode",'') = ifnull(b."CardCode",'') 
						and (exists(select 1 from APZ_OPPR where "Code" = a."Code" and ifnull("PakageSplit",'N') = 'N') or (ifnull(a."GroupCode",'') = ifnull(b."PakageNo",''))) and a."AttachmentId" = b."AttachmentId");
	--insert từ bảng tạm
	insert into APZ_D_PMO5("Code","CardCode","GroupCode","CreateDate","CreateTime","UserSign","AttachmentId")
	select "Code","CardCode","PakageNo", current_date, current_date,"UserSign","AttachmentId" from #APZ_D_PMO5Data where ifnull("Type",'') = '' or "Type" <> 'delete';
END

