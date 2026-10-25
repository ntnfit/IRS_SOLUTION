
CREATE PROCEDURE APZ_ResourceWorkSchedule_DeleteData(in code nvarchar(50))
AS
BEGIN

	declare checkCount int = 0;
	select IFNULL((select top 1 1 from ORSC where "U_WorkSchedule" = :code),0) into checkCount from DUMMY;
	if :checkCount = 1
	THEN

		select 800010, 'Can not delete resource "' || :code || '". It use in resource' FROM DUMMY;
		return;

	END IF;

	select IFNULL((select top 1 1 from APZ_ORWS where "Code" = :code),0) into checkCount from DUMMY;
	if :checkCount = 0
	THEN

		select 800011, ' Resource "' || :code || '" does not exists' FROM DUMMY;
		return;

	END IF;

	delete from APZ_RWS1 where "Code" = :code;
	Delete from APZ_ORWS where "Code" = :code;

	select 0, '' FROM DUMMY;
		
END



