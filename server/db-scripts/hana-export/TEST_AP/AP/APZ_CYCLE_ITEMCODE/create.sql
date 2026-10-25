
CREATE PROCEDURE APZ_Cycle_ItemCode()
AS 
BEGIN
select  "ItemCode", "ItemName" from oitm order by "ItemCode" Asc;
END;




