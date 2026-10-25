
CREATE PROCEDURE apz_searchsetting_save 
(IN FormId nvarchar(20), 
IN ItemId nvarchar(50), 
IN UserSign nvarchar(20), 
IN Branch integer, 
IN Period nvarchar(50), 
IN EmpId integer, 
IN EmpName nvarchar(200), 
IN Dept nvarchar(254), 
IN Status nvarchar(254))
 AS BEGIN 
 DELETE FROM APZ_SEARCHSETTING WHERE "FormId" = :FormId AND "ItemId" = :ItemId AND "UserSign" = :UserSign;
INSERT INTO APZ_SEARCHSETTING ("FormId", "ItemId", "UserSign", "Branch", "Period", "EmpId", "EmpName", "Dept", "Status") VALUES (:FormId, :ItemId, :UserSign, :Branch, :Period, :EmpId, :EmpName, :Dept, :Status);
END;




