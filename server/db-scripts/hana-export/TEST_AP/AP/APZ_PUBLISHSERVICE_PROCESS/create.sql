
CREATE PROCEDURE apz_publishservice_process(in transtype CHAR(1))
AS	
BEGIN	
	---------------------------- VARIABLE ------------------------------
	DECLARE error INT =0;
	DECLARE error_message NVARCHAR(254)='';
	DECLARE error_prefix INT=3500000;

	DECLARE ControllerName NVARCHAR(50);
	DECLARE ActionName	NVARCHAR(50);
	DECLARE AbsEntry int;
	declare checkCount int = 0;
	---------------------------- END VARIABLE -------------------------

	---------------------------- TEMP TABLE ---------------------------
	create local temporary table #APZ_OCPS_DATA as (select top 0 * from APZ_OCPS);
	create local temporary table #APZ_CPS1_DATA as (select top 0 * from APZ_CPS1);
	exec 'drop table #APZ_OCPS_DATA;';
	exec 'drop table #APZ_CPS1_DATA;';
	exec 'create local temporary table #APZ_OCPS_DATA as (select * from "#APZ_OCPS");';
	exec 'create local temporary table #APZ_CPS1_DATA as (select * from "#APZ_CPS1");';
	---------------------------- END TEMP TABLE -----------------------

	select IFNULL((SELECT TOP 1 LTRIM(RTRIM("ControllerName")) FROM #APZ_OCPS_DATA),'') into ControllerName from DUMMY;
	select IFNULL((SELECT TOP 1 LTRIM(RTRIM("ActionName")) FROM #APZ_OCPS_DATA),'') into ActionName from DUMMY;
	select IFNULL((SELECT TOP 1 "AbsEntry" FROM #APZ_OCPS_DATA),0) into AbsEntry from DUMMY;
	
	select IFNULL((SELECT COUNT(1) FROM APZ_OCPS T0 WHERE T0."ControllerName" = :ControllerName AND T0."ActionName" = :ActionName AND T0."AbsEntry" <> :AbsEntry),0) into checkCount from DUMMY;
	IF :checkCount > 0
	THEN
		error=:error_prefix+1;
		error_message=N'Controller or Action already exists';
		SELECT :error,:error_message from DUMMY;
		RETURN;
	END IF;
	 
	IF :ControllerName='' OR :ActionName=''
	THEN
		error=:error_prefix+2;
		error_message=N'Controller or Action is missing';
		SELECT :error,:error_message from DUMMY;
		RETURN;
	END IF;

	DELETE FROM APZ_OCPS WHERE "AbsEntry"=:AbsEntry;
	DELETE FROM APZ_CPS1 WHERE "AbsEntry"=:AbsEntry;
	IF :AbsEntry=0
	THEN
		select IFNULL((SELECT MAX("AbsEntry") FROM APZ_OCPS),0)+1 into AbsEntry from DUMMY;
    END IF;
    
	UPDATE #APZ_OCPS_DATA SET "AbsEntry"=:AbsEntry;
	UPDATE #APZ_CPS1_DATA SET "AbsEntry"=:AbsEntry;
	INSERT INTO APZ_OCPS SELECT * FROM #APZ_OCPS_DATA A;
	INSERT INTO APZ_CPS1 SELECT * FROM #APZ_CPS1_DATA A WHERE IFNULL(A."Name",'')<>'';

	SELECT :error,:error_prefix from DUMMY;
END



