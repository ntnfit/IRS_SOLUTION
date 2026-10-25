
CREATE PROCEDURE APZ_SubActivities_GetParentTask(in parentTask int default 0)
AS
BEGIN
	DECLARE tbl table ("ClgCode" int ,"Details" nvarchar(50000));
	DECLARE checkCount int = 0;
	if :parentTask is null
	then
		parentTask = 0;
	END IF;
	
	WHILE :parentTask != 0 and :checkCount < 51
	DO

		INSERT INTO :tbl SELECT TOP 1 "ClgCode","Details" FROM APZ_OCLG WHERE "ClgCode"=:parentTask;
		
		select IFNULL((SELECT TOP 1 "parentTask" FROM APZ_OCLG WHERE "ClgCode"=:parentTask),0) into parentTask from DUMMY;
	
		checkCount = :checkCount + 1;

	END WHILE;

	SELECT * FROM :tbl ORDER BY "ClgCode" ASC;

END





