

CREATE PROCEDURE APZ_D_Process_AlertTemplate (IN type char(1))
AS
BEGIN
	
	create local temporary table #OALTLocal as (select top 0 * from APZ_D_OALT);
	exec 'drop table #OALTLocal';
	exec 'create local temporary table #OALTLocal as (select * from #APZ_D_OALT)';
	
	--Update default value

	IF :type = 'A' THEN 
		UPDATE #OALTLocal SET "LogInstanc" = NULL, "CreateDate" = current_date, "CreateTime" = CAST(to_nvarchar(now(), 'hhmiss') AS int), "UpdateDate" = NULL, "UpdateTime" = NULL;
	ELSE 
		UPDATE #OALTLocal SET "LogInstanc" = NULL, "UpdateDate" = current_date, "UpdateTime" = CAST(to_nvarchar(now(), 'hhmiss') AS int);
	END IF;
	
	--Kiểm tra template code rỗng
	if exists(select 1 from #OALTLocal where IFNULL("Code", '') = '') then

		select '01' as "ErrorCode", 'template code is empty value' as "ErrorMessage" from dummy;
		return;
	end if;

	--Kiểm tra trùng template
	if exists(select 1 from #OALTLocal a inner join APZ_D_OALT b on a."Code" = b."Code") and :type = 'A' then
		select '05' as "ErrorCode", 'This template already exists' as "ErrorMessage" from dummy;
		return;
	end if;

	if :type = 'U' then
		delete from APZ_D_OALT where "Code" in(select a."Code" from APZ_D_OALT a inner join #OALTLocal b on a."Code" = b."Code");
	end if;

	insert into APZ_D_OALT select * from #OALTLocal;
	
END




