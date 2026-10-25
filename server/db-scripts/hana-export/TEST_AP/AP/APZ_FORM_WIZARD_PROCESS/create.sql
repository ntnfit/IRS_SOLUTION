

CREATE PROCEDURE apz_form_wizard_process(in transtype CHAR(1))
AS	
BEGIN	
	DECLARE error INT = 0;
	DECLARE error_message NVARCHAR(254)='';
	DECLARE error_prefix INT=4500000;

	DECLARE AbsEntry INT;
	DECLARE MaxAbsEntry INT;
	DECLARE Caption nvarchar(254);
	
	create local temporary table #OFWZ_Local as (select top 0 * from APZ_OFWZ);
	exec 'drop table #OFWZ_Local';
	exec 'create local temporary table #OFWZ_Local as (select * from #APZ_OFWZ)';
	
	create local temporary table #FWZ1_Local as (select top 0 * from APZ_FWZ1);
	exec 'drop table #FWZ1_Local';
	exec 'create local temporary table #FWZ1_Local as (select * from #APZ_FWZ1)';
	
	create local temporary table #FWZ2_Local as (select top 0 * from APZ_FWZ2);
	exec 'drop table #FWZ2_Local';
	exec 'create local temporary table #FWZ2_Local as (select * from #APZ_FWZ2)';
	
	create local temporary table #FWZ3_Local as (select top 0 * from APZ_FWZ3);
	exec 'drop table #FWZ3_Local';
	exec 'create local temporary table #FWZ3_Local as (select * from #APZ_FWZ3)';
	
	SELECT TOP 1 IFNULL("AbsEntry", 0) INTO AbsEntry FROM #OFWZ_Local;
	
	SELECT IFNULL((SELECT MAX("AbsEntry") FROM APZ_OFWZ),0) INTO MaxAbsEntry from dummy;

	
	SELECT TOP 1 IFNULL("Caption",'') INTO Caption FROM #OFWZ_Local;
	
	IF	IFNULL(:Caption,'')='' then

		error=error_prefix + 1;
		error_message=N'Caption is missing';
		SELECT :error, :error_message from dummy;
		RETURN;
	END IF;
	IF :AbsEntry=0 then
		
		AbsEntry= :MaxAbsEntry + 1;

	ELSE 

		DELETE FROM APZ_OFWZ WHERE "AbsEntry"=:AbsEntry;
		DELETE FROM APZ_FWZ1 WHERE "AbsEntry"=:AbsEntry;
		DELETE FROM APZ_FWZ2 WHERE "AbsEntry"=:AbsEntry;
		DELETE FROM APZ_FWZ3 WHERE "AbsEntry"=:AbsEntry;
	END	IF;
	
	UPDATE #OFWZ_Local SET "AbsEntry" =:AbsEntry;
	UPDATE #FWZ1_Local SET "AbsEntry" =:AbsEntry;
	UPDATE #FWZ2_Local SET "AbsEntry" =:AbsEntry;
	UPDATE #FWZ3_Local SET "AbsEntry" =:AbsEntry;
	 
	 INSERT INTO APZ_OFWZ
	 SELECT * FROM #OFWZ_Local;

	 INSERT INTO APZ_FWZ1
	 SELECT * FROM #FWZ1_Local;

	 INSERT INTO APZ_FWZ2
	 SELECT * FROM #FWZ2_Local;

	 INSERT INTO APZ_FWZ3
	 SELECT * FROM #FWZ3_Local where "LineId" is not null and "VisOrder" is not null;

	 
	SELECT :error, :error_prefix FROM DUMMY;
END




