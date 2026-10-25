
CREATE  PROCEDURE APZ_SalesOpportunity_GetLinkedActivities(in oprId int default 0)
AS
BEGIN

	declare rowCount int = 0;
	
	CREATE local temporary TABLE #tbl AS (SELECT top 0 CAST("ClgCode" AS nvarchar) AS "ClgCode", LEFT(CAST(IFNULL("CntctDate", '') AS nvarchar), 10) AS "CntctDate"
										, CASE WHEN "CntctTime" IS NULL THEN '' WHEN LENGTH(CAST("CntctTime" AS nvarchar)) = 2 
												THEN substring(CAST("CntctTime" AS nvarchar), 1, 1) || ':' || substring(CAST("CntctTime" AS nvarchar), 2, 1) WHEN LENGTH(CAST("CntctTime" AS nvarchar)) = 3 
												THEN substring(CAST("CntctTime" AS nvarchar), 1, 1) || ':' || substring(CAST("CntctTime" AS nvarchar), 2, 2) WHEN (LENGTH(CAST("CntctTime" AS nvarchar)) = 4) 
												THEN substring(CAST("CntctTime" AS nvarchar), 1, 2) || ':' || substring(CAST("CntctTime" AS nvarchar), 3, 2) END AS "CntctTime"
												, CAST("AttendUser" AS nvarchar) AS "HandledBy", CASE "Closed" WHEN 'Y' THEN 'Closed' WHEN 'N' THEN CAST("Recontact" AS nvarchar(11)) END AS "ActDate"
												, IFNULL("RecurPat", '') AS "RecurPat", IFNULL("Notes", '') AS "Notes", "OprLine" FROM oclg);
								
	INSERT INTO #tbl				
	SELECT CAST("ClgCode" AS nvarchar) AS "ClgCode", LEFT(CAST(IFNULL("CntctDate", '') AS nvarchar), 10) AS "CntctDate"
										, CASE WHEN "CntctTime" IS NULL THEN '' WHEN LENGTH(CAST("CntctTime" AS nvarchar)) = 2 
												THEN substring(CAST("CntctTime" AS nvarchar), 1, 1) || ':' || substring(CAST("CntctTime" AS nvarchar), 2, 1) WHEN LENGTH(CAST("CntctTime" AS nvarchar)) = 3 
												THEN substring(CAST("CntctTime" AS nvarchar), 1, 1) || ':' || substring(CAST("CntctTime" AS nvarchar), 2, 2) WHEN (LENGTH(CAST("CntctTime" AS nvarchar)) = 4) 
												THEN substring(CAST("CntctTime" AS nvarchar), 1, 2) || ':' || substring(CAST("CntctTime" AS nvarchar), 3, 2) END AS "CntctTime"
												, CAST("AttendUser" AS nvarchar) AS "HandledBy", CASE "Closed" WHEN 'Y' THEN 'Closed' WHEN 'N' THEN CAST("Recontact" AS nvarchar(11)) END AS "ActDate"
												, IFNULL("RecurPat", '') AS "RecurPat", IFNULL("Notes", '') AS "Notes", "OprLine" FROM oclg WHERE "OprId" = :oprId;
												
	select IFNULL((select count(1) from #tbl),0) into rowCount from DUMMY;
	
	if rowCount<= 0
	THEN
		INSERT INTO #tbl ("ClgCode", "CntctDate", "CntctTime", "HandledBy", "ActDate", "RecurPat", "Notes", "OprLine") VALUES ('', '', '', '', '', '', '', 0);
	END IF;
	select * from #tbl;

	-- select  CntctTime,Closed,Recontact from oclg where clgcode=30
END;




