
CREATE PROCEDURE apz_process_SaveKPIsSetting(in typeName char(1) default '')
AS
BEGIN
	declare error int =0;
	declare error_prefix int =800005;
	DECLARE absEntry NVARCHAR(20) = '';
	DECLARE newFrom timestamp;
	declare checkExistScore int;
	declare CheckDuplicate nvarchar(100) = '';
	declare currentDate timestamp = cast(current_date as date);
	declare currentTime int = EXTRACT(Hour FROM Current_date) * 100 + EXTRACT(minute FROM Current_date);
	declare docEntry int;
	
	create local temporary table #APZ_D_OKPMData as(select top 0 * from APZ_D_OKPM);
	exec 'insert into #APZ_D_OKPMData select * from #APZ_D_OKPM';
	
	SELECT TOP 1 "AbsEntry", "FromDate" into absEntry, newFrom FROM #APZ_D_OKPMData;
	select ifnull((select Count(*) from APZ_D_KPR1 a left join APZ_D_OKPR b on a."AbsEntry" = b."AbsEntry" 
														left join  APZ_D_OKPM c on b."Code" = c."Code" and b."ViewType" = c."ViewType" and b."Period" = c."Period"  and c."empID" = a."EmpId" 
										where c."AbsEntry" = :absEntry),0) into checkExistScore from DUMMY;
	-- Nếu delete
	IF :typeName = 'D'
	THEN
		-- todo: check condition có nam trong condition rule nào ko
		if(:checkExistScore >0)
		THEN
			SELECT  :error_prefix+1 "ErrorNumber", N'Can not delete this entry because of existing scores!' "ErrorMessage" from DUMMY;
			return;
		end IF;
		-- nếu ko thì xóa
		DELETE from APZ_D_OKPM WHERE "AbsEntry" = :absEntry;
		DELETE from APZ_D_KPM1 WHERE "AbsEntry" = :absEntry;
		
		SELECT :absEntry from DUMMY;
		RETURN;
	END IF;
	
	create local temporary table #APZ_D_KPM1Data ("AbsEntry" int, "KPI_Code" nvarchar(50), "Condition_Code" nvarchar(50), "FromValue" numeric(19,6), "Point" numeric(19,6), "ToValue" numeric(19,6));
	exec 'insert into #APZ_D_KPM1Data select "AbsEntry", "KPI_Code", "Condition_Code", "FromValue", "Point", "ToValue" from #APZ_D_KPM1';
	
	select ifnull((SELECT TOP 1 a."Code" from #APZ_D_OKPMData a 
											INNER JOIN APZ_D_OKPM b ON ifnull(a."AbsEntry",0) <> b."AbsEntry" AND a."Code" = b."Code" AND a."Period" = b."Period" and a."empID" = b."empID"),'') into CheckDuplicate from DUMMY;
	
	if(ifnull(:CheckDuplicate,'') != '')
	THEN
		SELECT  :error_prefix+1 "ErrorNumber", N'Period duplicated in database [Code = ' || cast(:CheckDuplicate as nvarchar) || ']' "ErrorMessage" from DUMMY;
		return;
	end IF;
	
	IF :typeName = 'A'
	THEN
		UPDATE #APZ_D_OKPMData	SET "CreateDate" = :currentDate,
								   "CreateTime" = :currentTime,
								    "UpdateDate" = :currentDate,
									"UpdateTime" = :currentTime;
			  
	ELSEIF :typeName = 'U'
	THEN
		BEGIN
			DECLARE oldFrom timestamp;
			SELECT TOP 1 "FromDate" into oldFrom FROM APZ_D_OKPM where "AbsEntry" = :absEntry;
			if(:checkExistScore >0 AND :oldFrom != :newFrom)
			THEN
				SELECT :error_prefix+1 "ErrorNumber", N'Can not edit From Date because of existing scores!' "ErrorMessage" FROM DUMMY;
				return;
			end IF;
			UPDATE #APZ_D_OKPMData SET "UpdateDate" = :currentDate, "UpdateTime" = :currentTime;
		END;
	END IF;
	
	update #APZ_D_OKPMData set "AbsEntry" = (Select ifnull(Max("AbsEntry"),0)+1 from APZ_D_OKPM) where ifnull("AbsEntry",0) = 0;
	
	select "AbsEntry" into docEntry from #APZ_D_OKPMData;
	update #APZ_D_KPM1Data set "AbsEntry" = :docEntry;
	
	delete from APZ_D_OKPM where "AbsEntry" = :docEntry;
	insert into APZ_D_OKPM select * from #APZ_D_OKPMData;

	 DELETE from APZ_D_KPM1 where "AbsEntry" = :docEntry;
	 insert into APZ_D_KPM1("AbsEntry", "KPI_Code", "Condition_Code", "FromValue", "Point", "ToValue") 
	 			select "AbsEntry", "KPI_Code", "Condition_Code", "FromValue", "Point", "ToValue" from #APZ_D_KPM1Data where "Point" is not null and ifnull("Condition_Code",'') != '';

	 select :docEntry from DUMMY;
END






