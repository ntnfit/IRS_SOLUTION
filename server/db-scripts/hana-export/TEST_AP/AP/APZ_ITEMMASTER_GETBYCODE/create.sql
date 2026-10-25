
CREATE PROCEDURE apz_itemmaster_getbycode 
( item nvarchar(50),  user nvarchar(20) default '',  branch integer default -1) 
AS BEGIN 
CREATE LOCAL TEMPORARY TABLE #tbl AS (SELECT * FROM OITM);
SELECT * FROM #tbl  WHERE "ItemCode" = :item;
DROP TABLE #tbl;
SELECT * FROM oitw WHERE "ItemCode" = :item;
END;




