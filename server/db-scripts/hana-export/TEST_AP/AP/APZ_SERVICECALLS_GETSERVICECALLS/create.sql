
CREATE PROCEDURE APZ_ServiceCalls_GetServiceCalls(
	in CardCode nvarchar(50) default '',
	in HandleBy int default 0,
	in CallType int default 0,
	in CallId int default 0,
	in FromDate timestamp default null,
	in ToDate timestamp  default null,
	in Tag nvarchar(50) default '')
AS
BEGIN
	DECLARE tagCount int =0;
	if :Tag!=''
	THEN
		select (SELECT COUNT(1) FROM Split(:Tag,';')) into tagCount from DUMMY;
	END IF;
	
	SELECT * FROM APZ_OSCL
	WHERE ("CardCode"=:CardCode OR :CardCode='') 
		AND ("HandleBy"=:HandleBy OR :HandleBy=0) 
		AND ("CallType"=:CallType OR :CallType=0) 
		AND ("CallID"=:CallId OR :CallId=0) 
		AND ("CreateDate" >= :FromDate OR :FromDate IS NULL)
		AND ("CreateDate" <= :ToDate OR :ToDate IS NULL)
		AND (ifnull(:Tag,'')='' OR "CallID" IN (SELECT a."CallID" FROM APZ_OSCL a where CompareListValueSplit(a."Tags", :Tag, ';', ';') = 1 GROUP BY "CallID" HAVING Count(1) >= :tagCount));
END




