
CREATE PROCEDURE apz_itemmaster_navigate (IN type integer, IN current nvarchar(50) default '', IN user nvarchar(20) default '', IN branch smallint default 0) 
AS 
BEGIN
declare CurrentId integer := 0;
declare MaxId integer := 0;
 CREATE LOCAL TEMPORARY TABLE #tbl AS (SELECT ROW_NUMBER() OVER ( ORDER BY "ItemCode") AS "ID", "ItemCode" FROM OITM);
select IFNULL((SELECT MAX("ID") FROM #tbl), 0) into MaxId from DUMMY;
IF :current <> '' THEN Select IFNULL((SELECT ID FROM #tbl WHERE "ItemCode" = :current), 0) into CurrentId from DUMMY;
-- First
END IF;
IF :type = 0 THEN SELECT "ItemCode" FROM #tbl WHERE "ID" = 1;
-- Previous
ELSE IF :type = 1 THEN IF :CurrentId = 0 THEN SELECT "ItemCode" FROM #tbl WHERE "ID" = 1;
ELSE IF :CurrentId = 1 THEN SELECT "ItemCode" FROM #tbl WHERE ID = :MaxId;
ELSE SELECT TOP 1 "ItemCode" FROM #tbl WHERE "ID" < :CurrentId;
END IF;
END IF;
-- Next
ELSE IF :type = 2 THEN IF :CurrentId = :MaxId THEN SELECT "ItemCode" FROM #tbl WHERE "ID" = 1;
ELSE IF :CurrentId = 0 THEN SELECT "ItemCode" FROM #tbl WHERE ID = (SELECT MAX("ID") FROM #tbl);
ELSE SELECT TOP 1 "ItemCode" FROM #tbl WHERE "ID" > :CurrentId;
END IF;
END IF;
-- Last
ELSE IF :type = 3 THEN SELECT "ItemCode" FROM #tbl WHERE "ID" = :MaxId;
END IF;
END IF;
END IF;
END IF;
DROP TABLE #tbl;
END;





