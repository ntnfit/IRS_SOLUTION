
Create   PROCEDURE APZ_D_ProjectManager_LoadData(in absEntry int default 0, in code nvarchar(50) default '', in bpCode nvarchar(50) default '', in status char(1) default ''
											, in startFrom timestamp default null, in startTo timestamp  default null, in userName nvarchar(254) default '', in filterString nvarchar(254) default '')
AS
BEGIN
	DECLARE MaxStart timestamp;
	DECLARE MaxEnd timestamp;
		CALL apz_func_OwnerOverShip(:userName,'1470000132', OwnerView);
	SELECT IFNULL((SELECT MAX("StartDate") FROM APZ_OPRG),CURRENT_DATE) into MaxStart from DUMMY;
	SELECT IFNULL((SELECT MAX("DueDate") FROM APZ_OPRG), CURRENT_DATE) into MaxEnd FROM DUMMY;
	if :absEntry>0
	THEN
		select a.*,IFNULL(b."firstName", '') || ' ' || IFNULL(b."middleName", '') || ' ' || IFNULL(b."lastName", '') AS "PMName", (select count(1) from APZ_COMMENTS c where a."AbsEntry"=c.OBJECT_CODE and ifnull(c.DETAIL_CODE,'')='' and c.OBJTYPE='1470000132') "CommentsCount" 
		,c."Code" as "CodeMaster"
		from APZ_OPRG
		a left join OHEM b on a.PM= b."empID" 
		 left join APZ_OPRG c on c."AbsEntry"= a."MasterEntry"
		where a."AbsEntry"=:absEntry and (ifnull(a."Owner",0)=0 or Exists (select 1 from :OwnerView xx where compareSplit(xx."OwnerId", a."Owner", ';') = 1));
		return;
	end IF;
	

	
	select a.*,IFNULL(b."firstName", '') || ' ' || IFNULL(b."middleName", '') || ' ' || IFNULL(b."lastName", '') AS "PMName", (select count(1) from APZ_COMMENTS c where a."AbsEntry"=c.OBJECT_CODE and ifnull(c.DETAIL_CODE,'')='' and c.OBJTYPE='1470000132') "CommentsCount"
	, c."Code" as "CodeMaster"
	from APZ_OPRG a 
	left join OHEM b on a.PM= b."empID"
	left join APZ_OPRG c on c."AbsEntry"= a."MasterEntry"
	 where 
			(ifnull(:code,'') ='' or a."Code" like N'%'||:code || '%') and
			(ifnull(:bpCode, '') = '' or a."CardCode" = :bpCode) and
			(ifnull(:status,'') = '' or a."PrgStatus"=:status)and
			(ifnull(:filterString,'')='' or (cast(a."AbsEntry" as nvarchar(100))= :filterString) or a."Code" like N'%'|| :filterString || '%' or a."Name" like N'%' || :filterString || '%') and
			(ifnull(a."StartDate",'1900-01-01') BETWEEN ifnull(:startFrom,'1900-01-01') AND ifnull(:startTo,:MaxStart)) and
			(ifnull(a."Owner",0)=0 or Exists (select 1 from :OwnerView xx where compareSplit(xx."OwnerId", a."Owner", ';') = 1))
			 order by a."AbsEntry" DESC;

end



