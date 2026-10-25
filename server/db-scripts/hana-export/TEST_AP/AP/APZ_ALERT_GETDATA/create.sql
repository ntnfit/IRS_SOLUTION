
CREATE PROCEDURE apz_alert_getdata(in Code INT)
AS
BEGIN

	DECLARE sqlText NVARCHAR(50000);

	select ifNULL((SELECT T1."QString" FROM oalt T0 INNER JOIN ouqr t1 ON t0."QueryId"=t1."IntrnalKey" WHERE t0."Code"=:Code ),'') into sqlText from dummy;

	EXECUTE immediate :sqlText;
	
end



