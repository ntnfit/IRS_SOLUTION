
CREATE PROCEDURE apz_BusinessMasterData_SearchDoc (
IN CardCode nvarchar(50) default '',
 IN CardName nvarchar(200) default '',
  IN CardFName nvarchar(200) default '',
   IN CardType nvarchar(200) default '',
    IN GroupCode integer default 0,
     IN ShipType integer default 0,
      IN UserCode nvarchar(200) default '',
       IN BPLId integer default 0)
AS BEGIN
declare dofilter char(1);
DECLARE DoMngMth CHAR(1);
select ifnull((select "DoFilter" from cinf),'N') into dofilter from DUMMY;

SELECT IFNULL((SELECT  "DoMngMth" FROM oadm ),'') into DoMngMth from DUMMY;
call apz_func_OwnerOverShip(:UserCode,2, ownnerShip);
SELECT * FROM ocrd a WHERE 
(:CardCode = '' OR (:CardCode <> '' AND a."CardCode" LIKE n'%' || :CardCode || '%')) 
AND (:CardName = '' OR (:CardName <> '' AND "CardName" LIKE n'%' || :CardName || '%')) 
AND (:CardFName = '' OR (:CardFName <> '' AND "CardFName" LIKE n'%' || :CardFName || '%')) 
AND (:CardType = '' OR (:CardType <> '' AND "CardType" LIKE :CardType))
			AND
			(:GroupCode=0 OR (:GroupCode<>0 and a."GroupCode" =:GroupCode))
			AND (:dofilter='N' OR (:dofilter='Y' AND :DoMngMth NOT IN ('A','B')) OR (:dofilter='Y' AND :DoMngMth IN ('A','B') AND	(IFNULL(A."OwnerCode",-1)=-1 OR (A."OwnerCode" IN (select distinct "OwnerId" from  :ownnerShip)))));
			
END




