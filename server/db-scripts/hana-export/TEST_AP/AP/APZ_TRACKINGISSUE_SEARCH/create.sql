
Create procedure APZ_TrackingIssue_Search
(
	in bpCode nvarchar(50) default '',
	in projectCode  nvarchar(50) default '',
	in projectOwner int default 0,
	in handleBy nvarchar(254)  default '',
	in reportBy nvarchar(254) default '',
	in id int default 0,
	in status char(1) default '',
	in fromDate timestamp default '',
	in toDate timestamp default '',
	in userName nvarchar(100) default ''
)
as
Begin
	declare userSign integer = IFNULL((SELECT UserId FROM OUSR WHERE USER_CODE = :userName), 0);
	declare authMode char(1) = IFNULL(nullif((select top 1 "AuthorizationMode" from APZ_D_OAPC),''), 'O');
	
	create local temporary table #tmpEmployees ("EmpId" int);
	call APZ_D_FilterEmployees (:userSign, '#tmpEmployees');
	
	call apz_func_OwnerOverShip(:UserName,'1470000130', OwnerView);
	
	if(:id>0)
	then
		SELECT a.*, b."Descriptio" AS "SolutionDesc",c."Color" as "ColorStatus", c."Name" AS "StatusName", IFNULL(d."firstName", '') || ' ' || IFNULL(d."middleName", '') || ' ' || IFNULL(d."lastName", '') AS "ReporterName"
				, IFNULL(e."firstName", '') || ' ' || IFNULL(e."middleName", '') || ' ' || IFNULL(e."lastName", '') AS "HandlerName", d."firstName" AS "RepFName", d."lastName" AS "RepLName", d."picture"
				, (SELECT COUNT(*) FROM APZ_D_UUPIM f WHERE f."ObjectType" = '1470000130' AND f."ObjectCode" = a."Id") AS "AttachmentCount"
				, (select "LinkThumbnails" from (select "LinkThumbnails", row_number() over (order by "LinkThumbnails") ID from APZ_D_UUPIM f where f."ObjectType" = '1470000171' and d."userId" = f."UserSign") x1 where ID = 1) as "LinkThumbnails",
				(SELECT COUNT(*) FROM APZ_CLG3 WHERE "CmtCode" IN (SELECT "CmtCode" FROM APZ_CLG1 WHERE ID = a."Id" AND "ObjType" = '1470000130') AND "UserSign" = userSign AND "Unread" = 'Y') AS "UnreadCount"
				 ,f."CardName" as "BpName"
				 FROM APZ_TGIE a
			left join oslt b on a."SolutionId"=b."SltCode"
			left join APZ_OTIS c on cast(a."Status" as int) = c."Code"
			left join OHEM d on a."Reporter" = d."empID"
			left join OHEM e on a."HandleBy" = e."empID"
			LEFT OUTER JOIN OCRD f ON f."CardCode" = a."BpCode"
		where "Id"=:id
			AND (ifnull(:userName,'') = '' or (
			(IFNULL(a."Reporter",0)=0 or Exists (select 1 from :OwnerView xx where compareSplit(xx."OwnerId", a."Reporter", ';') = 1))
				OR
				(IFNULL(a."HandleBy",0)=0 or Exists (select 1 from :OwnerView xx where compareSplit(xx."OwnerId", a."HandleBy", ';') = 1))
			));
		return;
	end if;
		SELECT a.*, b."Descriptio" AS "SolutionDesc",c."Color" as "ColorStatus", c."Name" AS "StatusName", IFNULL(d."firstName", '') || ' ' || IFNULL(d."middleName", '') || ' ' || IFNULL(d."lastName", '') AS "ReporterName"
				, IFNULL(e."firstName", '') || ' ' || IFNULL(e."middleName", '') || ' ' || IFNULL(e."lastName", '') AS "HandlerName", d."firstName" AS "RepFName", d."lastName" AS "RepLName", d."picture"
				, (SELECT COUNT(*) FROM APZ_D_UUPIM f WHERE f."ObjectType" = '1470000130' AND f."ObjectCode" = a."Id") AS "AttachmentCount" 
				, (select "LinkThumbnails" from (select "LinkThumbnails", row_number() over (order by "LinkThumbnails") ID from APZ_D_UUPIM f where f."ObjectType" = '1470000171' and d."userId" = f."UserSign") x1 where ID = 1) as "LinkThumbnails"
			,(SELECT COUNT(*) FROM APZ_CLG3 WHERE "CmtCode" IN (SELECT "CmtCode" FROM APZ_CLG1 WHERE ID = a."Id" AND "ObjType" = '1470000130') AND "UserSign" = userSign AND "Unread" = 'Y') AS "UnreadCount"
			,f."CardName" as "BpName"
			FROM APZ_TGIE a LEFT OUTER JOIN oslt b ON a."SolutionId" = b."SltCode" 
				LEFT OUTER JOIN APZ_OTIS c ON CAST(a."Status" AS integer) = c."Code" 
				LEFT OUTER JOIN OHEM d ON a."Reporter" = d."empID" 
				LEFT OUTER JOIN OHEM e ON a."HandleBy" = e."empID"
				LEFT OUTER JOIN OCRD f ON f."CardCode" = a."BpCode"
			where 
			ifnull("IssueDeleted", 'N') = 'N' and
				(ifnull(:bpCode, '') = '' or "BpCode" = :bpCode) and
				(ifnull(:projectCode,'') ='' or "ProjectCode"=:projectCode) and
				(ifnull(:projectOwner,0) = 0 or "ProjectOwner"=:projectOwner) and 
				(ifnull(:handleBy,'') = '' or a."HandleBy" in (SELECT cast(data as int) FROM split(:handleBy,';'))) and 
				(ifnull(:reportBy,'') ='' or a."Reporter"  in (SELECT cast(data as int) FROM split(:reportBy,';'))) and
				(ifnull(:id,0) = 0 or "Id"=:id) and
				(ifnull(:status, '') = '' or :status = '0' or cast(a."Status" as int) < cast(:status as int)) and
				(ifnull(:fromDate, '') = '' or CAST(ifnull("IssueDate", current_date) as Date) >= :fromDate) and
				(ifnull(:toDate,'') = '' or CAST(ifnull("IssueDate", current_date) as Date) <= :toDate)
				AND ( 
					(:authMode = 'R' AND (a."Reporter" in (select distinct "EmpId" from #tmpEmployees) or a."HandleBy" in (select distinct "EmpId" from #tmpEmployees))
					or
					(:authMode = 'O' and
						(
							ifnull(:userName,'') = ''
							or 
							(ifnull(a."Reporter",0)=0 or Exists (select 1 from :OwnerView xx where compareSplit(xx."OwnerId", a."Reporter", ';') = 1))
							or
							(IFNULL(a."HandleBy",0)=0 or Exists (select 1 from :OwnerView xx where compareSplit(xx."OwnerId", a."HandleBy", ';') = 1))
							
						 )
					)
					)
				)
				order by "IssueDate" DESC;
End;




