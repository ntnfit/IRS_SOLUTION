
CREATE PROCEDURE apz_searchsetting_get (IN FormId nvarchar(50), IN Itemid nvarchar(50), IN UserSign nvarchar(50)) AS BEGIN 
CREATE LOCAL TEMPORARY TABLE #tbl AS (SELECT TOP 0 * FROM APZ_SEARCHSETTING );
insert into #tbl select * from APZ_SEARCHSETTING WHERE "UserSign" = :UserSign AND "FormId" = :FormId AND "ItemId" = :Itemid;
SELECT * FROM #tbl;
END;




