
CREATE PROCEDURE APZ_SaveRptFileName
AS
BEGIN
	declare maxCode int;
	select ifnull((SELECT MAX("Code") from APZ_SAL5),0) into maxCode FROM DUMMY;

	create local temporary table #APZ_SAL5DATA as(select top 0 * from APZ_SAL5);
	exec 'drop table #APZ_SAL5DATA';
	exec 'create local temporary table #APZ_SAL5DATA as(select * from #APZ_SAL5)';
	update #APZ_SAL5DATA set "Code" = :maxCode +1 where "Code" is null or "Code" = 0;
	
	MERGE INTO APZ_SAL5 AS T USING #APZ_SAL5DATA AS S ON T."Code" = S."Code" 
	 	WHEN MATCHED THEN update set T."RptFileName" = S."RptFileName"
 		WHEN NOT MATCHED THEN  insert ("Code", "RptFileName") values (S."Code",	S."RptFileName");
END




