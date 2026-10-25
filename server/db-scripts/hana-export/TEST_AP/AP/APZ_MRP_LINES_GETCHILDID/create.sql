
CREATE procedure apz_MRP_LINES_GetChildID(in LineID int default 0)
as
BEGIN
	
	declare maxId int = 0;
	declare i int = 0;
	declare tempLineId int = 0;
	declare tempLevel int = 0;
	create local temporary table #DirectReports ("FatherID" int, "LineID" int, "Level" int, "ID" int);
	insert into #DirectReports Select "FatherLine","LineID",0, row_number() over(order by "LineID") From APZ_OLIN where "LineID"=:LineID;
	
	select ifnull((select max(ID) from #DirectReports), 0) into maxId from DUMMY;
	WHILE :i <= :maxId
	DO
	
		select ifnull((select "LineID" from #DirectReports where ID = :i), 0) into tempLineId from DUMMY;
		select ifnull((select "Level" from #DirectReports where ID = :i), 0) into tempLevel from DUMMY;
		
		insert into #DirectReports Select a."FatherLine",a."LineID",:tempLevel + 1, :maxId + row_number() over(order by a."LineID") From APZ_OLIN a where a."FatherLine" = :tempLineId;
		i = :i + 1;
		select ifnull((select max(ID) from #DirectReports), 0) into maxId from DUMMY;
	
	END WHILE;
	select string_agg("LineID", ',') from #DirectReports;
END



