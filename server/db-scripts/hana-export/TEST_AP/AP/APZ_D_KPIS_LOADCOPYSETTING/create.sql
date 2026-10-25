
CREATE PROCEDURE Apz_D_KPIs_LoadCopySetting(in absEntry int default 0)
AS
BEGIN
	declare kpiCode nvarchar(50) = '';
	declare viewType varchar(1) = '';
	declare period timestamp = '';
	if exists(select 1 from APZ_D_OKPM where "AbsEntry" = :absEntry)
	THEN
		select "Code", "ViewType","Period" into kpiCode, viewType, period from APZ_D_OKPM where "AbsEntry" = :absEntry;
	END if;

	CREATE local temporary TABLE #tblExisted ("empID" int);
	
	insert into #tblExisted SELECT "empID" FROM APZ_D_OKPM WHERE "Code" = :kpiCode AND "ViewType" = :viewType AND "Period" = :period;
	
	SELECT b.*, IFNULL(b."firstName", '') || ', ' || IFNULL(b."lastName", '') AS "empName", b."jobTitle", f."name" AS "PositionName", d."Name" AS "Department", e."Name" AS "BranchName"
			, CASE WHEN b."manager" IS NOT NULL THEN IFNULL(c."firstName", '') || ', ' || IFNULL(c."lastName", '') ELSE '' END AS "ManagerName", b."ExtEmpNo" 
		FROM OHEM b LEFT OUTER JOIN OHEM c ON b."manager" = c."empID" 
					LEFT OUTER JOIN OUDP d ON b."dept" = d."Code" 
					LEFT OUTER JOIN OUBR e ON b."branch" = e."Code" 
					LEFT OUTER JOIN OHPS f ON b."position" = f."posID" 
				WHERE b."Active" = 'Y' AND b."empID" NOT IN (SELECT "empID" FROM #tblExisted);

END




