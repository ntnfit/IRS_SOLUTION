
CREATE PROCEDURE APZ_ServiceCalls_SaveDoc
AS
BEGIN
		DECLARE callID int = 0;
		declare checkCount int = 0;
		DECLARE maxCallID int = 0;
		
		create local temporary table #dtAPZ_OSCLData as (select top 0 * from APZ_OSCL);
		exec 'DROP TABLE #dtAPZ_OSCLData;';
		exec 'create local temporary table #dtAPZ_OSCLData as (select * from "#dtAPZ_OSCL");';
		
		select ifNULL((SELECT TOP 1 "CallID" from #dtAPZ_OSCLData ORDER BY "CallID" DESC),0) into callID from DUMMY;

		IF :callID=0
		THEN
			--get max callid for adding	
			select IFNULL((SELECT TOP 1 "CallID" from APZ_OSCL ORDER BY "CallID" DESC),0) into maxCallID FROM DUMMY;
			UPDATE #dtAPZ_OSCLDATA SET "CallID"=:maxCallID+1;

			DELETE FROM APZ_OSCL WHERE "CallID"=:callID;
			INSERT INTO APZ_OSCL
				SELECT * from #dtAPZ_OSCLData;
		ELSE	

			DELETE FROM APZ_OSCL WHERE "CallID"=:callID;
			INSERT INTO APZ_OSCL SELECT * from #dtAPZ_OSCLData;

			select ifnull((select top 1 1 from M_TEMPORARY_TABLES where schema_name = current_schema and Table_Name = N'#dtSCL1'), 0) into checkCount from DUMMY;
			if :checkCount = 1
			THEN
				create local temporary table #tempSolution as (select top 0 * from SCL1);
				exec 'DROP TABLE #tempSolution;';
				exec 'create local temporary table #tempSolution as (select "srvcCallID","line","solutionID",NULL "objectType", NULL "logInstanc","userSign","createDate","userSign2","updateDate", NULL "VisOredr" from "#dtSCL1");';

				DELETE FROM SCL1 WHERE "srvcCallID"=:callID;
				INSERT INTO SCL1
					SELECT * FROM #tempSolution;
			END IF;

			select ifnull((select top 1 1 from M_TEMPORARY_TABLES where schema_name = current_schema and Table_Name = N'#dtAPZ_SCL3'), 0) into checkCount from DUMMY;
			if :checkCount = 1
			THEN

				DELETE FROM APZ_SCL3 WHERE "srvcCallID"=:callID;
				exec 'INSERT INTO APZ_SCL3 SELECT * FROM #dtAPZ_SCL3;';
			END IF;
		END IF;
		
END



