
CREATE procedure APZ_SUBACTIVITIES_REMOVETASK (in ClgCode int default 0)
as
begin

	DECLARE SubClgCode int;
	DECLARE MaxClgCode int = 0;
	DECLARE CurrentClgCode int = 0;
		
	create local temporary table #subTaskTable("ClgCode" int);
	insert into #subTaskTable select "ClgCode" from APZ_OCLG where "parentTask" = :ClgCode;
	
	select ifnull((SELECT MAX("ClgCode") from  #subTaskTable), 0) into MaxClgCode from dummy;
	select ifnull((SELECT MIN("ClgCode") from #subTaskTable), 0) into CurrentClgCode from dummy;
			
	IF :MaxClgCode > 0 then
	
		WHILE :CurrentClgCode <= :MaxClgCode DO
		
			select IFNULL((SELECT TOP 1 "ClgCode" FROM #subTaskTable WHERE "ClgCode" = :CurrentClgCode),0) into SubClgCode from dummy;
			--call removetask foreach subtask clg
			call APZ_SubActivities_RemoveTask (:SubClgCode);
			SELECT (:CurrentClgCode + 1) INTO CurrentClgCode FROM DUMMY;
			
		END WHILE;
		
	END IF;
	
			--delete main task
		DELETE FROM APZ_OCLG WHERE "ClgCode" = :ClgCode;
		DELETE FROM APZ_CLG1 WHERE "ObjType" = '1470000128' AND ID =:ClgCode;
		
		select :ClgCode from dummy;
		
end;




