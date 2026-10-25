
CREATE PROCEDURE APZ_QueryManager_GetCategory (IN userCode varchar(25))
AS
BEGIN
	create local temporary table #tbl (GroupId int);
	insert into #tbl select substring("menucode", 20)
	from OPRM where "menucode" like 'barSub_QueryAuthGrp%' and USER_CODE=:userCode and "authoriza"='F';
	
	SELECT "CategoryId", "CatName", "PermMask", "UserSign"
	FROM OQCN where "CategoryId" in (select a.CATEGORYID from QAG1 a join #tbl b on a.AUTHGRPID = b.GroupId
														      join OQAG c on c.AUTHGRPID = b.GroupId where c."Deleted" = 'N');
END



