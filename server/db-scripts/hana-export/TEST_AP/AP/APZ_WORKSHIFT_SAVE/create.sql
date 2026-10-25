
CREATE PROCEDURE APZ_WorkShift_Save
AS
BEGIN
	declare Id int=0;
	declare checkCount int = 0;
	declare lineNum int = 0;
	
	--Create Temp table
	create local temporary table #APZ_OEWSLocal as (select top 0 * from APZ_OEWS);
	exec 'DROP TABLE #APZ_OEWSLocal;';
	exec 'CREATE LOCAL TEMPORARY TABLE #APZ_OEWSLocal as (SELECT * from #APZ_OEWS);';
	
	create local temporary table #APZ_EWS1Local as (select top 0 * from APZ_EWS1);
	exec 'DROP TABLE #APZ_EWS1Local;';
	exec 'CREATE LOCAL TEMPORARY TABLE #APZ_EWS1Local as (SELECT * from #APZ_EWS1);';
	
	--Set LineNum for DataLine
	UPDATE #APZ_EWS1Local set "LineNum"= -1;
	select ifnull((select top 1 1 from #APZ_EWS1Local where "LineNum" = -1),0) into checkCount from DUMMY;
	lineNum = 0;
	while :checkCount = 1
	DO
		update top 1 #APZ_EWS1Local set "LineNum" = :lineNum where "LineNum" = -1;
		select ifnull((select top 1 1 from #APZ_EWS1Local where "LineNum" = -1),0) into checkCount from DUMMY;
		lineNum = :lineNum + 1;
	END WHILE;
	
	select ifnull((select top 1 "ShiftID" from #APZ_OEWSLocal),0) into Id from DUMMY;	
	
	If :Id = 0
	Then
		--Case Add New
		--Get Max Id & Update Id for temp table
		select ifnull((select max("ShiftID") from APZ_OEWS),0) into Id from DUMMY;
		update #APZ_OEWSLocal set "ShiftID"=:Id + 1;
		update #APZ_EWS1Local set "ShiftID"=:Id + 1;
	Else
		--Case Update
		--Delete current record in APZ_OEWS & APZ_EWS1
		delete from APZ_OEWS where "ShiftID"=:Id;
		delete from APZ_EWS1 where "ShiftID"=:Id;
		update #APZ_EWS1Local set "ShiftID"=:Id;
	End if;
	
	insert into APZ_OEWS select * from #APZ_OEWSLocal;
	insert into APZ_EWS1 select * from #APZ_EWS1Local;
	
	--Return Id inserted
	select :id,'' from DUMMY; 
END;




