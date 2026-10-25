
CREATE PROCEDURE APZ_BUSINESSMASTERDATA_GETRECORD(
in pos nvarchar(1) DEFAULT 'F',
in CardCode nvarchar(50) DEFAULT'',
in UserCode nvarchar(30) DEFAULT '',
in BPLId int DEFAULT 0
)
as
BEGIN
declare CCode nvarchar(50)='';
declare firstCode nvarchar(255)='';
declare lastCode nvarchar(255)='';
declare cloudImage nvarchar(500);
	
select IFNULL((SELECT TOP 1 ("CardCode") FROM  OCRD ORDER by "CardCode"), '') into firstCode from DUMMY;
select IFNULL((SELECT TOP 1 ("CardCode") FROM  OCRD ORDER by "CardCode" DESC), '') into lastCode from DUMMY;

if (:pos = 'F' OR (:CardCode = lastCode AND :pos = 'N')) then

	 CCode := firstCode;

end if;

if (:pos = 'L' OR (:CardCode = firstCode AND :pos = 'P')) then

	 CCode := lastCode;

end if;

if (:pos ='N'  AND :CardCode <> '' AND :CardCode <> lastCode) then

	WITH CTE AS (  select ROW_NUMBER() OVER (ORDER BY "CardCode") as RN, "CardCode" FROM ocrd) 
               SELECT  "Next Row" ."CardCode" into CCode
                 FROM CTE "Current Row" 
                     LEFT JOIN CTE "Next Row" 
                     ON  "Next Row".RN = "Current Row".RN + 1 
                     WHERE "Current Row"."CardCode" = :CardCode;
			

end if;

IF (:pos ='P'  AND :CardCode <> '' AND :CardCode <> :firstCode)
then

		 With CTE as ( select  ROW_NUMBER() OVER (ORDER BY "CardCode") as RN, "CardCode" FROM ocrd)
                        SELECT "Previous Row" ."CardCode"  into CCode
                        FROM CTE "Current Row" 
                     LEFT JOIN CTE "Previous Row"
                     ON  "Previous Row".RN = "Current Row".RN - 1 
                     WHERE "Current Row"."CardCode" = :CardCode;

end if;


select ifnull((select top 1 t."LinkThumbnails" from APZ_D_UPIM t where t."IsAvatar" = 'Y' and t."CardCode" = :CCode order by t."UploadDate", t."UploadTime" DESC),'') into cloudImage from dummy;
select 
	*, cloudImage as "CloudImage"
FROM ocrd
where "CardCode"=CCode;
END;




