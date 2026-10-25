

Create PROCEDURE APZ_D_SurveyRemove (IN docEntry int DEFAULT 0)
AS
BEGIN

	DELETE FROM APZ_D_OSUV where "DocEntry" = :docEntry;
	DELETE FROM APZ_D_SUV1 where "DocEntry" = :docEntry;
	SELECT 0, '' FROM DUMMY;
	
END



