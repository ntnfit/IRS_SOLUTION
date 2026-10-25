
CREATE  PROCEDURE apz_countingbin_GetPeriods
AS
BEGIN

	SELECT "U_PeriodCode" AS "PeriodCode", "U_PeriodName" AS "PeriodName", "U_FromDate" AS "FromDate", "U_ToDate" AS "ToDate", "U_Stat" AS "Stat" FROM "@INCP" WHERE IFNULL("U_Stat", 'N') = 'N';

END;



