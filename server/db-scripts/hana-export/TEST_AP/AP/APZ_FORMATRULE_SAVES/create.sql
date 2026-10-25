
CREATE PROCEDURE APZ_FormatRule_Saves
(
in userCode nvarchar(50),
in formId nvarchar(50),
in itemId nvarchar(50)
)
AS 
BEGIN
	declare checkUpdate int = 0;
	declare code int =0;
	create local temporary table #APZ_FCREData as (select top 0 * from APZ_FCRE);
	exec 'insert into #APZ_FCREData select * from "#APZ_FCRE"';
	select IFNULL((select Max("ExpressionId") from APZ_FCRE), 0) into code from DUMMY;
	
	select IFNULL((select top 1 1 from #APZ_FCREData where ifnull("ExpressionId" ,0) = 0), 0) into checkUpdate from DUMMY;
	
	WHILE :checkUpdate = 1
	DO
		code = :code + 1;
		UPDATE top 1 #APZ_FCREData set "ExpressionId" = :code where ifnull("ExpressionId" ,0) = 0;
		select IFNULL((select top 1 1 from #APZ_FCREData where ifnull("ExpressionId" ,0) = 0), 0) into checkUpdate from DUMMY;
	END WHILE;
	 
	DELETE FROM APZ_FCRE where "UserCode" = :userCode and "FormId" = :formId and "ItemId" = :itemId;
	INSERT INTO APZ_FCRE select * from #APZ_FCREData;
END;




