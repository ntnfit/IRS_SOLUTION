
CREATE PROCEDURE APZ_AboutRangeTime_Delete(in docEntry int)
AS
BEGIN

	if not exists(select 1 from APZ_OKPG where "DocEntry" = :docEntry)
	THEN
	
		select 10002, 'This entry does not exists' from DUMMY;
		return;
	
	END if;
	delete from APZ_OKPG where "DocEntry" = :docEntry;
	select 200, '' from DUMMY;
	
END;



