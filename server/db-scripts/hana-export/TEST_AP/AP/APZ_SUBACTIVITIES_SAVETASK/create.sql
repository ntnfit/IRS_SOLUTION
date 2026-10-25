
CREATE PROCEDURE APZ_SubActivities_SaveTask
AS
BEGIN

		DECLARE ClgCode int = 0;
		declare AbsEntry int = 0;
		
		create local temporary table #dtAPZ_OCLGData as (select top 0 * from APZ_OCLG);
		exec 'drop table #dtAPZ_OCLGData;';
		exec 'create local temporary table #dtAPZ_OCLGData as (select * from "#dtAPZ_OCLG");';
		
		select IFNULL((SELECT TOP 1 "ClgCode" from #dtAPZ_OCLGData ORDER BY "ClgCode" DESC),0) into ClgCode from DUMMY;
	
		select ifnull((SELECT top 1 "AtcEntry" FROM APZ_OCLG  WHERE  "ClgCode"= :ClgCode),0) into AbsEntry from DUMMY;

		call APZ_SetAttachment(AbsEntry);

		if :AbsEntry !=0 
		THEN
			UPDATE #dtAPZ_OCLGData SET "AtcEntry"=:AbsEntry;
		END IF;

	    DELETE FROM APZ_OCLG WHERE "ClgCode"=:ClgCode;
		INSERT INTO APZ_OCLG SELECT * FROM #dtAPZ_OCLGData;

	 	SELECT cast(:ClgCode as nvarchar(11)) ||  ',' || cast(:AbsEntry as nvarchar(11)) from DUMMY;
END



