
CREATE PROCEDURE APZ_Penalization_GetDoc 
AS BEGIN
 SELECT a.*, b."FromDate", b."ToDate", b."UserSign2", b."UpdateDate" FROM APZ_HPT2 a INNER JOIN APZ_OHWP b ON a."Period" = b."PeriodCode" AND a."Branch" = b."Branch";
END;




