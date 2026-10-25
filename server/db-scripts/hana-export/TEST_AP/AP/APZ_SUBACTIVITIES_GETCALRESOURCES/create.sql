
--CALL APZ_SubActivities_GetCalResources ('manager')
 CREATE PROCEDURE APZ_SubActivities_GetCalResources ( user nvarchar(20)) AS BEGIN user := IFNULL(NULLIF(:user, 'manager'), '');
CREATE LOCAL TEMPORARY TABLE #tblUSER AS (SELECT IFNULL(U_NAME, 'Unknown') || ' (USER)' AS "ResName", 'U' || CAST(USERID AS nvarchar) AS "ResourceId" FROM OUSR WHERE "Locked" = 'N' AND GROUPS <> 99);

CREATE LOCAL TEMPORARY TABLE #tblEMPL AS (SELECT IFNULL("lastName", '') || ',' || IFNULL("firstName", '') || ' (EMPL)' AS "ResName", 'E' || CAST("empID" AS nvarchar) AS "ResourceId" FROM OHEM WHERE "Active" = 'Y');

create LOCAL TEMPORARY table #tbl as (SELECT * FROM #tblUSER UNION ALL SELECT * FROM #tblEMPL);
SELECT DISTINCT (a."ResourceId"), a."ResName", IFNULL(b."Color", -1) AS "Color", IFNULL(b."Remarks", '') AS "Remarks", IFNULL(b."Checked", 'N') AS "Checked", -1 AS "CellColor", IFNULL(b."UserSign", '') AS "UserSign" FROM #tbl a LEFT OUTER JOIN APZ_OACS b ON a."ResourceId" = b."ResourceId" WHERE b."UserSign" = :user ORDER BY a."ResourceId" DESC;
END;




