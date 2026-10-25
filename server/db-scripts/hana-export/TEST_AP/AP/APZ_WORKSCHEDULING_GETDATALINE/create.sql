
CREATE  PROCEDURE APZ_WorkScheduling_GetDataLine(in docId int)
AS
BEGIN
	--select * from APZ_WSH1 where ID = @docId
	select "WorkDate", ID,(select string_agg("Shift", ',') from (SELECT DISTINCT "Shift" FROM APZ_WSH1 WHERE "WorkDate" = a."WorkDate" and "ID" = :docId) x)  AS "Shift", "Remark", cast(0 as int) "LinID" 
		from APZ_WSH1 a  where "ID" = :docId Group by "WorkDate", "ID", "Remark";
END



