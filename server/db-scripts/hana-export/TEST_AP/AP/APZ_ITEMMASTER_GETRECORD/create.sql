
--CALL APZ_ItemMaster_GetRecord ('TR0001','P','manager',-1)
CREATE PROCEDURE APZ_ItemMaster_GetRecord (IN ItemCode nvarchar(50) default '',IN TypeRecord nvarchar(1) default '',  IN UserCode nvarchar(50) default '', IN BPLId integer default 0, in IsAsset CHAR(1) default 'N') 

as
BEGIN
declare itCode nvarchar(255) = '';
declare firstCode nvarchar(255)='';
declare lastCode nvarchar(255)='';
declare cloudImage nvarchar(500);
select IFNULL((SELECT TOP 1 ("ItemCode") FROM OITM WHERE (("ItemType" <> 'F' AND :IsAsset<>'Y')  OR (:IsAsset='Y' AND "ItemType"='F')) ORDER BY "ItemCode"), '') into firstCode from DUMMY;

select IFNULL((SELECT TOP 1 ("ItemCode") FROM OITM WHERE (("ItemType" <> 'F' AND :IsAsset<>'Y')  OR (:IsAsset='Y' AND "ItemType"='F')) ORDER BY "ItemCode" DESC), '') into lastCode from DUMMY;

--First Record
IF (:TypeRecord = 'F' OR (:ItemCode = :lastCode AND :TypeRecord = 'N')) THEN itCode := :firstCode;
END IF;


--Next Record

IF (TypeRecord ='N'  AND ItemCode <> '' AND ItemCode <> :lastCode)
then
BEGIN
		 With CTE as ( select ROW_NUMBER() OVER (ORDER BY "ItemCode") as RN, * FROM  oitm WHERE (("ItemType" <> 'F' AND :IsAsset<>'Y')  OR (:IsAsset='Y' AND "ItemType"='F')))
                        SELECT  "Next Row" ."ItemCode" into itCode
                        FROM CTE "Current Row" 
                     LEFT JOIN CTE "Next Row" 
                     ON  "Next Row".RN = "Current Row".RN + 1 
                     WHERE "Current Row"."ItemCode" = :ItemCode;
END;
end if;

--Previous Record
IF (TypeRecord ='P'  AND ItemCode <> '' AND ItemCode <> :firstCode)
then
BEGIN
		 With CTE as ( select  ROW_NUMBER() OVER (ORDER BY "ItemCode") as RN, * FROM oitm WHERE (("ItemType" <> 'F' AND :IsAsset<>'Y')  OR (:IsAsset='Y' AND "ItemType"='F')))
                        SELECT "Previous Row" ."ItemCode" into itCode
                        FROM CTE "Current Row" 
                     LEFT JOIN CTE "Previous Row"
                     ON  "Previous Row".RN = "Current Row".RN - 1 
                     WHERE "Current Row"."ItemCode" = :ItemCode;
END;
end if;

--Last Record
IF (:TypeRecord = 'L' OR (:ItemCode = :firstCode AND :TypeRecord = 'P')) THEN itCode := :lastCode;
END IF;

CREATE LOCAL TEMPORARY TABLE #tbl AS (SELECT TOP 0 * FROM oitm);
insert into #tbl select Top 1 * from oitm  WHERE (("ItemType" <> 'F' AND :IsAsset<>'Y')  OR (:IsAsset='Y' AND "ItemType"='F')) AND "ItemCode" = :itCode;
UPDATE #tbl SET "validFor" = 'Y' WHERE IFNULL("validFor", 'N') = 'N' AND IFNULL("validFrom", '') = '' AND IFNULL("validTo", '') = '' AND IFNULL("frozenFor", 'N') = 'N' AND IFNULL("frozenFrom", '') = '' AND IFNULL("frozenTo", '') = '';
select ifnull((select top 1 "LinkThumbnails" from APZ_D_UUPIM where "ObjectCode" = :itCode and "ObjectType" = '810039' order by "UploadDate", "UploadTime" DESC),'') into cloudImage from dummy;
SELECT *, :cloudImage as "CloudImage" FROM #tbl;
DROP TABLE #tbl;
END




