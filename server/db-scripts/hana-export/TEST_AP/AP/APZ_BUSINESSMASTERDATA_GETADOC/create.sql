
CREATE  PROCEDURE apz_BusinessMasterData_GetADoc(
in CardCode nvarchar(50) default '',
in UserName  nvarchar(200) default '',
in BPLId int default 0)
as
BEGIN
	declare dofilter char(1);
	DECLARE DoMngMth CHAR(1);
	declare cloudImage nvarchar(500);
	select ifnull((select "DoFilter" from cinf),'N') into dofilter from DUMMY;
	SELECT IFNULL((SELECT  "DoMngMth" FROM oadm ),'') into DoMngMth from DUMMY;
	select ifnull((select top 1 t."LinkThumbnails" from APZ_D_UPIM t where t."IsAvatar" = 'Y' and t."CardCode" = :CardCode order by t."UploadDate", t."UploadTime" DESC),'') into cloudImage from dummy;
	--create local temporary table #tbl1 as (SELECT *,0 "CntctCode", cast('' as nvarchar(50)) "MailAddress",cast('' as nvarchar(254)) "CntPrsName", cast('' as nvarchar(50)) "PayDuMonth",0 "ExtraMonth",0 "ExtraDays"
	--				,cast('' as nvarchar(50)) "BslineDate" FROM OCRD);
	
	call apz_func_OwnerOverShip(:UserName,2, ownnerShip);
	
	SELECT A.*,B."CntctCode", D."Street" AS "MailAddress",B."Name" as "CntPrsName", F."PayDuMonth",F."ExtraMonth",F."ExtraDays",F."BslineDate",
	ifnull((select "LinkThumbnails" from (select "LinkThumbnails", row_number() over(order by "UploadDate", "UploadTime" DESC) as row_id from APZ_D_UPIM where "IsAvatar" = 'Y' and A."CardCode" = "CardCode") where row_id = 1),'') as "CloudImage"
						FROM OCRD A
	                    LEFT JOIN OCPR B ON A."CardCode" = B."CardCode" AND A."CntctPrsn" = B."Name"
	                    LEFT JOIN CRD1 D ON A."CardCode" = D."CardCode" AND A."ShipToDef" = D."Address" AND d."AddrType"='S'
	                    LEFT JOIN CRD1 E ON A."CardCode" = E."CardCode" AND A."BillToDef" = E."Address" AND E."AddrType"='B'
						LEFT JOIN OCTG F ON A."GroupNum"=F."GroupNum"
						WHERE A."CardCode"=:CardCode AND  (:dofilter='N' OR (:dofilter='Y' AND :DoMngMth NOT IN ('A','B')) OR (:dofilter='Y' AND :DoMngMth IN ('A','B')
								AND	(IFNULL(A."OwnerCode",-1)=-1 OR (A."OwnerCode" IN (select distinct "OwnerId" from :ownnerShip)))));
end



