
--APZ_Retail_GetUserPosAssignment 'manager'
CREATE PROCEDURE APZ_Retail_GetUserPosAssignment
(UserCode nvarchar(25) default '')
as
BEGIN
declare multiBranch varchar(1);
select IFNULL((SELECT TOP 1 "MltpBrnchs" FROM OADM), 'N') into multiBranch from DUMMY;

IF (:UserCode = '') THEN SELECT a."PosID", a."PosCode", a."Dscription", a."Store", IFNULL(b."IsDflt", 'N') AS "IsDflt", 'N' AS "Checked" FROM APZ_RPOS a LEFT OUTER JOIN APZ_POS1 b ON a."PosID" = b."PosID" LEFT OUTER JOIN APZ_STORE c ON a."Store" = c."Code" WHERE c."Locked" = 'N' AND a."Inactive" = 'N';
ELSE IF (:multiBranch = 'Y') THEN 
BEGIN
	CREATE LOCAL TEMPORARY TABLE #tblPos AS (SELECT top 0 a.* FROM APZ_RPOS a);
	insert into #tblPos select a.* from APZ_RPOS a INNER JOIN APZ_STORE b ON a."Store" = b."Code" INNER JOIN USR6 c ON b."BPLId" = c."BPLId" WHERE c."UserCode" = :UserCode;
	SELECT a."PosID", a."PosCode", a."Dscription", a."Store", IFNULL(b."IsDflt", 'N') AS "IsDflt", CASE WHEN b."PosID" IS NULL THEN 'N' ELSE 'Y' END AS "Checked" FROM #tblPos a LEFT OUTER JOIN APZ_POS1 b ON a."PosID" = b."PosID" AND b."UserCode" = :UserCode LEFT OUTER JOIN APZ_STORE c ON a."Store" = c."Code" WHERE c."Locked" = 'N' AND a."Inactive" = 'N';
END;
ELSE SELECT a."PosID", a."PosCode", a."Dscription", a."Store", IFNULL(b."IsDflt", 'N') AS "IsDflt", CASE WHEN b."PosID" IS NULL THEN 'N' ELSE 'Y' END AS "Checked" FROM APZ_RPOS a LEFT OUTER JOIN APZ_POS1 b ON a."PosID" = b."PosID" AND b."UserCode" = :UserCode LEFT OUTER JOIN APZ_STORE c ON a."Store" = c."Code" WHERE c."Locked" = 'N' AND a."Inactive" = 'N';
END IF;
END IF;
END




