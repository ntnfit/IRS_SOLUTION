
CREATE  PROCEDURE apz_Service_Solution_Search(in SltCode  int default 0, in ItemCode NVARCHAR(50) default NULL, in Status int default 0)
AS
Begin
	SELECT a.* FROM OSLT a WHERE (IFNULL(:SltCode, 0) = 0 OR a."SltCode" = :SltCode) AND (IFNULL(:ItemCode, '') = '' OR a."ItemCode" LIKE '%' || :ItemCode || '%') 
								AND (IFNULL(:Status, 0) = 0 OR a."StatusNum" = :Status) ORDER BY "SltCode" DESC;
END



