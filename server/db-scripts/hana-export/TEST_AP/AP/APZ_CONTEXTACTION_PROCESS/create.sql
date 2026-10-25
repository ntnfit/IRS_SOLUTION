
Create  Procedure apz_contextaction_process(in transtype CHAR(1), in formId nvarchar(50) default '', in itemId nvarchar(50) default '')
As
Begin
	declare error integer := 0;
	declare error_message nvarchar(254) := '';
	declare error_prefix integer := 3500000;
	
	create local temporary table #temp_apz_octa as (SELECT TOP 0 * from APZ_OCTA);
	create local temporary table #temp_apz_cta1 as (SELECT TOP 0 * from APZ_CTA1);
	create local temporary table #temp_apz_cta2 as (SELECT TOP 0 * from APZ_CTA2);
	create local temporary table #temp_apz_cta3 as (SELECT TOP 0 * from APZ_CTA3);
	create local temporary  table #temp_apz_cta4 as (SELECT TOP 0 * from APZ_CTA4);
	create local temporary  table #temp_apz_cta5 as (SELECT TOP 0 * from APZ_CTA5);
	execute immediate 'insert into #temp_apz_octa select * from "#APZ_OCTA" A WHERE IFNULL(A."Caption",'''')<>''''';
	execute immediate 'insert into #temp_apz_cta1 select * from "#APZ_CTA1" A WHERE (IFNULL(A."ParamName", '''') <> '''')';
	execute immediate 'insert into #temp_apz_cta2 select * from "#APZ_CTA2" A WHERE (IFNULL(A."SourceField", '''') <> '''' OR IFNULL(A."DfltValue", '''') <> '''')';
	execute immediate 'insert into #temp_apz_cta3 select * from "#APZ_CTA3" A WHERE IFNULL(CAST(A."FilePath" AS NVARCHAR(500)), '''') <> ''''';
	execute immediate 'insert into #temp_apz_cta4 select * from "#APZ_CTA4" A WHERE IFNULL(A."ParamName", '''') <> ''''';
	execute immediate 'insert into #temp_apz_cta5 select * from "#APZ_CTA5" A WHERE IFNULL(A."Name", '''') <> '''' or IFNULL(A."JsonType", '''') <> ''''';
	
	if ifnull(:formId, '') = ''
	THEN
		SELECT (SELECT TOP 1 "FormId" FROM #temp_apz_octa) INTO FormId FROM DUMMY;
	END IF;
	
	if ifnull(:itemId, '') = ''
	THEN
		SELECT (SELECT TOP 1 "ItemId" FROM #temp_apz_octa) INTO ItemId FROM DUMMY;
	END IF;
	
	IF IFNULL(:FormId, '') = '' OR IFNULL(:ItemId, '') = '' 
	THEN 
		error := :error_prefix + 1;
		error_message := n'Invalid value FormId-ItemId';
		SELECT :error, :error_message FROM DUMMY; 
		RETURN;
	END IF;
	DELETE FROM APZ_OCTA WHERE "FormId" = :FormId AND "ItemId" = :ItemId;
	DELETE FROM APZ_CTA1 WHERE "FormId" = :FormId AND "ItemId" = :ItemId;
	DELETE FROM APZ_CTA2 WHERE "FormId" = :FormId AND "ItemId" = :ItemId;
	DELETE FROM APZ_CTA3 WHERE "FormId" = :FormId AND "ItemId" = :ItemId;
	DELETE FROM APZ_CTA4 WHERE "FormId" = :FormId AND "ItemId" = :ItemId;
	DELETE from APZ_CTA5 WHERE "FormId"= :FormId AND "ItemId"=:ItemId;
	
	INSERT INTO APZ_OCTA (SELECT * FROM #temp_apz_octa A);
	INSERT INTO APZ_CTA1 (SELECT * FROM #temp_apz_cta1 A);
	INSERT INTO APZ_CTA2 (SELECT * FROM #temp_apz_cta2 A);
	INSERT INTO APZ_CTA3 (SELECT * FROM #temp_apz_cta3 A);
	INSERT INTO APZ_CTA4 (SELECT * FROM #temp_apz_cta4 A);
	INSERT INTO APZ_CTA5 (SELECT * FROM #temp_apz_cta5 A);
	
	SELECT :error, :error_prefix FROM DUMMY;
End;




