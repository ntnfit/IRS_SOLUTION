

Create PROCEDURE APZ_D_SurveyLineById (IN docEntry int DEFAULT 0)
AS
BEGIN
	select * from APZ_D_OSUV where "DocEntry" = :docEntry;
	select * from APZ_D_SUV1 where "DocEntry" = :docEntry;
END




