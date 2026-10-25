
CREATE PROCEDURE APZ_TimeSheeting_LoadOvertimeNew(in branch nvarchar(50), in period nvarchar(50))
as
BEGIN
	SELECT "Period", a."EmpId", sum(IFNULL("WorkTotal",0)*IFNULL(c."Rate",0)) "WorkTotal" 
                FROM APZ_OTSH a INNER JOIN OHEM b ON a."EmpId"=b."empID"
                                INNER JOIN APZ_OHOT c on a."OvertimeType"=c."Code"
                       WHERE "Period" =:Period and b."branch"=:Branch GROUP BY "Period", a."EmpId";
END



