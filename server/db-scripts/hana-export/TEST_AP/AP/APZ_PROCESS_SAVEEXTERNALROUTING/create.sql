
CREATE PROCEDURE apz_process_SaveExternalRouting
AS	
BEGIN	
	----------------------------- VARIABLE ------------------------------------
	DECLARE error INTEGER =0;
	DECLARE error_message NVARCHAR(254)='';
	DECLARE error_prefix INT=3600000;

	DECLARE Controller NVARCHAR(100);
	DECLARE ActionName	NVARCHAR(100);
	DECLARE AbsEntry INT;
	DECLARE DataString NVARCHAR(5000);
	DECLARE checkCount int = 0;
	declare newDocEntry int;
	----------------------------- END VARIABLE --------------------------------

	----------------------------- TEMP TABLE ----------------------------------
	create local temporary table #APZ_OURI_DATA as (select top 0 * from APZ_OURI);
	create local temporary table #APZ_URI1_DATA as (select top 0 * from APZ_URI1);
	create local temporary table #APZ_URI2_DATA as (select top 0 * from APZ_URI2);
	exec 'drop table #APZ_OURI_DATA;';
	exec 'drop table #APZ_URI1_DATA;';
	exec 'drop table #APZ_URI2_DATA;';
	exec 'create local temporary table #APZ_OURI_DATA as (select * from #APZ_OURI);';
	exec 'create local temporary table #APZ_URI1_DATA as (select * from #APZ_URI1);';
	exec 'create local temporary table #APZ_URI2_DATA as (select * from #APZ_URI2);';
	----------------------------- END TEMP TABLE ------------------------------
	
	select IFNULL((SELECT TOP 1 "Controller" FROM #APZ_OURI_DATA),'') into Controller from DUMMY;
	select IFNULL((SELECT TOP 1 "ActionName" FROM #APZ_OURI_DATA),'') into ActionName from DUMMY;
	select ifNULL((SELECT TOP 1 "AbsEntry" FROM #APZ_OURI_DATA),0) into AbsEntry from DUMMY;
	select IFNULL((SELECT TOP 1 "DataString" FROM #APZ_OURI_DATA),'') into DataString from DUMMY;
	
	IF :AbsEntry < 0 
	THEN
		error=:error_prefix+1;
		error_message=N'Invalid value AbsEntry';
		SELECT :error "ErrorNumber",:error_message "ErrorMessage" from DUMMY;
		RETURN;
	END IF;
	
	IF :Controller='' OR :ActionName='' OR :DataString = ''
	THEN
		error=:error_prefix+1;
		error_message=N'Invalid value Controller-Action Name-Data String';
		SELECT :error "ErrorNumber",:error_message "ErrorMessage" from DUMMY;
		RETURN;
	END IF;
	
	select IFNULL((Select count(1) from APZ_OURI where "Controller" = :Controller and "ActionName" = :ActionName and "AbsEntry" != :AbsEntry),0) into checkCount from DUMMY;
	
	IF	:checkCount > 0
	then
		error=:error_prefix+1;
		error_message=N'Pair Controller - Action Name existed!';
		SELECT :error "ErrorNumber",:error_message "ErrorMessage" from DUMMY;
		RETURN;
	END IF;

	----------------------------- XÓA DỮ LIỆU CÓ TRONG DATABASE -------------------------
	DELETE FROM APZ_OURI WHERE "AbsEntry" = :AbsEntry;
	DELETE FROM APZ_URI1 WHERE "AbsEntry" = :AbsEntry;
	DELETE FROM APZ_URI2 WHERE "AbsEntry" = :AbsEntry;
	----------------------------- END XÓA DỮ LIỆU ---------------------------------------

	----------------------------- CHUẨN HÓA DỮ LIỆU -------------------------------------
	select IFNULL((select Max("AbsEntry") from APZ_OURI),0) + 1 into newDocEntry from DUMMY;
	UPDATE #APZ_OURI_DATA set "AbsEntry" = :newDocEntry where IFNULL("AbsEntry",0) = 0;
	UPDATE #APZ_URI1_DATA set "AbsEntry" = :newDocEntry where IFNULL("AbsEntry",0) = 0;
	UPDATE #APZ_URI2_DATA set "AbsEntry" = :newDocEntry where IFNULL("AbsEntry",0) = 0;
	----------------------------- END CHUẨN HÓA DỮ LIỆU --------------------------------

	----------------------------- UPDATE DỮ LIỆU ---------------------------------------
	INSERT INTO APZ_OURI
	SELECT * FROM #APZ_OURI_DATA A;
	
	INSERT INTO APZ_URI1
	SELECT * FROM #APZ_URI1_DATA A WHERE IFNULL(A."ParamName",'')<>'';
	
	INSERT INTO APZ_URI2
	SELECT * FROM #APZ_URI2_DATA A WHERE (IFNULL(A."Name",'')<>'' OR IFNULL(A."JsonType",'')<>'' );
	----------------------------- END UPDATE DỮ LIỆU -----------------------------------
	 
	--SELECT @error,@error_prefix
END;




