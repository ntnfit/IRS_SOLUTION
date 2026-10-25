
CREATE PROCEDURE apz_SalesOpportunity_GetRecord(
	in positionValue nvarchar(1) default 'F',				--F, P, N, L: First, Previous, Next, Last
	in opprId int default -1,
	in UserCode nvarchar(30) default '',
	in BPLId int default 0)
as
BEGIN
	---------------------------------- VARIABLE -------------------------------------------------------
	declare CCode int;
	
	Declare firstCode int;
	Declare lastCode int;
	---------------------------------- END VARIABLE --------------------------------------------------

	select ifnull((select top 1 "OpprId" from OOPR ORDER by "OpprId"),0) into firstCode from DUMMY;
	select ifnull((select top 1 "OpprId" from OOPR  ORDER by "OpprId" desc),0) into lastCode from DUMMY;
	
	if :positionValue = 'F' OR :opprId = :lastCode AND :positionValue = 'N'
	THEN
		CCode = :firstCode;
	end IF;
	
	if :positionValue = 'L' OR :opprId = :firstCode AND :positionValue = 'P'
	THEN
		CCode = :lastCode;
	end IF;
	
	create local temporary table #cte as (select ROW_NUMBER() OVER (ORDER BY "OpprId") RN, "OpprId" FROM OOPR);
	
	if :positionValue ='P'  AND :opprId <> 0 AND :opprId <> :firstCode
	THEN			
		select IFNULL((SELECT prev."OpprId" FROM #CTE a LEFT JOIN #CTE prev ON prev.RN = a.RN - 1  WHERE a."OpprId" = :opprId),0) into CCode from DUMMY;
	end IF;
	
	if :positionValue ='N'  AND :opprId <> 0 AND :opprId <> :lastCode
	THEN
		select IFNULL((SELECT nex."OpprId" FROM #CTE a LEFT JOIN #CTE nex ON nex.RN = a.RN + 1  WHERE a."OpprId" =:opprId), 0) into CCode from DUMMY;
	end IF;
	
	select :CCode from DUMMY;
END



